using System;
using UnityEngine;
using Verse;

namespace rjw.Modules.Shared.Helpers
{
	public static  class AgeHelper
	{
		public const int ImmortalRaceAgeClamp = 25;
		public const int NonHumanRaceAgeClamp = 25;

		public static int ScaleToHumanAge(Pawn pawn, int humanLifespan = 80)
		{
			float pawnAge = pawn.ageTracker.AgeBiologicalYearsFloat;

			if (pawn.def.defName == "Human") return (int)pawnAge; // Human, no need to scale anything.

			float lifeExpectancy = pawn.RaceProps.lifeExpectancy;

			if (RJWSettings.UseAdvancedAgeScaling == true)
			{
				//pseudo-immortal & immortal races
				if (lifeExpectancy >= 500)
				{
					return CalculateForImmortals(pawn, humanLifespan);
				}

				if (lifeExpectancy != humanLifespan)
				{
					//other races
					return CalculateForNonHuman(pawn, humanLifespan);
				}
			}

			float ageScaling = humanLifespan / lifeExpectancy;
			float scaledAge = pawnAge * ageScaling;

			return (int)Mathf.Max(scaledAge, 1);
		}

		private static int CalculateForImmortals(Pawn pawn, int humanLifespan)
		{
			float age = pawn.ageTracker.AgeBiologicalYearsFloat;
			float lifeExpectancy = pawn.RaceProps.lifeExpectancy;
			float growth = pawn.ageTracker.Growth;

			//Growth and hacks
			{
				growth = ImmortalGrowthHacks(pawn, age, growth);

				if (growth < 1)
				{
					return (int)Mathf.Lerp(0, ImmortalRaceAgeClamp, growth);
				}
			}

			//curve
			{
				float life = age / lifeExpectancy;

				//Hypothesis : very long living races looks "young" until the end of their lifespan
				if (life < 0.9f)
				{
					return ImmortalRaceAgeClamp;
				}

				return (int)Mathf.LerpUnclamped(ImmortalRaceAgeClamp, humanLifespan, Mathf.InverseLerp(0.9f, 1, life));
			}
		}

		private static float ImmortalGrowthHacks(Pawn pawn, float age, float originalGrowth)
		{
			if (pawn.ageTracker.CurLifeStage.reproductive == false)
			{
				//Hopefully, reproductive life stage will mean that we're dealing with an adult
				return Math.Min(1, age / ImmortalRaceAgeClamp);
			}

			return 1;
		}

		private static int CalculateForNonHuman(Pawn pawn, int humanLifespan)
		{
			float age = pawn.ageTracker.AgeBiologicalYearsFloat;
			float lifeExpectancy = pawn.RaceProps.lifeExpectancy;
			float growth = pawn.ageTracker.Growth;

			//Growth and hacks
			{
				growth = NonHumanGrowthHacks(pawn, age, growth);

				if (growth < 1)
				{
					return (int)Mathf.Lerp(0, NonHumanRaceAgeClamp, growth);
				}
			}

			//curve
			{
				float life = age / lifeExpectancy;

				return (int)Mathf.LerpUnclamped(NonHumanRaceAgeClamp, humanLifespan, life);
			}
		}

		private static float NonHumanGrowthHacks(Pawn pawn, float age, float originalGrowth)
		{
			if (pawn.ageTracker.CurLifeStage.reproductive == false)
			{
				//Hopefully, reproductive life stage will mean that we're dealing with an adult
				return Math.Min(1, age / NonHumanRaceAgeClamp);
			}

			return 1;
		}

	}
}
