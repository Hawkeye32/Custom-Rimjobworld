using Verse;
using System.Linq;
using RimWorld;

namespace rjw
{
	[StaticConstructorOnStartup]
	public static class AddComp
	{
		static AddComp()
		{
			AddRJWComp();
		}

		/// <summary>
		/// This automatically adds the comp to all races on startup.
		/// </summary>
		public static void AddRJWComp()
		{
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs.Where(thingDef =>
					thingDef.race != null))
			{
				thingDef.comps.Add(new CompProperties_RJW());
				//Log.Message("AddRJWComp to race " + thingDef.label);
			}

			foreach (PawnKindDef pawnKindDef in DefDatabase<PawnKindDef>.AllDefs.Where(pawnKindDef => pawnKindDef.race.race != null))
			{
				RaceGroupDef raceGroupDef = null;
				if (RaceGroupDef_Helper.TryGetRaceGroupDef(pawnKindDef, out raceGroupDef))
				{
					//Log.Message("RaceGroupDef_Helper " + raceGroupDef.defName + " for " + pawnKindDef.race.defName);
					if (raceGroupDef.oviPregnancy)
					{
						if (pawnKindDef.race.comps.Any(x => x is CompProperties_EggLayer))
						{
							//Log.Message(pawnKindDef.race.defName + " was already egglayer");
						}
						else
						{
							CompProperties_EggLayer eggProps = OviHelper.GenerateEggLayerProperties(pawnKindDef, raceGroupDef);
							pawnKindDef.race.comps.Add(eggProps);
							//Log.Message(pawnKindDef.race.defName + " is now egglayer and lays " + eggProps.eggFertilizedDef.defName + " eggs");
						}
					}
				}
			}

			// For some reason eggs only grow if a pawn has a lifestage that is "milkable"
			// This might not be ideal...
			foreach (LifeStageDef lifeStage in DefDatabase<LifeStageDef>.AllDefs)
			{
				if (lifeStage.reproductive)
				{
					lifeStage.milkable = true;
				}
			}
		}
	}
}