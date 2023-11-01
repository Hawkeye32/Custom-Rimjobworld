using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Pawn tries to find animal to do loving/raping.
	/// </summary>
	public class JobGiver_Bestiality : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn.Drafted) return null;

			// Most checks are now done in ThinkNode_ConditionalBestiality

			if (!SexUtility.ReadyForLovin(pawn) && !xxx.is_frustrated(pawn))
				return null;

			Pawn target = BreederHelper.find_breeder_animal(pawn, pawn.Map);

			if (target == null) return null;
					
			if (xxx.can_rape(pawn))
			{
				return JobMaker.MakeJob(xxx.bestiality, target);
			}

			Building_Bed bed = pawn.ownership.OwnedBed;
			if (AnimalPenUtility.NeedsToBeManagedByRope(target)) return null;
			if (!xxx.can_be_fucked(pawn) || bed == null || !target.CanReach(bed, PathEndMode.OnCell, Danger.Some) || target.Downed) return null;

			// TODO: Should rename this to BestialityInBed or somesuch, since it's not limited to females.
			return JobMaker.MakeJob(xxx.bestialityForFemale, target, bed);
		}
	}
}