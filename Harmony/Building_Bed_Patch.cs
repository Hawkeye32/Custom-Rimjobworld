using HarmonyLib;
using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// patches Building_Bed to add stuff for WhoreBeds
/// 
/// Also contains smaller patches for RoomRoleWorker_Barracks (don't count whore beds) (disabled) and Toils_LayDown.ApplyBedThoughts (slept in brothel thought)
/// </summary>

namespace rjw
{
	public static class Building_Bed_Patch
	{
		
		private static readonly Color sheetColorForWhores = new Color(181 / 255f, 55 / 255f, 109 / 255f);

		// Set color for whore beds
		[HarmonyPatch(typeof(Building_Bed))]
		[HarmonyPatch(nameof(Building_Bed.DrawColorTwo), MethodType.Getter)]
		public static class Building_Bed_DrawColor_Patch
		{
			[HarmonyPostfix]
			public static void Postfix(Building_Bed __instance, ref Color __result)
			{
				if (__instance.IsAllowedForWhoringAll())
				{
					__result = sheetColorForWhores;
				}
			}
		}

		// add whoring toggles to beds
		[HarmonyPatch(typeof(Building_Bed), nameof(Building_Bed.GetGizmos))]
		public static class Building_Bed_GetGizmos_Patch
		{
			[HarmonyPostfix]
			public static void Postfix(Building_Bed __instance, ref IEnumerable<Gizmo> __result)
			{
				__result = Process(__instance, __result);
			}

			private static IEnumerable<Gizmo> Process(Building_Bed __instance, IEnumerable<Gizmo> __result)
			{
				
				var isPrisonCell = __instance.GetRoom()?.IsPrisonCell == true;
				if (!__instance.ForPrisoners && !__instance.Medical && __instance.def.building.bed_humanlike && __instance.Faction == Faction.OfPlayerSilentFail && !__instance.def.defName.Contains("Guest") && !isPrisonCell)
				{

					yield return
						new Command_Toggle
						{
							defaultLabel = "CommandBedAllowWhoringLabel".Translate(),
							defaultDesc = "CommandBedAllowWhoringDesc".Translate(),
							icon = ContentFinder<Texture2D>.Get("UI/Commands/AsWhore"),
							isActive = __instance.IsAllowedForWhoringOwner,
							toggleAction = __instance.ToggleAllowedForWhoringOwner,
							hotKey = KeyBindingDefOf.Misc5, // Guest Beds uses Misc4
							disabled = !__instance.def.HasAssignableCompFrom(typeof(CompAssignableToPawn_Bed)),
							disabledReason = "This bed type is not assignable to pawns."
						};

					yield return
						new Command_Toggle
						{
							defaultLabel = "CommandBedSetAsWhoreBedLabel".Translate(),
							defaultDesc = "CommandBedSetAsWhoreBedDesc".Translate(),
							icon = ContentFinder<Texture2D>.Get("UI/Commands/AsWhoreMany"),
							isActive = __instance.IsAllowedForWhoringAll,
							toggleAction = __instance.ToggleAllowedForWhoringAll,
							hotKey = KeyBindingDefOf.Misc6, // Guest Beds uses Misc4
							disabled = !__instance.def.HasAssignableCompFrom(typeof(CompAssignableToPawn_Bed)),
							disabledReason = "This bed type is not assignable to pawns."
						};
				}

				foreach (var gizmo in __result)
				{
					if (__instance.IsAllowedForWhoringAll())
					{
						if (gizmo is Command_Toggle && ((Command_Toggle)gizmo).defaultLabel == "CommandBedSetAsGuestLabel".Translate())
						{
							// hide set as guest bed
							continue;
						};
						// old: instead of hiding, just disable
						/*switch (gizmo)
						{
							case Command_Toggle toggle:
								{
									// Disable prisoner and medical, and guest buttons
									if (//toggle.defaultLabel == "CommandBedSetForPrisonersLabel".Translate() ||
										//toggle.defaultLabel == "CommandBedSetAsMedicalLabel".Translate() ||
										toggle.defaultLabel == "CommandBedSetAsGuestLabel".Translate()) gizmo.Disable();
									break;
								}
						}//*/
					}
					yield return gizmo;
				}
			}
		}

		// add description of whore price factor to inspect string (bottom left corner if item selected)
		[HarmonyPatch(typeof(Building_Bed), nameof(Building_Bed.GetInspectString))]
		public static class Building_Bed_GetInspectString_Patch
		{
			[HarmonyPostfix]
			public static void Postfix(Building_Bed __instance, ref string __result)
			{
				if (__instance.def.building.bed_humanlike && __instance.Faction == Faction.OfPlayerSilentFail && (__instance.IsAllowedForWhoringAll() || __instance.IsAllowedForWhoringOwner()))
				{
					__result = __result + "\n" + "WhorePriceCalcDesc".Translate(WhoreBed_Utility.CalculatePriceFactor(__instance).ToString("F2"));
					if (RJWSettings.DebugWhoring)
					{
						__result = __result + "\nbed.thingIDNumber: " + __instance.thingIDNumber.ToString();

						__result = __result + "\nscoreUpdateTickDelay: " + SaveStorage.DataStore.GetBedData(__instance).scoreUpdateTickDelay.ToString();

						if (SaveStorage.DataStore.GetBedData(__instance).reservedUntilGameTick > GenTicks.TicksGame)
						{
							__result = __result + "\nreserved by pawn id: " + SaveStorage.DataStore.GetBedData(__instance).reservedForPawnID.ToString();
						}
					}
				}
			}
		}

		// add whore price factor as overlay
		[HarmonyPatch(typeof(Building_Bed), nameof(Building_Bed.DrawGUIOverlay))]
		public static class Building_Bed_DrawGUIOverlay_Patch
		{
			[HarmonyPrefix]
			public static bool Prefix(Building_Bed __instance)
			{
				if (RJWSettings.show_whore_price_factor_on_bed && __instance.def.building.bed_humanlike && __instance.Faction == Faction.OfPlayerSilentFail) {
					// if whore bed, print price factor on it
					if (Find.CameraDriver.CurrentZoom == CameraZoomRange.Closest
						&& ((__instance.IsAllowedForWhoringOwner() && __instance.OwnersForReading.Any<Pawn>())
							|| __instance.IsAllowedForWhoringAll()))
					{
						Color defaultThingLabelColor = GenMapUI.DefaultThingLabelColor;

						// make string
						float whore_price_factor = WhoreBed_Utility.CalculatePriceFactor(__instance);
						string wpf;
						if (Math.Abs(whore_price_factor) >= 100)
						{
							wpf = ((int)whore_price_factor).ToString("D");
						}
						else if (Math.Abs(whore_price_factor) >= 10)
						{
							wpf = whore_price_factor.ToString("F1");
						}
						else
						{
							wpf = whore_price_factor.ToString("F2");
						}

						// get dimensions of text and make it appear above names
						Vector2 textsize = Text.CalcSize(wpf);
						Vector2 baseLabelPos = GenMapUI.LabelDrawPosFor(__instance, -0.4f); // -0.4f is copied from vanilla code
						baseLabelPos.y -= textsize.y * 0.75f;

						GenMapUI.DrawThingLabel(baseLabelPos, wpf, defaultThingLabelColor);

						if (__instance.IsAllowedForWhoringAll() && !__instance.OwnersForReading.Any<Pawn>())
						{
							// hide "Unowned" if whore bed with no owner
							return false;
						}
					}
				}
				// after drawing whore price factor, draw the actual names
				// could have been done as a postfix, but I started with a prefix, hoping I could get by with only one draw call
				return true;
			}

		}

		// barracks don't count whore beds, so room type switches to brothel sooner
		// disabled - barracks have their own slept in ~ debuff; doesn't really matter; put some effort in your brothels!
		/*[HarmonyPatch(typeof(RoomRoleWorker_Barracks), nameof(RoomRoleWorker_Barracks.GetScore))]
		public static class RoomRoleWorker_Barracks_GetScore_Patch
		{
			public static bool Prefix(Room room, ref float __result)
			{
				int num = 0;
				int num2 = 0;
				List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
				for (int i = 0; i < containedAndAdjacentThings.Count; i++)
				{
					Building_Bed building_Bed = containedAndAdjacentThings[i] as Building_Bed;
					if (building_Bed != null && building_Bed.def.building.bed_humanlike)
					{
						if (building_Bed.ForPrisoners)
						{
							__result = 0f;
							return false;
						}
						num++;
						if (!building_Bed.Medical && !building_Bed.IsAllowedForWhoringAll())
						{
							num2++;
						}
					}
				}
				if (num <= 1)
				{
					__result = 0f;
					return false;
				}
				__result = (float)num2 * 100100f;
				return false;
			}
		}*/

		// if pawns sleep in a brothel or a whoring bed, they get a thought
		[HarmonyPatch(typeof(Toils_LayDown), "ApplyBedThoughts")]
		public class Toils_LayDown_ApplyBedThoughts_Patch
		{
			[HarmonyPostfix]
			public static void Postfix(Pawn actor)
			{
				if (actor?.needs?.mood == null) return;

				Building_Bed building_Bed = actor.CurrentBed();
				
				actor?.needs?.mood?.thoughts?.memories?.RemoveMemoriesOfDef(WhoringThoughtDefOf.SleptInBrothel);

				if (building_Bed == null) return;

				if (building_Bed?.GetRoom()?.Role == WhoreBed_Utility.roleDefBrothel || building_Bed.IsAllowedForWhoringAll())
				{
					var memoryHandler = actor.needs.mood.thoughts.memories;
					int thoughtStage = 0;

					foreach (var thoughtDef in DefDatabase<ThoughtDef_Whore>.AllDefsListForReading)
					{
						var memory = memoryHandler.GetFirstMemoryOfDef(thoughtDef);
						if (memory?.CurStageIndex >= thoughtDef.stages.Count - 1)
						{
							thoughtStage = 1;
							break;
						}
					}

					memoryHandler.TryGainMemory(ThoughtMaker.MakeThought(WhoringThoughtDefOf.SleptInBrothel, thoughtStage));
				}
			}
		}

		// if room stats are updated, update beds within
		//	"necessary" if beds are toggled during pause
		[HarmonyPatch(typeof(Room), "UpdateRoomStatsAndRole")]
		public class Room_UpdateRoomStatsAndRole_Patch
		{
			[HarmonyPostfix]
			public static void Postfix(Room __instance)
			{
				// note: with room stat display enabled, this get's called quite often for whatever room the mouse hovers over
				//	even large outdoor areas
				//	where iterating over all things to find all beds (even if there are none) is expensive
				//	for now, skip doing anything if even the game decides it's not worth it
				//	(game sets role to None if region count >36 or something)
				//	- the beds will update eventually

				if (/*Find.PlaySettings.showRoomStats && */__instance.Role == RoomRoleDefOf.None)
					return;

				if (Find.TickManager.Paused)
				{
					// if paused, update immediately
					WhoreBed_Utility.CalculateBedFactorsForRoom(__instance);
				}
				else
				{
					// else, just make beds update as soon as needed
					WhoreBed_Utility.ResetTicksUntilUpdate(__instance);
				}
			}
		}
	}
}