using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Colonists and prisoners try to rape CP
	/// </summary>
	public class ThinkNode_ChancePerHour_RapeCP : ThinkNode_ChancePerHour
	{
		protected override float MtbHours(Pawn pawn)
		{
			var base_mtb = xxx.config.comfort_prisoner_rape_mtbh_mul; //Default 4.0

			float desire_factor;
			{
				var need_sex = pawn.needs.TryGetNeed<Need_Sex>();
				if (need_sex != null)
				{
					if (need_sex.CurLevel <= need_sex.thresh_frustrated())
						desire_factor = 0.10f;
					else if (need_sex.CurLevel <= need_sex.thresh_horny())
						desire_factor = 0.50f;
					else
						desire_factor = 1.00f;
				}
				else
					desire_factor = 1.00f;
			}

			float personality_factor;
			{
				personality_factor = 1.0f;
				if (xxx.has_traits(pawn))
				{
					// Most of the checks are done in the SexAppraiser.would_rape method.

					personality_factor = 1.0f;

					if (!RJWSettings.rape_beating)
					{
						if (xxx.is_bloodlust(pawn))
							personality_factor *= 0.5f;
					}

					if (xxx.is_nympho(pawn))
						personality_factor *= 0.5f;
					else if (xxx.is_prude(pawn) || pawn.story.traits.HasTrait(TraitDefOf.BodyPurist))
						personality_factor *= 2f;

					if (xxx.is_rapist(pawn))
						personality_factor *= 0.5f;
					if (xxx.is_psychopath(pawn))
						personality_factor *= 0.75f;
					else if (xxx.is_kind(pawn))
						personality_factor *= 5.0f;

					float rapeCount = pawn.records.GetValue(xxx.CountOfRapedHumanlikes) +
					                pawn.records.GetValue(xxx.CountOfRapedAnimals) +
					                pawn.records.GetValue(xxx.CountOfRapedInsects) +
					                pawn.records.GetValue(xxx.CountOfRapedOthers);

					// Pawns with few or no rapes are more reluctant to rape CPs.
					if (rapeCount < 3.0f)
						personality_factor *= 1.5f;
				}
			}

			float fun_factor;
			{
				if ((pawn.needs.joy != null) && (xxx.is_rapist(pawn) || xxx.is_psychopath(pawn)))
					fun_factor = Mathf.Clamp01(0.50f + pawn.needs.joy.CurLevel);
				else
					fun_factor = 1.00f;
			}

			float animal_factor = 1.0f;
			if (xxx.is_animal(pawn))
			{
				var parts = pawn.GetGenitalsList();
				// Much slower for female animals.
				animal_factor = Genital_Helper.has_male_bits(pawn, parts) ? 2.5f : 6f;
			}

			//if (xxx.is_animal(pawn)) { Log.Message("Chance for " + pawn + " : " + base_mtb * desire_factor * personality_factor * fun_factor * animal_factor); }
			return base_mtb * desire_factor * personality_factor * fun_factor * animal_factor;
		}

		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			try
			{
				return base.TryIssueJobPackage(pawn, jobParams);
			}
			catch (NullReferenceException)
			{
				return ThinkResult.NoJob; ;
			}
		}
	}
}