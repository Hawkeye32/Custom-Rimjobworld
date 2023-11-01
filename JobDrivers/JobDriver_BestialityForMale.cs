using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Multiplayer.API;

namespace rjw
{
	public class JobDriver_BestialityForMale : JobDriver_Rape
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, 1, 0, null, errorOnFailed);
		}

		[SyncMethod]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			//--ModLog.Message(" JobDriver_BestialityForMale::MakeNewToils() called");

			setup_ticks();
			var PartnerJob = xxx.gettin_bred;

			//this.FailOn (() => (!Partner.health.capacities.CanBeAwake) || (!comfort_prisoners.is_designated (Partner)));
			// Fail if someone else reserves the prisoner before the pawn arrives or colonist can't reach animal
			this.FailOn(() => !pawn.CanReserveAndReach(Partner, PathEndMode.Touch, Danger.Deadly));
			this.FailOn(() => Partner.HostileTo(pawn));
			this.FailOnDespawnedNullOrForbidden(iTarget);
			this.FailOn(() => pawn.Drafted);

			yield return Toils_Reserve.Reserve(iTarget, 1, 0);
			//ModLog.Message(" JobDriver_BestialityForMale::MakeNewToils() - moving towards animal");
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.Touch);
			yield return Toils_Interpersonal.WaitToBeAbleToInteract(pawn);
			yield return Toils_Interpersonal.GotoInteractablePosition(iTarget);

			if (xxx.is_kind(pawn)
				|| (xxx.CTIsActive && xxx.has_traits(pawn) && pawn.story.traits.HasTrait(xxx.RCT_AnimalLover)))
			{
				yield return TalkToAnimal(pawn, Partner);
				yield return TalkToAnimal(pawn, Partner);
			}

			if (Rand.Chance(0.6f))
				yield return TalkToAnimal(pawn, Partner);

			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);

			SexUtility.RapeTargetAlert(pawn, Partner);

			Toil StartPartnerJob = new Toil();
			StartPartnerJob.defaultCompleteMode = ToilCompleteMode.Instant;
			StartPartnerJob.socialMode = RandomSocialMode.Off;
			StartPartnerJob.initAction = delegate
			{
				//--ModLog.Message(" JobDriver_BestialityForMale::MakeNewToils() - Setting animal job driver");
				var dri = Partner.jobs.curDriver as JobDriver_SexBaseRecieverRaped;
				if (dri == null)
				{
					//wild animals may flee or attack
					if (pawn.Faction != Partner.Faction && Partner.RaceProps.wildness > Rand.Range(0.22f, 1.0f)
						&& !(pawn.TicksPerMoveCardinal < (Partner.TicksPerMoveCardinal / 2) && !Partner.Downed && xxx.is_not_dying(Partner)))
					{
						Partner.jobs.StopAll(); // Wake up animal if sleeping.

						float aggro = Partner.kindDef.RaceProps.manhunterOnTameFailChance;
						if (Partner.kindDef.RaceProps.predator)
							aggro += 0.2f;
						else
							aggro -= 0.1f;

						//wild animals may attack
						if (Rand.Chance(aggro) && Partner.CanSee(pawn))
						{
							Partner.rotationTracker.FaceTarget(pawn);
							LifeStageUtility.PlayNearestLifestageSound(Partner, (ls) => ls.soundAngry, null, 1.4f);
							ThrowMetaIconF(Partner.Position, Partner.Map, FleckDefOf.IncapIcon);
							ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_ColonistFleeing); //red '!'
							Partner.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter);
							if (Partner.kindDef.RaceProps.herdAnimal && Rand.Chance(0.2f))
							{ // 20% chance of turning the whole herd hostile...
								List<Pawn> packmates = Partner.Map.mapPawns.AllPawnsSpawned.Where(x =>
									x != Partner && x.def == Partner.def && x.Faction == Partner.Faction &&
									x.Position.InHorDistOf(Partner.Position, 24f) && x.CanSee(Partner)).ToList();

								foreach (Pawn packmate in packmates)
								{
									packmate.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter);
								}
							}
							Messages.Message(pawn.Name.ToStringShort + " is being attacked by " + xxx.get_pawnname(Partner) + ".", pawn, MessageTypeDefOf.ThreatSmall);
						}
						//wild animals may flee
						else
						{
							ThrowMetaIcon(Partner.Position, Partner.Map, ThingDefOf.Mote_ColonistFleeing);
							LifeStageUtility.PlayNearestLifestageSound(Partner, (ls) => ls.soundCall, g => g.soundCall);
							Partner.mindState.StartFleeingBecauseOfPawnAction(pawn);
							Partner.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.PanicFlee);
						}
						pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
					}
					else
					{
						Job gettin_bred = JobMaker.MakeJob(PartnerJob, pawn, Partner);
						Partner.jobs.StartJob(gettin_bred, JobCondition.InterruptForced, null, true);
					}
				}
			};
			yield return StartPartnerJob;

			Toil SexToil = new Toil();
			SexToil.defaultCompleteMode = ToilCompleteMode.Never;
			SexToil.defaultDuration = duration;
			SexToil.handlingFacing = true;
			SexToil.FailOn(() => Partner.CurJob?.def != PartnerJob);
			SexToil.initAction = delegate
			{
				Partner.pather.StopDead();
				Partner.jobs.curDriver.asleep = false;
				Start();
			};
			SexToil.tickAction = delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					if (xxx.is_zoophile(pawn))
						ThrowMetaIconF(pawn.Position, pawn.Map, FleckDefOf.Heart);
					else
						ThrowMetaIconF(pawn.Position, pawn.Map, xxx.mote_noheart);
				SexTick(pawn, Partner);
				//no hitting wild animals, and getting rect by their Manhunter
				/*
				if (pawn.IsHashIntervalTick (ticks_between_hits))
					roll_to_hit (pawn, Partner);
					*/
				SexUtility.reduce_rest(Partner, 1);
				SexUtility.reduce_rest(pawn, 2);
				if (ticks_left <= 0)
					ReadyForNextToil();
			};
			SexToil.AddFinishAction(delegate
			{
				End();
			});
			yield return SexToil;

			yield return new Toil
			{
				initAction = delegate
				{
					//ModLog.Message(" JobDriver_BestialityForMale::MakeNewToils() - creating aftersex toil");
					SexUtility.ProcessSex(Sexprops);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}

		[SyncMethod]
		private Toil TalkToAnimal(Pawn pawn, Pawn animal)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				pawn.interactions.TryInteractWith(animal, SexUtility.AnimalSexChat);
			};
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			toil.defaultCompleteMode = ToilCompleteMode.Delay;
			toil.defaultDuration = Rand.Range(120, 220);
			return toil;
		}
	}
}
