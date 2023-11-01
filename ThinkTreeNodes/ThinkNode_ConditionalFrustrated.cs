using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Pawn frustrated
	/// </summary>
	public class ThinkNode_ConditionalFrustrated : ThinkNode_Conditional
	{
		protected override bool Satisfied (Pawn p)
		{
			return xxx.is_frustrated(p);
		}
	}
}