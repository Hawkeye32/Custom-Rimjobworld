using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using HarmonyLib;
using Multiplayer.API;
using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Implementation;
using rjw.Modules.Interactions;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Enums;
using static HarmonyLib.Code;

namespace rjw
{
	public class SexUtility
	{
		private const float base_sat_per_fuck = 0.40f;
		private const float base_sat_per_quirk = 0.20f;

		public static readonly InteractionDef AnimalSexChat = DefDatabase<InteractionDef>.GetNamed("AnimalSexChat");

		private static readonly ThingDef cum = ThingDef.Named("FilthCum");
		private static readonly ThingDef girlcum = ThingDef.Named("FilthGirlCum");

		public static readonly List<InteractionDef> SexInterractions = DefDatabase<InteractionDef>.AllDefsListForReading.Where(x => x.HasModExtension<InteractionExtension>()).ToList();

		// Alert checker that is called from several jobs. Checks the pawn relation, and whether it should sound alert.
		// notification in top left corner
		// rape attempt
		public static void RapeTargetAlert(Pawn rapist, Pawn target)
		{
			if (target.IsDesignatedComfort() && rapist.jobs.curDriver.GetType() == typeof(JobDriver_RapeComfortPawn))
				if (!RJWPreferenceSettings.ShowForCP)
					return;
			if (target.IsDesignatedComfort() && rapist.jobs.curDriver.GetType() == typeof(JobDriver_Breeding))
				if (target.IsDesignatedBreeding())
					if (!RJWPreferenceSettings.ShowForBreeding)
						return;

			bool silent = false;
			PawnRelationDef relation = rapist.GetMostImportantRelation(target);
			string rapeverb = "rape";

			if (xxx.is_mechanoid(rapist)) rapeverb = "assault";
			else if (xxx.is_animal(rapist) || xxx.is_animal(target)) rapeverb = "breed";

			// TODO: Need to write a cherker method for family relations. Would be useful for other things than just this, such as incest settings/quirk.

			string message = (xxx.get_pawnname(rapist) + " is trying to " + rapeverb + " " + xxx.get_pawnname(target));
			message += relation == null ? "." : (", " + rapist.Possessive() + " " + relation.GetGenderSpecificLabel(target) + ".");

			switch (RJWPreferenceSettings.rape_attempt_alert)
			{
				case RJWPreferenceSettings.RapeAlert.Enabled:
					break;
				case RJWPreferenceSettings.RapeAlert.Humanlikes:
					if (!xxx.is_human(target))
						return;
					break;
				case RJWPreferenceSettings.RapeAlert.Colonists:
					if (!target.IsColonist)
						return;
					break;
				case RJWPreferenceSettings.RapeAlert.Silent:
					silent = true;
					break;
				default:
					return;
			}

			if (!silent)
			{
				Messages.Message(message, rapist, MessageTypeDefOf.NegativeEvent);
			}
			else
			{
				Messages.Message(message, rapist, MessageTypeDefOf.SilentInput);
			}
		}

		// Alert checker that is called from several jobs.
		// notification in top left corner
		// rape started
		public static void BeeingRapedAlert(Pawn rapist, Pawn target)
		{
			if (target.IsDesignatedComfort() && rapist.jobs.curDriver.GetType() == typeof(JobDriver_RapeComfortPawn))
				if (!RJWPreferenceSettings.ShowForCP)
					return;
			if (target.IsDesignatedComfort() && rapist.jobs.curDriver.GetType() == typeof(JobDriver_Breeding))
				if (target.IsDesignatedBreeding())
					if (!RJWPreferenceSettings.ShowForBreeding)
						return;

			bool silent = false;

			switch (RJWPreferenceSettings.rape_alert)
			{
				case RJWPreferenceSettings.RapeAlert.Enabled:
					break;
				case RJWPreferenceSettings.RapeAlert.Humanlikes:
					if (!xxx.is_human(target))
						return;
					break;
				case RJWPreferenceSettings.RapeAlert.Colonists:
					if (!target.IsColonist)
						return;
					break;
				case RJWPreferenceSettings.RapeAlert.Silent:
					silent = true;
					break;
				default:
					return;
			}

			if (!silent)
			{
				Messages.Message(xxx.get_pawnname(target) + " is getting raped.", target, MessageTypeDefOf.NegativeEvent);
			}
			else
			{
				Messages.Message(xxx.get_pawnname(target) + " is getting raped.", target, MessageTypeDefOf.SilentInput);
			}
		}

		// Quick method that return a body part by name. Used for checking if a pawn has a specific body part, etc.
		public static BodyPartRecord GetPawnBodyPart(Pawn pawn, string bodyPart)
		{
			return pawn.RaceProps.body.AllParts.Find(x => x.def == DefDatabase<BodyPartDef>.GetNamed(bodyPart));
		}

		public static void CumFilthGenerator(Pawn pawn)
		{
			if (pawn == null) return;
			if (pawn.Dead) return;
			if (xxx.is_slime(pawn)) return;
			if (!RJWSettings.cum_filth) return;

			// Larger creatures, larger messes.
			float pawn_cum = Math.Min(80 / ScaleToHumanAge(pawn), 2.0f) * pawn.BodySize;

			// Increased output if the pawn has the Messy quirk.
			if (xxx.has_quirk(pawn, "Messy"))
				pawn_cum *= 2.0f;

			var parts = pawn.GetGenitalsList();

			if (Genital_Helper.has_vagina(pawn, parts))
				FilthMaker.TryMakeFilth(pawn.PositionHeld, pawn.MapHeld, girlcum, pawn.LabelIndefinite(), (int)Math.Max(pawn_cum / 2, 1.0f));

			if (Genital_Helper.has_penis_fertile(pawn, parts))
				FilthMaker.TryMakeFilth(pawn.PositionHeld, pawn.MapHeld, cum, pawn.LabelIndefinite(), (int)Math.Max(pawn_cum, 1.0f));
		}

		// The pawn may or may not clean up the mess after fapping.
		[SyncMethod]
		public static bool ConsiderCleaning(Pawn fapper)
		{
			if (!RJWSettings.cum_filth) return false;
			if (!xxx.has_traits(fapper) || fapper.story == null) return false;
			if (fapper.WorkTagIsDisabled(WorkTags.Cleaning)) return false;

			float do_cleaning = 0.5f; // 50%

			if (!fapper.PositionHeld.Roofed(fapper.Map))
				do_cleaning -= 0.25f; // Less likely to clean if outdoors.

			if (xxx.CTIsActive && fapper.story.traits.HasTrait(xxx.RCT_NeatFreak))
				do_cleaning += 1.00f;

			if (xxx.has_quirk(fapper, "Messy"))
				do_cleaning -= 0.75f;

			switch (fapper.needs?.rest?.CurCategory)
			{
				case RestCategory.Exhausted:
					do_cleaning -= 0.5f;
					break;
				case RestCategory.VeryTired:
					do_cleaning -= 0.3f;
					break;
				case RestCategory.Tired:
					do_cleaning -= 0.1f;
					break;
				case RestCategory.Rested:
					do_cleaning += 0.3f;
					break;
			}

			if (fapper.story.traits.DegreeOfTrait(TraitDefOf.NaturalMood) == -2) // Depressive
				do_cleaning -= 0.3f;
			if (fapper.story.traits.DegreeOfTrait(TraitDefOf.Industriousness) == 2) // Industrious
				do_cleaning += 1.0f;
			else if (fapper.story.traits.DegreeOfTrait(TraitDefOf.Industriousness) == 1) // Hard worker
				do_cleaning += 0.5f;
			else if (fapper.story.traits.DegreeOfTrait(TraitDefOf.Industriousness) == -1) // Lazy
				do_cleaning -= 0.5f;
			else if (fapper.story.traits.DegreeOfTrait(TraitDefOf.Industriousness) == -2) // Slothful
				do_cleaning -= 1.0f;

			if (xxx.is_ascetic(fapper))
				do_cleaning += 0.2f;

			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			return Rand.Chance(do_cleaning);
		}

		/// <summary>Handles after-sex trait and thought gain, and fluid creation. Initiator of the act (whore, rapist, female zoophile, etc) should be first.</summary>
		[SyncMethod]
		public static void Aftersex(SexProps props)
		{
			if (props.sexType == xxx.rjwSextype.Masturbation)
			{
				AfterMasturbation(props);
				return;
			}

			bool bothInMap = false;

			if (!props.partner.Dead)
				bothInMap = props.pawn.Map != null && props.partner.Map != null; //Added by Hoge. false when called this function for despawned pawn: using for background rape like a kidnappee

			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			if (bothInMap)
			{
				//Catch-all timer increase, for ensuring that pawns don't get stuck repeating jobs.
				if (!props.isCoreLovin)
				{
					props.pawn.rotationTracker.Face(props.partner.DrawPos);
					props.pawn.rotationTracker.FaceCell(props.partner.Position);
				}
				if (!props.partner.Dead)
				{
					if (!props.isCoreLovin)
					{
						props.partner.rotationTracker.Face(props.pawn.DrawPos);
						props.partner.rotationTracker.FaceCell(props.pawn.Position);
					}
					if (RJWSettings.sounds_enabled)
					{
						if (props.isRape)
						{
							if (Rand.Value > 0.30f)
								LifeStageUtility.PlayNearestLifestageSound(props.partner, (ls) => ls.soundAngry, null, 1.2f);
							else
								LifeStageUtility.PlayNearestLifestageSound(props.partner, (ls) => ls.soundCall, g => g.soundCall, 1.2f);

							props.pawn.Drawer.Notify_MeleeAttackOn(props.partner);
							props.partner.stances.stagger.StaggerFor(Rand.Range(10, 300));
						}
						else
							LifeStageUtility.PlayNearestLifestageSound(props.partner, (ls) => ls.soundCall, g => g.soundCall);
					}
					if (props.sexType == xxx.rjwSextype.Vaginal || props.sexType == xxx.rjwSextype.DoublePenetration)
						if (xxx.is_Virgin(props.partner))
						{
							//TODO: bind virginity to parts of pawn
							/*
							string thingdef_penis_name = Genital_Helper.get_penis_all(pawn)?.def.defName ?? "";
							ThingDef thingdef_penis = null;

							Log.Message("SexUtility::thingdef_penis_name " + thingdef_penis_name);
							Log.Message("SexUtility::thingdef_penis 1 " + thingdef_penis);

							if (thingdef_penis_name != "")
								thingdef_penis = (from x in DefDatabase<ThingDef>.AllDefs where x.defName == thingdef_penis_name select x).RandomElement();
							Log.Message("SexUtility::thingdef_penis 2 " + thingdef_penis);

							partner.TakeDamage(new DamageInfo(DamageDefOf.Stab, 1, 999, -1.0f, null, xxx.genitals, thingdef_penis));
							*/
						}
				}

				if (RJWSettings.sounds_enabled && props.isCoreLovin)
					SoundDef.Named("Cum").PlayOneShot(!props.partner.Dead
						? new TargetInfo(props.partner.Position, props.pawn.Map)
						: new TargetInfo(props.pawn.Position, props.pawn.Map));

				if (props.isRape)
				{
					if (Rand.Value > 0.30f)
						LifeStageUtility.PlayNearestLifestageSound(props.pawn, (ls) => ls.soundAngry, null, 1.2f);
					else
						LifeStageUtility.PlayNearestLifestageSound(props.pawn, (ls) => ls.soundCall, g => g.soundCall, 1.2f);
				}
				else
					LifeStageUtility.PlayNearestLifestageSound(props.pawn, (ls) => ls.soundCall, g => g.soundCall);
			}

			if (props.usedCondom)
			{
				if (CondomUtility.UsedCondom != null)
					GenSpawn.Spawn(CondomUtility.UsedCondom, props.pawn.Position, props.pawn.Map);
				CondomUtility.useCondom(props.pawn);
				CondomUtility.useCondom(props.partner);
			}
			else
			{
				if (props.isCoreLovin) // non core handled by jobdriver
				{
					//apply cum to floor:
					CumFilthGenerator(props.pawn);
					CumFilthGenerator(props.partner);

					PregnancyHelper.impregnate(props);
					TransferNutrition(props);
				}
			}

			List<Trait> newInitiatorTraits = new(), newReceiverTraits = new();

			if (props.isRape && !props.partner.Dead)
				AfterSexUtility.processBrokenPawn(props.partner, newReceiverTraits);

			//Satisfy(pawn, partner, sextype, rape);

			//TODO: below is fucked up, unfuck it someday
			AfterSexUtility.UpdateRecords(props);
			GiveTraits(props, newInitiatorTraits);
			GiveTraits(props.GetForPartner(), newReceiverTraits);

			if (RJWSettings.sendTraitGainLetters)
			{
				SendTraitGainLetter(props.pawn, newInitiatorTraits);
				SendTraitGainLetter(props.partner, newReceiverTraits);
			}
		}

		// <summary>Solo acts.</summary>
		public static void AfterMasturbation(SexProps props)
		{
			IncreaseTicksToNextLovin(props.pawn);

			//apply cum to floor:
			CumFilthGenerator(props.pawn);

			AfterSexUtility.UpdateRecords(props);

			// No traits from solo. Enable if some are edded. (Voyerism?)
			//check_trait_gain(pawn);
		}

		// Scales alien lifespan to human age. 
		// Some aliens have broken lifespans, that can be manually corrected here.
		public static int ScaleToHumanAge(Pawn pawn, int humanLifespan = 80)
		{
			float pawnAge = pawn.ageTracker.AgeBiologicalYearsFloat;
			float pawnLifespan = pawn.RaceProps.lifeExpectancy;

			if (pawn.def.defName == "Human") return (int)pawnAge; // Human, no need to scale anything.

			// Xen races, all broken and need a fix.
			if (pawn.def.defName.ContainsAny("Alien_Sergal", "Alien_SergalNME", "Alien_Xenn", "Alien_Racc", "Alien_Ferrex", "Alien_Wolvx", "Alien_Frijjid", "Alien_Fennex") && pawnLifespan >= 2000f)
			{
				pawnAge = Math.Min(pawnAge, 80f); // Clamp to 80.
				pawnLifespan = 80f;
			}
			if (pawn.def.defName.ContainsAny("Alien_Gnoll", "Alien_StripedGnoll") && pawnLifespan >= 2000f)
			{
				pawnAge = Math.Min(pawnAge, 60f); // Clamp to 60.
				pawnLifespan = 60f; // Mature faster than humans.
			}

			// Immortal races that mature at similar rate to humans.
			if (pawn.def.defName.ContainsAny("LF_Dragonia", "LotRE_ElfStandardRace", "Alien_Crystalloid", "Alien_CrystalValkyrie"))
			{
				pawnAge = Math.Min(pawnAge, 40f); // Clamp to 40 - never grow 'old'.
				pawnLifespan = 80f;
			}

			float age_scaling = humanLifespan / pawnLifespan;
			float scaled_age = pawnAge * age_scaling;

			if (scaled_age < 1)
				scaled_age = 1;

			return (int)scaled_age;
		}

		// Used in complex impregnation calculation. Pawns/animals with similar parts have better compatibility.
		public static float BodySimilarity(Pawn pawn, Pawn partner)
		{
			float size_adjustment = Mathf.Lerp(0.3f, 1.05f, 1.0f - Math.Abs(pawn.BodySize - partner.BodySize));

			//ModLog.Message(" Size adjustment: " + size_adjustment);

			List<BodyPartDef> pawn_partlist = new List<BodyPartDef> { };
			List<BodyPartDef> pawn_mismatched = new List<BodyPartDef> { };
			List<BodyPartDef> partner_mismatched = new List<BodyPartDef> { };

			//ModLog.Message("Checking compatibility for " + xxx.get_pawnname(pawn) + " and " + xxx.get_pawnname(partner));
			bool pawnHasHands = pawn.health.hediffSet.GetNotMissingParts().Any(part => part.IsInGroup(BodyPartGroupDefOf.RightHand) || part.IsInGroup(BodyPartGroupDefOf.LeftHand));

			foreach (BodyPartRecord part in pawn.RaceProps.body.AllParts)
			{
				pawn_partlist.Add(part.def);
			}
			float pawn_count = pawn_partlist.Count();

			foreach (BodyPartRecord part in partner.RaceProps.body.AllParts)
			{
				partner_mismatched.Add(part.def);
			}
			float partner_count = partner_mismatched.Count();

			foreach (BodyPartDef part in pawn_partlist)
			{
				if (partner_mismatched.Contains(part))
				{
					pawn_mismatched.Add(part);
					partner_mismatched.Remove(part);
				}
			}

			float pawn_mismatch = pawn_mismatched.Count() / pawn_count;
			float partner_mismatch = partner_mismatched.Count() / partner_count;

			//ModLog.Message("Body type similarity for " + xxx.get_pawnname(pawn) + " and " + xxx.get_pawnname(partner) + ": " + Math.Round(((pawn_mismatch + partner_mismatch) * 50) * size_adjustment, 1) + "%");

			return ((pawn_mismatch + partner_mismatch) / 2) * size_adjustment;
		}

		public static void SatisfyPersonal(SexProps props, float satisfaction = 0.4f)
		{
			Pawn pawn = props.pawn;
			Pawn partner = props.partner;
			//--Log.Message("xxx::satisfy( " + pawn_name + ", " + partner_name + ", " + violent + "," + isCoreLovin + " ) - modifying partner satisfaction");
			var sex_need = pawn?.needs?.TryGetNeed<Need_Sex>();
			if (sex_need == null) return;

			float Trait_Satisfaction = 1f;
			float Quirk_Satisfaction = 1f;
			float Stat_Satisfaction = 0f;
			float Circumstances_Satisfaction = 0f; // violence/broken

			float joysatisfaction = satisfaction;

			// Bonus satisfaction from traits
			if (pawn != null && partner != null)
			{
				if (xxx.is_animal(partner) && xxx.is_zoophile(pawn))
				{
					Trait_Satisfaction += 0.5f;
				}
				if (partner.Dead && xxx.is_necrophiliac(pawn))
				{
					Trait_Satisfaction += 0.5f;
				}
			}

			// Calculate bonus satisfaction from quirks
			var quirkCount = Quirk.CountSatisfiedQuirks(props);
			Quirk_Satisfaction += quirkCount * base_sat_per_quirk;

			// Apply sex satisfaction stat (min 0.1 default 1) as a modifier to total satisfaction
			Stat_Satisfaction += Math.Max(xxx.get_sex_satisfaction(pawn), 0.1f);

			// Apply extra multiplier for special/violence circumstances
			Circumstances_Satisfaction += get_satisfaction_circumstance_multiplier(props);

			satisfaction *= Trait_Satisfaction * Quirk_Satisfaction * Stat_Satisfaction;

			joysatisfaction = satisfaction;
			satisfaction *= Circumstances_Satisfaction;
			if (!RJWSettings.Disable_RecreationDrain)
				joysatisfaction *= (Circumstances_Satisfaction - 1);
			else
				joysatisfaction *= Circumstances_Satisfaction;

			//Log.Message("SatisfyPersonal( " + pawn + ", " + satisfaction + " ) - setting pawn sexneed");

			sex_need.CurLevel += satisfaction;

			if (quirkCount > 0)
			{
				Quirk.AddThought(pawn);
			}

			var joy_need = pawn.needs.TryGetNeed<Need_Joy>();
			if (joy_need == null) return;

			joysatisfaction *= joysatisfaction > 0 ? 0.5f : 1f;              // convert half of positive satisfaction to joy
																			 //Log.Message("SatisfyPersonal( " + pawn + ", " + joysatisfaction + " ) - setting pawn joyneed");
			pawn.needs.joy.CurLevel += joysatisfaction;

			//add Ahegao
			//sex&joy > 95%
			//sex&broken

			var lev = sex_need.CurLevel;
			if (lev >= sex_need.thresh_ahegao() &&
				(joy_need.CurLevelPercentage > sex_need.thresh_ahegao()) ||
				(AfterSexUtility.BodyIsBroken(pawn) && pawn.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken)?.CurStageIndex >= 3))
			{
				var thoughtDef = DefDatabase<ThoughtDef>.GetNamed("RJW_Ahegao");
				pawn.needs.mood.thoughts.memories.TryGainMemory(thoughtDef);
			}
		}

		public static void GiveTraits(SexProps props, List<Trait> newTraits)
		{
			var pawn = props.pawn;
			var partner = props.partner;

			if (!xxx.has_traits(pawn) || pawn.records.GetValue(xxx.CountOfSex) <= 10)
			{
				return;
			}

			GiveQuirkTraits(props, newTraits, 0.05f);

			if (props.IsInitiator())
			{
				if (RJWSettings.AddTrait_Rapist && !xxx.is_rapist(pawn) && !xxx.is_masochist(pawn) && props.isRape && pawn.records.GetValue(xxx.CountOfRapedHumanlikes) > 0.12 * pawn.records.GetValue(xxx.CountOfSex))
				{
					var chance = 0.5f;
					if (xxx.is_kind(pawn)) chance -= 0.25f;
					if (xxx.is_prude(pawn)) chance -= 0.25f;
					if (xxx.is_zoophile(pawn)) chance -= 0.25f; // Less interested in raping humanlikes.
					if (xxx.is_ascetic(pawn)) chance -= 0.2f;
					if (xxx.is_bloodlust(pawn)) chance += 0.2f;
					if (xxx.is_psychopath(pawn)) chance += 0.25f;

					if (Rand.Chance(chance))
					{
						Trait rapist = new Trait(xxx.rapist);
						pawn.story.traits.GainTrait(rapist);
						newTraits.Add(rapist);
						//--Log.Message(xxx.get_pawnname(pawn) + " aftersex, not rapist, adding rapist trait");
					}
				}

				if (RJWSettings.AddTrait_Necrophiliac && !xxx.is_necrophiliac(pawn) && partner.Dead && pawn.records.GetValue(xxx.CountOfSexWithCorpse) > 0.5 * pawn.records.GetValue(xxx.CountOfSex))
				{
					Trait necropphiliac = new Trait(xxx.necrophiliac);
					pawn.story.traits.GainTrait(necropphiliac);
					newTraits.Add(necropphiliac);
					//Log.Message(xxx.get_pawnname(necro) + " aftersex, not necro, adding necro trait");					
				}
			}

			if (RJWSettings.AddTrait_Zoophiliac && !xxx.is_zoophile(pawn) && xxx.is_animal(partner)
				&& (pawn.records.GetValue(xxx.CountOfSexWithAnimals) + pawn.records.GetValue(xxx.CountOfSexWithInsects) > 0.5 * pawn.records.GetValue(xxx.CountOfSex)))
			{
				Trait zoophile = new Trait(xxx.zoophile);
				pawn.story.traits.GainTrait(zoophile);
				newTraits.Add(zoophile);

				MemoryThoughtHandler memories = pawn.needs.mood.thoughts.memories;
				foreach (ThoughtDef memory in new[] {xxx.got_bred, xxx.got_anal_bred, xxx.got_groped, xxx.got_licked})
				{
					memories.RemoveMemoriesOfDef(memory);
				}
				//--Log.Message(xxx.get_pawnname(pawn) + " aftersex, not zoo, adding zoo trait");
			}

			if (RJWSettings.AddTrait_Nymphomaniac && !xxx.is_nympho(pawn))
			{
				if (pawn.health.hediffSet.HasHediff(RJWHediffDefOf.HumpShroomAddiction) && pawn.health.hediffSet.HasHediff(RJWHediffDefOf.HumpShroomEffect))
				{
					Trait nymphomaniac = new Trait(xxx.nymphomaniac);
					pawn.story.traits.GainTrait(nymphomaniac);
					newTraits.Add(nymphomaniac);
					//Log.Message(xxx.get_pawnname(pawn) + " is HumpShroomAddicted, not nymphomaniac, adding nymphomaniac trait");
				}
			}
		}

		[SyncMethod]
		private static void GiveQuirkTraits(SexProps props, List<Trait> newTraits, float traitGainChance = 0.05f)
		{
			var pawn = props.pawn;
			if (RJWPreferenceSettings.PlayerIsFootSlut && !pawn.Has(Quirk.Podophile))
			{
				if (props.sexType == xxx.rjwSextype.Footjob)
					if (props.IsSubmissive())
					{
						if (Rand.Chance(traitGainChance) || pawn.story.traits.HasTrait(xxx.footSlut))
						{
							pawn.Add(Quirk.Podophile);
							if (RJWSettings.AddTrait_FootSlut)
							{
								Trait footSlut = new Trait(xxx.footSlut);
								pawn.story.traits.GainTrait(footSlut);
								newTraits.Add(footSlut);
							}
						}
					}
			}
			if (RJWPreferenceSettings.PlayerIsCumSlut && !pawn.Has(Quirk.Cumslut))
			{
				if (props.sexType == xxx.rjwSextype.Fellatio)
					if (props.IsSubmissive())
					{
						if (Rand.Chance(traitGainChance) || pawn.story.traits.HasTrait(xxx.cumSlut))
						{
							pawn.Add(Quirk.Cumslut);
							if (RJWSettings.AddTrait_CumSlut)
							{
								Trait cumSlut = new Trait(xxx.cumSlut);
								pawn.story.traits.GainTrait(cumSlut);
								newTraits.Add(cumSlut);
							}
						}
					}
			}
			if (RJWPreferenceSettings.PlayerIsButtSlut && !pawn.Has(Quirk.Buttslut))
			{
				if (props.sexType == xxx.rjwSextype.Anal)
					if (props.IsSubmissive())
					{
						if (Rand.Chance(traitGainChance) || pawn.story.traits.HasTrait(xxx.buttSlut))
						{
							pawn.Add(Quirk.Buttslut);
							if (RJWSettings.AddTrait_ButtSlut)
							{
								Trait buttSlut = new Trait(xxx.buttSlut);
								pawn.story.traits.GainTrait(buttSlut);
								newTraits.Add(buttSlut);
							}
						}
					}
			}
		}

		private static void SendTraitGainLetter(Pawn pawn, List<Trait> newTraits)
		{
			if (newTraits.Count == 0 || !PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				return;
			}

			TaggedString letterLabel;
			TaggedString letterDescription;
			if (newTraits.Count == 1)
			{
				string traitLabel = newTraits[0].Label;
				
				string letterLabelKey = TranslationKeyFor("LetterLabelGainedTraitFromSex", newTraits[0]);
				letterLabel = letterLabelKey.Translate(traitLabel.Named("TRAIT"), pawn.Named("PAWN")).CapitalizeFirst();
				
				string letterDescriptionKey = TranslationKeyFor("GainedTraitFromSex", newTraits[0]);
				letterDescription = letterDescriptionKey.Translate(traitLabel.Named("TRAIT"), pawn.Named("PAWN")).CapitalizeFirst();
			}
			else
			{
				letterLabel = "LetterLabelGainedMultipleTraitsFromSex".Translate(pawn.Named("PAWN"));
				letterDescription = "GainedMultipleTraitsFromSex".Translate(pawn.Named("PAWN"));

				letterDescription += "\n";
				foreach (var trait in newTraits)
				{
					letterDescription += "\n" + "GainedMultipleTraitsFromSexListItem".Translate(trait.LabelCap);
				}
			}

			Find.LetterStack.ReceiveLetter(letterLabel, letterDescription, LetterDefOf.NeutralEvent, pawn);

			string TranslationKeyFor(string stem, Trait trait)
			{
				string traitKey = stem + trait.def.defName;
				string traitKeyWithDegree = traitKey + trait.Degree;
				if (traitKeyWithDegree.CanTranslate())
				{
					return traitKeyWithDegree;
				}
				if (traitKey.CanTranslate())
				{
					return traitKey;
				}
				return stem;
			}
		}

		// Checks if enough time has passed from previous lovin'.
		public static bool ReadyForLovin(Pawn pawn)
		{
			return Find.TickManager.TicksGame > pawn.mindState.canLovinTick;
		}

		// Checks if enough time has passed from previous search for a hookup.
		// Checks if hookups allowed during working hours, exlcuding nymphs
		public static bool ReadyForHookup(Pawn pawn)
		{
			if (!xxx.is_nympho(pawn) && RJWHookupSettings.NoHookupsDuringWorkHours && ((pawn.timetable != null) ? pawn.timetable.CurrentAssignment : TimeAssignmentDefOf.Anything) == TimeAssignmentDefOf.Work) return false;
			return Find.TickManager.TicksGame > CompRJW.Comp(pawn).NextHookupTick;
		}

		private static void IncreaseTicksToNextLovin(Pawn pawn)
		{
			if (pawn == null || pawn.Dead) return;
			int currentTime = Find.TickManager.TicksGame;
			if (pawn.mindState.canLovinTick <= currentTime)
				pawn.mindState.canLovinTick = currentTime + GenerateMinTicksToNextLovin(pawn);
		}

		[SyncMethod]
		public static int GenerateMinTicksToNextLovin(Pawn pawn)
		{
			if (DebugSettings.alwaysDoLovin) return 100;
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());

			float tick = 1.0f;

			// Nymphs automatically get the tick increase from the trait influence on sex drive.
			if (xxx.is_animal(pawn))
			{
				//var mateMtbHours = pawn.RaceProps.mateMtbHours / 24 * GenDate.TicksPerDay;
				//if (mateMtbHours > 0)
				//	interval = mateMtbHours
				if (RJWSettings.Animal_mating_cooldown == 0)
					tick = 0.75f;
				else
					return RJWSettings.Animal_mating_cooldown * 2500;
			}
			else if (xxx.is_prude(pawn))
				tick = 1.5f;

			if (pawn.Has(Quirk.Vigorous))
				tick *= 0.8f;

			float sex_drive = xxx.get_sex_drive(pawn);
			if (sex_drive <= 0.05f)
				sex_drive = 0.05f;

			float interval = AgeConfigDef.Instance.lovinIntervalHoursByAge.Evaluate(ScaleToHumanAge(pawn));
			float rinterval = Math.Max(0.5f, Rand.Gaussian(interval, 0.3f));
			return (int)(tick * rinterval * (2500.0f / sex_drive));
		}

		public static void IncreaseTicksToNextHookup(Pawn pawn)
		{
			if (pawn == null || pawn.Dead)
				return;

			// There are 2500 ticks per rimworld hour. Sleeping an hour between checks seems like a good start.
			// We could get fancier and weight it by sex drive and stuff, but would people even notice?
			const int TicksBetweenHookups = 2500;

			int currentTime = Find.TickManager.TicksGame;
			CompRJW.Comp(pawn).NextHookupTick = currentTime + TicksBetweenHookups;
		}

		/// <summary>
		/// <para>Determines the sex type and handles the log output.</para>
		/// <para>`props.pawn` should be initiator of the act (rapist, whore, etc).</para>
		/// <para>`props.partner` should be the target.</para>
		/// </summary>
		public static void ProcessSex(SexProps props)
		{
			//Log.Message("usedCondom=" + usedCondom);
			if (props.pawn == null || props.partner == null)
			{
				if (props.pawn == null)
					ModLog.Error("[SexUtility] ERROR: pawn is null.");
				if (props.partner == null)
					ModLog.Error("[SexUtility] ERROR: partner is null.");
				return;
			}

			IncreaseTicksToNextLovin(props.pawn);
			IncreaseTicksToNextLovin(props.partner);

			Aftersex(props);

			AfterSexUtility.think_about_sex(props);
		}

		[SyncMethod]
		public static SexProps SelectSextype(Pawn pawn, Pawn partner, bool rape, bool whoring)
		{
			var SP = new SexProps();
			SP.pawn = pawn;
			SP.partner = partner;
			SP.isRape = rape;
			SP.isWhoring = whoring;

			//Caufendra's magic is happening here
			InteractionInputs inputs = new InteractionInputs()
			{
				Initiator = pawn,
				Partner = partner,
				IsRape = rape,
				IsWhoring = whoring
			};

			//this should be added as a static readonly but ... since the class is so big,
			//it's probably best not to overload the static constructor
			ILewdInteractionService lewdInteractionService = LewdInteractionService.Instance;

			InteractionOutputs outputs = lewdInteractionService.GenerateInteraction(inputs);

			SP.sexType = outputs.Generated.RjwSexType;
			SP.rulePack = outputs.Generated.RulePack.defName;
			SP.dictionaryKey = outputs.Generated.InteractionDef.Interaction;

			return SP;
		}

		public static void LogSextype(Pawn giving, Pawn receiving, string rulepack, InteractionDef dictionaryKey)
		{
			List<RulePackDef> extraSentencePacks = new List<RulePackDef>();
			if (!rulepack.NullOrEmpty())
				extraSentencePacks.Add(RulePackDef.Named(rulepack));
			LogSextype(giving, receiving, extraSentencePacks, dictionaryKey);
		}

		public static void LogSextype(Pawn giving, Pawn receiving, List<RulePackDef> extraSentencePacks, InteractionDef dictionaryKey)
		{
			if (extraSentencePacks.NullOrEmpty())
			{
				extraSentencePacks = new List<RulePackDef>();
				string extraSentenceRulePack = SexRulePackGet(dictionaryKey);
				if (!extraSentenceRulePack.NullOrEmpty())
					extraSentencePacks.Add(RulePackDef.Named(extraSentenceRulePack));
			}
			PlayLogEntry_Interaction playLogEntry = new PlayLogEntry_Interaction(dictionaryKey, giving, receiving, extraSentencePacks);
			Find.PlayLog.Add(playLogEntry);
		}

		[SyncMethod]
		public static string SexRulePackGet(InteractionDef dictionaryKey)
		{
			var extension = Modules.Interactions.Helpers.InteractionHelper.GetWithExtension(dictionaryKey).Extension;
			string extraSentenceRulePack = "";
			if (!extension.rulepack_defs.NullOrEmpty())
			{
				extraSentenceRulePack = extension.rulepack_defs.RandomElement();
			}

			try
			{
				if (RulePackDef.Named(extraSentenceRulePack) != null)
				{
				}
			}
			catch
			{
				ModLog.Warning("RulePackDef " + extraSentenceRulePack + " for " + dictionaryKey + " not found");
				extraSentenceRulePack = "";
			}
			return extraSentenceRulePack;
		}

		public static xxx.rjwSextype rjwSextypeGet(InteractionDef dictionaryKey)
		{
			var extension = Modules.Interactions.Helpers.InteractionHelper.GetWithExtension(dictionaryKey).Extension;
			var sextype = xxx.rjwSextype.None;
			if (!extension.rjwSextype.NullOrEmpty())
				sextype = ParseHelper.FromString<xxx.rjwSextype>(extension.rjwSextype);

			if (RJWSettings.DevMode) ModLog.Message("rjwSextypeGet:dictionaryKey " + dictionaryKey + " sextype " + sextype);
			return sextype;
		}

		[SyncMethod]
		public static void Sex_Beatings(SexProps props)
		{
			Pawn pawn = props.pawn;
			Pawn partner = props.partner;
			if ((xxx.is_animal(pawn) && xxx.is_animal(partner)))
				return;

			//dont remember what it does, probably manhunter stuff or not? disable and wait reports
			//if (!xxx.is_human(pawn))
			//	return;

			//If a pawn is incapable of violence/has low melee, they most likely won't beat their partner
			if (pawn.skills?.GetSkill(SkillDefOf.Melee).Level < 1)
				return;

			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			float rand_value = Rand.Value;
			//float rand_value = RJW_Multiplayer.RJW_MP_RAND();
			float victim_pain = partner.health.hediffSet.PainTotal;
			// bloodlust makes the aggressor more likely to hit the prisoner
			float beating_chance = xxx.config.base_chance_to_hit_prisoner * (xxx.is_bloodlust(pawn) ? 1.5f : 1.0f);
			// psychopath makes the aggressor more likely to hit the prisoner past the significant_pain_threshold
			float beating_threshold = xxx.is_psychopath(pawn) ? xxx.config.extreme_pain_threshold : pawn.HostileTo(partner) ? xxx.config.significant_pain_threshold : xxx.config.minor_pain_threshold;

			//--Log.Message("roll_to_hit:  rand = " + rand_value + ", beating_chance = " + beating_chance + ", victim_pain = " + victim_pain + ", beating_threshold = " + beating_threshold);
			if ((victim_pain < beating_threshold && rand_value < beating_chance) || (rand_value < (beating_chance / 2) && xxx.is_bloodlust(pawn)))
			{
				Sex_Beatings_Dohit(pawn, partner, props.isRapist);
			}
		}

		public static void Sex_Beatings_Dohit(Pawn pawn, Pawn partner, bool isRape = false)
		{
			//--Log.Message("   done told her twice already...");
			if (InteractionUtility.TryGetRandomVerbForSocialFight(pawn, out Verb v))
			{
				//Log.Message("   v. : " + v);
				//Log.Message("   v.GetDamageDef : " + v.GetDamageDef());
				//Log.Message("   v.v.tool - " + v.tool.label);
				//Log.Message("   v.v.tool.power base - " + v.tool.power);
				var orgpower = v.tool.power;
				//in case something goes wrong
				try
				{
					//Log.Message("   v.v.tool.power base - " + v.tool.power);
					if (RJWSettings.gentle_rape_beating || !isRape)
					{
						v.tool.power = 0;
						//partner.stances.stunner.StunFor(600, pawn);
					}
					//Log.Message("   v.v.tool.power mod - " + v.tool.power);
					var guilty = true;
					if (!pawn.guilt.IsGuilty)
					{
						guilty = false;
					}
					pawn.meleeVerbs.TryMeleeAttack(partner, v);

					if (pawn.guilt.IsGuilty && !guilty)
						pawn.guilt.Notify_Guilty(0);
				}
				catch
				{ }
				v.tool.power = orgpower;
				//Log.Message("   v.v.tool.power reset - " + v.tool.power);
			}
		}

		// Overrides the current clothing. Defaults to nude, with option to keep headgear on.
		public static void DrawNude(Pawn pawn, bool keep_hat_on = false)
		{
			if (!xxx.is_human(pawn)) return;
			if (pawn.Map != Find.CurrentMap) return;
			if (RJWPreferenceSettings.sex_wear == RJWPreferenceSettings.Clothing.Clothed) return;

			//undress
			pawn.Drawer.renderer.graphics.ClearCache();
			pawn.Drawer.renderer.graphics.apparelGraphics.Clear();
			//pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
			//Log.Message("DrawNude: " + pawn.Name);

			//add "clothes"
			foreach (Apparel current in pawn.apparel.WornApparel.Where(x
				=> x.def is bondage_gear_def
				|| (!x.def.thingCategories.NullOrEmpty() && x.def.thingCategories.Any(x => x.defName.ToLower().ContainsAny("vibrator", "piercing", "strapon")))
				|| RJWPreferenceSettings.sex_wear == RJWPreferenceSettings.Clothing.Headgear
				|| keep_hat_on
				&& (!x.def.apparel.bodyPartGroups.NullOrEmpty() && (x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead)
					|| x.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead)))))
			{
				ApparelGraphicRecord item;
				if (ApparelGraphicRecordGetter.TryGetGraphicApparel(current, pawn.story.bodyType, out item))
				{
					pawn.Drawer.renderer.graphics.apparelGraphics.Add(item);
				}
			}
			GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
		}

		public static void reduce_rest(Pawn pawn, float x = 1f)
		{
			if (pawn.Has(Quirk.Vigorous)) x -= x / 2;

			Need_Rest need_rest = pawn.needs.TryGetNeed<Need_Rest>();
			if (need_rest == null)
				return;

			need_rest.CurLevel -= need_rest.RestFallPerTick * x;
		}
		public static void OffsetPsyfocus(Pawn pawn, float x = 0)//0-1
		{
			if (ModsConfig.RoyaltyActive)
			{
				//pawn.psychicEntropy.Notify_Meditated();
				if (pawn.HasPsylink)
				{
					pawn.psychicEntropy.OffsetPsyfocusDirectly(x);
				}
			}
		}

		//Takes the nutrition away from the one penetrating(probably) and injects it to the one on the receiving end
		//As with everything in the mod, this could be greatly extended, current focus though is to prevent starvation of those caught in a huge horde of rappers (that may happen with some mods) 
		public static void TransferNutrition(SexProps props)
		{
			//Log.Message("xxx::TransferNutrition:: " + xxx.get_pawnname(pawn) + " => " + xxx.get_pawnname(partner)); 
			if (props.partner?.needs == null)
			{
				//Log.Message("xxx::TransferNutrition() failed due to lack of transfer equipment or pawn ");
				return;
			}
			if (props.pawn?.needs == null)
			{
				//Log.Message("xxx::TransferNutrition() failed due to lack of transfer equipment or pawn ");
				return;
			}

			//transfer nutrition from presumanbly "giver" to partner
			if (props.sexType == xxx.rjwSextype.Sixtynine ||
				props.sexType == xxx.rjwSextype.Oral ||
				props.sexType == xxx.rjwSextype.Cunnilingus ||
				props.sexType == xxx.rjwSextype.Fellatio)
			{
				Pawn pawn = props.pawn;
				Pawn partner = props.partner;
				Pawn giver, receiver;
				List<Hediff> pawnparts = pawn.GetGenitalsList();
				List<Hediff> partnerparts = partner.GetGenitalsList();
				bool t = false;

				if (props.sexType == xxx.rjwSextype.Sixtynine || Genital_Helper.has_penis_fertile(pawn, pawnparts) || Genital_Helper.has_ovipositorF(pawn, pawnparts))
				{
					giver = pawn;
					receiver = partner;

					Need_Food need = giver.needs?.TryGetNeed<Need_Food>();
					if (need == null)
					{
						//Log.Message("TransferNutrition() " + xxx.get_pawnname(giver) + " doesn't track nutrition in itself, probably shouldn't feed the others");
						return;
					}
					float nutrition_amount = Math.Min(need.MaxLevel / 15f, need.CurLevel); //body size is taken into account implicitly by need.MaxLevel
					giver.needs.food.CurLevel = need.CurLevel - nutrition_amount;
					//Log.Message("TransferNutrition() " + xxx.get_pawnname(giver) + " sent " + nutrition_amount + " of nutrition");

					if (receiver.needs?.TryGetNeed<Need_Food>() != null)
					{
						//Log.Message("xxx::TransferNutrition() " +  xxx.get_pawnname(receiver) + " can receive");
						receiver.needs.food.CurLevel += nutrition_amount;
					}
					TransferThirst(giver, receiver);
					t = true;
				}

				if (props.sexType == xxx.rjwSextype.Sixtynine || (t == false && props.isCoreLovin && (Genital_Helper.has_penis_fertile(partner, partnerparts) || Genital_Helper.has_ovipositorF(partner, partnerparts))))
				{
					giver = partner;
					receiver = pawn;

					Need_Food need = giver.needs?.TryGetNeed<Need_Food>();
					if (need == null)
					{
						//Log.Message("TransferNutrition() " + xxx.get_pawnname(giver) + " doesn't track nutrition in itself, probably shouldn't feed the others");
						return;
					}
					float nutrition_amount = Math.Min(need.MaxLevel / 15f, need.CurLevel); //body size is taken into account implicitly by need.MaxLevel
					giver.needs.food.CurLevel = need.CurLevel - nutrition_amount;
					//Log.Message("TransferNutrition() " + xxx.get_pawnname(giver) + " sent " + nutrition_amount + " of nutrition");

					if (receiver.needs?.TryGetNeed<Need_Food>() != null)
					{
						//Log.Message("TransferNutrition() " +  xxx.get_pawnname(receiver) + " can receive");
						receiver.needs.food.CurLevel += nutrition_amount;
					}
					TransferThirst(giver, receiver);
				}
			}
			TransferNutritionSucc(props);
		}

		public static void TransferThirst(Pawn pawn, Pawn receiver)
		{
			if (xxx.DubsBadHygieneIsActive)
			{
				Need DBHThirst = receiver.needs?.AllNeeds.Find(x => x.def == xxx.DBHThirst);
				if (DBHThirst != null)
				{
					//Log.Message("TransferThirst() " + xxx.get_pawnname(receiver) + " decreasing thirst");
					receiver.needs.TryGetNeed(DBHThirst.def).CurLevel += 0.1f;
				}
			}
		}

		public static void TransferNutritionSucc(SexProps props)
		{
			//succubus mana and rest regen stuff
			if (props.sexType == xxx.rjwSextype.Oral ||
				props.sexType == xxx.rjwSextype.Cunnilingus ||
				props.sexType == xxx.rjwSextype.Fellatio ||
				props.sexType == xxx.rjwSextype.Vaginal ||
				props.sexType == xxx.rjwSextype.Anal ||
				props.sexType == xxx.rjwSextype.DoublePenetration)
			{
				if (xxx.has_traits(props.partner))
				{
					bool gainrest = false;
					if (xxx.RoMIsActive && (props.partner.story.traits.HasTrait(xxx.Succubus) || props.partner.story.traits.HasTrait(xxx.Warlock)))
					{
						Need TM_Mana = props.partner?.needs?.AllNeeds.Find(x => x.def == xxx.TM_Mana);
						if (TM_Mana != null)
						{
							//Log.Message("TransferNutritionSucc() " + xxx.get_pawnname(partner) + " increase mana");
							props.partner.needs.TryGetNeed(TM_Mana.def).CurLevel += 0.1f;
						}
						gainrest = true;
					}

					if (xxx.NightmareIncarnationIsActive)
					{
						foreach (var x in props.partner.AllComps?.Where(x => x.props?.ToString() == "NightmareIncarnation.CompProperties_SuccubusRace"))
						{
							Need NI_Need_Mana = props.partner?.needs?.AllNeeds.Find(x => x.def == xxx.NI_Need_Mana);
							if (NI_Need_Mana != null)
							{
								//Log.Message("TransferNutritionSucc() " + xxx.get_pawnname(partner) + " increase mana");
								props.partner.needs.TryGetNeed(NI_Need_Mana.def).CurLevel += 0.1f;
							}
							gainrest = true;
							break;
						}
					}

					if (gainrest)
					{
						Need_Rest need1 = props.pawn.needs?.TryGetNeed<Need_Rest>();
						Need_Rest need2 = props.partner.needs?.TryGetNeed<Need_Rest>();
						if (need1 != null && need2 != null)
						{
							//Log.Message("TransferNutritionSucc() " + xxx.get_pawnname(partner) + " increase rest");
							props.partner.needs.TryGetNeed(need2.def).CurLevel += 0.25f;
							//Log.Message("TransferNutritionSucc() " + xxx.get_pawnname(pawn) + " decrease rest");
							props.pawn.needs.TryGetNeed(need1.def).CurLevel -= 0.25f;
						}
					}
				}

				if (xxx.has_traits(props.pawn))
				{
					bool gainrest = false;
					if (xxx.RoMIsActive && (props.pawn.story.traits.HasTrait(xxx.Succubus) || props.pawn.story.traits.HasTrait(xxx.Warlock)))
					{
						Need TM_Mana = props.pawn?.needs?.AllNeeds.Find(x => x.def == xxx.TM_Mana);
						if (TM_Mana != null)
						{
							//Log.Message("TransferNutritionSucc() " + xxx.get_pawnname(pawn) + " increase mana");
							props.pawn.needs.TryGetNeed(TM_Mana.def).CurLevel += 0.1f;
						}
					}

					if (xxx.NightmareIncarnationIsActive)
					{
						foreach (var x in props.pawn.AllComps?.Where(x => x.props?.ToString() == "NightmareIncarnation.CompProperties_SuccubusRace"))
						{
							Need NI_Need_Mana = props.pawn?.needs?.AllNeeds.Find(x => x.def == xxx.NI_Need_Mana);
							if (NI_Need_Mana != null)
							{
								//Log.Message("TransferNutritionSucc() " + xxx.get_pawnname(partner) + " increase mana");
								props.pawn.needs.TryGetNeed(NI_Need_Mana.def).CurLevel += 0.1f;
							}
							gainrest = true;
							break;
						}
					}

					if (gainrest)
					{
						Need_Rest need1 = props.partner.needs.TryGetNeed<Need_Rest>();
						Need_Rest need2 = props.pawn.needs.TryGetNeed<Need_Rest>();
						if (need1 != null && need2 != null)
						{
							//Log.Message("TransferNutritionSucc() " + xxx.get_pawnname(pawn) + " increase rest");
							props.pawn.needs.TryGetNeed(need2.def).CurLevel += 0.25f;
							//Log.Message("TransferNutritionSucc() " + xxx.get_pawnname(partner) + " decrease rest");
							props.partner.needs.TryGetNeed(need1.def).CurLevel -= 0.25f;
						}
					}
				}
			}
		}

		public static float get_broken_consciousness_debuff(Pawn pawn)
		{
			if (pawn == null)
			{
				return 1.0f;
			}

			try
			{
				Hediff broken = pawn.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken);
				foreach (PawnCapacityModifier capMod in broken.CapMods)
				{
					if (capMod.capacity == PawnCapacityDefOf.Consciousness)
					{
						if (RJWSettings.DevMode) ModLog.Message("Broken Pawn Consciousness factor: " + capMod.postFactor);
						return capMod.postFactor;
					}
				}

				// fallback
				return 1.0f;
			}
			catch (NullReferenceException)
			{
				//Log.Warning(e.ToString());
				return 1f;
			}
		}

		public static float get_satisfaction_circumstance_multiplier(SexProps props)
		{
			// Get a multiplier for satisfaction that should apply on top of the sex satisfaction stat
			// This is mostly for traits that only affect satisfaction in some circumstances

			Pawn pawn = props.pawn;
			Boolean isViolentSex = props.isRape;

			float multiplier = 1.0f;
			if (xxx.is_human(pawn))
			{
				// Negate consciousness debuffs for broken pawns (counters the sex satisfaction score being affected by low consciousness)
				multiplier = (1.0f / get_broken_consciousness_debuff(pawn));

				// Multiplier bonus for violent traits and violent sex
				if (!props.isReceiver)
				{
					// Rapists/Bloodlusts get 50% more satisfaction from violent encounters, and less from non-violent
					if (isViolentSex)
					{
						if (xxx.is_rapist(pawn) || xxx.is_bloodlust(pawn) || xxx.is_psychopath(pawn))
							multiplier += 0.5f;
						else
							multiplier -= 0.2f;
					}
					else
					{
						if (xxx.is_rapist(pawn) || xxx.is_bloodlust(pawn))
							multiplier -= 0.2f;
						else
							multiplier += 0.2f;
					}
				}
				else if (props.isReceiver)
				{
					// Masochists get 50% more satisfaction from receiving violent sex and 20% less from normal sex
					if (isViolentSex)
					{
						if (xxx.is_masochist(pawn))
							multiplier += 0.5f;
						else
							multiplier -= 0.2f;
					}
					else
					{
						if (xxx.is_masochist(pawn))
							multiplier -= 0.2f;
						else
							multiplier += 0.2f;
					}

					// Multiplier for broken pawns
					if (isViolentSex && AfterSexUtility.BodyIsBroken(pawn))
					{
						// Add bonus satisfaction based on stage
						switch (pawn.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken).CurStageIndex)
						{
							case 0:
								break;

							case 1:
								// 50% bonus for stage 2
								//multiplier += 0.5f;
								break;

							case 2:
								// 100% bonus for stage 2
								multiplier += 1.0f;
								break;
						}
					}
				}
				//ModLog.Message("Sex satisfaction multiplier: " + multiplier);
			}
			return multiplier;
		}
	}
}
