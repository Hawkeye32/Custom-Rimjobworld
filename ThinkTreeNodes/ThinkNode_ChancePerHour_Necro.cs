using System;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{
	/// <summary>
	/// Necro rape corpse
	/// </summary>
	public class ThinkNode_ChancePerHour_Necro : ThinkNode_ChancePerHour
	{
		protected override float MtbHours(Pawn pawn)
		{
			float base_mtb = xxx.config.comfort_prisoner_rape_mtbh_mul; // Default of 4.0

			float desire_factor;
			{
				var need_sex = pawn.needs.TryGetNeed<Need_Sex>();
				if (need_sex != null)
				{
					if (need_sex.CurLevel <= need_sex.thresh_frustrated())
						desire_factor = 0.15f;
					else if (need_sex.CurLevel <= need_sex.thresh_horny())
						desire_factor = 0.60f;
					else if (need_sex.CurLevel <= need_sex.thresh_satisfied())
						desire_factor = 1.00f;
					else // Recently had sex.
						desire_factor = 2.00f;
				}
				else
					desire_factor = 1.00f;
			}

			float personality_factor;
			{
				personality_factor = 1.0f;

				if (xxx.is_nympho(pawn))
					personality_factor *= 0.8f;
				if (xxx.is_prude(pawn) || pawn.story.traits.HasTrait(TraitDefOf.BodyPurist))
					personality_factor *= 2f;
				if (xxx.is_psychopath(pawn))
					personality_factor *= 0.5f;

				// Pawns with no necrophiliac trait should first try to find other outlets.
				if (!xxx.is_necrophiliac(pawn))
					personality_factor *= 8f;
				else
					personality_factor *= 0.5f;

				// Less likely to engage in necrophilia if the pawn has a lover.
				if (!xxx.isSingleOrPartnerNotHere(pawn))
					personality_factor *= 1.25f;

				// Pawns with no records of prior necrophilia are less likely to engage in it.
				if (pawn.records.GetValue(xxx.CountOfSexWithCorpse) == 0)
					personality_factor *= 16f;
				else if (pawn.records.GetValue(xxx.CountOfSexWithCorpse) > 10)
					personality_factor *= 0.8f;
			}

			float fun_factor;
			{
				if ((pawn.needs.joy != null) && (xxx.is_necrophiliac(pawn)))
					fun_factor = Mathf.Clamp01(0.50f + pawn.needs.joy.CurLevel);
				else
					fun_factor = 1.00f;
			}

			// I'm normally against gender factors, but necrophilia is far more frequent among males. -Zaltys
			float gender_factor = (pawn.gender == Gender.Male) ? 1.0f : 3.0f;

			return base_mtb * desire_factor * personality_factor * fun_factor * gender_factor;
		}

		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			try
			{
				return base.TryIssueJobPackage(pawn, jobParams);
			}
			catch (NullReferenceException)
			{
				return ThinkResult.NoJob;
			}
		}
	}
}