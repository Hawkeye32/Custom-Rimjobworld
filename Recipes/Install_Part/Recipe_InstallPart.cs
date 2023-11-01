using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Recipe_InstallPart : rjw_CORE_EXPOSED.Recipe_InstallOrReplaceBodyPart
	{
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			GenderHelper.Sex before = GenderHelper.GetSex(pawn);

			base.ApplyOnPawn(pawn, part, billDoer, ingredients, bill);

			GenderHelper.Sex after = GenderHelper.GetSex(pawn);

			GenderHelper.ChangeSex(pawn, before, after);
		}
	}

	public class Recipe_InstallGenitals : Recipe_InstallPart
	{
		public override bool blocked(Pawn p)
		{
			return (Genital_Helper.genitals_blocked(p) || xxx.is_slime(p));//|| xxx.is_demon(p)
		}
	}

	public class Recipe_InstallBreasts : Recipe_InstallPart
	{
		public override bool blocked(Pawn p)
		{
			return (Genital_Helper.breasts_blocked(p) || xxx.is_slime(p));//|| xxx.is_demon(p)
		}
	}

	public class Recipe_InstallAnus : Recipe_InstallPart
	{
		public override bool blocked(Pawn p)
		{
			return (Genital_Helper.anus_blocked(p) || xxx.is_slime(p));//|| xxx.is_demon(p)
		}
	}
}