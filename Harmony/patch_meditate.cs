using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// disable meditation effects for nymphs (i.e meditation on throne)
	/// </summary>
	[HarmonyPatch(typeof(JobDriver_Meditate), "MeditationTick")]
	internal static class PATCH_JobDriver_Meditate_MeditationTick
	{
		[HarmonyPrefix]
		private static bool Disable_For_Nymph(JobDriver_Meditate __instance)
		{
			Pawn pawn = __instance.pawn;

			if (xxx.is_nympho(pawn))
			{
				//ModLog.Message("JobGiver_Meditate::MeditationTick for nymph " + xxx.get_pawnname(pawn) + " __instance " + __instance);
				CompProperties_MeditationFocus t0 = __instance.Focus.Thing?.def?.comps?.Find(x => x is CompProperties_MeditationFocus) as CompProperties_MeditationFocus;
				if (t0 != null)
					if (t0.focusTypes.Contains(xxx.SexMeditationFocus))
						return true;

				return false;
			}

			//ModLog.Message("JobGiver_Meditate::MeditationTick pass");
			return true;
		}
	}

	[HarmonyPatch(typeof(JobGiver_Meditate), "TryGiveJob")]
	internal static class PATCH_JobGiver_Meditate_TryGiveJob
	{
		[HarmonyPostfix]
		public static void Disable_For_Nymph(ref Job __result, Pawn pawn)
		{
			if (__result != null)
				if (xxx.is_nympho(pawn))
				{
					//ModLog.Message("JobGiver_Meditate::TryGiveJob for nymph " + xxx.get_pawnname(pawn) + " job " + __result);
					CompProperties_MeditationFocus t1 = __result.targetA.Thing?.def.comps.Find(x => x is CompProperties_MeditationFocus) as CompProperties_MeditationFocus;
					CompProperties_MeditationFocus t2 = __result.targetB.Thing?.def.comps.Find(x => x is CompProperties_MeditationFocus) as CompProperties_MeditationFocus;
					CompProperties_MeditationFocus t3 = __result.targetC.Thing?.def.comps.Find(x => x is CompProperties_MeditationFocus) as CompProperties_MeditationFocus;
					//ModLog.Message("JobGiver_Meditate::TryGiveJob targetA " + t1);
					//ModLog.Message("JobGiver_Meditate::TryGiveJob targetB " + t2);
					//ModLog.Message("JobGiver_Meditate::TryGiveJob targetC " + t3);
					if (t1 != null)
						if (t1.focusTypes.Contains(xxx.SexMeditationFocus))
							return;
					if (t2 != null)
						if (t2.focusTypes.Contains(xxx.SexMeditationFocus))
							return;
					if (t3 != null)
						if (t3.focusTypes.Contains(xxx.SexMeditationFocus))
							return;

					//ModLog.Message("JobGiver_Meditate::Disable_For_Nymph no valid targets fail job");
					__result = null;
					//ModLog.Message("JobGiver_Meditate::Disable_For_Nymph " + xxx.get_pawnname(pawn) + " job " + __result);
				}
		}
	}
}
