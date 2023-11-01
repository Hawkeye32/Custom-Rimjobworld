using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	class OviHelper
	{
		static readonly IDictionary<PawnKindDef, ThingDef> FertileEggPawnKinds = new Dictionary<PawnKindDef, ThingDef>();
		static readonly IDictionary<PawnKindDef, ThingDef> UnfertileEggPawnKinds = new Dictionary<PawnKindDef, ThingDef>();
		public static CompProperties_EggLayer GenerateEggLayerProperties(PawnKindDef pawnKindDef, RaceGroupDef raceGroupDef)
		{
			CompProperties_EggLayer comp = new CompProperties_EggLayer();
			comp.eggFertilizedDef = DefDatabase<ThingDef>.GetNamed(raceGroupDef.eggFertilizedDef);
			comp.eggUnfertilizedDef = DefDatabase<ThingDef>.GetNamed(raceGroupDef.eggUnfertilizedDef);
			comp.eggProgressUnfertilizedMax = raceGroupDef.eggProgressUnfertilizedMax;
			comp.eggLayIntervalDays = raceGroupDef.eggLayIntervalDays;

			return comp;
		}

	}
}