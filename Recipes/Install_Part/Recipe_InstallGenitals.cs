using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Recipe_InstallGenitals : Recipe_InstallPrivates
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			var gen_blo = Genital_Helper.genitals_blocked(p);
			foreach (BodyPartRecord part in base.GetPartsToApplyOn(p, r))
				if ((!gen_blo) || (part != xxx.genitals))
					yield return part;
			
		}
	}
}