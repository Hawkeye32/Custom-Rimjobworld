using RimWorld;
using rjw.Modules.Shared.Extensions;
using rjw.Modules.Shared.Logs;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobGiver_JoinInBed : ThinkNode_JobGiver
	{
		private readonly static ILog _log = LogManager.GetLogger<JobGiver_JoinInBed>();

		protected override Job TryGiveJob(Pawn pawn)
		{
			if (!RJWHookupSettings.HookupsEnabled)
			{
				_log.Debug("Hookups disabled");
				return null;
			}

			if (pawn.Drafted)
				return null;

			if (!SexUtility.ReadyForHookup(pawn))
				return null;

			string name = pawn.GetName();

			_log.Debug($"{name} was ready");

			// We increase the time right away to prevent the fairly expensive check from happening too frequently
			SexUtility.IncreaseTicksToNextHookup(pawn);

			// If the pawn is a whore, or recently had sex, skip the job unless they're really horny
			if (!xxx.is_frustrated(pawn) && (xxx.is_whore(pawn) || !SexUtility.ReadyForLovin(pawn)))
			{
				_log.Debug($"{name} but wasn't ready");
				return null;
			}

			// This check attempts to keep groups leaving the map, like guests or traders, from turning around to hook up
			if (pawn.mindState?.duty?.def == DutyDefOf.TravelOrLeave)
			{
				// TODO: Some guest pawns keep the TravelOrLeave duty the whole time, I think the ones assigned to guard the pack animals.
				// That's probably ok, though it wasn't the intention.
				if (RJWSettings.DebugLogJoinInBed) _log.Debug($"JoinInBed.TryGiveJob:({xxx.get_pawnname(pawn)}): has TravelOrLeave, no time for lovin!");
				return null;
			}

			if (pawn.CurJob == null || pawn.CurJob.def == JobDefOf.LayDown)
			{
				//--Log.Message("   checking pawn and abilities");
				if (CasualSex_Helper.CanHaveSex(pawn))
				{
					//--Log.Message("   finding partner");
					Pawn partner = CasualSex_Helper.find_partner(pawn, pawn.Map, true);

					//--Log.Message("   checking partner");
					if (partner == null)
					{
						_log.Debug($"{name} didn't find a partner");
						return null;
					}

					// Can never be null, since find checks for bed.
					Building_Bed bed = partner.CurrentBed();

					// Interrupt current job.
					if (pawn.CurJob != null && pawn.jobs.curDriver != null)
						pawn.jobs.curDriver.EndJobWith(JobCondition.InterruptForced);

					//--Log.Message("   returning job");
					return JobMaker.MakeJob(xxx.casual_sex, partner, bed);
				}
			}

			return null;
		}
	}
}
