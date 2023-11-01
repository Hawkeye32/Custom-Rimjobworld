using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Recipe_RemoveBreasts : Recipe_RemovePart
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			if (Genital_Helper.has_breasts(p))
			{
				bool blocked = Genital_Helper.breasts_blocked(p) || xxx.is_slime(p);//|| xxx.is_demon(p)

				foreach (BodyPartRecord part in p.health.hediffSet.GetNotMissingParts())
					if (r.appliedOnFixedBodyParts.Contains(part.def) && (!blocked))
						yield return part;
			}
		}
	}
}