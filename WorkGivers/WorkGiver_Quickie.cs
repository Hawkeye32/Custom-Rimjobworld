using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Assigns a pawn to rape
	/// </summary>
	public class WorkGiver_Quickie : WorkGiver_Sexchecks
	{
		public override bool MoreChecks(Pawn pawn, Thing t, bool forced = false)
		{

			Pawn target = t as Pawn;
			if (target == pawn)
			{
				//JobFailReason.Is("no self rape", null);
				return false;
			}
			if (!WorkGiverChecks(pawn, t, forced))
				return false;

			if (!xxx.is_human(target))
			{
				return false;
			}

			if(target.GetPosture() == PawnPosture.LayingInBed) 
			{
				return false;
			}
			if (!pawn.CanReserve(target, xxx.max_rapists_per_prisoner, 0))
				return false;

			if (!(pawn.IsDesignatedHero() || RJWSettings.override_control))
				return false;
			//ModLog.Message("" + this.GetType().ToString() + " extended checks: can start sex");
			return true;
		}

		public override bool WorkGiverChecks(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn target = t as Pawn;
			if (pawn.HostileTo(target))
			{
				return false;
			}
			return true;
		}
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(xxx.quick_sex, t as Pawn);
		}
	}
}