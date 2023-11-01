using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace rjw
{
	public static class PawnExtensions
	{
		public static bool RaceHasFertility(this Pawn pawn)
		{
			// True by default.
			if (RaceGroupDef_Helper.TryGetRaceGroupDef(pawn, out var raceGroupDef))
				return raceGroupDef.hasFertility;
			return true;
		}

		public static bool RaceHasPregnancy(this Pawn pawn)
		{
			// True by default.
			if (RaceGroupDef_Helper.TryGetRaceGroupDef(pawn, out var raceGroupDef))
				return raceGroupDef.hasPregnancy;
			return true;
		}

		public static bool RaceHasOviPregnancy(this Pawn pawn)
		{
			// False by default.
			if (RaceGroupDef_Helper.TryGetRaceGroupDef(pawn, out var raceGroupDef))
				return raceGroupDef.oviPregnancy;
			return false;
		}

		public static bool RaceImplantEggs(this Pawn pawn)
		{
			// False by default.
			if (RaceGroupDef_Helper.TryGetRaceGroupDef(pawn, out var raceGroupDef))
				return raceGroupDef.ImplantEggs;
			return false;
		}

		public static bool RaceHasSexNeed(this Pawn pawn)
		{
			// True by default.
			if (RaceGroupDef_Helper.TryGetRaceGroupDef(pawn, out var raceGroupDef))
				return raceGroupDef.hasSexNeed;
			return true;
		}

		public static bool TryAddRacePart(this Pawn pawn, SexPartType sexPartType)
		{
			return RaceGroupDef_Helper.TryAddRacePart(pawn, sexPartType);
		}

		public static bool Has(this Pawn pawn, Quirk quirk)
		{
			return xxx.has_quirk(pawn, quirk.Key);
		}

		public static bool Has(this Pawn pawn, RaceTag tag)
		{
			if (RaceGroupDef_Helper.TryGetRaceGroupDef(pawn, out var raceGroupDef))
			{
				return raceGroupDef.tags != null && raceGroupDef.tags.Contains(tag.Key);
			}
			else
			{
				return tag.DefaultWhenNoRaceGroupDef(pawn);
			}
		}

		public static void Add(this Pawn pawn, Quirk quirk)
		{
			QuirkAdder.Add(pawn, quirk);
		}
		
		public static bool IsSexyRobot(this Pawn pawn)
		{
			return AndroidsCompatibility.IsAndroid(pawn);
		}

		// In theory I think this should involve RaceGroupDef.
		public static bool IsUnsexyRobot(this Pawn pawn)
		{
			return !IsSexyRobot(pawn)
				&& (xxx.is_mechanoid(pawn) || pawn.kindDef.race.defName.ToLower().Contains("droid"));
		}

		public static bool IsAnimal(this Pawn pawn)
		{
			return xxx.is_animal(pawn);
		}

		public static bool IsVisiblyPregnant(this Pawn pawn)
		{
			return pawn.IsPregnant(true);
		}

		public static bool IsPregnant(this Pawn pawn, bool mustBeVisible = false)
		{
			var set = pawn.health.hediffSet;
			return set.HasHediff(HediffDefOf.PregnantHuman, mustBeVisible) || 
				set.HasHediff(HediffDefOf.Pregnant, mustBeVisible) ||
				Hediff_BasePregnancy.KnownPregnancies().Any(x => set.HasHediff(HediffDef.Named(x), mustBeVisible));
		}

		public static List<Hediff> GetGenitalsList(this Pawn pawn)
		{
			List<Hediff> set;
			try//error at world gen
			{
				set = pawn.GetRJWPawnData().genitals;
				if (set.NullOrEmpty())
				{
					var partBPR = Genital_Helper.get_genitalsBPR(pawn);
					set = Genital_Helper.get_PartsHediffList(pawn, partBPR);
					pawn.GetRJWPawnData().genitals = set;
				}

			}
			catch
			{
				var partBPR = Genital_Helper.get_genitalsBPR(pawn);
				set = Genital_Helper.get_PartsHediffList(pawn, partBPR);
			}
			return set;
		}

		public static List<Hediff> GetBreastList(this Pawn pawn)
		{
			List<Hediff> set;
			try//error at world gen
			{
				set = pawn.GetRJWPawnData().breasts;
				if (set.NullOrEmpty())
				{
					var partBPR = Genital_Helper.get_breastsBPR(pawn);
					set = Genital_Helper.get_PartsHediffList(pawn, partBPR);
					pawn.GetRJWPawnData().breasts = set;
				}

			}
			catch
			{
				var partBPR = Genital_Helper.get_breastsBPR(pawn);
				set = Genital_Helper.get_PartsHediffList(pawn, partBPR);
			}
			return set;
		}
		public static List<Hediff> GetUdderList(this Pawn pawn)
		{
			List<Hediff> set;
			try//error at world gen
			{
				set = pawn.GetRJWPawnData().udders;
				if (set.NullOrEmpty())
				{
					var partBPR = Genital_Helper.get_uddersBPR(pawn);
					set = Genital_Helper.get_PartsHediffList(pawn, partBPR);
					pawn.GetRJWPawnData().udders = set;
				}

			}
			catch
			{
				var partBPR = Genital_Helper.get_breastsBPR(pawn);
				set = Genital_Helper.get_PartsHediffList(pawn, partBPR);
			}
			return set;
		}
		public static List<Hediff> GetAnusList(this Pawn pawn)
		{
			List<Hediff> set;
			try//error at world gen
			{
				set = pawn.GetRJWPawnData().anus;
				if (set.NullOrEmpty())
				{
					var partBPR = Genital_Helper.get_anusBPR(pawn);
					set = Genital_Helper.get_PartsHediffList(pawn, partBPR);
					pawn.GetRJWPawnData().anus = set;
				}

			}
			catch
			{
				var partBPR = Genital_Helper.get_anusBPR(pawn);
				set = Genital_Helper.get_PartsHediffList(pawn, partBPR);
			}
			return set;
		}

		public static List<Hediff> GetTorsoList(this Pawn pawn)
		{
			List<Hediff> set;
			try//error at world gen
			{
				set = pawn.GetRJWPawnData().torso;
				if (set.NullOrEmpty())
				{
					var partBPR = Genital_Helper.get_torsoBPR(pawn);
					set = Genital_Helper.get_PartsHediffList(pawn, partBPR);
					pawn.GetRJWPawnData().torso = set;
				}

			}
			catch
			{
				var partBPR = Genital_Helper.get_torsoBPR(pawn);
				set = Genital_Helper.get_PartsHediffList(pawn, partBPR);
			}
			return set;
		}
		public static SexProps GetRMBSexPropsCache(this Pawn pawn)
		{
			SexProps set;
			set = pawn.GetRJWPawnData().SexProps;
			return set;
		}

		public static PawnData GetRJWPawnData(this Pawn pawn)
		{
			return SaveStorage.DataStore.GetPawnData(pawn);
		}
	}
}
