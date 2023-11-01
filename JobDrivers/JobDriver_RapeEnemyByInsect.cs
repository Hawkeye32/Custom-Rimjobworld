using System.Linq;
using RimWorld;
using Verse;

namespace rjw
{
	internal class JobDriver_RapeEnemyByInsect : JobDriver_RapeEnemy
	{
		public override bool CanUseThisJobForPawn(Pawn rapist)
		{
			if (rapist.CurJob != null && (rapist.CurJob.def != JobDefOf.LayDown || rapist.CurJob.def != JobDefOf.Wait_Wander || rapist.CurJob.def != JobDefOf.GotoWander))
				return false;

			return xxx.is_insect(rapist);
		}

		public override float GetFuckability(Pawn rapist, Pawn target)
		{
			//Female plant Eggs to everyone.
			//if (rapist.gender == Gender.Female) //Genital_Helper.has_ovipositorF(rapist);
			//{
			//	//only rape when target dont have eggs yet
			//	//if ((from x in target.health.hediffSet.GetHediffs<Hediff_InsectEgg>() where (x.IsParent(rapist)) select x).Count() > 0)
			//	{
			//		return 1f;
			//	}
			//}
			////Male rape to everyone.
			////Feritlize eggs to target with planted eggs.
			//else //Genital_Helper.has_ovipositorM(rapist);
			//{
			//	//only rape target when can fertilize
			//	//if ((from x in target.health.hediffSet.GetHediffs<Hediff_InsectEgg>() where (x.IsParent(rapist) && !x.fertilized) select x).Count() > 0)
			//	if ((from x in target.health.hediffSet.GetHediffs<Hediff_InsectEgg>() where x.IsParent(rapist) select x).Count() > 0)
			//	{
			//		return 1f;
			//	}
			//}
			return 1f;
		}
	}
}