using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using Multiplayer.API;


namespace rjw
{
	/// <summary>
	/// Harmony patch to toggle the RJW designation box showing
	/// </summary>
	[HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
	[StaticConstructorOnStartup]
	public static class RJW_corner_toggle
	{
		static readonly Texture2D icon = ContentFinder<Texture2D>.Get("UI/Commands/ComfortPrisoner_off");

		[HarmonyPostfix]
		public static void add_RJW_toggle(WidgetRow row, bool worldView)
		{
			if (worldView) return;
			if (!RJWSettings.show_RJW_designation_box) return;
			row.ToggleableIcon(ref RJWSettings.show_RJW_designation_box, icon, "RJW_designation_box_desc".Translate());
		}
	}

	///<summary>
	///Compact button group containing rjw designations on pawn
	///</summary>
	[HarmonyPatch(typeof(Pawn), "GetGizmos")]
	[StaticConstructorOnStartup]
	static class Rjw_buttons
	{
		[HarmonyPostfix]
		[HarmonyPriority(99)]
		static IEnumerable<Gizmo> add_designation_box(IEnumerable<Gizmo> __result, Pawn __instance)
		{
			foreach (var gizmo in __result)
			{
				yield return gizmo; 
			}

			if (!RJWSettings.show_RJW_designation_box) yield break;
			if (!(__instance.Faction == Faction.OfPlayer || __instance.IsPrisonerOfColony)) yield break;
			//ModLog.Message("Harmony patch submit_button is called");
			var pawn = __instance;
			yield return new RJWdesignations(pawn); 
		}
	}

	///<summary>
	///Submit gizmo
	///</summary>
	[HarmonyPatch(typeof(Pawn), "GetGizmos")]
	[StaticConstructorOnStartup]
	static class submit_button
	{
		[HarmonyPostfix]
		[HarmonyPriority(101)]
		static IEnumerable<Gizmo> add_button(IEnumerable<Gizmo> __result, Pawn __instance)
		{
			foreach (var gizmo in __result)
			{
				yield return gizmo;
			}
			
			//ModLog.Message("Harmony patch submit_button is called");
			var pawn = __instance;
			var enabled = RJWSettings.submit_button_enabled;

			if (enabled && pawn.IsColonistPlayerControlled && pawn.Drafted)
				if (pawn.CanChangeDesignationColonist())
					if (!(pawn.kindDef.race.defName.Contains("Droid") && !AndroidsCompatibility.IsAndroid(pawn)))
					{
						yield return new Command_Action
						{
							defaultLabel = "CommandSubmit".Translate(),
							icon = submit_icon,
							defaultDesc = "CommandSubmitDesc".Translate(),
							action = delegate
							{
								LayDownAndAccept(pawn);
							},
							hotKey = KeyBindingDefOf.Misc3
						}; 
					}

		}

		static Texture2D submit_icon = ContentFinder<Texture2D>.Get("UI/Commands/Submit", true);
		static HediffDef submit_hediff = HediffDef.Named("Hediff_Submitting");

		[SyncMethod]
		static void LayDownAndAccept(Pawn pawn)
		{
			//Log.Message("Submit button is pressed for " + pawn);
			pawn.health.AddHediff(submit_hediff);
		}
	}
}