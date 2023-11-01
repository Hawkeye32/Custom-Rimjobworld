using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Nymph nothing to do, seek sex-> join in bed
	/// </summary>
	public class ThinkNode_ConditionalNympho : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			//ModLog.Message("ThinkNode_ConditionalNympho " + p);

			if (xxx.is_nympho(p))
				if (p.Faction == null || !p.Faction.IsPlayer)
					return false;
				else
					return true;

			return false;
		}
	}
}