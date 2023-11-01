using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Attempts to give a breeding job to an eligible animal.
	/// </summary>
	public class JobGiver_Breed : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn animal)
		{
			//ModLog.Message(" JobGiver_Breed::TryGiveJob( " + xxx.get_pawnname(animal) + " ) ReadyForLovin " + (SexUtility.ReadyForLovin(animal)));

			if (!SexUtility.ReadyForLovin(animal))
				return null;

			//ModLog.Message(" ready to breed::is_healthy " + xxx.is_healthy(animal) + " can_rape " + xxx.can_rape(animal));
			if (xxx.is_healthy(animal) && xxx.can_rape(animal))
			{
				//search for desiganted target to sex
				Pawn designated_target = BreederHelper.find_designated_breeder(animal, animal.Map);
				if (designated_target != null)
				{
					return JobMaker.MakeJob(xxx.animalBreed, designated_target);
				}
			}
			return null;
		}
	}
}