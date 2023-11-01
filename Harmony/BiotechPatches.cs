using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

// Non-pregnancy Biotech-related patches
namespace rjw
{
    [HarmonyPatch]
    class LifeStageWorker_HumanlikeX_Notify_LifeStageStarted
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            const string lifeStageStarted = nameof(LifeStageWorker.Notify_LifeStageStarted);
            yield return AccessTools.Method(typeof(LifeStageWorker_HumanlikeChild), lifeStageStarted);
            yield return AccessTools.Method(typeof(LifeStageWorker_HumanlikeAdult), lifeStageStarted);
        }

        // Fixes errors caused by trying to spawn a biotech-only effector when a child starts a new lifestage
        // and by trying to send a biotech-only letter when a child turns three
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> FixLifeStageStartError(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            PropertyInfo thingSpawned = AccessTools.DeclaredProperty(typeof(Thing), nameof(Thing.Spawned));
            MethodInfo shouldSendNotificationsAbout = AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.ShouldSendNotificationAbout));
            bool foundAny = false;

            foreach (var instruction in instructions)
            {
                yield return instruction;

                // if (pawn.Spawned) SpawnBiotechOnlyEffector()
                // => if (pawn.Spawned && ModsConfig.IsBiotechActive) SpawnBiotechOnlyEffector()
                if (instruction.Calls(thingSpawned.GetMethod) || instruction.Calls(shouldSendNotificationsAbout))
                {
                    yield return CodeInstruction.Call(typeof(ModsConfig), "get_BiotechActive");
                    yield return new CodeInstruction(OpCodes.And);
                    foundAny = true;
                }
            }

            if (!foundAny)
            {
                ModLog.Error("Failed to patch " + original.Name);
            }
        }
    }

    [HarmonyPatch(typeof(WidgetsWork), "get_WorkBoxBGTex_AgeDisabled")]
    class WidgetsWork_WorkBoxBGTex_AgeDisabled
    {
        [HarmonyPrefix]
        static bool DontLoadMissingTexture(ref Texture2D __result)
        {
            if (!ModsConfig.BiotechActive)
            {
                __result = WidgetsWork.WorkBoxBGTex_Awful;
                return false;
            }

            return true;
        }
    }

    // If biotech is disabled, TeachOpportunity will NRE due to a null ConceptDef argument
    // Silence the error that interrupts AgeTick results by adding a null check.
    [HarmonyPatch(typeof(LessonAutoActivator), nameof(LessonAutoActivator.TeachOpportunity), new [] {typeof(ConceptDef), typeof(Thing), typeof(OpportunityType)})]
    static class Patch_LessonAutoActivator_TeachOpportunity
    {
        public static bool Prefix(ConceptDef conc)
        {
            // call the underlying method if concept is non-null
            return conc != null;
        }
    }

    // If biotech is disabled, make the Baby() method return false in the targeted method.
    [HarmonyPatch]
    static class Patch_DevelopmentalStageExtensions_Baby_Callers
    {

        static IEnumerable<MethodBase> TargetMethods()
        {
            // Make babies follow adult rules for food eligibility instead of requiring milk/baby food
            yield return AccessTools.Method(typeof(FoodUtility), nameof(FoodUtility.WillEat_NewTemp), new [] {typeof(Pawn), typeof(ThingDef), typeof(Pawn), typeof(bool), typeof(bool)});
            // Make babies able to be fed in bed as a patient in lieu of nursing
            yield return AccessTools.Method(typeof(WorkGiver_FeedPatient), nameof(WorkGiver_FeedPatient.HasJobOnThing), new [] {typeof(Pawn), typeof(Thing), typeof(bool)});
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> BabyMethodShouldReturnFalse(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            bool foundAny = false;
            MethodInfo developmentalStageIsBabyMethod = AccessTools.Method(typeof(DevelopmentalStageExtensions), nameof(DevelopmentalStageExtensions.Baby));
            List<CodeInstruction> codeInstructions = instructions.ToList();
            foreach (CodeInstruction instruction in codeInstructions)
            {
                yield return instruction;
                if (instruction.Calls(developmentalStageIsBabyMethod))
                {
                    foundAny = true;
                    if (!ModsConfig.BiotechActive)
                    {
                        // After calling the Baby() method, AND the result with 0, to make it as if DevelopmentalStageExtensions::Baby() returned false.
                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                        yield return new CodeInstruction(OpCodes.And);
                    }
                }
            }
            if (!foundAny)
            {
                ModLog.Error("Failed to patch " + original.Name);
            }
        }
    }

	// Check for fertile penis instead of gender when compiling list of pawns that could fertilize ovum
	[HarmonyPatch]
	static class Patch_HumanOvum_CanFertilizeReport
	{
		static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(HumanOvum), "CanFertilizeReport", new[] { typeof(Pawn) });
		}

		static bool hasPenis(Pawn p)
		{
			// need to use current hediffSet, else has_penis_fertile reads data from save (an issue if a pawn lost their penis since then)
			List<Hediff> parts = p?.health?.hediffSet?.hediffs;
			return Genital_Helper.has_penis_fertile(p, parts);
		}

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> CheckForPenisInsteadOfGender(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			MethodInfo checkForPenis = AccessTools.Method(typeof(Patch_HumanOvum_CanFertilizeReport), nameof(Patch_HumanOvum_CanFertilizeReport.hasPenis));
			bool foundGenderCheck = false;

			foreach (CodeInstruction ci in instructions)
			{
				string cis = ci.ToString();
				if (cis.Contains("Verse.Gender Verse.Pawn::gender"))
				{
					// check for fertile penis instead of male gender
					foundGenderCheck = true;
					yield return new CodeInstruction(OpCodes.Call, checkForPenis);
				}
				else
				{
					yield return ci;
				}
			}
			if (!foundGenderCheck)
			{
				ModLog.Warning("Failed to patch " + original.Name);
			}
		}
	}

	// If babies are malnourished, mark them as needing urgent medical rest, despite being permanently downed.
	[HarmonyPatch(typeof(HealthAIUtility), nameof(HealthAIUtility.ShouldSeekMedicalRestUrgent), new [] {typeof(Pawn)})]
    static class Patch_HealthAIUtility_ShouldSeekMedicalRestUrgent
    {
        public static bool Prefix(Pawn pawn, ref bool __result)
        {
            if (!ModsConfig.BiotechActive && pawn.DevelopmentalStage.Baby())
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
