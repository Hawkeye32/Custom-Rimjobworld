using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Try to solicit pawn to have sex with
	/// </summary>
	public class WorkGiver_Solicit : WorkGiver_Sexchecks
	{
		public override bool MoreChecks(Pawn pawn, Thing t, bool forced = false)
		{
			//ModLog.Message("" + this.GetType().ToString() + " base checks: pass");

			Pawn target = t as Pawn;
			if (target == pawn)
			{
				//JobFailReason.Is("no self solicit", null);
				return false;
			}
			if (!WorkGiverChecks(pawn, t, forced))
				return false;

			if (!xxx.is_human(target))
			{
				return false;
			}
			//if (!pawn.CanReserve(target, xxx.max_rapists_per_prisoner, 0))
			//	return false;

			//ModLog.Message("WorkGiver_Sex::" + SexAppraiser.would_fuck(target, pawn));
			//if (SexAppraiser.would_fuck(target, pawn) < 0.1f)
			//{
			//	return false;
			//}

			//ModLog.Message("" + this.GetType().ToString() + " extended checks: can start sex");
			return true;
		}

		public override bool WorkGiverChecks(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn target = t as Pawn;
			if (pawn.HostileTo(target) || target.IsDesignatedComfort())
			{
				return false;
			}
			return true;
		}
		
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			//TODO:: fix bed stealing during join other pawn
			//Building_Bed bed = pawn.ownership.OwnedBed;
			//if (bed == null)
			//	bed = (t as Pawn).ownership.OwnedBed;

			Building_Bed bed = (pawn as Pawn).ownership.OwnedBed;
			
			if (bed == null)
				return null;

			//if (pawn.CurrentBed() != (t as Pawn).CurrentBed())
			//	return null;

			return JobMaker.MakeJob(xxx.whore_inviting_visitors, t as Pawn, bed);
		}
	}
}