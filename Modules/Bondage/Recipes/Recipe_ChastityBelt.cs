using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Recipe_InstallChastityBelt : Recipe_InstallImplant
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			return base.GetPartsToApplyOn(p, r);
		}
	}

	public class Recipe_UnlockChastityBelt : Recipe_InstallImplant
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			return base.GetPartsToApplyOn(p, r);
		}
	}
}