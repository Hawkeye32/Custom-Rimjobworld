using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Recipe_RemovePart : rjw_CORE_EXPOSED.Recipe_RemoveBodyPart
	{
		// Quick and dirty method to guess whether the player is harvesting the genitals or amputating them
		// due to infection. The core code can't do this properly because it considers the private part
		// hediffs as "unclean".
		public override bool blocked(Pawn p)
		{
			return xxx.is_slime(p);//|| xxx.is_demon(p)
		}

		public bool is_harvest(Pawn p, BodyPartRecord part)
		{
			foreach (Hediff hed in p.health.hediffSet.hediffs)
			{
				if ((hed.Part?.def == part.def) && hed.def.isBad && (hed.Severity >= 0.70f))
					return false;
			}
			return true;
		}

		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			foreach (BodyPartRecord part in base.GetPartsToApplyOn(p, r))
			{
				if (r.appliedOnFixedBodyParts.Contains(part.def) && !blocked(p))
					yield return part;
			}
		}

		public override void ApplyOnPawn(Pawn p, BodyPartRecord part, Pawn doer, List<Thing> ingredients, Bill bill)
		{
			var har = is_harvest(p, part);

			base.ApplyOnPawn(p, part, doer, ingredients, bill);

			if (har)
			{
				if (!p.Dead)
				{
					//Log.Message("alive harvest " + part);
					ThoughtUtility.GiveThoughtsForPawnOrganHarvested(p, doer);
				}
				else
				{
					//Log.Message("dead harvest " + part);
					ThoughtUtility.GiveThoughtsForPawnExecuted(p, doer, PawnExecutionKind.OrganHarvesting);
				}
			}
		}

		public override string GetLabelWhenUsedOn(Pawn p, BodyPartRecord part)
		{
			return recipe.label.CapitalizeFirst();
		}
	}
	/*
	public class Recipe_RemovePenis : Recipe_RemovePart
	{
		public override bool blocked(Pawn p)
		{
			return (Genital_Helper.genitals_blocked(p)
				|| !(Genital_Helper.has_penis(p) 
					&& Genital_Helper.has_penis_infertile(p) 
					&& Genital_Helper.has_ovipositorF(p)));
		}
	}

	public class Recipe_RemoveVagina : Recipe_RemovePart
	{
		public override bool blocked(Pawn p)
		{
			return (Genital_Helper.genitals_blocked(p)
				|| !Genital_Helper.has_vagina(p));
		}
	}

	public class Recipe_RemoveGenitals : Recipe_RemovePart
	{
		public override bool blocked(Pawn p)
		{
			return (Genital_Helper.genitals_blocked(p)
				|| !Genital_Helper.has_genitals(p));
		}
	}

	public class Recipe_RemoveBreasts : Recipe_RemovePart
	{
		public override bool blocked(Pawn p)
		{
			return (Genital_Helper.breasts_blocked(p)
				|| !Genital_Helper.has_breasts(p));
		}
	}

	public class Recipe_RemoveAnus : Recipe_RemovePart
	{
		public override bool blocked(Pawn p)
		{
			return (Genital_Helper.anus_blocked(p)
				|| !Genital_Helper.has_anus(p));
		}
	}
	*/
}