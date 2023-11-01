using System;
using System.Collections.Generic;
using System.Linq;
using Multiplayer.API;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_SexQuick : JobDriver_SexBaseInitiator
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, xxx.max_rapists_per_prisoner, 0, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			//ModLog.Message("" + this.GetType().ToString() + "::MakeNewToils() called");
			setup_ticks();
			var PartnerJob = xxx.getting_quickie;

			this.FailOnDespawnedNullOrForbidden(iTarget);
			this.FailOn(() => !Partner.health.capacities.CanBeAwake);
			this.FailOn(() => pawn.Drafted);

			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);

			Toil findQuickieSpot = new Toil();
			findQuickieSpot.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			findQuickieSpot.initAction = delegate
			{
				//Needs this earlier to decide if current place is good enough
				var all_pawns = pawn.Map.mapPawns.AllPawnsSpawned.Where(x
				=> x.Position.DistanceTo(pawn.Position) < 100
				&& xxx.is_human(x)
				&& x != pawn
				&& x != Partner
				).ToList();

				FloatRange temperature = pawn.ComfortableTemperatureRange();
				float cellTemp = pawn.Position.GetTemperature(pawn.Map);

				if (Partner.IsPrisonerInPrisonCell() || (!CasualSex_Helper.MightBeSeen(all_pawns, pawn.Position, pawn, Partner) && (cellTemp > temperature.min && cellTemp < temperature.max)))
				{
					ReadyForNextToil();
				}
				else
				{
					var spot = CasualSex_Helper.FindSexLocation(pawn, Partner);
					pawn.pather.StartPath(spot, PathEndMode.OnCell);
					//sometimes errors with stuff like vomiting
					//sometimes partner is null ??? no idea how to fix this
					Partner?.jobs.StopAll();
					Job job = JobMaker.MakeJob(JobDefOf.GotoMindControlled, spot);
					Partner?.jobs.StartJob(job, JobCondition.InterruptForced);
				}
			};
			yield return findQuickieSpot;

			Toil WaitForPartner = new Toil();
			WaitForPartner.defaultCompleteMode = ToilCompleteMode.Delay;
			WaitForPartner.initAction = delegate
			{
				ticksLeftThisToil = 5000;
			};
			WaitForPartner.tickAction = delegate
			{
				pawn.GainComfortFromCellIfPossible();
				if (pawn.Position.DistanceTo(Partner.Position) <= 1f)
				{
					ReadyForNextToil();
				}
			};
			yield return WaitForPartner;

			Toil StartPartnerJob = new Toil();
			StartPartnerJob.defaultCompleteMode = ToilCompleteMode.Instant;
			StartPartnerJob.socialMode = RandomSocialMode.Off;
			StartPartnerJob.initAction = delegate
			{
				Job gettingQuickie = JobMaker.MakeJob(PartnerJob, pawn, Partner);
				Partner.jobs.StartJob(gettingQuickie, JobCondition.InterruptForced);
			};
			yield return StartPartnerJob;

			Toil SexToil = new Toil();
			SexToil.defaultCompleteMode = ToilCompleteMode.Never;
			SexToil.socialMode = RandomSocialMode.Off;
			SexToil.defaultDuration = duration;
			SexToil.handlingFacing = true;
			SexToil.FailOn(() => Partner.CurJob?.def != PartnerJob);
			SexToil.initAction = delegate
			{
				Partner.pather.StopDead();
				Partner.jobs.curDriver.asleep = false;

				Start();
				Sexprops.usedCondom = CondomUtility.TryUseCondom(pawn) || CondomUtility.TryUseCondom(Partner);
			};
			SexToil.AddPreTickAction(delegate
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					ThrowMetaIconF(pawn.Position, pawn.Map, FleckDefOf.Heart);
				SexTick(pawn, Partner);
				SexUtility.reduce_rest(Partner, 1);
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
