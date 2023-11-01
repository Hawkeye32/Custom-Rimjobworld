using System.Linq;
using Verse;

namespace rjw
{
	/// <summary>
	/// Patch all races into rjw parts recipes 
	/// </summary>

	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		static HarmonyPatches()
		{
			//summons carpet bombing

			//inject races into rjw recipes
			foreach (RecipeDef x in	DefDatabase<RecipeDef>.AllDefsListForReading.Where(x => x.IsSurgery && (x.targetsBodyPart || !x.appliedOnFixedBodyParts.NullOrEmpty())))
			{
				if (x.appliedOnFixedBodyParts.Contains(xxx.genitalsDef)
					|| x.appliedOnFixedBodyParts.Contains(xxx.breastsDef)
					|| x.appliedOnFixedBodyParts.Contains(xxx.anusDef)
					|| x.modContentPack?.PackageId == "rim.job.world" 
					|| x.modContentPack?.PackageId == "Safe.Job.World" //*sigh*
					//|| x.modContentPack?.PackageId == "Abraxas.RJW.RaceSupport" // for udders?
					)

					foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs.Where(thingDef =>
							thingDef.race != null && (
							thingDef.race.Humanlike ||
							thingDef.race.Animal
							)))
					{
						//filter out something, probably?
						//if (thingDef.race. == "Human")
						//	continue;

						if (!x.recipeUsers.Contains(thingDef))
						{
							x.recipeUsers.Add(item: thingDef);
							//Log.Message("recipe: " + x.defName + ", thing: " + thingDef.defName);

						}
					}
			}
		}
	}
}
