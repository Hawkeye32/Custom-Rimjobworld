using System;
using RimWorld;
using Verse;
using HarmonyLib;

namespace rjw
{
	[HarmonyPatch(typeof(CompAbilityEffect_WordOfLove), "ValidateTarget")]
	internal static class PATCH_CompAbilityEffect_WordOfLove_ValidateTarget
	{
		[HarmonyPrefix]
		static bool GenderChecks(ref LocalTargetInfo target, LocalTargetInfo ___selectedTarget, ref bool __result)
		{
			Pawn pawn = ___selectedTarget.Pawn;
			Pawn pawn2 = target.Pawn;
			if (pawn != pawn2 && pawn != null && pawn2 != null)
			{
				__result = !xxx.is_asexual(pawn) && (xxx.is_bisexual(pawn) || xxx.is_pansexual(pawn) || (xxx.is_heterosexual(pawn) && pawn.gender != pawn2.gender) || (xxx.is_homosexual(pawn) && pawn.gender == pawn2.gender));
				if (__result == false)
				{ Messages.Message("AbilityCantApplyWrongAttractionGender".Translate(pawn, pawn2), pawn, MessageTypeDefOf.RejectInput, false); }
				return false;
			}
			return true;
		}
	}
}
