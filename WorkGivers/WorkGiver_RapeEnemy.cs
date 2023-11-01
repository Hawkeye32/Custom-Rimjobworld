using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Assigns a pawn to rape enemy
	/// </summary>
	public class WorkGiver_RapeEnemy : WorkGiver_Rape
	{
		public override bool WorkGiverChecks(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn target = t as Pawn;
			if (!pawn.HostileTo(target))
			{
				if (RJWSettings.DevMode) JobFailReason.Is("not hostile", null);
					return false;
			}
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(xxx.RapeEnemy, t);
		}
	}
}