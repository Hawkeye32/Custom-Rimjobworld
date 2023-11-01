using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Pawn HornyOrFrustrated
	/// </summary>
	public class ThinkNode_ConditionalHornyOrFrustrated : ThinkNode_Conditional
	{
		protected override bool Satisfied (Pawn p)
		{
			return xxx.is_hornyorfrustrated(p);
		}
	}
}