using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Multiplayer.API;

namespace rjw
{
	/// <summary>
	/// This is the driver for animals mating.
	/// </summary>
	public class JobDriver_Mating : JobDriver_Rape
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, BreederHelper.max_animals_at_once, 0, null, errorOnFailed);
		}

		[SyncMethod]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			setup_ticks();
			var partnerJob = xxx.gettin_loved;

			//--Log.Message("JobDriver_Mating::MakeNewToils() - setting fail conditions");
			this.FailOnDespawnedNullOrForbidden(iTarget);
			this.FailOn(() => !pawn.CanReserve(Partner, BreederHelper.max_animals_at_once, 0)); // Fail if someone else reserves the target before the animal arrives.
			this.FailOn(() => !pawn.CanReach(Partner, PathEndMode.Touch, Danger.Some)); // Fail if animal cannot reach target.
			this.FailOn(() => pawn.Drafted);

			// Path to target
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);

			var startPartnerJob = new Toil();
			startPartnerJob.defaultCompleteMode = ToilCompleteMode.Instant;
			startPartnerJob.socialMode = RandomSocialMode.Off;
			startPartnerJob.initAction = delegate
			{
				if (Partner.jobs.curDriver is JobDriver_SexBaseRecieverLoved) return;

				var bed = Partner.CurrentBed();

				Partner.jobs.StartJob(
					JobMaker.MakeJob(partnerJob, pawn),
					JobCondition.InterruptForced
				);

				if (bed is not null)
					if (Partner.jobs.curDriver is JobDriver_SexBaseRecieverLoved driver)
						driver.Set_bed(bed);
			};
			yield return startPartnerJob;

			// Mate target
			var sexToil = new Toil();
			sexToil.defaultCompleteMode = ToilCompleteMode.Never;
			sexToil.defaultDuration = duration;
			sexToil.handlingFacing = true;
			sexToil.initAction = delegate
			{
				Partner.pather.StopDead();
				Partner.jobs.curDriver.asleep = false;

				Start();
				if (xxx.is_human(Partner))
					Sexprops.usedCondom = CondomUtility.TryUseCondom(Partner);
			};
			sexToil.tickAction = delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					ThrowMetaIconF(pawn.Position, pawn.Map, FleckDefOf.Heart);
				SexTick(pawn, Partner);
				if (!Partner.Dead)
					SexUtility.reduce_rest(Partner, 1);
				SexUtility.reduce_rest(pawn, 2);
				if (ticks_left <= 0)
					ReadyForNextToil();
			};
			sexToil.FailOn(() => Partner.CurJob?.def != partnerJob);
			sexToil.AddFinishAction(End);
			yield return sexToil;

			yield return new Toil
			{
				initAction = () => SexUtility.ProcessSex(Sexprops),
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}
