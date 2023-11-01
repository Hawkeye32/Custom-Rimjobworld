using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Recipe_InstallAnus : Recipe_InstallPrivates
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			var gen_blo = Genital_Helper.anus_blocked(p);
			foreach (BodyPartRecord part in base.GetPartsToApplyOn(p, r))
				if ((!gen_blo) || (part != xxx.anus))
					yield return part;
		}
	}
}