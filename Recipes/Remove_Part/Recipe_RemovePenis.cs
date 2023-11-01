using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Recipe_RemovePenis : Recipe_RemovePart
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			if (Genital_Helper.has_penis(p) || Genital_Helper.has_penis_infertile(p) || Genital_Helper.has_ovipositorF(p))
			{
				bool blocked = Genital_Helper.genitals_blocked(p) || xxx.is_slime(p);

				foreach (var part in p.health.hediffSet.GetNotMissingParts())
					if (r.appliedOnFixedBodyParts.Contains(part.def) && (!blocked))
						yield return part;
			}
		}
	}
}