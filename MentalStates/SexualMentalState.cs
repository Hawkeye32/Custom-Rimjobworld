using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse.AI;
using Verse;

namespace rjw
{
	public class SexualMentalState : MentalState
	{
		public override void MentalStateTick()
		{
			if (this.pawn.IsHashIntervalTick(150))
			{
				if (xxx.is_satisfied(pawn))
					if (!(pawn.jobs?.curDriver is JobDriver_Sex))
					{
						this.RecoverFromState();
						return;
					}
			}
			base.MentalStateTick();
		}
	}

	public class SexualMentalStateWorker : MentalStateWorker
	{
		public override bool StateCanOccur(Pawn pawn)
		{
			if (base.StateCanOccur(pawn))
			{
				return xxx.is_human(pawn) && xxx.can_rape(pawn) && xxx.is_hornyorfrustrated(pawn);
			}
			else
			{
				return false;
			}
		}
	}

	public class SexualMentalBreakWorker : MentalBreakWorker
	{
		public override float CommonalityFor(Pawn pawn, bool moodCaused = false)
		{
			if (xxx.is_human(pawn))
			{
				var need_sex = pawn.needs.TryGetNeed<Need_Sex>();
				if (need_sex != null)
					return base.CommonalityFor(pawn) * (def as SexualMentalBreakDef).commonalityMultiplierBySexNeed.Evaluate(need_sex.CurLevelPercentage * 100f);
				else
					return 0;
			}
			else
			{
				return 0;
			}
		}
	}
	public class SexualMentalStateDef : MentalStateDef
	{
	}
	public class SexualMentalBreakDef : MentalBreakDef
	{
		public SimpleCurve commonalityMultiplierBySexNeed;
	}
}
