using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Multiplayer.API;
using System.Collections.Generic;

namespace rjw
{
	public static class AfterSexUtility
	{
		//aftersex thoughts
		public static readonly ThoughtDef got_raped = DefDatabase<ThoughtDef>.GetNamed("GotRaped");
		public static readonly ThoughtDef got_anal_raped = DefDatabase<ThoughtDef>.GetNamed("GotAnalRaped");
		public static readonly ThoughtDef got_anal_raped_byfemale = DefDatabase<ThoughtDef>.GetNamed("GotAnalRapedByFemale");
		public static readonly ThoughtDef got_raped_unconscious = DefDatabase<ThoughtDef>.GetNamed("GotRapedUnconscious");
		public static readonly ThoughtDef masochist_got_raped_unconscious = DefDatabase<ThoughtDef>.GetNamed("MasochistGotRapedUnconscious");

		public static readonly ThoughtDef got_bred = DefDatabase<ThoughtDef>.GetNamed("GotBredByAnimal");
		public static readonly ThoughtDef got_anal_bred = DefDatabase<ThoughtDef>.GetNamed("GotAnalBredByAnimal");
		public static readonly ThoughtDef got_licked = DefDatabase<ThoughtDef>.GetNamed("GotLickedByAnimal");
		public static readonly ThoughtDef got_groped = DefDatabase<ThoughtDef>.GetNamed("GotGropedByAnimal");

		public static readonly ThoughtDef masochist_got_raped = DefDatabase<ThoughtDef>.GetNamed("MasochistGotRaped");
		public static readonly ThoughtDef masochist_got_anal_raped = DefDatabase<ThoughtDef>.GetNamed("MasochistGotAnalRaped");
		public static readonly ThoughtDef masochist_got_anal_raped_byfemale = DefDatabase<ThoughtDef>.GetNamed("MasochistGotAnalRapedByFemale");
		public static readonly ThoughtDef masochist_got_bred = DefDatabase<ThoughtDef>.GetNamed("MasochistGotBredByAnimal");
		public static readonly ThoughtDef masochist_got_anal_bred = DefDatabase<ThoughtDef>.GetNamed("MasochistGotAnalBredByAnimal");
		public static readonly ThoughtDef masochist_got_licked = DefDatabase<ThoughtDef>.GetNamed("MasochistGotLickedByAnimal");
		public static readonly ThoughtDef masochist_got_groped = DefDatabase<ThoughtDef>.GetNamed("MasochistGotGropedByAnimal");
		public static readonly ThoughtDef allowed_animal_to_breed = DefDatabase<ThoughtDef>.GetNamed("AllowedAnimalToBreed");
		public static readonly ThoughtDef allowed_animal_to_lick = DefDatabase<ThoughtDef>.GetNamed("AllowedAnimalToLick");
		public static readonly ThoughtDef allowed_animal_to_grope = DefDatabase<ThoughtDef>.GetNamed("AllowedAnimalToGrope");
		public static readonly ThoughtDef zoophile_got_bred = DefDatabase<ThoughtDef>.GetNamed("ZoophileGotBredByAnimal");
		public static readonly ThoughtDef zoophile_got_anal_bred = DefDatabase<ThoughtDef>.GetNamed("ZoophileGotAnalBredByAnimal");
		public static readonly ThoughtDef zoophile_got_licked = DefDatabase<ThoughtDef>.GetNamed("ZoophileGotLickedByAnimal");
		public static readonly ThoughtDef zoophile_got_groped = DefDatabase<ThoughtDef>.GetNamed("ZoophileGotGropedByAnimal");
		public static readonly ThoughtDef hate_my_rapist = DefDatabase<ThoughtDef>.GetNamed("HateMyRapist");
		public static readonly ThoughtDef kinda_like_my_rapist = DefDatabase<ThoughtDef>.GetNamed("KindaLikeMyRapist");
		public static readonly ThoughtDef allowed_me_to_get_raped = DefDatabase<ThoughtDef>.GetNamed("AllowedMeToGetRaped");
		public static readonly ThoughtDef stole_some_lovin = DefDatabase<ThoughtDef>.GetNamed("StoleSomeLovin");
		public static readonly ThoughtDef bloodlust_stole_some_lovin = DefDatabase<ThoughtDef>.GetNamed("BloodlustStoleSomeLovin");
		public static readonly ThoughtDef violated_corpse = DefDatabase<ThoughtDef>.GetNamed("ViolatedCorpse");
		public static readonly ThoughtDef gave_virginity = DefDatabase<ThoughtDef>.GetNamed("FortunateGaveVirginity");
		public static readonly ThoughtDef lost_virginity = DefDatabase<ThoughtDef>.GetNamed("UnfortunateLostVirginity");
		public static readonly ThoughtDef took_virginity = DefDatabase<ThoughtDef>.GetNamed("TookVirginity");

		//violent - mark true when pawn rape partner
		//Note: violent is not reliable, since either pawn could be the rapist. Check jobdrivers instead, they're still active since this is called before ending the job.
		public static void think_about_sex(SexProps props)
		{
			Pawn receiving = props.partner;//TODO: replace this shit at some point when interaction has reversed roles
			think_about_sex(props.pawn, props.partner, receiving == props.pawn, props, props.isWhoring);
			think_about_sex(props.partner, props.pawn, receiving == props.partner, props, false);
		}
		public static void think_about_sex(Pawn pawn, Pawn partner, bool isReceiving, SexProps props, bool whoring = false)
		{
			// Partner should never be null, but just in case something gets changed elsewhere..
			if (partner == null)
			{
				ModLog.Error("xxx::think-after_sex( ERROR: " + xxx.get_pawnname(pawn) + " has no partner. This should not be called from solo acts. Sextype: " + props.sexType);
				return;
			}

			// Both pawns are now checked individually, instead of giving thoughts to the partner.
			//Can just return if the currently checked pawn is dead or can't have thoughts, which simplifies the checks.
			if (pawn.Dead || !xxx.is_human(pawn))
				return;

			bool masochist = xxx.is_masochist(pawn);
			bool zoophile = xxx.is_zoophile(pawn);
			bool guilty = props.canBeGuilty;
			if (HasLoveEnhancer(pawn, partner))
			{
				masochist = true;
				zoophile = true;
			}
			if (masochist)
			{
				if (RJWSettings.rape_beating)
					pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.HarmedMe, partner);
			}
			if (!masochist)
			{
				//TODO: someday rough sex?
				//foreach (DirectPawnRelation relation in pawn.relations.DirectRelations)
				//{
				//	if (relation.otherPawn == partner)
				//		if (relation.def == PawnRelationDefOf.Lover ||
				//			relation.def == PawnRelationDefOf.Fiance ||
				//			relation.def == PawnRelationDefOf.Spouse)
				//		{
				//			masochist = true;
				//			break;
				//		}
				//}
				//if (pawn.IsPrisoner || masochist || is_slave(pawn) || pawn.IsDesignatedComfort())
				if (pawn.IsPrisoner || xxx.is_slave(pawn) || pawn.IsDesignatedComfort())
				{
					guilty = false;
				}
			}
			else
				guilty = false;

			ThoughtDef thoughtgain = null;
			//unconscious pawns has no thoughts
			//and if they has sex, its probably rape, since they have no choice
			if (!pawn.health.capacities.CanBeAwake)
			{
				thoughtgain = xxx.is_masochist(pawn) ? masochist_got_raped_unconscious : got_raped_unconscious;
				pawn.needs.mood.thoughts.memories.TryGainMemory(thoughtgain);
				return;
			}

			// TODO: refactor all these repeated arguments to be a `ref struct`.
			// This would be a breaking change, though.

			thoughtgain = think_about_sex_Bestiality(pawn, partner, isReceiving, props, whoring, masochist, zoophile, guilty);

			if (thoughtgain == null)
				thoughtgain = think_about_sex_Rapist(pawn, partner, isReceiving, props, whoring, masochist, zoophile, guilty);

			if (thoughtgain == null)
				thoughtgain = think_about_sex_Victim(pawn, partner, isReceiving, props, whoring, masochist, zoophile, guilty);

			if (thoughtgain == null)
				thoughtgain = think_about_sex_Consensual(pawn, partner, isReceiving, props, whoring, masochist, zoophile, guilty);

			if (thoughtgain != null)
			{
				var newMemory = FinalizeThought(thoughtgain, pawn, partner, isReceiving, props, whoring, masochist, zoophile, guilty);
				pawn.needs.mood?.thoughts.memories.TryGainMemory(newMemory, partner);
			}
		}

		public static Thought_Memory FinalizeThought(
			ThoughtDef thoughtgain,
			Pawn pawn,
			Pawn partner,
			bool isReceiving,
			SexProps props,
			bool whoring,
			bool masochist,
			bool zoophile,
			bool guilty
		  )
		{
			var newMemory = (Thought_Memory)ThoughtMaker.MakeThought(thoughtgain);

			// Apply mood boost for vanilla lovin' from love enhancer.
			if (thoughtgain == ThoughtDefOf.GotSomeLovin && HasLoveEnhancer(pawn, partner))
				newMemory.moodPowerFactor = 1.5f;

			return newMemory;
		}

		public static ThoughtDef think_about_sex_Bestiality(
			Pawn pawn,
			Pawn partner,
			bool isReceiving,
			SexProps props,
			bool whoring,
			bool masochist,
			bool zoophile,
			bool guilty
			)
		{

			//ModLog.Message("xxx::think-after_sex( ERROR: 3");
			// Thoughts for animal-on-colonist.
			ThoughtDef thoughtDef = null;
			if (xxx.is_animal(partner) && isReceiving)
			{
				if (!zoophile && !props.isRape)
				{
					//ModLog.Message("xxx::think-after_sex( ERROR: 3.1");
					if (props.sexType == xxx.rjwSextype.Oral)
						thoughtDef = (allowed_animal_to_lick);
					else if (props.sexType == xxx.rjwSextype.Anal || props.sexType == xxx.rjwSextype.Vaginal)
						thoughtDef = (allowed_animal_to_breed);
					else //Other rarely seen sex types, such as fingering (by primates, monster girls, etc)
						thoughtDef = (allowed_animal_to_grope);
				}
				else
				{
					//ModLog.Message("xxx::think-after_sex( ERROR: 3.2");
					if (!zoophile)
					{
						if (props.sexType == xxx.rjwSextype.Oral)
							thoughtDef = (masochist ? masochist_got_licked : got_licked);
						else if (props.sexType == xxx.rjwSextype.Vaginal)
							thoughtDef = (masochist ? masochist_got_bred : got_bred);
						else if (props.sexType == xxx.rjwSextype.Anal)
							thoughtDef = (masochist ? masochist_got_anal_bred : got_anal_bred);
						else //Other types
							thoughtDef = (masochist ? masochist_got_groped : got_groped);
					}
					else
					{
						if (props.sexType == xxx.rjwSextype.Oral)
							thoughtDef = (zoophile_got_licked);
						else if (props.sexType == xxx.rjwSextype.Vaginal)
							thoughtDef = (zoophile_got_bred);
						else if (props.sexType == xxx.rjwSextype.Anal)
							thoughtDef = (zoophile_got_anal_bred);
						else //Other types
							thoughtDef = (zoophile_got_groped);
					}
				}
				if (RJWSettings.DevMode) ModLog.Message("This being Called?");
				think_about_sex_Bestiality_TameAttempt(pawn, partner, isReceiving, props, whoring, masochist, zoophile, guilty);
			}
			return thoughtDef;
		}

		public static void think_about_sex_Bestiality_TameAttempt(
			Pawn pawn,
			Pawn partner,
			bool isReceiving,
			SexProps props,
			bool whoring,
			bool masochist,
			bool zoophile,
			bool guilty
			)
		{
			if (RJWSettings.DevMode) ModLog.Message("This being Called again?");
			if (pawn.CurJob != null)
					if (!partner.Dead && zoophile && (pawn.CurJob.def != xxx.gettin_raped) && partner.Faction == null && pawn.Faction == Faction.OfPlayer)
					{
						
						InteractionDef intDef = !partner.AnimalOrWildMan() ? InteractionDefOf.RecruitAttempt : InteractionDefOf.TameAttempt;
						pawn.interactions.TryInteractWith(partner, intDef);
					}
		}

		public static ThoughtDef think_about_sex_Rapist(
			Pawn pawn,
			Pawn partner,
			bool isReceiving,
			SexProps props,
			bool whoring,
			bool masochist,
			bool zoophile,
			bool guilty
			)
		{
			ThoughtDef thoughtDef = null;
			if (partner.Dead || (partner.CurJob != null && partner.CurJob.def == xxx.gettin_raped) || pawn.jobs?.curDriver is JobDriver_Rape)
			{
				thoughtDef = (xxx.is_rapist(pawn) || xxx.is_bloodlust(pawn) || xxx.is_psychopath(pawn)) ? bloodlust_stole_some_lovin : stole_some_lovin;

				if ((xxx.is_necrophiliac(pawn) || xxx.is_psychopath(pawn)) && partner.Dead)
				{
					thoughtDef = (violated_corpse);
				}
			}
			return thoughtDef;
		}

		public static ThoughtDef think_about_sex_Victim(
			Pawn pawn,
			Pawn partner,
			bool isReceiving,
			SexProps props,
			bool whoring,
			bool masochist,
			bool zoophile,
			bool guilty
			)
		{
			ThoughtDef thoughtDef = null;
			if (pawn.CurJob != null && pawn.CurJob.def == xxx.gettin_raped || partner.jobs?.curDriver is JobDriver_Rape)
			{
				if (xxx.is_human(partner))
				{
					if (props.sexType == xxx.rjwSextype.Anal)
					{
						if (xxx.is_female(partner))
						{
							thoughtDef = masochist ? masochist_got_anal_raped_byfemale : got_anal_raped_byfemale;
						}
						else
						{
							thoughtDef = masochist ? masochist_got_anal_raped : got_anal_raped;
						}
					}
					else
					{
						thoughtDef = masochist ? masochist_got_raped : got_raped;
					}

					ThoughtDef pawn_thought_about_rapist = masochist ? kinda_like_my_rapist : hate_my_rapist;
					pawn.needs.mood.thoughts.memories.TryGainMemory(pawn_thought_about_rapist, partner);
					if (guilty)
						partner.guilt.Notify_Guilty(GenDate.TicksPerDay);

				}

				think_about_sex_Victim_Blame(pawn, partner, isReceiving, props, whoring, masochist, zoophile, guilty);
			}
			return thoughtDef;
		}
		public static void think_about_sex_Victim_Blame(
			Pawn pawn,
			Pawn partner,
			bool isReceiving,
			SexProps props,
			bool whoring,
			bool masochist,
			bool zoophile,
			bool guilty
			)
		{
			if (pawn.Faction != null && pawn.Map != null && !masochist && !(xxx.is_animal(partner) && zoophile))
			{
				foreach (Pawn bystander in pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction).Where(x => !xxx.is_animal(x) && x != pawn && x != partner && !x.Downed && !x.Suspended))
				{
					// dont see through walls, dont see whole map, only 15 cells around
					if (pawn.CanSee(bystander) && pawn.Position.DistanceTo(bystander.Position) < 15)
					{
						pawn.needs.mood.thoughts.memories.TryGainMemory(allowed_me_to_get_raped, bystander);
					}
				}
			}
		}

		public static ThoughtDef think_about_sex_Consensual(
			Pawn pawn,
			Pawn partner,
			bool isReceiving,
			SexProps props,
			bool whoring,
			bool masochist,
			bool zoophile,
			bool guilty
			)
		{
			ThoughtDef thoughtDef = null;
			if (xxx.is_human(partner))
			{
				if (!props.isCoreLovin && !whoring)
				{
					// human partner and not part of rape or necrophilia. add the vanilla GotSomeLovin thought
					thoughtDef = ThoughtDefOf.GotSomeLovin;
				}
			}
			return thoughtDef;
		}

		public static bool HasLoveEnhancer(Pawn pawn) =>
			pawn.health.hediffSet.hediffs.Any((Hediff h) => h.def == HediffDefOf.LoveEnhancer);

		public static bool HasLoveEnhancer(Pawn pawn, Pawn partner) =>
			HasLoveEnhancer(pawn) || HasLoveEnhancer(partner);

		public static void UpdateRecords(SexProps props)
		{
			UpdateRecordsInitiator(props);
			if (props.sexType != xxx.rjwSextype.Masturbation)
				UpdateRecordsPartner(props);
		}

		private static void UpdateRecordsInitiator(SexProps props)
		{

			Pawn pawn = props.pawn;
			Pawn partner = props.partner;

			if (props.sexType == xxx.rjwSextype.Masturbation)
			{
				pawn.records.Increment(xxx.CountOfFappin);
				return;
			}

			//bool isVirginSex = xxx.is_Virgin(pawn); //need copy value before count increase.
			//ThoughtDef currentThought = null;

			pawn.records.Increment(xxx.CountOfSex);
			//bool masochist = xxx.is_masochist(pawn);
			//bool zoophile = xxx.is_zoophile(pawn);
			//if (HasLoveEnhancer(pawn, partner))
			//{
			//	masochist = true;
			//	zoophile = true;
			//}

			if (props.partner.Dead)
			{
				pawn.records.Increment(xxx.CountOfSexWithCorpse);
			}
			if (!props.isRape)
			{
				if (xxx.is_human(partner))
				{
					pawn.records.Increment(xxx.CountOfSexWithHumanlikes);
					//currentThought = isLoveSex ? xxx.gave_virginity : null;
				}
				else if (xxx.is_insect(partner))
				{
					pawn.records.Increment(xxx.CountOfSexWithInsects);
				}
				else if (xxx.is_animal(partner))
				{
					pawn.records.Increment(xxx.CountOfSexWithAnimals);
					//currentThought = zoophile ? xxx.gave_virginity : null;
				}
				else
				{
					pawn.records.Increment(xxx.CountOfSexWithOthers);
				}
			}
			else
			{
				//if (!pawnIsRaper)
				//{
				//	currentThought = masochist ? xxx.gave_virginity : xxx.lost_virginity;
				//}

				if (xxx.is_human(partner))
				{
					pawn.records.Increment(xxx.CountOfRapedHumanlikes);
					//if (pawnIsRaper && (xxx.is_rapist(pawn) || xxx.is_bloodlust(pawn)))
					//	currentThought = xxx.gave_virginity;
				}
				else if (xxx.is_insect(partner))
				{
					pawn.records.Increment(xxx.CountOfSexWithInsects);
					pawn.records.Increment(xxx.CountOfRapedInsects);
				}
				else if (xxx.is_animal(partner))
				{
					pawn.records.Increment(xxx.CountOfSexWithAnimals);
					pawn.records.Increment(xxx.CountOfRapedAnimals);
					//if (zoophile) currentThought = xxx.gave_virginity;
				}
				else
				{
					pawn.records.Increment(xxx.CountOfSexWithOthers);
					pawn.records.Increment(xxx.CountOfRapedOthers);
				}
			}

			//if (isVirginSex) //&& (sextype == rjwSextype.Vaginal || sextype == rjwSextype.DoublePenetration))
			//{
			//	Log.Message(xxx.get_pawnname(pawn) + " | " + xxx.get_pawnname(partner) + " | " + currentThought);
			//	Log.Message("1");
			//	if (!is_animal(partner))//passive
			//	{
			//		if (currentThought != null)
			//			partner.needs.mood.thoughts.memories.TryGainMemory(currentThought);
			//	}
			//	Log.Message("2");
			//	if (!is_animal(pawn))//active
			//	{
			//		currentThought = took_virginity;
			//		pawn.needs.mood.thoughts.memories.TryGainMemory(currentThought);
			//	}
			//}
		}

		private static void UpdateRecordsPartner(SexProps props)
		{

			if (props.partner == null || props.partner.Dead) // masturbation or corpse fucking
				return;

			Pawn pawn = props.partner;
			Pawn partner = props.pawn;

			//bool isVirginSex = xxx.is_Virgin(pawn); //need copy value before count increase.
			//ThoughtDef currentThought = null;

			pawn.records.Increment(xxx.CountOfSex);
			//bool masochist = xxx.is_masochist(pawn);
			//bool zoophile = xxx.is_zoophile(pawn);
			//if (HasLoveEnhancer(pawn, partner))
			//{
			//	masochist = true;
			//	zoophile = true;
			//}

			if (!props.isRape)
			{
				if (xxx.is_human(partner))
				{
					pawn.records.Increment(xxx.CountOfSexWithHumanlikes);
					//currentThought = isLoveSex ? xxx.gave_virginity : null;
				}
				else if (xxx.is_insect(partner))
				{
					pawn.records.Increment(xxx.CountOfSexWithInsects);
				}
				else if (xxx.is_animal(partner))
				{
					pawn.records.Increment(xxx.CountOfSexWithAnimals);
					//currentThought = zoophile ? xxx.gave_virginity : null;
				}
				else
				{
					pawn.records.Increment(xxx.CountOfSexWithOthers);
				}
			}
			else
			{
				//if (!pawnIsRaper)
				//{
				//	currentThought = masochist ? xxx.gave_virginity : xxx.lost_virginity;
				//}

				if (xxx.is_human(partner))
				{
					pawn.records.Increment(xxx.CountOfBeenRapedByHumanlikes);
					//if (pawnIsRaper && (xxx.is_rapist(pawn) || xxx.is_bloodlust(pawn)))
					//	currentThought = xxx.gave_virginity;
				}
				else if (xxx.is_insect(partner))
				{
					pawn.records.Increment(xxx.CountOfSexWithInsects);
					pawn.records.Increment(xxx.CountOfBeenRapedByInsects);
				}
				else if (xxx.is_animal(partner))
				{
					pawn.records.Increment(xxx.CountOfSexWithAnimals);
					pawn.records.Increment(xxx.CountOfBeenRapedByAnimals);
					//if (zoophile) currentThought = xxx.gave_virginity;
				}
				else
				{
					pawn.records.Increment(xxx.CountOfSexWithOthers);
					pawn.records.Increment(xxx.CountOfBeenRapedByOthers);
				}
			}

			//if (isVirginSex) //&& (sextype == rjwSextype.Vaginal || sextype == rjwSextype.DoublePenetration))
			//{
			//	Log.Message(xxx.get_pawnname(pawn) + " | " + xxx.get_pawnname(partner) + " | " + currentThought);
			//	Log.Message("1");
			//	if (!is_animal(partner))//passive
			//	{
			//		if (currentThought != null)
			//			partner.needs.mood.thoughts.memories.TryGainMemory(currentThought);
			//	}
			//	Log.Message("2");
			//	if (!is_animal(pawn))//active
			//	{
			//		currentThought = took_virginity;
			//		pawn.needs.mood.thoughts.memories.TryGainMemory(currentThought);
			//	}
			//}
		}

		//============↓======Section of processing the broken body system===============↓=============
		public static bool BodyIsBroken(Pawn pawn)
		{
			return pawn.health.hediffSet.HasHediff(xxx.feelingBroken);
		}

		[SyncMethod]
		public static bool BadlyBroken(Pawn pawn, out Trait addedTrait)
		{
			addedTrait = null;
			if (!BodyIsBroken(pawn))
			{
				return false;
			}

			int stage = pawn.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken).CurStageIndex;
			if (stage >= 3)
			{
				//when broken make character masochist
				//todo remove/replace social/needs dubuffs
				if (RJWSettings.AddTrait_Masocist && !xxx.is_masochist(pawn))
				{
					var chance = 0.05f;
					if (Rand.Chance(chance))
					{
						if (xxx.is_rapist(pawn))
						{
							pawn.story.traits.allTraits.Remove(pawn.story.traits.GetTrait(xxx.rapist));
							//Log.Message(xxx.get_pawnname(pawn) + " BadlyBroken, switch rapist -> masochist");
						}
						addedTrait = new Trait(xxx.masochist);
						pawn.story.traits.GainTrait(addedTrait);
						//Log.Message(xxx.get_pawnname(pawn) + " BadlyBroken, not masochist, adding masochist trait");

						pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(got_raped);
						pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(got_licked);
						pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(hate_my_rapist);
						pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(allowed_me_to_get_raped);
						pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(got_anal_raped);
						pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(got_anal_raped_byfemale);
					}
				}
				if (pawn.IsPrisonerOfColony)
				{
					pawn.guest.resistance = Mathf.Max(pawn.guest.resistance - 1f, 0f);
					//Log.Message(xxx.get_pawnname(pawn) + " BadlyBroken, reduce prisoner resistance");
				}
			}
			return stage > 1;
		}

		//add variant for eggs?
		public static void processBrokenPawn(Pawn pawn, List<Trait> addedTraits)
		{
			// Called after rape/breed
			if (pawn is null)
				return;

			if (xxx.is_human(pawn) && !pawn.Dead && pawn.records != null)
			{
				if (xxx.has_traits(pawn))
				{
					if (xxx.is_slime(pawn))
						return;

					if (!BodyIsBroken(pawn))
						pawn.health.AddHediff(xxx.feelingBroken);
					else
					{
						float brokenSeverityGain = xxx.feelingBroken.initialSeverity;
						int feelingBrokenStage = pawn.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken).CurStageIndex;

						if (xxx.RoMIsActive)
							if (pawn.story.traits.HasTrait(xxx.Succubus))
								brokenSeverityGain *= 0.25f;

						if (pawn.story.traits.HasTrait(TraitDefOf.Tough))
						{
							brokenSeverityGain *= 0.5f;
						}
						if (pawn.story.traits.HasTrait(TraitDefOf.Wimp))
						{
							brokenSeverityGain *= 2.0f;
						}
						if (pawn.story.traits.HasTrait(TraitDefOf.Nerves))
						{
							int currentNervesDegree = pawn.story.traits.DegreeOfTrait(TraitDefOf.Nerves);

							switch (currentNervesDegree)
							{
								case -2:
									brokenSeverityGain *= 2.0f;
									break;
								case -1:
									brokenSeverityGain *= 1.5f;
									break;
								case 1:
									brokenSeverityGain *= 0.5f;
									break;
								case 2:
									brokenSeverityGain *= 0.25f;
									break;
							}

							if (RJWSettings.AddTrait_Nerves)
							{
								int newNervesDegree = 0;
								if (feelingBrokenStage >= 3 && currentNervesDegree > -2)
								{
									newNervesDegree = -2;
								}
								else if (feelingBrokenStage >= 2 && currentNervesDegree > -1)
								{
									newNervesDegree = -1;
								}

								if (newNervesDegree < 0)
								{
									pawn.story.traits.RemoveTrait(pawn.story.traits.GetTrait(TraitDefOf.Nerves));
									Trait newNerves = new Trait(TraitDefOf.Nerves, newNervesDegree);
									pawn.story.traits.GainTrait(newNerves);
									addedTraits.Add(newNerves);
								}
							}
						}
						else if (RJWSettings.AddTrait_Nerves && feelingBrokenStage > 1)
						{
							pawn.story.traits.GainTrait(new Trait(TraitDefOf.Nerves, -1));
							Trait nervous = new Trait(TraitDefOf.Nerves, -1);
							pawn.story.traits.GainTrait(nervous);
							addedTraits.Add(nervous);

						}
						pawn.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken).Severity += brokenSeverityGain;
					}

					BadlyBroken(pawn, out Trait newMasochistTrait);
					if (newMasochistTrait != null)
					{
						addedTraits.Add(newMasochistTrait);
					}
				}
			}
		}

		//============↑======Section of processing the broken body system===============↑=============
	}
}
