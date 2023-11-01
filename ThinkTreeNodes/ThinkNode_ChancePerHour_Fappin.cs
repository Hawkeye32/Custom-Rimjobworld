using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;

namespace rjw
{
	public class ThinkNode_ChancePerHour_Fappin : ThinkNode_ChancePerHour
	{
		public static float get_fappin_mtb_hours(Pawn p)
		{
			return (xxx.is_nympho(p) ? 0.5f : 1.0f) * rjw_CORE_EXPOSED.LovePartnerRelationUtility.LovinMtbSinglePawnFactor(p);
		}

		protected override float MtbHours(Pawn p)
		{
			// No fapping for animals... for now, at least.
			// Maybe enable this for monsters girls and such in future, but that'll need code changes to avoid errors.
			if (xxx.is_animal(p))
				return -1.0f;

			bool is_horny = xxx.is_hornyorfrustrated(p);
			if (is_horny)
			{
				bool isAlone = !p.Map.mapPawns.AllPawnsSpawned.Any(x => p.CanSee(x) && xxx.is_human(x));
				// More likely to fap if alone.
				float aloneFactor = isAlone ? 0.6f : 1.2f;
				if (xxx.has_quirk(p, "Exhibitionist"))
					aloneFactor = isAlone ? 1.0f : 0.6f;

				// More likely to fap if nude.
				float clothingFactor = p.apparel.PsychologicallyNude ? 0.8f : 1.0f;

				float SexNeedFactor = (4 - xxx.need_some_sex(p)) / 2f;
				return get_fappin_mtb_hours(p) * SexNeedFactor * aloneFactor * clothingFactor;
			}

			return -1.0f;
		}
	}
}