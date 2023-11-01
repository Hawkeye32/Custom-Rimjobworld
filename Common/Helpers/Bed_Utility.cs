using Verse;
using Verse.AI;
using System.Linq;
using RimWorld;
using System;

namespace rjw
{
	public static class Bed_Utility
	{
		public static bool bed_has_at_least_two_occupants(Building_Bed bed)
		{
			return bed.CurOccupants.Count() >= 2;
		}

		public static bool in_same_bed(Pawn pawn, Pawn partner)
		{
			if (pawn.InBed() && partner.InBed())
			{
				if (pawn.CurrentBed() == partner.CurrentBed())
					return true;
			}
			return false;
		}

		public static bool is_laying_down_alone(Pawn pawn)
		{
			if (pawn == null || !pawn.InBed()) return false;

			if (pawn.CurrentBed() != null)
				return !bed_has_at_least_two_occupants(pawn.CurrentBed());
			return true;
		}

		public static IntVec3 SleepPosOfAssignedPawn(this Building_Bed bed, Pawn pawn)
		{
			int slotIndex = 0;

			if (bed.OwnersForReading.Contains(pawn))
			{
				for (byte i = 0; i < bed.OwnersForReading.Count; i++)
				{
					if (bed.OwnersForReading[i] == pawn)
					{
						slotIndex = i;
					}
				}
			}
			else
			{
				// get random position
				slotIndex = GenTicks.TicksGame % bed.SleepingSlotsCount;
			}

			return bed.GetSleepingSlotPos(slotIndex);
		}
		public static void FailOnBedNoLongerUsable(this Toil toil, TargetIndex bedIndex, Building_Bed bed)
		{
			if (toil == null)
			{
				throw new ArgumentNullException(nameof(toil));
			}

			toil.FailOnDespawnedOrNull(bedIndex);
			toil.FailOn(bed.IsBurning);
			toil.FailOn(() => HealthAIUtility.ShouldSeekMedicalRestUrgent(toil.actor));
			toil.FailOn(() => toil.actor.IsColonist && !toil.actor.CurJob.ignoreForbidden && !toil.actor.Downed && bed.IsForbidden(toil.actor));
		}
	}
}