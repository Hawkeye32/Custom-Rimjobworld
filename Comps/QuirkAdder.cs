using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using RimWorld;
using Multiplayer.API;

namespace rjw
{
	public static class QuirkAdder
	{
		public static void Add(Pawn pawn, Quirk quirk, bool warnOnFail = false)
		{
			if (!pawn.Has(quirk))
			{
				var hasFertility = pawn.RaceHasFertility();
				if (quirk == Quirk.Fertile && (!hasFertility || CompRJW.Comp(pawn).quirks.ToString().Contains(Quirk.Infertile.Key)))
				{
					if (warnOnFail)
					{
						ModLog.Warning($"Tried to add quirk {quirk} to {pawn.Name}, but either {pawn.def} does not have fertility or {pawn.Name} has a conflicting quirk");
					}
					return;
				}
				if (quirk == Quirk.Infertile && (!hasFertility || CompRJW.Comp(pawn).quirks.ToString().Contains(Quirk.Fertile.Key)))
				{
					if (warnOnFail)
					{
						ModLog.Warning($"Tried to add quirk {quirk} to {pawn.Name}, but either {pawn.def} does not have fertility or {pawn.Name} has a conflicting quirk");
					}
					return;
				}
				// No fair having a fetish for your own race.
				// But tags don't conflict so having a fetish for robot plant dragons is fine.
				if (quirk.RaceTag != null && pawn.Has(quirk.RaceTag))
				{
					if (warnOnFail)
					{
						ModLog.Warning($"Tried to add quirk {quirk} to {pawn.Name}, but {pawn} already has the associated race tag {quirk.RaceTag}");
					}
					return;
				}
				if (quirk == Quirk.Fertile)
				{
					var fertility = HediffDef.Named("IncreasedFertility");
					if (fertility != null)
						pawn.health.AddHediff(fertility);
				}
				if (quirk == Quirk.Infertile)
				{
					var infertility = HediffDef.Named("DecreasedFertility");
					if (infertility != null)
						pawn.health.AddHediff(infertility);
				}
				if ((quirk == Quirk.Buttslut && !RJWPreferenceSettings.PlayerIsButtSlut)
					|| (quirk == Quirk.Podophile && !RJWPreferenceSettings.PlayerIsFootSlut)
					|| (quirk == Quirk.Cumslut && !RJWPreferenceSettings.PlayerIsCumSlut))
				{
					if (warnOnFail)
					{
						ModLog.Warning($"Tried to add quirk {quirk} to {pawn.Name}, but all relevant sex types are pseudo-disabled (i.e. given minimum weight in settings)");
					}
					return;
				}

				CompRJW.Comp(pawn).quirks.AppendWithComma(quirk.Key);
				CompRJW.Comp(pawn).quirksave = CompRJW.Comp(pawn).quirks.ToString();
				quirk.DoAfterAdd(pawn);
			}
		}
		public static void Remove(Pawn pawn, Quirk quirk)
		{
			if (pawn.Has(quirk))
			{
				if (quirk == Quirk.Fertile)
				{
					var fertility = HediffDef.Named("IncreasedFertility");
					if (fertility != null)
						pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(fertility));
				}
				if (quirk == Quirk.Infertile)
				{
					var infertility = HediffDef.Named("DecreasedFertility");
					if (infertility != null)
						pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(infertility));
				}

				//CompRJW.Comp(pawn).quirks.AppendWithComma(quirk.Key);
				CompRJW.Comp(pawn).quirks.Replace(quirk.Key, null);

				if (CompRJW.Comp(pawn).quirks.Length == 0)
					CompRJW.Comp(pawn).quirks.Append("None");

				CompRJW.Comp(pawn).quirksave = CompRJW.Comp(pawn).quirks.ToString();
			}
		}

		public static void Clear(Pawn pawn)
		{
			Hediff fertility = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("IncreasedFertility"));
			if (fertility != null)
				pawn.health.RemoveHediff(fertility);
			Hediff infertility = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("DecreasedFertility"));
			if (infertility != null)
				pawn.health.RemoveHediff(infertility);

			CompRJW.Comp(pawn).quirks = new StringBuilder();

			if (CompRJW.Comp(pawn).quirks.Length == 0)
				CompRJW.Comp(pawn).quirks.Append("None");

			CompRJW.Comp(pawn).quirksave = CompRJW.Comp(pawn).quirks.ToString();
		}

		public static void Generate(Pawn pawn)
		{
			if (!pawn.RaceHasSexNeed() || (pawn.kindDef.race.defName.ToLower().Contains("droid") && !AndroidsCompatibility.IsAndroid(pawn)))
			{
				return;
			}
			else if (pawn.IsAnimal())
			{
				GenerateForAnimal(pawn);
			}
			else
			{
				GenerateForHumanlike(pawn);
			}
		}

		[SyncMethod]
		static void GenerateForHumanlike(Pawn pawn)
		{
			var count = Rand.RangeInclusive(0, RJWPreferenceSettings.MaxQuirks);
			var list = Quirk.All.ToList();
			list.Shuffle();

			// Some quirks may be hard for a given pawn to indulge in.
			// For example a female homosexual will have a hard time satisfying an impregnation fetish.
			// But rimworld is a weird place and you never know what the pawn will be capable of in the future.
			// We still don't want straight up contradictory results like fertile + infertile.
			var hasFertility = pawn.RaceHasFertility();
			var actual = new List<Quirk>();
			foreach (var quirk in list)
			{
				if (count == 0)
				{
					break;
				}

				// These special cases are sort of hacked in.
				// In theory there should be a general way for the quirk itself to decide when it applies.
				if (quirk == Quirk.Fertile && (!hasFertility || actual.Contains(Quirk.Infertile)))
				{
					continue;
				}
				if (quirk == Quirk.Infertile && (!hasFertility || actual.Contains(Quirk.Fertile)))
				{
					continue;
				}
				// Have to earn these.
				if (quirk == Quirk.Breeder || quirk == Quirk.Incubator)
				{
					continue;
				}
				// No fair having a fetish for your own race.
				// But tags don't conflict so having a fetish for robot plant dragons is fine.
				if (quirk.RaceTag != null && pawn.Has(quirk.RaceTag))
				{
					continue;
				}

				count--;
				actual.Add(quirk);
			}

			foreach (var quirk in actual)
			{
				pawn.Add(quirk);
			}
		}

		[SyncMethod]
		static void GenerateForAnimal(Pawn pawn)
		{
			if (Rand.Chance(0.1f))
			{
				pawn.Add(Quirk.Messy);
			}

			if (!pawn.RaceHasFertility())
			{
				return;
			}

			if (Rand.Chance(0.1f))
			{
				pawn.Add(Quirk.Fertile);
			}
			else if (Rand.Chance(0.1f))
			{
				pawn.Add(Quirk.Infertile);
			}
		}
	}
}
