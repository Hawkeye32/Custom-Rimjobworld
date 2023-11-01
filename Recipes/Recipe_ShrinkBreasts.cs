using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Linq;
using System;

namespace rjw
{
	public class Recipe_ShrinkBreasts : Recipe_Surgery
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

			if (!BreastSize_Helper.TryGetBreastSize(pawn, out var oldSize))
			{
				throw new ApplicationException("Recipe_ShrinkBreasts could not find any breasts to shrink.");
			}

			var oldBoobs = pawn.health.hediffSet.GetFirstHediffOfDef(BreastSize_Helper.GetHediffDef(oldSize));
			var newSize = oldSize - 1;
			var newBoobs = BreastSize_Helper.GetHediffDef(newSize);

			// I can't figure out how to spawn a stack of 2 meat.
			for (var i = 0; i < 2; i++)
			{
				GenSpawn.Spawn(pawn.RaceProps.meatDef, billDoer.Position, billDoer.Map);
			}

			GenderHelper.ChangeSex(pawn, () =>
			{
				BreastSize_Helper.HurtBreasts(pawn, part, 5);
				if (pawn.health.hediffSet.PartIsMissing(part))
				{
					return;
				}
				pawn.health.RemoveHediff(oldBoobs);
				pawn.health.AddHediff(newBoobs, part);
			});
		}

		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipeDef)
		{
			yield break;
			var chest = Genital_Helper.get_breastsBPR(pawn);

			if (Genital_Helper.breasts_blocked(pawn))
			{
				yield break;
			}

			if (BreastSize_Helper.TryGetBreastSize(pawn, out var size)
				//&& size > BreastSize_Helper.GetSize(Genital_Helper.flat_breasts))
				)
			{
				yield return chest;
			}
		}
	}
}