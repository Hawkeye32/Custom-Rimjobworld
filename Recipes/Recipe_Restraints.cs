using RimWorld;
using System.Collections.Generic;
using Verse;

namespace rjw
{
	/// <summary>
	/// Removes heddifs (restraints/cocoon)
	/// </summary>
	public class Recipe_RemoveRestraints : Recipe_RemoveHediff
	{
		public override bool AvailableOnNow(Thing pawn, BodyPartRecord part = null)
		{
			return true;
		}

		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			List<Hediff> allHediffs = pawn.health.hediffSet.hediffs;
			int i = 0;
			while (true)
			{
				if (i >= allHediffs.Count)
				{
					yield break;
				}
				if (allHediffs[i].def == recipe.removesHediff && allHediffs[i].Visible)
				{
					break;
				}
				i++;
			}

			yield return allHediffs[i].Part;
		}
	}
}
