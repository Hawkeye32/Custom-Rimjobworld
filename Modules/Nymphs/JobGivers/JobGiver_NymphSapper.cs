using System;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;

namespace rjw
{
	// Token: 0x020006B5 RID: 1717
	public class JobGiver_NymphSapper : ThinkNode_JobGiver
	{
		// Token: 0x06002E54 RID: 11860 RVA: 0x001041A7 File Offset: 0x001023A7
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_NymphSapper jobGiver_NymphSapper = (JobGiver_NymphSapper)base.DeepCopy(resolve);
			jobGiver_NymphSapper.canMineMineables = this.canMineMineables;
			jobGiver_NymphSapper.canMineNonMineables = this.canMineNonMineables;
			return jobGiver_NymphSapper;
		}

		// Token: 0x06002E55 RID: 11861 RVA: 0x001041D0 File Offset: 0x001023D0
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (!RJWSettings.NymphSappers)
				return null;
			if (!xxx.is_nympho(pawn))
				return null;

			IntVec3 intVec;
			{
				IAttackTarget attackTarget;
				if (!(from x in pawn.Map.mapPawns.FreeColonistsAndPrisonersSpawned
				where !x.ThreatDisabled(pawn) && pawn.CanReach(x, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.PassAllDestroyableThings)
				select x).TryRandomElement(out attackTarget))
				{
					return null;
				}
				intVec = attackTarget.Thing.Position;
			}
			using (PawnPath pawnPath = pawn.Map.pathFinder.FindPath(pawn.Position, intVec, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassDoors, false), PathEndMode.OnCell))
			{
				IntVec3 cellBeforeBlocker;
				Thing thing = pawnPath.FirstBlockingBuilding(out cellBeforeBlocker, pawn);
				if (thing != null)
				{
					Job job = DigUtility.PassBlockerJob(pawn, thing, cellBeforeBlocker, this.canMineMineables, this.canMineNonMineables);
					if (job != null)
					{
						return job;
					}
				}
			}
			return JobMaker.MakeJob(JobDefOf.Goto, intVec, 500, true);
		}

		// Token: 0x04001A64 RID: 6756
		private bool canMineMineables = false;

		// Token: 0x04001A65 RID: 6757
		private bool canMineNonMineables = false;

		// Token: 0x04001A66 RID: 6758
		private const float ReachDestDist = 10f;

		// Token: 0x04001A67 RID: 6759
		private const int CheckOverrideInterval = 500;
	}
}
