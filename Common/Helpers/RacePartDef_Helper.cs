using Multiplayer.API;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace rjw
{
	class RacePartDef_Helper
	{
		/// <summary>
		/// Returns true if a partAdders was chosen (even if that part is "no part").
		/// </summary>
		[SyncMethod]
		public static bool TryRacePartDef_partAdders(Pawn pawn)
		{
			//RaceGroupDef_Helper.TryGetRaceGroupDef(pawn.kindDef, out var raceGroupDef);
			if (!RaceGroupDef_Helper.TryGetRaceGroupDef(pawn.kindDef, out var raceGroupDef))
				return false;
			if (raceGroupDef.partAdders.NullOrEmpty())
				return false;

			List<BodyPartRecord> pawnParts = pawn.RaceProps?.body?.AllParts;
			pawnParts = pawnParts.FindAll(record => !pawn.health.hediffSet.PartIsMissing(record));

			foreach (var Adder in raceGroupDef.partAdders)
			{
				if (Adder.chance <= Rand.Value)
					continue;

				var racePartDef = DefDatabase<RacePartDef>.GetNamedSilentFail(Adder.rjwPart);

				if (racePartDef == null)
				{
					//No part found
					continue;
				}

				if (!racePartDef.TryGetHediffDef(out var rjwPartDef))
				{
					//No part found
					continue;
				}

				pawnParts = pawnParts.FindAll(record => Adder.bodyParts.Contains(record.def.defName));

				foreach (BodyPartRecord record in pawnParts)
				{
					var hediff = RacePartDef_Helper.MakePart(rjwPartDef, pawn, record, racePartDef);
					pawn.health.AddHediff(hediff, record);
				}
			}
			return true;
		}

		/// <summary>
		/// Returns true if a race part was chosen (even if that part is "no part").
		/// </summary>
		[SyncMethod]
		public static bool TryChooseRacePartDef(RaceGroupDef raceGroupDef, SexPartType sexPartType, out RacePartDef racePartDef)
		{
			var partNames = raceGroupDef.GetRacePartDefNames(sexPartType);
			if (partNames == null)
			{
				// Missing list, so nothing was chosen.
				racePartDef = null;
				return false;
			}
			else if (!partNames.Any())
			{
				// Empty list, so "no part" was chosen.
				racePartDef = RacePartDef.None;
				return true;
			}

			var chances = raceGroupDef.GetChances(sexPartType);
			var hasChances = chances != null && chances.Count() > 0;

			if (hasChances && chances.Count() != partNames.Count())
			{
				// No need for this to be runtime, should probably be a config error in RaceGroupDef.
				ModLog.Error($"RaceGroupDef named {raceGroupDef.defName} has {partNames.Count()} parts but {chances.Count()} chances for {sexPartType}.");
				racePartDef = null;
				return false;
			}

			string partName;
			if (hasChances)
			{
				var indexes = partNames.Select((x, i) => i);
				partName = partNames[indexes.RandomElementByWeight(i => chances[i])];
			}
			else
			{
				partName = partNames.RandomElement();
			}

			racePartDef = DefDatabase<RacePartDef>.GetNamedSilentFail(partName);
			if (racePartDef == null)
			{
				ModLog.Error($"Could not find a RacePartDef named {partName} referenced by RaceGroupDef named {raceGroupDef.defName}.");
				return false;
			}
			else
			{
				return true;
			}
		}

		[SyncMethod]
		public static Hediff MakePart(HediffDef hediffDef, Pawn pawn, BodyPartRecord bodyPartRecord, RacePartDef racePartDef)
		{
			var hediff = HediffMaker.MakeHediff(hediffDef, pawn, bodyPartRecord);
			var compHediff = hediff.TryGetComp<CompHediffBodyPart>();
			if (compHediff != null)
			{
				compHediff.initComp(pawn);
				if (racePartDef.fluidType != null)
				{
					compHediff.FluidType = racePartDef.fluidType;
				}
				if (racePartDef.fluidModifier != null)
				{
					compHediff.FluidModifier = racePartDef.fluidModifier.Value;
				}
				if (racePartDef.severityCurve != null && racePartDef.severityCurve.Any())
				{
					// Size math is in flux right now, but the idea is that for an individual pawn
					// the size chosen in the RaceGroupDef is the size reported in the UI regardless of pawn's BodySize.

					// Check for males to only get nipple sized (flat) breasts
					// Trap calculation is done in initComp method above, by this point only pawns with SizeBase of 0.01
					// should be those that failed the Trap calc, are male, or are very unlucky on their breast size roll
					// Check for is male at the end because its possible (though very unlikely) for a female to roll a 0.01, and
					// the severity curve for that race's breasts may actually have it as impossible for a female to be that small
					if (compHediff.SizeBase == 0.01f && hediff.def.defName.ToLower().Contains("breast") && pawn.gender == Gender.Male)
					{
						compHediff.updatesize();
					}
					else
					{
						var severity = racePartDef.severityCurve.Evaluate(Rand.Value);
						compHediff.SizeBase = severity;
						compHediff.EffSize = severity;
						compHediff.updatesize(severity);
					}
				}
				else
				{
					compHediff.updatesize();
				}
			}

			return hediff;
		}

		/// <summary>
		/// Generates and logs RacePartDef xml shells for each RJW hediff so they can be manually saved as a def file and referenced by RaceGroupDefs.
		/// In theory this could be done automatically at run time. But then we also might want to add real configuration
		/// to the shells so for now just checking in the shells.
		/// </summary>
		public static void GeneratePartShells()
		{
			var defs = DefDatabase<HediffDef_PartBase>.AllDefs.OrderBy(def => def.defName);
			var template = "\t<rjw.RacePartDef>\n\t\t<defName>{0}</defName>\n\t\t<hediffName>{0}</hediffName>\n\t</rjw.RacePartDef>";
			var strings = defs.Select(def => string.Format(template, def.defName));
			ModLog.Message(" RacePartDef shells:\n" + string.Join("\n", strings));
		}
	}
}
