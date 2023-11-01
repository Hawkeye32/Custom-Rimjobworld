using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_UseItemOn : JobDriver_UseItem
	{
		public static Toil pickup_item(Pawn p, Thing item)
		{
			return new Toil
			{
				initAction = delegate
				{
					p.carryTracker.TryStartCarry(item, 1);
					if (item.Spawned) // If the item is still spawned that means the pawn failed to pick it up
						p.jobs.curDriver.EndJobWith(JobCondition.Incompletable);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}

		protected TargetIndex iitem = TargetIndex.A;

		protected TargetIndex itar = TargetIndex.B;

		protected Thing item
		{
			get
			{
				return base.job.GetTarget(iitem).Thing;
			}
		}

		protected Thing tar
		{
			get
			{
				return base.job.GetTarget(itar).Thing;
			}
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (tar == null)
				foreach (var toil in base.MakeNewToils())
					yield return toil;
			else
			{
				// Find the pawn to use the item on.
				Pawn other;
				{
					var corpse = tar as Corpse;
					other = (corpse == null) ? (Pawn)tar : corpse.InnerPawn;
				}

				this.FailOnDespawnedNullOrForbidden(itar);
				if (!other.Dead)
					this.FailOnAggroMentalState(itar);
				yield return Toils_Reserve.Reserve(itar);

				if ((pawn.inventory != null) && pawn.inventory.Contains(item))
				{
					yield return Toils_Misc.TakeItemFromInventoryToCarrier(pawn, iitem);
				}
				else
				{
					yield return Toils_Reserve.Reserve(iitem);
					yield return Toils_Goto.GotoThing(iitem, PathEndMode.ClosestTouch).FailOnForbidden(iitem);
					yield return pickup_item(pawn, item);
				}

				yield return Toils_Goto.GotoThing(itar, PathEndMode.Touch);

				yield return new Toil
				{
					initAction = delegate
					{
						if (!other.Dead)
							PawnUtility.ForceWait(other, 60);
					},
					defaultCompleteMode = ToilCompleteMode.Delay,
					defaultDuration = 60
				};

				yield return new Toil
				{
					initAction = delegate
					{
						var effective_item = item;

						// Drop the item if it's some kind of apparel. This is because ApparelTracker.Wear only works properly
						// if the apparel to wear is spawned. (I'm just assuming that DoEffect for apparel wears it, which is
						// true for bondage gear)
						if ((effective_item as Apparel) != null)
						{
							Thing dropped_thing;
							if (pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out dropped_thing))
								effective_item = dropped_thing as Apparel;
							else
							{
								ModLog.Error("Unable to drop " + effective_item.Label + " for use on " + xxx.get_pawnname(other) + " (apparel must be dropped before use)");
								effective_item = null;
							}
						}

						if (effective_item != null)
						{
							var eff = effective_item.TryGetComp<CompUseEffect>();
							if (eff != null)
								eff.DoEffect(other);
							else
								ModLog.Error("Unable to get CompUseEffect for use of " + effective_item.Label + " on " + xxx.get_pawnname(other) + " by " + xxx.get_pawnname(pawn));
						}
					},
					defaultCompleteMode = ToilCompleteMode.Instant
				};
			}
		}
	}
}