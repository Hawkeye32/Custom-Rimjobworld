using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_RapeComfortPawn : JobDriver_Rape
	{
		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (RJWSettings.DebugRape) ModLog.Message("" + this.GetType().ToString() + "::MakeNewToils() called");
			setup_ticks();
			var PartnerJob = xxx.gettin_raped;

			this.FailOnDespawnedNullOrForbidden(iTarget);
			//this.FailOn(() => (!Partner.health.capacities.CanBeAwake) || (!comfort_prisoners.is_designated(Partner)));//this is wrong
			this.FailOn(() => (!Partner.IsDesignatedComfort()));
			this.FailOn(() => !pawn.CanReserve(Partner, xxx.max_rapists_per_prisoner, 0)); // Fail if someone else reserves the prisoner before the pawn arrives
			this.FailOn(() => pawn.Drafted);
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);

			SexUtility.RapeTargetAlert(pawn, Partner);

			Toil StartPartnerJob = new Toil();
			StartPartnerJob.defaultCompleteMode = ToilCompleteMode.Instant;
			StartPartnerJob.socialMode = RandomSocialMode.Off;
			StartPartnerJob.initAction = delegate
			{
				if (Partner.jobs.curDriver is JobDriver_SexBaseRecieverRaped) return;

				var bed = Partner.CurrentBed();

				Partner.jobs.StartJob(
					JobMaker.MakeJob(PartnerJob, pawn),
					lastJobEndCondition: JobCondition.InterruptForced
				);

				if (bed is not null)
					if (Partner.jobs.curDriver is JobDriver_SexBaseRecieverRaped driver)
						driver.Set_bed(bed);
			};
			yield return StartPartnerJob;

			Toil SexToil = new Toil();
			SexToil.defaultCompleteMode = ToilCompleteMode.Never;
			SexToil.defaultDuration = duration;
			SexToil.handlingFacing = true;
			SexToil.initAction = delegate
			{
				Partner.pather.StopDead();
				Partner.jobs.curDriver.asleep = false;

				Start();
				// Unlike normal rape try use comfort prisoner condom
				CondomUtility.GetCondomFromRoom(Partner);
				Sexprops.usedCondom = CondomUtility.TryUseCondom(Partner);

				if (RJWSettings.DebugRape) ModLog.Message("JobDriver_RapeComfortPawn::MakeNewToils() - reserving prisoner");
				//pawn.Reserve(Partner, xxx.max_rapists_per_prisoner, 0);
			};
			SexToil.tickAction = delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					ThrowMetaIconF(pawn.Position, pawn.Map, FleckDefOf.Heart);
				SexTick(pawn, Partner);
				SexUtility.reduce_rest(Partner, 1);
				SexUtility.reduce_rest(pawn, 2);
				if (ticks_left <= 0)
					ReadyForNextToil();
			};
			SexToil.FailOn(() => Partner.CurJob?.def != PartnerJob);
			SexToil.AddFinishAction(End);
			yield return SexToil;

			yield return new Toil
			{
				initAction = delegate
				{
					// Trying to add some interactions and social logs
					SexUtility.ProcessSex(Sexprops);
					Partner.records.Increment(xxx.GetRapedAsComfortPawn);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}
