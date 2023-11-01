using System.Collections.Generic;
using System.Linq;
using Verse;

namespace rjw
{
	/// <summary>
	/// IUD - prevent pregnancy
	/// </summary>
	public class Recipe_InstallIUD : Recipe_InstallImplantToExistParts
	{
		public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
		{
			if (RJWPregnancySettings.UseVanillaPregnancy)
			{
				return false;
			}
			return base.AvailableOnNow(thing, part);
		}

		// Let's comment it out. What's the worst that could happen?
		// public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		// {
		// 	if (!xxx.is_female(pawn))
		// 	{
		// 		return Enumerable.Empty<BodyPartRecord>();
		// 	}
		// 	return base.GetPartsToApplyOn(pawn, recipe);
		// }
	}
}
