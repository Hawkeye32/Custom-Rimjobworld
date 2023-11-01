using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Called to determine if the animal can mate(vanilla reproductory sex) with animals.
	/// </summary>
	public class ThinkNode_ConditionalMate : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			//ModLog.Message("ThinkNode_ConditionalMate " + xxx.get_pawnname(p));
			return (xxx.is_animal(p) && RJWSettings.animal_on_animal_enabled);
		}
	}
}
