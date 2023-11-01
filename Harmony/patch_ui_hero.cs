using System.Linq;
using Verse;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace rjw
{
	/// <summary>
	/// Patch ui for hero mode
	/// - disable pawn control for non owned hero
	/// - disable equipment management for non owned hero
	/// hardcore mode:
	/// - disable equipment management for non hero
	/// - disable pawn rmb menu for non hero
	/// - remove drafting widget for non hero
	/// </summary>

	//disable forced works(rmb workgivers)
	[HarmonyPatch(typeof(FloatMenuMakerMap), "CanTakeOrder")]
	[StaticConstructorOnStartup]
	static class disable_FloatMenuMakerMap
	{
		[HarmonyPostfix]
		static void NonHero_disable_controls(ref bool __result, Pawn pawn)
		{
			if (RJWSettings.RPG_hero_control)
			{
				if ((pawn.IsDesignatedHero() && !pawn.IsHeroOwner()))
				{
					__result = false;   //not hero owner, disable menu
					return;
				}

				if (!pawn.IsDesignatedHero() && RJWSettings.RPG_hero_control_HC)
				{
					if (pawn.Drafted && pawn.CanChangeDesignationPrisoner() && pawn.CanChangeDesignationColonist())
					{
						//allow control over drafted pawns, this is limited by below disable_Gizmos patch
					}
					else
					{
						__result = false; //not hero, disable menu
					}
				}
			}
		}
	}

	//TODO: disable equipment management
	/*
	//disable equipment management
	[HarmonyPatch(typeof(ITab_Pawn_Gear), "CanControl")]
	static class disable_equipment_management
	{
		[HarmonyPostfix]
		static bool this_is_postfix(ref bool __result, Pawn selPawnForGear)
		{
			Pawn pawn = selPawnForGear;

			if (RJWSettings.RPG_hero_control)
			{
				if ((pawn.IsDesignatedHero() && !pawn.IsHeroOwner()))	//not hero owner, disable drafting
				{
					__result = false;   //not hero owner, disable menu
				}
				else if (!pawn.IsDesignatedHero() && RJWSettings.RPG_hero_control_HC)   //not hero, disable drafting
				{
					if (false)
					{
						//add some filter for bots and stuff? if there is such stuff
						//so it can be drafted and controlled for fighting
					}
					else
					{
						__result = false; //not hero, disable menu
					}
				}
			}
			return true;
		}
	}
	*/

	//TODO: allow shared control over non colonists(droids, etc)?
	//disable command gizmos
	[HarmonyPatch(typeof(Pawn), "GetGizmos")]
	[StaticConstructorOnStartup]
	static class disable_Gizmos
	{
		[HarmonyPostfix]
		[HarmonyPriority(100)]
		static IEnumerable<Gizmo> NonHero_disable_gizmos(IEnumerable<Gizmo> __result, Pawn __instance)
		{
			
			Pawn pawn = __instance;
			string disablementReason = string.Empty; 

			if (RJWSettings.RPG_hero_control)
			{
				if ((pawn.IsDesignatedHero() && !pawn.IsHeroOwner()))	//not hero owner, disable drafting
				{
					disablementReason = "ForHeroRefuse1Desc";
				}
				else if (!pawn.IsDesignatedHero() && RJWSettings.RPG_hero_control_HC)   //not hero, disable drafting
				{
					//no permission to change designation for NON prisoner hero/ other player
					if (pawn.CanChangeDesignationPrisoner() && pawn.CanChangeDesignationColonist()
							&& (pawn.kindDef.race.defName.Contains("AIRobot")
							|| (pawn.kindDef.race.defName.Contains("Droid") && !pawn.kindDef.race.defName.Contains("AndDroid"))
							|| pawn.kindDef.race.defName.Contains("RPP_Bot")
							))
					//if (false) 
					{
						//add some filter for bots and stuff? if there is such stuff
						//so it can be drafted and controlled for fighting
					}
					else
					{
						disablementReason = "ForHeroRefuseHCDesc";
					}
				}
			}

			foreach (var gizmo in __result)
			{
				if ( disablementReason.NullOrEmpty() || (!(gizmo is Command)) )  //we do not filter out non-command Gizmos, such as shield bar or psychic entropy
				{
					yield return gizmo;
				}

				//ModLog.Message("Gizmo for " + xxx.get_pawnname(__instance) + " type: " + gizmo.GetType()+ ": " + gizmo);

				if (!disablementReason.NullOrEmpty())
				{
					if (gizmo is Verse.Command_VerbTarget) { //weapon icons 
						gizmo.Disable(disablementReason.Translate());
						yield return gizmo; 
					}
				}
				//all other command gizmos are dropped
				
			}	
		}
	}
}
