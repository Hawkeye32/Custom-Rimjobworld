using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_SexBaseReciever : JobDriver_Sex
	{
		//give this poor driver some love other than (Partner.jobs?.curDriver is JobDriver_SexBaseReciever)
		public List<Pawn> parteners = new();

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref parteners, "parteners", LookMode.Reference);
		}

		/// <summary>
		/// Adds a predicate that a partner must fulfill to be considered valid.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		public void AddPartnerCondition(Predicate<Pawn> predicate) =>
			this.FailOn(() => parteners.Count > 0 && !parteners.Any(predicate));

		protected virtual void DoSetup()
		{
			setup_ticks();

			// Add sex initiator, so this wont fail before they start their job.
			parteners.AddDistinct(Partner);

			// Handles ending the job when no partners remain.
			AddEndCondition(() => parteners.Count <= 0 ? JobCondition.Succeeded : JobCondition.Ongoing);

			// These make sure at least one partner is still valid.  In a perfect world,
			// the partners will properly remove themselves when they're no longer
			// trying to screw this pawn ...but this is not a perfect world.
			AddPartnerCondition(MustBeSpawned);
			AddPartnerCondition(MustBeAwake);
			AddPartnerCondition(MustNotBeDrafted);
			AddPartnerCondition(MustBeMySexInitiator);
		}

		/// <summary>
		/// Checks that the partner is actually spawned on this map.
		/// </summary>
		protected bool MustBeSpawned(Pawn partner) =>
			partner.Spawned && partner.Map == pawn.Map;

		/// <summary>
		/// Checks that the partner is capable of being awake.
		/// </summary>
		protected bool MustBeAwake(Pawn partner) =>
			partner.health.capacities.CanBeAwake;

		/// <summary>
		/// Checks that the partner is not drafted.
		/// </summary>
		protected bool MustNotBeDrafted(Pawn partner) =>
			!partner.Drafted;
		
		/// <summary>
		/// Checks that the partner is still trying to fuck this driver's pawn.
		/// </summary>
		protected bool MustBeMySexInitiator(Pawn partner) =>
			(partner.jobs.curDriver as JobDriver_SexBaseInitiator)?.Partner == pawn;
	}
}