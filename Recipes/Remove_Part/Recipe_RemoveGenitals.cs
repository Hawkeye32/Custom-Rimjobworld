using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Recipe_RemoveGenitals : Recipe_RemovePart
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			if (Genital_Helper.has_genitals(p))
			{
				bool blocked = Genital_Helper.genitals_blocked(p) || xxx.is_slime(p);//|| xxx.is_demon(p)

				foreach (BodyPartRecord part in p.health.hediffSet.GetNotMissingParts())
					if (r.appliedOnFixedBodyParts.Contains(part.def) && (!blocked))
						yield return part;
			}
		}
	}
}