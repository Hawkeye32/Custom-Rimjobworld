using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Rapist, chance to trigger random rape
	/// </summary>
	public class ThinkNode_ConditionalRapist : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			if (!RJWSettings.rape_enabled)
				return false;
			
			if (xxx.is_animal(p))
				return false;

			if (!xxx.is_rapist(p))
				return false;

			// No free will while designated for rape.
			if (!RJWSettings.designated_freewill)
				if ((p.IsDesignatedComfort() || p.IsDesignatedBreeding()))
					return false;

			if (!xxx.isSingleOrPartnerNotHere(p))
			{
				return false;
			}
			else
				return true;
		}
	}
}