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
		public static void adding_RJW_toggle(WidgetRow row, bool worldView)
		{
			if (worldView) return;
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
		static void this_is_postfix(ref IEnumerable<Gizmo> __result, ref Pawn __instance)
		{
			if (!RJWSettings.show_RJW_designation_box) return;
			if (!(__instance.Faction == Faction.OfPlayer || __instance.IsPrisonerOfColony)) return;
			//Log.Message("[rjw]Harmony patch submit_button is called");
			var pawn = __instance;
			var gizmos = __result.ToList();
			gizmos.Add(new RJWdesignations(pawn));
			__result = gizmos.AsEnumerable();
		}
	}

AnimalTab
Extensions
        public static bool Pregnant( this Pawn pawn )
        {
            // get hediff
            var _hediff = pawn.health.hediffSet.GetFirstHediffOfDef( HediffDefOf.Pregnant );

            // if pregnant, and pregnancy is far enough advanced to be visible
            if ( _hediff?.Visible ?? false )
                return true;

            // not (visibly) pregnant.
            return false;
        }	///<summary>
	///Submit gizmo
	///</summary>
	[HarmonyPatch(typeof(Pawn), "GetGizmos")]
	[StaticConstructorOnStartup]
	static class submit_button
	{
		[HarmonyPostfix]
		static void this_is_postfix(ref IEnumerable<Gizmo> __result, ref Pawn __instance)
		{
			//Log.Message("[rjw]Harmony patch submit_button is called");
			var pawn = __instance;
			var gizmos = __result.ToList();
			var enabled = RJWSettings.submit_button_enabled;

			if (enabled && pawn.IsColonistPlayerControlled && pawn.Drafted)
				if (pawn.CanChangeDesignationColonist())
					if (!(pawn.kindDef.race.defName.Contains("Droid") && !AndroidsCompatibility.IsAndroid(pawn)))
					{
						gizmos.Add(new Command_Action
						{
							defaultLabel = "CommandSubmit".Translate(),
							icon = submit_icon,
							defaultDesc = "CommandSubmitDesc".Translate(),
							action = delegate
							{
								LayDownAndAccept(pawn);
							},
							hotKey = KeyBindingDefOf.Misc3
						});
					}
			__result = gizmos.AsEnumerable();
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