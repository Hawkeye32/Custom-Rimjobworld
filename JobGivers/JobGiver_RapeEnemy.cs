using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{
	/// <summary>
	/// Pawn try to find enemy to rape.
	/// </summary>
	public class JobGiver_RapeEnemy : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (RJWSettings.DebugRape) ModLog.Message(" JobGiver_RapeEnemy::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) called0");
			//ModLog.Message(" JobGiver_RapeEnemy::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) 0 " + SexUtility.ReadyForLovin(pawn));
			//ModLog.Message(" JobGiver_RapeEnemy::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) 1 " + (xxx.need_some_sex(pawn) <= 1f));
			//ModLog.Message(" JobGiver_RapeEnemy::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) 2 " + !(SexUtility.ReadyForLovin(pawn) || xxx.need_some_sex(pawn) <= 1f));
			//ModLog.Message(" JobGiver_RapeEnemy::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) 1 " + Find.TickManager.TicksGame);
			//ModLog.Message(" JobGiver_RapeEnemy::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) 2 " + pawn.mindState.canLovinTick);

			if (pawn.Drafted) return null;

			if (pawn.health.hediffSet.HasHediff(HediffDef.Named("Hediff_RapeEnemyCD")) || !pawn.health.capacities.CanBeAwake || !(SexUtility.ReadyForLovin(pawn) || xxx.need_some_sex(pawn) <= 1f))
			//if (pawn.health.hediffSet.HasHediff(HediffDef.Named("Hediff_RapeEnemyCD")) || !pawn.health.capacities.CanBeAwake || (SexUtility.ReadyForLovin(pawn) || xxx.is_human(pawn) ? xxx.need_some_sex(pawn) <= 1f : false))
				return null;

			if (!xxx.can_rape(pawn)) return null;
			if (RJWSettings.DebugRape) ModLog.Message(" JobGiver_RapeEnemy::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) can rape");

			JobDef_RapeEnemy rapeEnemyJobDef = null;
			int? highestPriority = null;
			foreach (JobDef_RapeEnemy job in DefDatabase<JobDef_RapeEnemy>.AllDefs)
			{
				if (job.CanUseThisJobForPawn(pawn))
				{
					if (highestPriority == null)
					{
						rapeEnemyJobDef = job;
						highestPriority = job.priority;
					}
					else if (job.priority > highestPriority)
					{
						rapeEnemyJobDef = job;
						highestPriority = job.priority;
					}
				}
			}

			if (rapeEnemyJobDef == null)
			{
				if (RJWSettings.DebugRape) ModLog.Message(" JobGiver_RapeEnemy::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) no valid rapeEnemyJobDef found");
				return null;
			}

			if (RJWSettings.DebugRape) ModLog.Message(" JobGiver_RapeEnemy::ChoosedJobDef( " + xxx.get_pawnname(pawn) + " ) - " + rapeEnemyJobDef?.ToString() + " choosen");
			Pawn victim = rapeEnemyJobDef?.FindVictim(pawn, pawn.Map);

			if (RJWSettings.DebugRape) ModLog.Message(" JobGiver_RapeEnemy::FoundVictim( " + xxx.get_pawnname(victim) + " )");

			//prevents 10 job stacks error, no idea whats the prob with JobDriver_Rape
			//if (victim != null)
			pawn.health.AddHediff(HediffDef.Named("Hediff_RapeEnemyCD"), null, null, null);

			return victim != null ? JobMaker.MakeJob(rapeEnemyJobDef, victim) : null;
			/*
			else
			{
				if (RJWSettings.DebugRape) ModLog.Message("" + this.GetType().ToString() + "::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) - unable to find victim");
				pawn.mindState.canLovinTick = Find.TickManager.TicksGame + Rand.Range(75, 150);
			}
			*/
			//else {  if (RJWSettings.DebugRape) ModLog.Message(" JobGiver_RapeEnemy::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) - too fast to play next"); }
		}
	}
}