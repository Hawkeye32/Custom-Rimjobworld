using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;

namespace rjw
{
	public abstract class JobDriver_SexBaseInitiator : JobDriver_Sex
	{

		public void Start()
		{
			bool isWhoring = false;
			bool isRape = false;
			var receiverDriver = Partner?.jobs?.curDriver as JobDriver_SexBaseReciever;

			isEndytophile = xxx.has_quirk(pawn, "Endytophile");
			isAnimalOnAnimal = xxx.is_animal(pawn) && xxx.is_animal(Partner);

			if (Partner == null || Partner == pawn)
			{
				// HERE COULD BE YOUR AD
			}
			else if (Partner.Dead)
			{
				isRape = true;
			}
			else if (receiverDriver is not null)
			{
				receiverDriver.parteners.AddDistinct(pawn);

				//prevent downed Receiver standing up and interrupting rape
				if (Partner.health.hediffSet.HasHediff(xxx.submitting))
					Partner.health.AddHediff(xxx.submitting);

				//(Target.jobs.curDriver as JobDriver_SexBaseReciever).parteners.Count; //TODO: add multipartner support so sex doesn't repeat, maybe, someday
				isRape = Partner?.CurJob.def == xxx.gettin_raped;
				isWhoring = pawn?.CurJob.def == xxx.whore_is_serving_visitors;

				//toggles
				NymphSucc();
				RoMSucc();
				NISucc();
			}

			if (Sexprops == null)
			{
				//Log.Message("rulePack1: " + Sexprops);
				Sexprops = pawn.GetRMBSexPropsCache();
				//Log.Message("rulePack2: " + Sexprops);
				pawn.GetRJWPawnData().SexProps = null;
				//Log.Message("rulePack3: " + Sexprops);
				//Log.Message("rulePack4: " + pawn.GetRJWPawnData().SexProps);

				if (Sexprops == null)
				{
					Sexprops = SexUtility.SelectSextype(pawn, Partner, isRape, isWhoring);
					Sexprops.isRapist = isRape;
					Sexprops.isWhoring = isWhoring;
				}
				var interaction = Modules.Interactions.Helpers.InteractionHelper.GetWithExtension(Sexprops.dictionaryKey);
				Sexprops.isRevese = interaction.HasInteractionTag(InteractionTag.Reverse);
			}

			if (receiverDriver is not null && receiverDriver.parteners.Count == 1 && receiverDriver.Sexprops == null)
			{
				receiverDriver.Sexprops = new SexProps()
				{
					pawn = Partner,
					partner = pawn,
					sexType = Sexprops.sexType,
					dictionaryKey = Sexprops.dictionaryKey,
					usedCondom = Sexprops.usedCondom,
					isRape = isRape,
					isReceiver = true,
					isRevese = Sexprops.isRevese
				};
			}
			SexUtility.LogSextype(Sexprops.pawn, Sexprops.partner, Sexprops.rulePack, Sexprops.dictionaryKey);
		}

		//public void Change(xxx.rjwSextype sexType)
		//{
		//	if (pawn.jobs?.curDriver is JobDriver_SexBaseInitiator)
		//	{
		//		(pawn.jobs.curDriver as JobDriver_SexBaseInitiator).increase_time(duration);
		//		Sexprops = SexUtility.SelectSextype(pawn, Partner, isRape, isWhoring, Partner);
		//		sexType = Sexprops.SexType;
		//		SexUtility.LogSextype(Sexprops.Giver, Sexprops.Reciever, Sexprops.RulePack, Sexprops.DictionaryKey);
		//	}
		//	if (Partner.jobs?.curDriver is JobDriver_SexBaseReciever)
		//	{
		//		(Partner.jobs.curDriver as JobDriver_SexBaseReciever).increase_time(duration);
		//		Sexprops = SexUtility.SelectSextype(pawn, Partner, isRape, isWhoring, Partner);
		//		sexType = Sexprops.SexType;
		//		SexUtility.LogSextype(Sexprops.Giver, Sexprops.Reciever, Sexprops.RulePack, Sexprops.DictionaryKey);
		//	}
		//	sexType = sexType
		//}

		public void End()
		{
			if (xxx.is_human(pawn))
				pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
			GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);

			if (Partner?.jobs?.curDriver is JobDriver_SexBaseReciever receiverDriver)
			{
				if (receiverDriver.parteners.Count == 1 && !Sexprops.isCoreLovin)
				{
					var isPenetrative = Sexprops.sexType switch
					{
						xxx.rjwSextype.Anal => true,
						xxx.rjwSextype.Vaginal => true,
						xxx.rjwSextype.DoublePenetration => true,
						_ => false
					};
					if (!isPenetrative) goto notKnotting;

					// reverse interaction, check if penetrating partner can knot
					var ableToKnot = Sexprops.isRevese ? canKnott(Partner) : canKnott(pawn);
					if (!ableToKnot) goto notKnotting;

					pawn.jobs.jobQueue.EnqueueFirst(JobMaker.MakeJob(xxx.knotted, Partner));
					Partner.jobs.jobQueue.EnqueueFirst(JobMaker.MakeJob(xxx.knotted, pawn));
				}

			notKnotting:
				receiverDriver.parteners.Remove(pawn);
			}
		}

		public bool canKnott(Pawn pawn)
		{
			// In case of necro, dead pawns can never knot.
			if (pawn.Dead) return false;

			foreach (var y in pawn.GetGenitalsList())
				if (PartProps.TryGetProps(y, out var propslist))
					if (propslist.Contains("Knotted")) return true;
			return false;
		}

		/// <summary>
		/// non succubus focus gain
		/// </summary>
		public void NymphSucc()
		{
			if (MeditationFocusTypeAvailabilityCache.PawnCanUse(pawn, xxx.SexMeditationFocus))
			{
				shouldGainFocus = true;
				SexUtility.OffsetPsyfocus(pawn, 0.01f);
			}
			else if (xxx.is_zoophile(pawn) && xxx.is_animal(Partner) && MeditationFocusTypeAvailabilityCache.PawnCanUse(pawn, MeditationFocusDefOf.Natural))
			{
				shouldGainFocus = true;
			}

			if (MeditationFocusTypeAvailabilityCache.PawnCanUse(Partner, xxx.SexMeditationFocus))
			{
				shouldGainFocusP = true;
			}
			else if (xxx.is_zoophile(Partner) && xxx.is_animal(pawn) && MeditationFocusTypeAvailabilityCache.PawnCanUse(Partner, MeditationFocusDefOf.Natural))
			{
				shouldGainFocusP = true;
			}
		}

		/// <summary>
		/// Rimworld of Magic succubus focus gain
		/// </summary>
		public void RoMSucc()
		{
			if (xxx.RoMIsActive)
			{
				if (xxx.has_traits(pawn))
					if (pawn.story.traits.HasTrait(xxx.Succubus))
					{
						isSuccubus = true;
					}

				if (xxx.has_traits(Partner))
					if (Partner.story.traits.HasTrait(xxx.Succubus))
					{
						isSuccubusP = true;
					}
			}
		}
		/// <summary>
		/// Nightmare Incarnation succubus focus gain
		/// </summary>
		public void NISucc()
		{
			if (xxx.NightmareIncarnationIsActive)
			{
				if (xxx.has_traits(pawn))
					foreach (var x in pawn.AllComps?.Where(x => x?.props?.ToStringSafe() == "NightmareIncarnation.CompProperties_SuccubusRace"))
					{
						isSuccubus = true;
						break;
					}

				if (xxx.has_traits(Partner))
					foreach (var x in Partner.AllComps?.Where(x => x?.props?.ToStringSafe() == "NightmareIncarnation.CompProperties_SuccubusRace"))
					{
						isSuccubusP = true;
						break;
					}
			}
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			//ModLog.Message("shouldreserve " + shouldreserve);
			if (shouldreserve && Target != null)
				return pawn.Reserve(Target, job, xxx.max_rapists_per_prisoner, stackCount, null, errorOnFailed);
			else if (shouldreserve && Bed != null)
				return pawn.Reserve(Bed, job, Bed.SleepingSlotsCount, 0, null, errorOnFailed);
			else
				return true; // No reservations needed.

			//return this.pawn.Reserve(this.Partner, this.job, 1, 0, null) && this.pawn.Reserve(this.Bed, this.job, 1, 0, null);
		}
	}
}