using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Multiplayer.API;

namespace rjw
{
	public class JobDriver_BestialityForFemale : JobDriver_SexBaseInitiator
	{
		public IntVec3 SleepSpot => Bed.SleepPosOfAssignedPawn(pawn);
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, 1, 0, null, errorOnFailed);
		}

		[SyncMethod]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			setup_ticks();
			var PartnerJob = xxx.gettin_loved;

			this.FailOnDespawnedOrNull(iTarget);
			this.FailOnDespawnedNullOrForbidden(iBed);
			this.FailOn(() => !pawn.CanReserveAndReach(Partner, PathEndMode.Touch, Danger.Deadly));
			this.FailOn(() => pawn.Drafted);
			this.FailOn(() => Partner.IsFighting());
			this.FailOn(() => !Partner.CanReach(pawn, PathEndMode.Touch, Danger.Deadly));

			yield return Toils_Reserve.Reserve(iTarget, 1, 0);
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.Touch);

			var gotoBed = new Toil();
			gotoBed.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			gotoBed.initAction = delegate
			{
				pawn.pather.StartPath(SleepSpot, PathEndMode.OnCell);
				Partner.jobs.StopAll();
				Job job = JobMaker.MakeJob(JobDefOf.GotoMindControlled, SleepSpot);
				Partner.jobs.StartJob(job, JobCondition.InterruptForced);
			};
			gotoBed.FailOnBedNoLongerUsable(iBed);
			gotoBed.AddFailCondition(() => Partner.Downed);
			yield return gotoBed;

			var waitInBed = new Toil();
			waitInBed.defaultCompleteMode = ToilCompleteMode.Delay;
			waitInBed.initAction = delegate
			{
				ticksLeftThisToil = 5000;
			};
			waitInBed.tickAction = delegate
			{
				pawn.GainComfortFromCellIfPossible();
				if (IsInOrByBed(Bed, Partner) && pawn.PositionHeld == Partner.PositionHeld)
				{
					ReadyForNextToil();
				}
			};
			waitInBed.FailOn(() => pawn.GetRoom(RegionType.Set_Passable) == null);
			yield return waitInBed;

			var StartPartnerJob = new Toil();
			StartPartnerJob.defaultCompleteMode = ToilCompleteMode.Instant;
			StartPartnerJob.socialMode = RandomSocialMode.Off;
			StartPartnerJob.initAction = delegate
			{
				var gettin_loved = JobMaker.MakeJob(PartnerJob, pawn, Bed);
				Partner.jobs.StartJob(gettin_loved, JobCondition.InterruptForced);
			};
			yield return StartPartnerJob;

			var SexToil = new Toil();
			SexToil.defaultCompleteMode = ToilCompleteMode.Never;
			SexToil.socialMode = RandomSocialMode.Off;
			SexToil.handlingFacing = true;
			SexToil.initAction = delegate
			{
				Start();

				// TODO: replace this quick n dirty way
				CondomUtility.GetCondomFromRoom(pawn);
				// Try to use whore's condom first, then client's
				Sexprops.usedCondom = CondomUtility.TryUseCondom(pawn) || CondomUtility.TryUseCondom(Partner);
			};
			SexToil.FailOn(() => Partner.Dead);
			SexToil.FailOn(() => Partner.CurJob?.def != PartnerJob);
			SexToil.AddPreTickAction(delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					if (xxx.is_zoophile(pawn))
						ThrowMetaIconF(pawn.Position, pawn.Map, FleckDefOf.Heart);
					else
						ThrowMetaIconF(pawn.Position, pawn.Map, xxx.mote_noheart);
				SexTick(pawn, Partner);
				SexUtility.reduce_rest(Partner, 2);
				SexUtility.reduce_rest(pawn, 1);
				if (ticks_left <= 0)
					ReadyForNextToil();
			});
			SexToil.AddFinishAction(End);
			yield return SexToil;

			yield return new Toil
			{
				initAction = () => SexUtility.ProcessSex(Sexprops),
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}
