using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;

namespace rjw
{
	/// <summary>
	/// Assigns a pawn to fap in bed
	/// </summary>
	public class WorkGiver_Masturbate_Bed : WorkGiver_Sexchecks
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);

		public override bool MoreChecks(Pawn pawn, Thing t, bool forced = false)
		{
			if (pawn.Position == t.Position)
			{
				//use quickfap
				return false;
			}
			//ModLog.Message("" + this.GetType().ToString() + " base checks: pass");
			Building_Bed target = t as Building_Bed;
			if (!(target is Building_Bed))
			{
				if (RJWSettings.DevMode) JobFailReason.Is("not a bed");
				return false;
			}
			if (!pawn.CanReserve(target))
			{
				//if (RJWSettings.DevMode) JobFailReason.Is("not a bed");
				return false;
			}

			if (!(pawn.IsDesignatedHero() || RJWSettings.override_control))
				if (!RJWSettings.WildMode)
				{
					if (!target.OwnersForReading.Contains(pawn) && !xxx.is_psychopath(pawn))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("not my bed");
						return false;
					}
					if (!xxx.is_nympho(pawn))
						if (!xxx.is_hornyorfrustrated(pawn))
						{
							if (RJWSettings.DevMode) JobFailReason.Is("not horny enough");
							return false;
						}
					if (target.CurOccupants.Count() != 0)
					{
						if (target.CurOccupants.Count() == 1 && !target.CurOccupants.Contains(pawn))
						{
							if (RJWSettings.DevMode) JobFailReason.Is("bed not empty");
							return false;
						}
						if (target.CurOccupants.Count() > 1)
						{
							if (RJWSettings.DevMode) JobFailReason.Is("bed not empty");
							return false;
						}
					}

					//TODO: more exhibitionsts checks?
					bool canbeseen = false;
					foreach (Pawn bystander in pawn.Map.mapPawns.AllPawnsSpawned.Where(x => xxx.is_human(x) && x != pawn))
					{
						// dont see through walls, dont see whole map, only 15 cells around
						if (bystander.CanSee(target) && bystander.Position.DistanceTo(target.Position) < 15)
						{
							//if (!LovePartnerRelationUtility.LovePartnerRelationExists(pawn, bystander))
							canbeseen = true;
						}
					}
					if (!xxx.has_quirk(pawn, "Exhibitionist") && canbeseen)
					{
						if (RJWSettings.DevMode) JobFailReason.Is("can be seen");
						return false;
					}
					if (xxx.has_quirk(pawn, "Exhibitionist") && !canbeseen)
					{
						if (RJWSettings.DevMode) JobFailReason.Is("can not be seen");
						return false;
					}
				}

			//ModLog.Message("" + this.GetType().ToString() + " extended checks: can start sex");
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(xxx.Masturbate, pawn, t, t.Position);
		}
	}
}