using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Called to determine if the non animal is eligible for a Bestiality job
	/// </summary>
	public class ThinkNode_ConditionalBestiality : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			//if (p.Faction != null && p.Faction.IsPlayer)
			//	ModLog.Message("ThinkNode_ConditionalBestiality " + xxx.get_pawnname(p) + " is animal: " + xxx.is_animal(p));

			// No bestiality for animals, animal-on-animal is handled in Breed job.
			if (xxx.is_animal(p))
				return false;

			// Bestiality off
			if (!RJWSettings.bestiality_enabled)
				return false;

			// No free will while designated for rape.
			if (!RJWSettings.designated_freewill)
				if ((p.IsDesignatedComfort() || p.IsDesignatedBreeding()))
					return false;

			return true;
		}
	}
}