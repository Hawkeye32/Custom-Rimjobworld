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
	[HarmonyPatch(typeof(PawnRenderer), "DrawBodyApparel")]
	static class Patch_PawnRenderer_DrawBodyApparel
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
		[HarmonyPrefix]
		static bool prefix_DrawBodyApparel(ref Pawn pawn)
		{
			if (pawn.jobs?.curDriver is JobDriver_Sex)
			{
				ModLog.Message(pawn.Name+ " sex");
				SexUtility.DrawNude(pawn);
				return false;
			}
			return true;
		}
	}
}