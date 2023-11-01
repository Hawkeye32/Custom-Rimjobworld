using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Pawn is horny
	/// </summary>
	public class ThinkNode_ConditionalHorny : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			return xxx.is_horny(p);
		}
	}
}