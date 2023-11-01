using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Assigns a pawn to cleanup/collect sex fluids
	/// </summary>
	//TODO: add sex fluid collection/cleaning
	public class WorkGiver_CleanSexStuff : WorkGiver_Sexchecks
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.filth);

		public override bool MoreChecks(Pawn pawn, Thing t, bool forced = false)
		{
			return false;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return null;
		}
	}
}