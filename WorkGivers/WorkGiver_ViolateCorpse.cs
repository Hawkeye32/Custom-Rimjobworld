using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Assigns a pawn to rape a corpse
	/// </summary>
	public class WorkGiver_ViolateCorpse : WorkGiver_Sexchecks
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Corpse);

		public override bool MoreChecks(Pawn pawn, Thing t, bool forced = false)
		{
			//ModLog.Message("" + this.GetType().ToString() + " base checks: pass");
			if (!RJWSettings.necrophilia_enabled) return false;

			//Pawn target = (t as Corpse).InnerPawn;
			if (!pawn.CanReserve(t, xxx.max_rapists_per_prisoner, 0))
				return false;

			if (!(pawn.IsDesignatedHero() || RJWSettings.override_control))
				if (!RJWSettings.WildMode)
				{
					if (xxx.is_necrophiliac(pawn) && !xxx.is_hornyorfrustrated(pawn))
						{
						if (RJWSettings.DevMode) JobFailReason.Is("not horny enough");
							return false;
						}
					if (!xxx.is_necrophiliac(pawn))
						if ((t as Corpse).CurRotDrawMode != RotDrawMode.Fresh)
						{
							if (RJWSettings.DevMode) JobFailReason.Is("refuse to rape rotten");
							return false;
						}
						else if (!xxx.is_frustrated(pawn))
							{
							if (RJWSettings.DevMode) JobFailReason.Is("not horny enough");
								return false;
							}
					//ModLog.Message("WorkGiver_ViolateCorpse::" + SexAppraiser.would_fuck(pawn, t as Corpse));
					if (SexAppraiser.would_fuck(pawn, t as Corpse) > 0.1f)
					{
						return false;
					}
				}
			//ModLog.Message("" + this.GetType().ToString() + " extended checks: can start sex");
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(xxx.RapeCorpse, t as Corpse);
		}
	}
}