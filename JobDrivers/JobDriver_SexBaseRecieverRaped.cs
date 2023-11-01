using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_SexBaseRecieverRaped : JobDriver_SexBaseReciever
	{
		protected override IEnumerable<Toil> MakeNewToils()
		{
			DoSetup();

			var get_raped = new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Never,
				handlingFacing = true,
				socialMode = RandomSocialMode.Off,
				initAction = () =>
				{
					pawn.pather.StopDead();
					pawn.jobs.curDriver.asleep = false;

					SexUtility.BeeingRapedAlert(Partner, pawn);
				},
				tickAction = () =>
				{
					if (parteners.Count <= 0) return;
					if (!pawn.IsHashIntervalTick(ticks_between_hearts / parteners.Count)) return;

					if (xxx.is_masochist(pawn) || xxx.is_psychopath(pawn))
						ThrowMetaIconF(pawn.Position, pawn.Map, FleckDefOf.Heart);
					else
						ThrowMetaIconF(pawn.Position, pawn.Map, xxx.mote_noheart);
				}
			};
			get_raped.AddFinishAction(() =>
			{
				if (xxx.is_human(pawn))
					pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
				GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);

				if (Bed != null && pawn.Downed)
				{
					Job toBed = JobMaker.MakeJob(JobDefOf.Rescue, pawn, Bed);
					toBed.count = 1;
					Partner.jobs.jobQueue.EnqueueFirst(toBed);
					//Log.Message(xxx.get_pawnname(Initiator) + ": job tobed:" + tobed);
				}
				else if (pawn.HostileTo(Partner))
					pawn.health.AddHediff(xxx.submitting);
				else if (RJWSettings.rape_beating)
					pawn.stances.stunner.StunFor(600, pawn);
			});
			
			yield return get_raped;
		}
	}
}