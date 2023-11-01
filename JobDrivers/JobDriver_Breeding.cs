using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Multiplayer.API;

namespace rjw
{
	/// <summary>
	/// This is the driver for animals mounting breeders.
	/// </summary>
	public class JobDriver_Breeding : JobDriver_Rape
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, BreederHelper.max_animals_at_once, 0);
		}

		[SyncMethod]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			setup_ticks();
			var PartnerJob = xxx.gettin_raped;

			//--Log.Message("JobDriver_Breeding::MakeNewToils() - setting fail conditions");
			this.FailOnDespawnedNullOrForbidden(iTarget);
			this.FailOn(() => !pawn.CanReserve(Partner, BreederHelper.max_animals_at_once, 0)); // Fail if someone else reserves the target before the animal arrives.
			this.FailOn(() => !pawn.CanReach(Partner, PathEndMode.Touch, Danger.Some)); // Fail if animal cannot reach target.
			this.FailOn(() => pawn.Drafted);

			// Path to target
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);

			//if (!(pawn.IsDesignatedBreedingAnimal() && Partner.IsDesignatedBreeding()));
			if (!(pawn.IsAnimal() && Partner.IsAnimal()))
				SexUtility.RapeTargetAlert(pawn, Partner);

			var StartPartnerJob = new Toil();
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

			// Use happy heart instead of bad-touch heart when accepting of breeding.
			var fleckDef = xxx.is_zoophile(pawn) || xxx.is_animal(pawn) ? FleckDefOf.Heart : xxx.mote_noheart;

			// Breed target
			var SexToil = new Toil();
			SexToil.defaultCompleteMode = ToilCompleteMode.Never;
			SexToil.defaultDuration = duration;
			SexToil.handlingFacing = true;
			SexToil.initAction = delegate
			{
				Partner.pather.StopDead();
				Partner.jobs.curDriver.asleep = false;
				Start();
			};
			SexToil.tickAction = delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					ThrowMetaIconF(pawn.Position, pawn.Map, fleckDef);
				SexTick(pawn, Partner);
				if (!Partner.Dead)
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
					//Log.Message("JobDriver_Breeding::MakeNewToils() - Calling aftersex");
					//// Trying to add some interactions and social logs
					Sexprops.isRape = !(pawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, Partner) ||
					 	(xxx.is_animal(pawn) && (pawn.RaceProps.wildness - pawn.RaceProps.petness + 0.18f) > Rand.Range(0.36f, 1.8f)));
					SexUtility.ProcessSex(Sexprops);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}
