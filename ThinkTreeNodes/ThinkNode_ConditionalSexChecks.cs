using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Called to determine if the pawn can engage in sex.
	/// This should be used as the first conditional for sex-related thinktrees.
	/// </summary>
	public class ThinkNode_ConditionalSexChecks : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			//ModLog.Message("ThinkNode_ConditionalSexChecks " + xxx.get_pawnname(p));
			//if (p.Faction != null && p.Faction.IsPlayer)
			//	ModLog.Message("ThinkNode_ConditionalSexChecks " + xxx.get_pawnname(p) + " is animal: " + xxx.is_animal(p));

			// Downed, Drafted and Awake are checked in core ThinkNode_ConditionalCanDoConstantThinkTreeJobNow.
			if (p.Map == null)
				return false;

			// Pawn(animal) is fogged, no sex, save tps
			if (p.Fogged())
				return false;

			// Setting checks.
			if (!xxx.can_do_loving(p))
				return false;
			else if (xxx.is_animal(p) && !RJWSettings.bestiality_enabled && !RJWSettings.animal_on_animal_enabled)
				return false;

			// State checks. No sex while trying to leave map.
			if (p.mindState?.duty?.def != null)
				if (p.mindState.duty.def == DutyDefOf.SleepForever)
					return false;
				else if (p.mindState.duty.def == DutyDefOf.EnterTransporter)
					return false;
				else if (p.mindState.duty.def == DutyDefOf.EnterTransporterAndDefendSelf)
					return false;
				else if (p.mindState.duty.def == DutyDefOf.ExitMapBest)
					return false;
				else if (p.mindState.duty.def == DutyDefOf.ExitMapBestAndDefendSelf)
					return false;
				else if (p.mindState.duty.def == DutyDefOf.ExitMapNearDutyTarget)
					return false;
				else if (p.mindState.duty.def == DutyDefOf.ExitMapRandom)
					return false;
				else if (p.mindState.duty.def == DutyDefOf.PrepareCaravan_CollectAnimals)
					return false;
				else if (p.mindState.duty.def == DutyDefOf.PrepareCaravan_GatherAnimals)
					return false;
				else if (p.mindState.duty.def == DutyDefOf.PrepareCaravan_GatherDownedPawns)
					return false;
				else if (p.mindState.duty.def == DutyDefOf.PrepareCaravan_GatherItems)
					return false;

			// No sex while starving or badly hurt.
			return ((!p.needs?.food?.Starving) ?? true && (xxx.is_healthy_enough(p) || !xxx.is_human(p)));
		}
	}
}
