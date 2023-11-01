using RimWorld;
using Verse;

namespace rjw
{
	public class ThoughtWorker_Bound : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (p.apparel != null)
			{
				bool bound = false, gagged = false;
				foreach (var app in p.apparel.WornApparel)
				{
					var gear_def = app.def as bondage_gear_def;
					if (gear_def != null)
					{
						bound |= gear_def.gives_bound_moodlet;
						gagged |= gear_def.gives_gagged_moodlet;
					}
				}

				if (bound && gagged)
					return ThoughtState.ActiveAtStage(2);
				else if (gagged)
					return ThoughtState.ActiveAtStage(1);
				else if (bound)
					return ThoughtState.ActiveAtStage(0);
			}
			return ThoughtState.Inactive;
		}
	}
}