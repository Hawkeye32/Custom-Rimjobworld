using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	public class JobDriver_Knotted : JobDriver_Goto
	{
		/// <summary>
		/// Number of ticks to smooth out the start and end transitions.
		/// </summary>
		const int TweenTicks = 10;

		/// <summary>
		/// The pawn's partner.
		/// </summary>
		public Pawn Partner => job.GetTarget(TargetIndex.A).Pawn;

		/// <summary>
		/// Set this to apply a positional offset to this pawn.
		/// </summary>
		private Vector3 forcedBodyOffset = Vector3.zero;
		public override Vector3 ForcedBodyOffset => forcedBodyOffset;

		protected override IEnumerable<Toil> MakeNewToils()
		{
			if (job.def.waitAfterArriving <= 0)
				yield break;

			var waiter = Toils_General.Wait(job.def.waitAfterArriving);

			// TODO: Deal with clothing better.  If the pawn was fucking without
			// clothes, they should still be drawn nude.  Since tying is a separate
			// job and we don't know if this was something like an animal "breeding"
			// someone in their clothing, it isn't straightforward to tell when
			// they should remain nude.  This is probably a sign this should not
			// have been a separate job but actually an extra toil of sex jobs.

			// Just in case.  Animals may only pant, whine, or tug during a tie...
			if (xxx.is_animal(pawn))
				waiter.socialMode = RandomSocialMode.Off;

			// We should have a partner set.  If we don't, use fallback behavior
			// of just waiting (which is how knotting used to work), with none
			// of the partner related enhancements.
			if (Partner is not Pawn partner)
			{
				ModLog.Warning($"Knotting job for {xxx.get_pawnname(pawn)} did not receive a partner.");
				yield return waiter;
				yield break;
			}

			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOnDowned(TargetIndex.A);

			// Detect when the tie breaks early.  This can happen for a number of
			// reasons, but I often saw it when an animal starts being trained
			// during the tie.
			this.FailOn(() => !IsCouplingValid());

			// If another mod patches this method, they can use it to add more
			// complex animation or positioning to the waiter.
			AddAnimation(waiter);

			yield return waiter;
		}

		/// <summary>
		/// <para>Applies some special animaion handling during the tie.</para>
		/// <para>Animation systems can patch this to attach their own actions to
		/// enable more complex animation.</para>
		/// </summary>
		/// <param name="waiter">The wait toil.</param>
		void AddAnimation(Toil waiter)
		{
			if (Partner is not Pawn partner) return;

			// Only if the tie involves an animal will we turn the tie butt-to-butt.
			// Human-likes with knots will just hold their partner close.  <3
			if (!InvolvesAnimal()) return;

			var initTicks = waiter.defaultDuration;
			var remTicks = initTicks;

			waiter.handlingFacing = true;
			waiter.AddPreTickAction(delegate ()
			{
				pawn.rotationTracker.Face(partner.DrawPos);
				pawn.Rotation = pawn.Rotation.Opposite;

				var newOffset = GetOffset();
				if (newOffset == Vector3.zero) return;

				// Tween the offset at the start and end.
				// Can't really smooth it out for an interrupted tie, though.
				var tweenTicks = Math.Min(TweenTicks, Math.Min(initTicks - remTicks, remTicks));
				var curTween = (float)tweenTicks / TweenTicks;
				forcedBodyOffset = newOffset * curTween;

				remTicks -= 1;
			});
		}

		/// <summary>
		/// Gets an offset for the current pawn to improve the visuals.
		/// </summary>
		/// <returns>An offset to apply.</returns>
		public Vector3 GetOffset()
		{
			// Must be an animal.
			if (!xxx.is_animal(pawn))
				return Vector3.zero;

			// Partner must exist and be human.
			if (Partner is not Pawn partner || xxx.is_animal(partner))
				return Vector3.zero;

			// Only applicable for east or west facings.
			if (pawn.Rotation != Rot4.East && pawn.Rotation != Rot4.West)
				return Vector3.zero;

			// Nudge the animal for a better lineup.
			var offset = partner.BodySize * 0.15f;
			var sign = pawn.Rotation == Rot4.West ? -1f : 1f;
			return new Vector3(sign * offset, 0f, -0.2f);
		}

		/// <summary>
		/// Whether this tie involves at least one animal.
		/// </summary>
		/// <returns>Whether the partner and/or pawn is an animal.</returns>
		public bool InvolvesAnimal() => xxx.is_animal(pawn) || xxx.is_animal(Partner);

		/// <summary>
		/// <para>Makes sure that the designated partner is still coupled to this pawn.</para>
		/// <para>For hardiness, this actually checks the partner pawn to see if they're
		/// using either `JobDriver_Sex` or `JobDriver_Knotted`, and the partner is still
		/// this pawn.</para>
		/// </summary>
		/// <returns>Whether the coupling is valid.</returns>
		public bool IsCouplingValid()
		{
			if (Partner is not Pawn partner) return false;

			var result = partner.jobs.curDriver switch
			{
				JobDriver_Knotted knottedDriver => knottedDriver.Partner == pawn,
				JobDriver_Sex sexDriver => sexDriver.Partner == pawn,
				_ => false
			};
			if (result) return true;

			// It's possible our partner is queued to tie, but between jobs.
			if (partner.jobs.jobQueue.FirstOrDefault()?.job is not { } job) return false;
			if (job.def != xxx.knotted) return false;
			return job.AnyTargetIs(pawn);
		}
	}
}
