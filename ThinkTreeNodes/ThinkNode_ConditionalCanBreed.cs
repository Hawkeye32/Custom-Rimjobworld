using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Called to determine if the animal is eligible for a breed job
	/// </summary>
	public class ThinkNode_ConditionalCanBreed : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			//ModLog.Message("ThinkNode_ConditionalCanBreed " + p);

			//Rimworld of Magic polymorphed humanlikes also get animal think node
			//if (p.Faction != null && p.Faction.IsPlayer)
			//	ModLog.Message("ThinkNode_ConditionalCanBreed " + xxx.get_pawnname(p) + " is animal: " + xxx.is_animal(p));

			// No Breed jobs for humanlikes, that's handled by bestiality.
			if (!xxx.is_animal(p))
				return false;

			// Animal stuff disabled.
			if (!RJWSettings.bestiality_enabled && !RJWSettings.animal_on_animal_enabled)
				return false;

			//return p.IsDesignatedBreedingAnimal() || RJWSettings.WildMode;
			return p.IsDesignatedBreedingAnimal();
		}
	}
}