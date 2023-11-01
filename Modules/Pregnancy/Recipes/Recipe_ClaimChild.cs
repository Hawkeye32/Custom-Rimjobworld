using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Recipe_ClaimChild :  RecipeWorker
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			if (pawn != null)//I have no idea how it works but Recipe_ShutDown : RecipeWorker does not return values when not applicable
			{
				//Log.Message("RJW Claim child check on " + pawn);
				if (xxx.is_human(pawn) && !pawn.IsColonist)
				{
					if ( (pawn.ageTracker.CurLifeStageIndex < 2))//Guess it is hardcoded for now to `baby` and `toddler` of standard 4 stages of human life
					{
						BodyPartRecord brain = pawn.health.hediffSet.GetBrain();
						if (brain != null)
						{
							//Log.Message("RJW Claim child is applicable");
							yield return brain;
						}
					}
				}
			}
		}

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (pawn == null)
			{
				ModLog.Error("Error applying medical recipe, pawn is null");
				return;
			}
			pawn.SetFaction(Faction.OfPlayer);
			//we could do 
			//pawn.SetFaction(billDoer.Faction);
			//but that is useless because GetPartsToApplyOn does not support factions anyway and all recipes are hardcoded to player.
		}
	}
}