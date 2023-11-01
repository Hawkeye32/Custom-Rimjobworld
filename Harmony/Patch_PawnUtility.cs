using HarmonyLib;
using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// patches PawnUtility to fix humanlike childrens post birth
/// </summary>
namespace rjw
{
	[HarmonyPatch(typeof(PawnUtility), "TrySpawnHatchedOrBornPawn")]
	static class Patch_PawnUtility
	{
		//[HarmonyPrefix]
		//static void prefix_TrySpawnHatchedOrBornPawn(ref Pawn pawn, Thing motherOrEgg)
		//{
		//	//ModLog.Message("prefix_TrySpawnHatchedOrBornPawn::");
		//	//ModLog.Message(" " + __result);
		//	//ModLog.Message(" " + xxx.get_pawnname(pawn));
		//	//ModLog.Message(" " + xxx.get_pawnname(motherOrEgg as Pawn));
		//	//ModLog.Message(" " + RJWPregnancySettings.humanlike_pregnancy_enabled);
		//	//ModLog.Message(" " + xxx.is_human(pawn));
		//	//ModLog.Message(" " + !xxx.is_mechanoid(pawn));
		//	//string last_name = NameTriple.FromString(pawn.Name.ToStringFull).Last;
		//	//ModLog.Message(" prefix_TrySpawnHatchedOrBornPawn Baby surname will be " + last_name);
		//}

		//resets pawn/doesn't work for some races with C# constructors
		[HarmonyPostfix]
		static void Process_Child_HatchOrBirth(ref bool __result, ref Pawn pawn, Thing motherOrEgg)
		{
			//ModLog.Message("postfix_TrySpawnHatchedOrBornPawn::");
			//ModLog.Message(" " + __result);
			//ModLog.Message(" " + xxx.get_pawnname(pawn));
			//ModLog.Message(" " + xxx.get_pawnname(motherOrEgg as Pawn));
			//ModLog.Message(" " + RJWPregnancySettings.humanlike_pregnancy_enabled);
			//ModLog.Message(" " + xxx.is_human(pawn));
			//ModLog.Message(" " + !xxx.is_mechanoid(pawn));
			//string last_name = NameTriple.FromString(pawn.Name.ToStringFull).Last;
			//ModLog.Message(" postfix_TrySpawnHatchedOrBornPawn 1 Baby surname will be " + last_name);
			//var skin_whiteness = pawn.story.melanin;
			//var last_name1 = pawn.story.birthLastName;
			if (__result
				&& RJWPregnancySettings.humanlike_pregnancy_enabled
				&& xxx.is_human(pawn)
				&& !xxx.is_mechanoid(pawn))
			{
				if ((motherOrEgg is Pawn && (motherOrEgg as Pawn).IsPregnant())	//vanilla + rjw pregnancy
					|| (motherOrEgg.TryGetComp<CompHatcher>() != null))			//vanilla + rjw eggnancy
				{
					changestory(pawn);
					removeimplants(pawn);
					removeskills(pawn);
					grow(pawn);
				}
			}
			//ModLog.Message(" postfix_TrySpawnHatchedOrBornPawn 2 Baby surname will be " + NameTriple.FromString(pawn.Name.ToStringFull).Last);
			//pawn.story.melanin = skin_whiteness;
			//pawn.story.birthLastName = last_name1;
		}

		static void removeimplants(Pawn pawn)
		{
			if (!pawn.health.hediffSet.hediffs.NullOrEmpty())
			{
				var hdlist = pawn.health.hediffSet.hediffs.ToList();
				foreach (Hediff hd in hdlist)
				{
					if (hd == null) continue;

					// remove DeathAcidifier
					//if (hd.def.defName == "DeathAcidifier")
					//{
					//	pawn.health.RemoveHediff(hd);
					//	continue;
					//}

					// remove implants
					if (hd is Hediff_Implant)
					{
						var part = hd.Part;
						pawn.health.RestorePart(part);
					}

					// remove immortality
					if (xxx.ImmortalsIsActive && hd.def == xxx.IH_Immortal)
					{
						pawn.health.RemoveHediff(hd);
					}
				}
			}
		}

		//resets/doest work for some races
		static void changestory(Pawn pawn)
		{
			if (!xxx.RimWorldChildrenIsActive)
			{
				pawn.story.Childhood = null;
				pawn.story.Adulthood = null;

				// set child to tribal
				try
				{
					if (!ModsConfig.BiotechActive)
						pawn.story.Childhood = DefDatabase<BackstoryDef>.AllDefsListForReading.FirstOrDefault(x => x.defName.Contains("rjw_childT"));
					else
						pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("Newborn79");
				}
				catch (Exception e)
				{
					ModLog.Warning(e.ToString());
				}
			}
		}

		static void grow(Pawn pawn)
		{
			if (!xxx.RimWorldChildrenIsActive && !ModsConfig.BiotechActive)
			{
				// add growing 
				if (!pawn.Dead)
				{
					pawn.health.AddHediff(xxx.RJW_BabyState, null, null);
					Hediff_SimpleBaby pawnstate = (Hediff_SimpleBaby)pawn.health.hediffSet.GetFirstHediffOfDef(xxx.RJW_BabyState);
					if (pawnstate != null)
					{
						pawnstate.GrowUpTo(0, true);
					}
				}
			}
		}

		static void removeskills(Pawn pawn)
		{
			// remove skills
			foreach (var skill in pawn.skills?.skills)
				skill.Level = 0;
		}
	}
}