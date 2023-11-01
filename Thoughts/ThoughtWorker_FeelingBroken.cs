using RimWorld;
using Verse;

namespace rjw
{
	public class ThoughtWorker_FeelingBroken : ThoughtWorker
	{
		public static int Clamp(int value, int min, int max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}

		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			var brokenstages = p.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken);
			if (brokenstages != null && brokenstages.CurStageIndex != 0)
			{
				if (xxx.is_masochist(p) && brokenstages.CurStageIndex >= 2)
				{
					return ThoughtState.ActiveAtStage(2); // begging for more
				}
				return ThoughtState.ActiveAtStage(Clamp(brokenstages.CurStageIndex - 1, 0, 1));
			}
			return ThoughtState.Inactive;
		}
	}
}