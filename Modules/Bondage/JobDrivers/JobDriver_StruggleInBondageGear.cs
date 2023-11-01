using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_StruggleInBondageGear : JobDriver
	{
		public Apparel target_gear
		{
			get
			{
				return (Apparel)TargetA.Thing;
			}
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.target_gear, this.job, 1, -1, null, errorOnFailed);
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return new Toil
			{
				initAction = delegate
				{
					pawn.pather.StopDead();
				},
				defaultCompleteMode = ToilCompleteMode.Delay,
				defaultDuration = 60
			};
			yield return new Toil
			{
				initAction = delegate
				{
					if (PawnUtility.ShouldSendNotificationAbout(pawn))
					{
						var pro = (pawn.gender == Gender.Male) ? "his" : "her";
						Messages.Message(xxx.get_pawnname(pawn) + " struggles to remove " + pro + " " + target_gear.def.label + ". It's no use!", pawn, MessageTypeDefOf.NegativeEvent);
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}