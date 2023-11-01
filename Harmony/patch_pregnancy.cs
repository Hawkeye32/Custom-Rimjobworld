using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using Multiplayer.API;
using UnityEngine;
using Verse;
using RimWorld;
using rjw.Modules.Testing;

using Seeded = rjw.Modules.Rand.Seeded;

namespace rjw
{
	[HarmonyPatch(typeof(Hediff_Pregnant), "DoBirthSpawn")]
	internal static class PATCH_Hediff_Pregnant_DoBirthSpawn
	{
		/// <summary>
		/// This one overrides vanilla pregnancy hediff behavior.
		/// 0 - try to find suitable father for debug pregnancy
		/// 1st part if character pregnant and rjw pregnancies enabled - creates rjw pregnancy and instantly births it instead of vanilla
		/// 2nd part if character pregnant with rjw pregnancy - birth it
		/// 3rd part - debug - create rjw/vanila pregnancy and birth it
		/// </summary>
		/// <param name="mother"></param>
		/// <param name="father"></param>
		/// <returns></returns>
		[HarmonyPrefix]
		[SyncMethod]
		private static bool on_begin_DoBirthSpawn(ref Pawn mother, ref Pawn father)
		{
			//--Log.Message("patches_pregnancy::PATCH_Hediff_Pregnant::DoBirthSpawn() called");

			if (mother == null)
			{
				ModLog.Error("Hediff_Pregnant::DoBirthSpawn() - no mother defined -> exit");
				return false;
			}

			//CompEggLayer compEggLayer = mother.TryGetComp<CompEggLayer>();
			//if (compEggLayer != null)
			//{
			//	ProcessVanillaEggPregnancy(mother, father);
			//	return false;
			//}

			//vanilla debug?
			if (mother.gender == Gender.Male)
			{
				ModLog.Error("Hediff_Pregnant::DoBirthSpawn() - mother is male -> exit");
				return false;
			}

			// get a reference to the hediff we are applying
			//do birth for vanilla pregnancy Hediff
			//if using rjw pregnancies - add RJW pregnancy Hediff and birth it instead
			Hediff_Pregnant self = (Hediff_Pregnant)mother.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant);
			if (self != null)
			{
				return ProcessVanillaPregnancy(self, mother, father);
			}

			// do birth for existing RJW pregnancies
			if (ProcessRJWPregnancy(mother, father))
			{
				return false;
			}

			return ProcessDebugPregnancy(mother, father);
		}

		private static bool ProcessVanillaEggPregnancy(Pawn mother, Pawn father)
		{
			CompEggLayer compEggLayer = mother.TryGetComp<CompEggLayer>();
			if (compEggLayer != null)
			{
				if (!compEggLayer.FullyFertilized)
				{
					if (father == null)
						compEggLayer.Fertilize(mother);
					else
						compEggLayer.Fertilize(father);

					compEggLayer.ProduceEgg();
				}
			}

			CompHatcher compHatcher = mother.TryGetComp<CompHatcher>();
			if (compHatcher != null)
			{
				compHatcher.Hatch();
			} 

			ModLog.Message("patches_pregnancy::PATCH_Hediff_Pregnant::DoBirthSpawn():ProcessVanillaEggPregnancy birthing:" + xxx.get_pawnname(mother));
			return true;
		}


		private static bool ProcessVanillaPregnancy(Hediff_Pregnant pregnancy, Pawn mother, Pawn father)
		{
			void CreateAndBirth<T>() where T : Hediff_BasePregnancy
			{
				T hediff = Hediff_BasePregnancy.Create<T>(mother, father);
				hediff.GiveBirth();
				if (pregnancy != null)
					mother.health.RemoveHediff(pregnancy);
			}

			if (father == null)
			{
				father = Hediff_BasePregnancy.Trytogetfather(ref mother);
			}

			ModLog.Message("patches_pregnancy::PATCH_Hediff_Pregnant::DoBirthSpawn():Vanilla_pregnancy birthing:" + xxx.get_pawnname(mother));
			if (RJWPregnancySettings.animal_pregnancy_enabled && ((father == null || xxx.is_animal(father)) && xxx.is_animal(mother)))
			{
				//RJW Bestial pregnancy animal-animal
				ModLog.Message(" override as Bestial birthing(animal-animal): Father-" + xxx.get_pawnname(father) + " Mother-" + xxx.get_pawnname(mother));
				CreateAndBirth<Hediff_BestialPregnancy>();
				return false;
			}
			else if (RJWPregnancySettings.bestial_pregnancy_enabled && ((xxx.is_animal(father) && xxx.is_human(mother)) || (xxx.is_human(father) && xxx.is_animal(mother))))
			{
				//RJW Bestial pregnancy human-animal
				ModLog.Message(" override as Bestial birthing(human-animal): Father-" + xxx.get_pawnname(father) + " Mother-" + xxx.get_pawnname(mother));
				CreateAndBirth<Hediff_BestialPregnancy>();
				return false;
			}
			else if (RJWPregnancySettings.humanlike_pregnancy_enabled && (xxx.is_human(father) && xxx.is_human(mother)))
			{
				//RJW Humanlike pregnancy
				ModLog.Message(" override as Humanlike birthing: Father-" + xxx.get_pawnname(father) + " Mother-" + xxx.get_pawnname(mother));
				CreateAndBirth<Hediff_HumanlikePregnancy>();
				return false;
			}
			else
			{
				ModLog.Warning("Hediff_Pregnant::DoBirthSpawn() - checks failed, vanilla pregnancy birth");
				ModLog.Warning("Hediff_Pregnant::DoBirthSpawn(): Father-" + xxx.get_pawnname(father) + " Mother-" + xxx.get_pawnname(mother));
				//vanilla pregnancy code, no effects on rjw

				return true;
			}
		}

		private static bool ProcessRJWPregnancy(Pawn mother, Pawn father)
		{
			var p = PregnancyHelper.GetPregnancies(mother);
			if (p.NullOrEmpty())
			{
				return false;
			}

			var birth = false; 
			foreach (var x in p)
			{
				if (x is Hediff_BasePregnancy)
				{
					var preg = x as Hediff_BasePregnancy;
					ModLog.Message($"patches_pregnancy::{preg.GetType().Name}::DoBirthSpawn() birthing:" + xxx.get_pawnname(mother));
					preg.GiveBirth();
					birth = true;
				}
			}

			return birth;
		}

		private static bool ProcessDebugPregnancy(Pawn mother, Pawn father)
		{
			void CreateAndBirth<T>() where T : Hediff_BasePregnancy
			{
				T hediff = Hediff_BasePregnancy.Create<T>(mother, father);
				hediff.GiveBirth();
			}
			//CreateAndBirth<Hediff_HumanlikePregnancy>();
			//CreateAndBirth<Hediff_BestialPregnancy>();
			//CreateAndBirth<Hediff_MechanoidPregnancy>();
			//return false;

			//debug, add RJW pregnancy and birth it
			ModLog.Message("patches_pregnancy::PATCH_Hediff_Pregnant::DoBirthSpawn():Debug_pregnancy birthing:" + xxx.get_pawnname(mother));
			if (father == null)
			{
				father = Hediff_BasePregnancy.Trytogetfather(ref mother);

				if (RJWPregnancySettings.bestial_pregnancy_enabled && ((xxx.is_animal(father) || xxx.is_animal(mother)))
					|| (xxx.is_animal(mother) && RJWPregnancySettings.animal_pregnancy_enabled))
				{
					//RJW Bestial pregnancy
					ModLog.Message(" override as Bestial birthing, mother: " + xxx.get_pawnname(mother));
					CreateAndBirth<Hediff_BestialPregnancy>();
				}
				else if (RJWPregnancySettings.humanlike_pregnancy_enabled && ((father == null || xxx.is_human(father)) && xxx.is_human(mother)))
				{
					//RJW Humanlike pregnancy
					ModLog.Message(" override as Humanlike birthing, mother: " + xxx.get_pawnname(mother));
					CreateAndBirth<Hediff_HumanlikePregnancy>();
				}
				else
				{
					ModLog.Warning("Hediff_Pregnant::DoBirthSpawn() - debug vanilla pregnancy birth");
					return true;
				}
			}
			return false;
		}
	}


	[HarmonyPatch(typeof(Hediff_Pregnant), "Tick")]
	class PATCH_Hediff_Pregnant_Tick
	{
		[HarmonyPrefix]
		static bool abort_on_missing_genitals(Hediff_Pregnant __instance)
		{
			if (__instance.pawn.IsHashIntervalTick(1000))
			{
				if (!Genital_Helper.has_vagina(__instance.pawn))
				{
					__instance.pawn.health.RemoveHediff(__instance);
				}
			}
			return true;
		}
	}

	// Enables pregnancy approach for same-gender couples as long as they have the right genitals,
	// prevents the UI from bugging out if the first pawn is genderless, and ensures that the
	// "Pawns are sterile" report is not misapplied in case of mpreg.
	[HarmonyPatch(typeof(PregnancyUtility), nameof(PregnancyUtility.CanEverProduceChild))]
	class PregnancyUtility_CanEverProduceChild
	{
		static Type thisType = typeof(PregnancyUtility_CanEverProduceChild);

		static readonly MethodInfo SterileMethod =
			AccessTools.Method(typeof(Pawn), nameof(Pawn.Sterile));
		static readonly MethodInfo GetPregnancyHediffMethod =
			AccessTools.Method(typeof(PregnancyUtility), nameof(PregnancyUtility.GetPregnancyHediff));

		// TODO from Feb-2023: Uncomment the second version once `rjw-race-support` gets
		// its shit together and patches their gender check.
		static bool BypassGenderChecks => true;
		// static bool BypassGenderChecks =>
		// 	!ModsConfig.IsActive("erdelf.HumanoidAlienRaces") ||
		// 	!ModsConfig.IsActive("ASMR.RJW.RaceSupport");

		[HarmonyPrefix]
		static bool CheckPlumbing(Pawn first, Pawn second, ref AcceptanceReport __result)
		{
			if (first.Dead || second.Dead)
			{
				return true;
			}
			
			var plumbingReport = GetPlumbingReport(first, second);
			if (plumbingReport.Accepted)
			{
				return true;
			}

			__result = plumbingReport;
			return false;
		}

		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> SkipGenderCheckAndMakeSterilityCheckNotSexist(
			IEnumerable<CodeInstruction> instructionEnumberable, ILGenerator generator)
		{
			var instructions = instructionEnumberable.ToList();

			bool foundGenderCheck = false;
			bool foundSupposedLocalFemaleAssignment = false;
			bool foundFemaleSterilityCheck = false;
			bool foundMaleSterilityCheck = false;

			// Break this method up into blocks, either where a value is returned or where
			// we set a local variable.
			static bool findBlock(CodeInstruction il) => il.opcode == OpCodes.Ret || il.IsStloc();
			(var curIns, var nextIns) = instructions.SplitAfter(findBlock);

			while(curIns.Count > 0)
			{
				if (curIns.Any((il) => il.Is(OpCodes.Ldstr, "PawnsHaveSameGender")))
				{
					// Processing the same-gender check.

					// We want to bypass this check, unless the Alien Races mod is active.
					// RJW's compatibility mod can instead patch its `GenderReproductionCheck`
					// method to alter its behavior.
					if (!BypassGenderChecks)
					{
						foundGenderCheck = true;
						goto yieldOriginal;
					}

					// Due to a quirk in how the optimizer handles branches, we must insert our
					// bypass AFTER the conditional, otherwise the compiler just removes it.
					// Isn't it great when documentation doesn't mention stuff like this?
					(var beforeBr, var afterBr) = curIns.SplitAfter((il) => il.Branches(out _));

					// Verify that we found the branch.
					if (afterBr.Count is 0) goto yieldOriginal;

					foreach (var il in beforeBr) yield return il;
					if (beforeBr.LastOrDefault() is { } br && br.Branches(out var bypassLabel))
					{
						foundGenderCheck = true;

						// Adds `&& false` after the conditional.
						yield return new(OpCodes.Ldc_I4_0);
						yield return new(OpCodes.Brfalse_S, bypassLabel);
					}
					foreach (var il in afterBr) yield return il;
					goto setupNextLoop;
				}
				else if (curIns.Any((il) => il.IsStlocOf(1)))
				{
					// Processing the initialization of `pawn2` into local #1:
					// Pawn pawn2 = (first.gender == Gender.Female) ? first : second;

					// This needs to be changed to just select the other pawn, the one that is
					// not the assumed father.  It is possible that Alien Races has already
					// patched this, so we'll need to take that into consideration.

					// Locate the branch instruction.
					(var beforeBr, var afterBr) = curIns.SplitAfter((il) => il.Branches(out _));

					// Verify that we found the branch.
					if (afterBr.Count is 0) goto yieldOriginal;
					if (beforeBr.LastOrDefault() is not { } br) goto yieldOriginal;
					if (!br.Branches(out var label)) goto yieldOriginal;

					foundSupposedLocalFemaleAssignment = true;

					// Rewrite to:
					// Pawn pawn2 = (first != pawn) ? first : second;

					// Make sure the labels are preserved.
					yield return curIns[0].Replace(new CodeInstruction(OpCodes.Ldarg_0));
					yield return new(OpCodes.Ldloc_0);
					yield return new(OpCodes.Bne_Un_S, label);
					// The remaining instructions handle the selection of the pawn, so we
					// can spit those out without changes.
					foreach (var il in afterBr) yield return il;
					goto setupNextLoop;
				}
				else if (curIns.Any((il) => il.IsStlocOf(6)))
				{
					// Processing the initialization of `flag5` into local #6:
					// bool flag5 = pawn2.Sterile(false) && PregnancyUtility.GetPregnancyHediff(pawn2) == null;

					// This needs to be changed to call `PregnancyHelper.GetPregnancy`.

					// Look for the call `pawn2.Sterile(false)`.
					(var callToSterile, var remainder) = curIns.CutMatchingSequence(
						(il) => il.opcode == OpCodes.Ldloc_1,
						(il) => il.opcode == OpCodes.Ldc_I4_0,
						(il) => il.Calls(SterileMethod),
						(il) => il.opcode == OpCodes.Brfalse_S
					);

					// Verify we found this sequence and abort if not.
					if (callToSterile.Count == 0) goto yieldOriginal;

					// Look for the call `PregnancyUtility.GetPregnancyHediff(pawn2)`.
					(var callToPregnancy, remainder) = remainder.CutMatchingSequence(
						(il) => il.opcode == OpCodes.Ldloc_1,
						(il) => il.Calls(GetPregnancyHediffMethod)
					);

					// Verify we found this sequence and abort if not.
					if (callToPregnancy.Count == 0) goto yieldOriginal;

					foundFemaleSterilityCheck = true;

					// Rewrite this to call `PregnancyHelper.GetPregnancy` instead.
					// We're keeping the call to `Sterile`.
					foreach (var il in callToSterile) yield return il;
					// Setup and call.
					yield return new(OpCodes.Ldloc_1);
					yield return CodeInstruction.Call(typeof(PregnancyHelper), nameof(PregnancyHelper.GetPregnancy));
					// The remaining instructions just check for `null`, so should be good.
					foreach (var il in remainder) yield return il;
					goto setupNextLoop;
				}
				else if (curIns.Any((il) => il.IsStlocOf(7)))
				{
					// Processing the initialization of `flag6` into local #7:
					// bool flag6 = pawn.Sterile(false);

					// We need to add a call to `PregnancyHelper.GetPregnancy` just like for
					// the female, since this pawn may actually be a futa or something.

					// Look for the call `pawn.Sterile(false)`.
					(var callToSterile, var remainder) = curIns.CutMatchingSequence(
						(il) => il.opcode == OpCodes.Ldloc_0,
						(il) => il.opcode == OpCodes.Ldc_I4_0,
						(il) => il.Calls(SterileMethod)
					);

					// Verify we found this sequence and abort if not.
					if (callToSterile.Count == 0) goto yieldOriginal;

					// There should only be 1 instruction left, which sets the variable.
					if (remainder.Count != 1) goto yieldOriginal;

					foundMaleSterilityCheck = true;

					// We need to setup a label pointing to the instruction that will set
					// `false` if the call to `pawn.Sterile(false)` returns false.
					var labelToFalse = generator.DefineLabel();
					var IL_SetToFalse = new CodeInstruction(OpCodes.Ldc_I4_0);
					IL_SetToFalse.labels.Add(labelToFalse);

					// We also need a label on the instruction that sets the variable.
					var labelToSet = generator.DefineLabel();
					remainder[0].labels.Add(labelToSet);

					foreach (var il in callToSterile) yield return il;
					// If the return value was false, skip to `IL_SetToFalse`.
					yield return new(OpCodes.Brfalse_S, labelToFalse);
					// Now, write `PregnancyHelper.GetPregnancy(pawn) == null`.
					yield return new(OpCodes.Ldloc_0);
					yield return CodeInstruction.Call(typeof(PregnancyHelper), nameof(PregnancyHelper.GetPregnancy));
					yield return new(OpCodes.Ldnull);
					yield return new(OpCodes.Ceq);
					// If it was `true`, skip over the next instruction so that is set to the variable.
					yield return new(OpCodes.Br_S, labelToSet);
					// Otherwise, we'll set `false`.
					yield return IL_SetToFalse;
					// The remaining instruction just sets the variable to whatever was last pushed
					// to the stack.
					foreach (var il in remainder) yield return il;
					goto setupNextLoop;
				}

			yieldOriginal:
				foreach (var il in curIns)
					yield return il;
			
			setupNextLoop:
				(curIns, nextIns) = nextIns.SplitAfter(findBlock);
			}

			if (!foundGenderCheck)
				ModLog.Error("Failed to patch PregnancyUtility.CanEverProduceChild: Could not find gender check");
			if (!foundSupposedLocalFemaleAssignment)
				ModLog.Error("Error when patching PregnancyUtility.CanEverProduceChild: Could not find assignment to local variable `pawn2`");
			if (!foundFemaleSterilityCheck)
				ModLog.Error("Error when patching PregnancyUtility.CanEverProduceChild: Could not find sterility check on local variable `pawn2`");
			if (!foundMaleSterilityCheck)
				ModLog.Error("Error when patching PregnancyUtility.CanEverProduceChild: Could not find sterility check on local variable `pawn`");
		}

		static AcceptanceReport GetPlumbingReport(Pawn first, Pawn second)
		{
			bool firstHasPenis = Genital_Helper.has_penis_fertile(first);
			bool firstHasVagina = Genital_Helper.has_vagina(first);

			if (!firstHasPenis && !firstHasVagina)
			{
				return "PawnLacksFunctionalGenitals".Translate(first.Named("PAWN")).Resolve();
			}

			bool secondHasPenis = Genital_Helper.has_penis_fertile(second);
			bool secondHasVagina = Genital_Helper.has_vagina(second);

			if (!secondHasPenis && !secondHasVagina)
			{
				return "PawnLacksFunctionalGenitals".Translate(second.Named("PAWN")).Resolve();
			}
			if ((firstHasPenis && secondHasVagina) || (firstHasVagina && secondHasPenis))
			{
				return true;
			}
			if (firstHasPenis && !secondHasVagina)
			{
				return "PawnLacksVagina".Translate(second.Named("PAWN")).Resolve();
			}
			if (firstHasVagina && !secondHasPenis)
			{
				return "PawnLacksFunctionalPenis".Translate(second.Named("PAWN")).Resolve();
			}
			return true;
		}

		/// <summary>
		/// <para>Tests various pairings to make sure that that the patch is operating
		/// correctly.  The results of this test may be affected by other mods,
		/// so keep that in mind.</para>
		/// <para>This doesn't go into too much depth, mostly just checking colonists
		/// and a few animals.  It does not currently test integration with Alien
		/// Races or anything.  RJW's Alien Races compatibility mod should provide
		/// its own auto-test for that.</para>
		/// </summary>
		[DebugOutput("RJW Auto-Tests", onlyWhenPlaying = true)]
		public static void TestCanEverProduceChild()
		{
			var logger = Modules.Shared.Logs.LogManager.GetLogger("TestCanEverProduceChild");
			var sb = new StringBuilder();
			var passedColor = GenColor.FromHex("66CC00");
			var failedColor = GenColor.FromHex("FF3300");

			var maleColonistRequest = new PawnGenerationRequest(
				kind: PawnKindDefOf.Colonist,
				fixedGender: Gender.Male,
				excludeBiologicalAgeRange: new(0f, 18f)
			).RequestDefaults();

			var femaleColonistRequest = new PawnGenerationRequest(
				kind: PawnKindDefOf.Colonist,
				fixedGender: Gender.Female,
				allowPregnant: false,
				excludeBiologicalAgeRange: new(0f, 18f)
			).RequestDefaults();

			var maleThrumboRequest = new PawnGenerationRequest(
				kind: PawnKindDefOf.Thrumbo,
				fixedGender: Gender.Male,
				excludeBiologicalAgeRange: new(0f, 8f)
			).RequestDefaults();

			var femaleThrumboRequest = new PawnGenerationRequest(
				kind: PawnKindDefOf.Thrumbo,
				fixedGender: Gender.Female,
				allowPregnant: false,
				excludeBiologicalAgeRange: new(0f, 8f)
			).RequestDefaults();

			// Checks regarding basic sex-parts.

			Trial("Male Colonist + Female Colonist", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				TryToReport(male, female, true);
			});

			Trial("Male Colonist + Male Colonist", () =>
			{
				var male1 = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var male2 = TestHelper.GenerateSeededPawn(maleColonistRequest);
				TryToReport(male1, male2, false);
			});

			Trial("Female Colonist + Female Colonist", () =>
			{
				var female1 = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				var female2 = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				TryToReport(female1, female2, false);
			});

			Trial("Futa Colonist + Female Colonist", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				if (TestHelper.MakeIntoFuta(male))
					TryToReport(male, female, true);
				else
				{
					var text = "Failed to Create Futa".Colorize(failedColor);
					sb.AppendLineTagged($"  {text}");
				}
			});

			Trial("Male Colonist + Futa Colonist", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				if (TestHelper.MakeIntoFuta(female))
					TryToReport(male, female, true);
				else
				{
					var text = "Failed to Create Futa".Colorize(failedColor);
					sb.AppendLineTagged($"  {text}");
				}
			});

			Trial("Futa Colonist + Futa Colonist", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				if (TestHelper.MakeIntoFuta(male) && TestHelper.MakeIntoFuta(female))
					TryToReport(male, female, true);
				else
				{
					var text = "Failed to Create Futa".Colorize(failedColor);
					sb.AppendLineTagged($"  {text}");
				}
			});

			// Tests regarding pregnancy.

			Trial("Male Colonist + Pregnant Female Colonist (Vanilla)", () =>
			{
				if (!ModsConfig.BiotechActive)
				{
					sb.AppendLine("  Biotech Not Active");
					return;
				}

				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				if (VanillaHumanPregnancy(female, male) is { } hediff)
				{
					female.health.AddHediff(hediff);
					TryToReport(male, female, true);
				}
				else
				{
					var text = "Failed to Impregnate".Colorize(failedColor);
					sb.AppendLineTagged($"  {text}");
				}
			});

			Trial("Male Colonist + Pregnant Female Colonist (RJW)", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				PregnancyHelper.AddPregnancyHediff(female, male);
				TryToReport(male, female, true);
			});

			Trial("Pregnant Male Colonist + Female Colonist (Vanilla)", () =>
			{
				if (!ModsConfig.BiotechActive)
				{
					sb.AppendLine("  Biotech Not Active");
					return;
				}

				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				if (VanillaHumanPregnancy(male, female) is { } hediff)
				{
					male.health.AddHediff(hediff);
					TryToReport(male, female, true);
				}
				else
				{
					var text = "Failed to Impregnate".Colorize(failedColor);
					sb.AppendLineTagged($"  {text}");
				}
			});

			Trial("Pregnant Male Colonist + Female Colonist (RJW)", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				PregnancyHelper.AddPregnancyHediff(male, female);
				TryToReport(male, female, true);
			});

			// Tests regarding artificial genitalia and sterilization.

			Trial("Sterilized Male Colonist + Female Colonist", () =>
			{
				if (!ModsConfig.BiotechActive)
				{
					sb.AppendLine("  Biotech Not Active");
					return;
				}

				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				male.health.AddHediff(HediffDefOf.Sterilized);
				TryToReport(male, female, false);
			});

			Trial("Artificially Male Colonist + Female Colonist", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				if (TestHelper.GiveArtificialGenitals(male))
					TryToReport(male, female, false);
				else
				{
					var text = "Failed to Render Artifical".Colorize(failedColor);
					sb.AppendLineTagged($"  {text}");
				}
			});

			Trial("Male Colonist + Sterilized Female Colonist", () =>
			{
				if (!ModsConfig.BiotechActive)
				{
					sb.AppendLine("  Biotech Not Active");
					return;
				}

				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				female.health.AddHediff(HediffDefOf.Sterilized);
				TryToReport(male, female, false);
			});

			Trial("Male Colonist + Artificially Female Colonist", () =>
			{
				// The hydraulic vagina is still somehow fertile, btw.
				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				if (TestHelper.GiveArtificialGenitals(female))
					TryToReport(male, female, true);
				else
				{
					var text = "Failed to Render Artifical".Colorize(failedColor);
					sb.AppendLineTagged($"  {text}");
				}
			});

			// Checks to verify the gender checks were disabled properly.

			Trial("Male Colonist + Cuntboy Colonist", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				female.gender = Gender.Male;
				TryToReport(male, female, true);
			});

			Trial("Dickgirl Colonist + Female Colonist", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				male.gender = Gender.Female;
				TryToReport(male, female, true);
			});

			// Tests versus animals.

			Trial("Male Thrumbo + Female Thrumbo", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleThrumboRequest);
				var female = TestHelper.GenerateSeededPawn(femaleThrumboRequest);
				TryToReport(male, female, true);
			});

			Trial("Male Thrumbo + Female Colonist", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleThrumboRequest);
				var female = TestHelper.GenerateSeededPawn(femaleColonistRequest);
				TryToReport(male, female, true);
			});

			Trial("Male Colonist + Female Thrumbo", () =>
			{
				var male = TestHelper.GenerateSeededPawn(maleColonistRequest);
				var female = TestHelper.GenerateSeededPawn(femaleThrumboRequest);
				TryToReport(male, female, true);
			});

			Find.WindowStack.Add(new Dialog_MessageBox(sb.ToString().Trim()));

			void Trial(string title, Action action)
			{
				try
				{
					sb.AppendLine(title);
					action();
				}
				catch (Exception ex)
				{
					var result = "Exception".Colorize(failedColor);
					sb.AppendLineTagged($"  {result} (see console logs)");
					logger.Error("Auto-test exception.", ex);
				}
				finally
				{
					sb.AppendLine();
				}
			}

			// Tries to give the `mother` a human pregnancy.
			Hediff_Pregnant VanillaHumanPregnancy(Pawn mother, Pawn father)
			{
				using var _seeded = Seeded.With(mother);
				var hediff = (Hediff_Pregnant)HediffMaker.MakeHediff(HediffDefOf.PregnantHuman, mother);
				hediff.Severity = 0.1f;
				var inheritedGeneSet = PregnancyUtility.GetInheritedGeneSet(father, mother, out var success);
				if (success)
				{
					hediff.SetParents(null, father, inheritedGeneSet);
					return hediff;
				}
				return null;
			}

			// Try the two pawns in both argument positions.
			void TryToReport(Pawn pawn1, Pawn pawn2, bool expectAccepted)
			{
				var text = expectAccepted ? "Accepted" : "Rejected";
				sb.AppendLine($"  Expecting: {text}");
				var result1 = PregnancyUtility.CanEverProduceChild(pawn1, pawn2);
				sb.AppendLine("  Given Order");
				sb.AppendLineTagged($"    Status: {ReportToStatus(result1, expectAccepted)}");
				sb.AppendLineTagged($"    Reason: {ReportToReason(result1)}");
				var result2 = PregnancyUtility.CanEverProduceChild(pawn2, pawn1);
				sb.AppendLine("  Reversed Order");
				sb.AppendLineTagged($"    Status: {ReportToStatus(result2, expectAccepted)}");
				sb.AppendLineTagged($"    Reason: {ReportToReason(result2)}");
			}

			string ReportToStatus(AcceptanceReport report, bool expectAccepted) =>
				(report.Accepted, expectAccepted) switch
				{
					(true, true) or (false, false) => "Passed".Colorize(passedColor),
					_ => "Failed".Colorize(failedColor)
				};

			string ReportToReason(AcceptanceReport report) =>
				report.Reason.NullOrEmpty() ? "(None)" : report.Reason;
		}
	}

	[HarmonyPatch(typeof(PregnancyUtility), "RandomLastName")]
	static class Patch_PregnancyUtility_RandomLastName
	{
		public static bool Prefix(ref Pawn geneticMother, ref Pawn birthingMother, ref Pawn father)
		{
			if (geneticMother != null)
				if (geneticMother.Name == null || !xxx.is_human(geneticMother))
				{
					geneticMother = null;
				}
			if (father != null)
				if (father.Name == null || !xxx.is_human(father))
				{
					father = null;
				}
			if (birthingMother != null)
				if (birthingMother.Name == null || !xxx.is_human(birthingMother))
				{
					birthingMother = null;
				}
			return true;
		}
	}

	// Fixes CompEggLayer and mating for humanlikes.
	// Note that this involves skipping over a check for the Biotech DLC, but as of build 1.4.3534 this doesn't unlock
	// any Biotech-only features or anything like that.
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.Sterile))]
	class Pawn_Sterile
	{
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> NotOwningBiotechWillNotMakeYouSterileButModdingProbablyWill(
			IEnumerable<CodeInstruction> instructions)
		{
			PropertyInfo biotechActive = AccessTools.DeclaredProperty(typeof(ModsConfig), nameof(ModsConfig.BiotechActive));
			bool foundBiotechCheck = false;

			foreach (var instruction in instructions)
			{
				// if (ModsConfig.BiotechActive) ...
				// => if (true) ...
				if (instruction.Calls(biotechActive.GetMethod))
				{
					yield return new CodeInstruction(OpCodes.Ldc_I4_1);
					foundBiotechCheck = true;
					continue;
				}
				yield return instruction;
			}

			if (!foundBiotechCheck)
			{
				ModLog.Error("Failed to patch Pawn.Sterile: Could not find `ModsConfig.BiotechActive`");
			}
		}
	}

	// Make the game use RJW's fertility capacity in vanilla pregnancy chance calculations
	[HarmonyPatch]
	class Various_GetStatValue
	{
		static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(PregnancyUtility), nameof(PregnancyUtility.CanEverProduceChild));
			yield return AccessTools.Method(typeof(PregnancyUtility), nameof(PregnancyUtility.PregnancyChanceForPawn));
			yield return AccessTools.Method(typeof(Pawn), nameof(Pawn.Sterile));
		}

		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UseRjwFertilityInstead(IEnumerable<CodeInstruction> instructionEnumerable, MethodBase original)
		{
			bool foundFertilityStatDefLoad = false;
			FieldInfo statDefOfFertility = AccessTools.DeclaredField(typeof(StatDefOf), nameof(StatDefOf.Fertility));

			var instructions = instructionEnumerable.ToList();
			for (int i = 0; i < instructions.Count; i++)
			{
				// pawn.GetStatValue(StatDefOf.Fertility)
				// => pawn.health.capacities.GetLevel(xxx.reproduction)
				if (instructions[i].LoadsField(statDefOfFertility) && i + 3 < instructions.Count)
				{
					foundFertilityStatDefLoad = true;
					yield return CodeInstruction.LoadField(typeof(Pawn), nameof(Pawn.health)).WithLabels(instructions[i].ExtractLabels());
					yield return CodeInstruction.LoadField(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.capacities));
					yield return CodeInstruction.LoadField(typeof(xxx), nameof(xxx.reproduction));
					yield return CodeInstruction.Call(typeof(PawnCapacitiesHandler), nameof(PawnCapacitiesHandler.GetLevel));

					// Skip GetStatValue call
					i += 3;
					continue;
				}
				yield return instructions[i];
			}

			if (!foundFertilityStatDefLoad)
			{
				ModLog.Error($"Failed to patch {original.Name}: Could not find `pawn.GetStatValue(StatDefOf.Fertility)`");
			}
		}
	}

	// Mostly prevent pawns born from RJW pregnancies from having no name.
	[HarmonyPatch(typeof(PawnBioAndNameGenerator), nameof(PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo))]
	class PawnBioAndNameGenerator_GiveAppropriateBioAndNameTo
	{
		[HarmonyPrefix]
		static void FixBabyNameIfNoBiotech(ref bool newborn)
		{
			newborn = newborn && RJWPregnancySettings.UseVanillaPregnancy;
		}
	}

	// Adjust the pregnancy approach tooltip to reflect the fact that it no longer affects the chance of pregnancy directly.
	[HarmonyPatch(typeof(PregnancyUtility), nameof(PregnancyUtility.GetDescription))]
	class PregnancyUtility_GetDescription
	{
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> ModifyPregnancyApproachTooltip(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				if (instruction.LoadsConstant("PregnancyChance"))
				{
					yield return new CodeInstruction(OpCodes.Ldstr, "VaginalWeight");
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}

	[HarmonyPatch(typeof(PawnColumnWorker_Pregnant), "GetIconFor")]
	public class PawnColumnWorker_Patch_Icon
	{
		public static void Postfix(Pawn pawn, ref Texture2D __result)
		{
			if (pawn.IsVisiblyPregnant()) __result = ContentFinder<Texture2D>.Get("UI/Icons/Animal/Pregnant", true);
		}
	}

	[HarmonyPatch(typeof(PawnColumnWorker_Pregnant), "GetTooltipText")]
	public class PawnColumnWorker_Patch_Tooltip
	{
		public static bool Prefix(Pawn pawn, ref string __result)
		{
			// Handles multi-pregnancy by getting the one nearest to birth.
			var pregHediff = PregnancyHelper.GetPregnancy(pawn);
			(var ticksCompleted, var ticksToBirth) = PregnancyHelper.GetProgressTicks(pregHediff);
			__result = "PregnantIconDesc".Translate(
				ticksCompleted.ToStringTicksToDays("F0"),
				ticksToBirth.ToStringTicksToDays("F0")
			);
			return false;
		}
	}

	[HarmonyPatch(typeof(TransferableUIUtility), "DoExtraIcons")]
	public class TransferableUIUtility_Patch_Icon
	{
		//private static readonly Texture2D PregnantIcon = ContentFinder<Texture2D>.Get("UI/Icons/Animal/Pregnant", true);
		public static void Postfix(Transferable trad, Rect rect, ref float curX, Texture2D ___PregnantIcon)
		{
			Pawn pawn = trad.AnyThing as Pawn;
			if (pawn?.health?.hediffSet != null && pawn.IsVisiblyPregnant())
			{
				Rect rect3 = new Rect(curX - 24f, (rect.height - 24f) / 2f, 24f, 24f);
				curX -= 24f;
				if (Mouse.IsOver(rect3))
				{
					TooltipHandler.TipRegion(rect3, PawnColumnWorker_Pregnant.GetTooltipText(pawn));
				}
				GUI.DrawTexture(rect3, ___PregnantIcon);
			}
		}
	}

	//[HarmonyPatch(typeof(AutoSlaughterManager))]
	//public class AutoSlaughterManager_AnimalsToSlaughter_rjw_preg
	//{
	//	public static void Postfix(AutoSlaughterManager __instance)
	//	{
	//		List<Pawn> __result = new List<Pawn>();
	//		var any_ins = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
	//		__result = (List<Pawn>)(__instance.GetType().GetField("AnimalsToSlaughter", any_ins).GetValue(__instance));
	//		//(__instance.GetType().GetField("AnimalsToSlaughter", any_ins).SetValue(__instance, newresult));

	//		//List<Pawn> __result = new List<Pawn>((List < Pawn > )typeof(AutoSlaughterManager).GetField("AnimalsToSlaughter", BindingFlags.Instance | BindingFlags.NonPublic));
	//		if (!__result.NullOrEmpty())
	//		{
	//			var newresult = __result;
	//			foreach (var pawn in __result)
	//			{
	//				if (pawn?.health?.hediffSet != null && pawn.IsPregnant(true))
	//				{
	//					if (__instance.configs.Any(x => x.allowSlaughterPregnant != true && x.animal == pawn.def))
	//					{
	//						newresult.Remove(pawn);
	//					}
	//				}

	//			}
	//			__result = newresult;
	//		}

	//		//return __result;
	//	}
	//}
}
