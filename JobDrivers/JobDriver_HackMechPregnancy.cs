using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace rjw
{
	//biotech JobDriver_ControlMech
	public class JobDriver_HackMechPregnancy : JobDriver
	{
		private const TargetIndex MechInd = TargetIndex.A;

		private Pawn Mech => (Pawn)job.GetTarget(TargetIndex.A).Thing;

		private int MechControlTime => Mathf.RoundToInt(Mech.GetStatValue(StatDefOf.ControlTakingTime) * 60f);

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Mech, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (ModsConfig.BiotechActive)
			{
				this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
				yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
				yield return Toils_General.WaitWith(TargetIndex.A, MechControlTime, useProgressBar: true, maintainPosture: true, maintainSleep: false, TargetIndex.A).WithEffect(EffecterDefOf.ControlMech, TargetIndex.A);
				Toil toil = ToilMaker.MakeToil("MakeNewToils");
				toil.initAction = delegate
				{
					Hediff_MechanoidPregnancy pregnancy = (Hediff_MechanoidPregnancy)Mech.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("RJW_pregnancy_mech"));
					pregnancy.Hack();
				};
				toil.PlaySoundAtEnd(SoundDefOf.ControlMech_Complete);
				yield return toil;
			}
		}
	}
}