using RimWorld;
using System;
using Verse;
using System.Collections.Generic;

namespace rjw
{
	public class Recipe_PregnancyHackMech : RecipeWorker
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			BodyPartRecord part = pawn.RaceProps.body.corePart;
			if (recipe.appliedOnFixedBodyParts[0] != null)
				part = pawn.RaceProps.body.AllParts.Find(x => x.def == recipe.appliedOnFixedBodyParts[0]);
			if (part != null && pawn.health.hediffSet.HasHediff(HediffDef.Named("RJW_pregnancy_mech"), part, true))
			{
				Hediff_MechanoidPregnancy pregnancy = (Hediff_MechanoidPregnancy)pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("RJW_pregnancy_mech"));
				//Log.Message("RJW_pregnancy_mech hack check: " + pregnancy.is_checked);
				if (pregnancy.is_checked && !pregnancy.is_hacked)
					yield return part;
			}
		}

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{

			if (pawn.health.hediffSet.HasHediff(HediffDef.Named("RJW_pregnancy_mech")))
			{
				Hediff_MechanoidPregnancy pregnancy = (Hediff_MechanoidPregnancy)pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("RJW_pregnancy_mech"));
				pregnancy.Hack();
			}
		}
	}
}
