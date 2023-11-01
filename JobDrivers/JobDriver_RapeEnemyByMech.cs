using RimWorld;
using Verse;

namespace rjw
{
	internal class JobDriver_RapeEnemyByMech : JobDriver_RapeEnemy
	{
		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			if (rapist.CurJob != null && (rapist.CurJob.def != JobDefOf.LayDown || rapist.CurJob.def != JobDefOf.Wait_Wander || rapist.CurJob.def != JobDefOf.GotoWander))
				return false;

			return xxx.is_mechanoid(rapist);
		}

		public override float GetFuckability(Pawn rapist, Pawn target)
		{
			//Plant stuff into humanlikes.
			if (xxx.is_human(target))
				return 1f;
			else
				return 0f;
		}

	}
}