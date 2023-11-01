using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Linq;
using System;

namespace rjw
{
	public class Recipe_GrowBreasts : Recipe_Surgery
	{
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (billDoer != null)
			{
				if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
				{
					return;
				}
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
				{
					billDoer,
					pawn
				});
			}

			var oldBoobs = pawn.health.hediffSet.hediffs.FirstOrDefault(hediff => hediff.def == bill.recipe.removesHediff);
			var newBoobs = bill.recipe.addsHediff;
			var newSize = BreastSize_Helper.GetSize(newBoobs);

			GenderHelper.ChangeSex(pawn, () =>
			{
				BreastSize_Helper.HurtBreasts(pawn, part, 3 * (newSize - 1));
				if (pawn.health.hediffSet.PartIsMissing(part))
				{
					return;
				}
				if (oldBoobs != null)
				{
					pawn.health.RemoveHediff(oldBoobs);
				}
				pawn.health.AddHediff(newBoobs, part);
			});
		}

		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipeDef)
		{
			yield break;
			var chest = Genital_Helper.get_breastsBPR(pawn);

			if (pawn.health.hediffSet.PartIsMissing(chest)
				|| Genital_Helper.breasts_blocked(pawn))
			{
				yield break;
			}

			var old = recipeDef.removesHediff;
			if (old == null ? BreastSize_Helper.HasNipplesOnly(pawn, chest) : pawn.health.hediffSet.HasHediff(old, chest))
			{
				yield return chest;
			}
		}
	}
}