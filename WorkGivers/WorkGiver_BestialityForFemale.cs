using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Assigns a pawn to breed animal(passive)
	/// </summary>
	public class WorkGiver_BestialityForFemale : WorkGiver_Sexchecks
	{
		public override bool MoreChecks(Pawn pawn, Thing t, bool forced = false)
		{
			//ModLog.Message("" + this.GetType().ToString() + ":: base checks: pass");
			if (!RJWSettings.bestiality_enabled) return false;

			Pawn target = t as Pawn;
			if (!WorkGiverChecks(pawn, t, forced))
				return false;

			if (!xxx.can_be_fucked(pawn))
			{
				if (RJWSettings.DevMode) JobFailReason.Is("pawn cant be fucked");
				return false;
			}

			if (!(pawn.IsDesignatedHero() || RJWSettings.override_control))
				if (!RJWSettings.WildMode)
				{
					if (!xxx.is_zoophile(pawn) && !xxx.is_frustrated(pawn))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("not willing to have sex with animals");
						return false;
					}
					if (!xxx.is_hornyorfrustrated(pawn))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("not horny enough");
						return false;
					}
					if (!xxx.is_healthy_enough(target))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("target not healthy enough");
						return false;
					}

					//ModLog.Message("WorkGiver_BestialityForFemale::" + SexAppraiser.would_fuck_animal(pawn, target));
					if (SexAppraiser.would_fuck_animal(pawn, target) < 0.1f)
					{
						return false;
					}
					//add some more fancy conditions from JobGiver_Bestiality?
				}

			//ModLog.Message("" + this.GetType().ToString() + ":: extended checks: can start sex");
			return true;
		}

		public override bool WorkGiverChecks(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn target = t as Pawn;
			if (!xxx.is_animal(target))
			{
				return false;
			}
			Building_Bed bed = pawn.ownership.OwnedBed;
			if (bed == null)
			{
				if (RJWSettings.DevMode) JobFailReason.Is("pawn has no bed");
				return false;
			}
			if (!target.CanReach(bed, PathEndMode.OnCell, Danger.Some) || target.Downed)
			{
				if (RJWSettings.DevMode) JobFailReason.Is("target cant reach bed");
				return false;
			}
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Building_Bed bed = pawn.ownership.OwnedBed;
			return JobMaker.MakeJob(xxx.bestialityForFemale, t, bed);
		}
	}
}