using RimWorld;
using Verse;

namespace rjw
{
	internal class JobDriver_RapeEnemyToParasite : JobDriver_RapeEnemy
	{
		//not implemented
		public JobDriver_RapeEnemyToParasite()
		{
			this.requireCanRape = false;
		}

		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			if (rapist.CurJob != null && (rapist.CurJob.def != JobDefOf.LayDown || rapist.CurJob.def != JobDefOf.Wait_Wander || rapist.CurJob.def != JobDefOf.GotoWander))
				return false;

			return false;
		}
	}
}