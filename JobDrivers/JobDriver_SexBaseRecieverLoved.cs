using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using System;

namespace rjw
{
	public class JobDriver_SexBaseRecieverLoved : JobDriver_SexBaseReciever
	{
		protected override void DoSetup()
		{
			base.DoSetup();

			// More/less hearts based on opinion.
			try
			{
				if (pawn.relations.OpinionOf(Partner) < 0)
					ticks_between_hearts += 50;
				else if (pawn.relations.OpinionOf(Partner) > 60)
					ticks_between_hearts -= 25;
			}
			catch
			{
				ModLog.Warning("Failed to resolve pawn relations, if on save load, this shouldn't matter too much.");
			}

			// For consensual sex, drafting the recipient will interrupt the job.
			this.FailOn(() => pawn.Drafted);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			DoSetup();
			// ModLog.Message("JobDriver_GettinLoved::MakeNewToils is called");
			// ModLog.Message("" + Partner.CurJob.def);

			if (Partner.CurJob.def == xxx.casual_sex) // sex in bed
			{
				this.KeepLyingDown(iBed);
				yield return Toils_Reserve.Reserve(iTarget, 1, 0);
				yield return Toils_Reserve.Reserve(iBed, Bed.SleepingSlotsCount, 0);

				var get_loved = MakeSexToil();
				get_loved.FailOn(() => Partner.CurJob?.def != xxx.casual_sex);
				yield return get_loved;
			}
			else if (Partner.CurJob.def == xxx.quick_sex)
			{
				yield return Toils_Reserve.Reserve(iTarget, 1, 0);

				var get_loved = MakeSexToil();
				get_loved.handlingFacing = false;
				yield return get_loved;
			}
			else if (Partner.CurJob.def == xxx.whore_is_serving_visitors)
			{
				this.FailOn(() => Partner.CurJob == null);
				yield return Toils_Reserve.Reserve(iTarget, 1, 0);

				var get_loved = MakeSexToil();
				get_loved.FailOn(() => Partner.CurJob?.def != xxx.whore_is_serving_visitors);
				yield return get_loved;
			}
			else if (Partner.CurJob.def == xxx.bestialityForFemale)
			{
				this.FailOn(() => Partner.CurJob == null);
				yield return Toils_Reserve.Reserve(iTarget, 1, 0);

				var get_loved = MakeSexToil();
				get_loved.FailOn(() => Partner.CurJob?.def != xxx.bestialityForFemale);
				yield return get_loved;
			}
			else if (Partner.CurJob.def == xxx.animalMate)
			{
				this.FailOn(() => Partner.CurJob == null);
				yield return Toils_Reserve.Reserve(iTarget, 1, 0);

				var get_loved = MakeSexToil();
				get_loved.FailOn(() => Partner.CurJob?.def != xxx.animalMate);
				yield return get_loved;
			}
		}

		private Toil MakeSexToil()
		{
			// Any other consensual sex besides casual sex is in a bed.
			var get_loved
				= Partner.CurJob.def != xxx.casual_sex ? new Toil()
				: Toils_LayDown.LayDown(iBed, true, false, false, false);

			get_loved.defaultCompleteMode = ToilCompleteMode.Never;
			get_loved.socialMode = RandomSocialMode.Off;
			get_loved.handlingFacing = true;
			get_loved.tickAction = () =>
			{
				if (pawn.IsHashIntervalTick(ticks_between_hearts))
					ThrowMetaIconF(pawn.Position, pawn.Map, FleckDefOf.Heart);
			};
			get_loved.AddFinishAction(() =>
			{
				if (xxx.is_human(pawn))
					pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
				GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
			});

			return get_loved;
		}
	}

	// Used by the quickie job for some reason.  Future expansion?
	public class JobDriver_SexBaseRecieverQuickie : JobDriver_SexBaseRecieverLoved { }
}