using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace rjw
{
	/// <summary>
	/// Sterilization
	/// </summary>
	public class Recipe_InstallImplantToExistParts : Recipe_InstallImplant
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			bool blocked = Genital_Helper.genitals_blocked(pawn) || xxx.is_slime(pawn);

			if (!blocked)
				foreach (BodyPartRecord record in pawn.RaceProps.body.AllParts.Where(x => recipe.appliedOnFixedBodyParts.Contains(x.def)))
				{
					if (!pawn.health.hediffSet.hediffs.Any((Hediff x) => x.def == recipe.addsHediff))
					{
						yield return record;
					}
				}
		}
	}
}
