using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// count (rjw) bodypart with missing rjw part as missing to prevent damage
	/// </summary>
	[HarmonyPatch(typeof(HediffSet), "PartIsMissing")]
	internal static class PATCH_HediffSet_GetRandomNotMissingPart
	{
		[HarmonyPostfix]
		private static void rjwPartIsMissing(ref bool __result, ref HediffSet __instance, BodyPartRecord part)
		{
			if (__result == true)
				return;

			var pawn = __instance.pawn;

			if (__instance.pawn != null)
			{
				if (part.def == xxx.genitalsDef && pawn.GetGenitalsList().NullOrEmpty())
					__result = true;
				else if (part.def == xxx.breastsDef && pawn.GetBreastList().NullOrEmpty())
					__result = true;
				else if (part.def == xxx.anusDef && pawn.GetAnusList().NullOrEmpty())
					__result = true;
			}
		}
	}

	/// <summary>
	/// count (rjw) bodypart with missing rjw part as missing to prevent damage
	/// </summary>
	[HarmonyPatch(typeof(HediffSet), "GetNotMissingParts")]
	internal static class PATCH_HediffSet_GetRandomNotMissingPart
	{
		[HarmonyPostfix]
		private static IEnumerable<BodyPartRecord> rjwGetNotMissingParts(IEnumerable<BodyPartRecord> __result, Pawn pawn)
		{
			//IEnumerable<BodyPartRecord> t;
			foreach (var bpr in __result)
			{
				if (bpr.def == xxx.genitalsDef && pawn.GetGenitalsList().NullOrEmpty())
					continue;
				else if (bpr.def == xxx.breastsDef && pawn.GetBreastList().NullOrEmpty())
					continue;
				else if (bpr.def == xxx.anusDef && pawn.GetAnusList().NullOrEmpty())
					continue;

				yield return bpr;
			}
		}
	}

	/// <summary>
	/// count rjw bodypart with missing rjw part as missing to prevent damage
	/// </summary>
	[HarmonyPatch(typeof(DamageWorker.DamageResult), "AddPart")]
	internal static class PATCH_DamageWorker_AddPart
	{
		[HarmonyPrefix]
		private static bool Disable_damage(Thing hitThing, BodyPartRecord part)
		{
			Pawn pawn = hitThing as Pawn;

			if (pawn != null)
			{
				if (part.def == xxx.genitalsDef && pawn.GetGenitalsList().NullOrEmpty())
					return false;
				if (part.def == xxx.breastsDef && pawn.GetBreastList().NullOrEmpty())
					return false;
				if (part.def == xxx.anusDef && pawn.GetAnusList().NullOrEmpty())
					return false;
			}

			return true;
		}
	}
}
