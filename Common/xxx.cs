using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Multiplayer.API;
using System.Collections.ObjectModel;
//using static RimWorld.Planet.CaravanInventoryUtility;
//using RimWorldChildren;

namespace rjw
{
	public static class xxx
	{
		public static readonly BindingFlags ins_public_or_no = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public static readonly config config = DefDatabase<config>.GetNamed("the_one");

		//HARDCODED MAGIC USED ACROSS DOZENS OF FILES, this is as bad place to put it as any other
		//Should at the very least be encompassed in the related designation type
		public static readonly int max_rapists_per_prisoner = 6;

		//CombatExtended
		public static bool CombatExtendedIsActive;
		public static HediffDef MuscleSpasms;

		//RomanceDiversified
		public static bool RomanceDiversifiedIsActive; //A dirty way to check if the mod is active
		public static TraitDef straight;
		public static TraitDef faithful;
		public static TraitDef philanderer;
		public static TraitDef polyamorous;

		//Psychology
		public static bool PsychologyIsActive;
		public static TraitDef prude;
		public static TraitDef lecher;
		public static TraitDef polygamous;

		//Other poly mods
		public static bool AnyPolyamoryModIsActive;
		public static bool AnyPolygamyModIsActive;

		//[SYR] Individuality
		public static bool IndividualityIsActive;
		public static TraitDef SYR_CreativeThinker;
		public static TraitDef SYR_Haggler;

		//Rimworld of Magic
		public static bool RoMIsActive;
		public static TraitDef Succubus;
		public static TraitDef Warlock;
		public static NeedDef TM_Mana;
		public static HediffDef TM_ShapeshiftHD;

		//Nightmare Incarnation
		public static bool NightmareIncarnationIsActive;
		public static NeedDef NI_Need_Mana;

		//Consolidated
		public static bool CTIsActive;
		public static TraitDef RCT_NeatFreak;
		public static TraitDef RCT_Savant;
		public static TraitDef RCT_Inventor;
		public static TraitDef RCT_AnimalLover;

		//SimpleSlavery
		public static bool SimpleSlaveryIsActive;
		public static HediffDef Enslaved;

		//Dubs Bad Hygiene
		public static bool DubsBadHygieneIsActive;
		public static NeedDef DBHThirst;

		//Alien Framework
		public static bool AlienFrameworkIsActive;
		public static TraitDef xenophobia; // Degrees: 1: xenophobe, -1: xenophile

		//Immortals
		public static bool ImmortalsIsActive;
		public static HediffDef IH_Immortal;

		//Children&Pregnancy
		public static bool RimWorldChildrenIsActive;
		public static HediffDef BabyState;
		public static HediffDef PostPregnancy;
		public static HediffDef Lactating;
		public static HediffDef NoManipulationFlag;
		public static ThoughtDef IGaveBirthFirstTime;
		public static ThoughtDef IGaveBirth;
		public static ThoughtDef PartnerGaveBirth;

		//Babies and Children(pretty much above + this)
		public static HediffDef BnC_RJW_PostPregnancy;

		//RJW-EX
		public static bool RjwExIsActive;
		public static HediffDef RjwEx_Armbinder;

		//RJW Children
		public static HediffDef RJW_BabyState = DefDatabase<HediffDef>.GetNamed("RJW_BabyState");
		public static HediffDef RJW_NoManipulationFlag = DefDatabase<HediffDef>.GetNamed("RJW_NoManipulationFlag");

		//rjw
		public static readonly TraitDef nymphomaniac = DefDatabase<TraitDef>.GetNamed("Nymphomaniac");
		public static readonly TraitDef rapist = DefDatabase<TraitDef>.GetNamed("Rapist");
		public static readonly TraitDef masochist = DefDatabase<TraitDef>.GetNamed("Masochist");
		public static readonly TraitDef necrophiliac = DefDatabase<TraitDef>.GetNamed("Necrophiliac");
		public static readonly TraitDef zoophile = DefDatabase<TraitDef>.GetNamed("Zoophile");

		public static readonly TraitDef footSlut = DefDatabase<TraitDef>.GetNamed("FootSlut");
		public static readonly TraitDef cumSlut = DefDatabase<TraitDef>.GetNamed("CumSlut");
		public static readonly TraitDef buttSlut = DefDatabase<TraitDef>.GetNamed("ButtSlut");

		//The Hediff to prevent reproduction
		public static readonly HediffDef sterilized = DefDatabase<HediffDef>.GetNamed("Sterilized");

		//The Hediff for broken body(resulted from being raped as CP for too many times)
		public static readonly HediffDef feelingBroken = DefDatabase<HediffDef>.GetNamed("FeelingBroken");

		public static readonly HediffDef submitting = DefDatabase<HediffDef>.GetNamed("Hediff_Submitting");

		public static PawnCapacityDef reproduction = DefDatabase<PawnCapacityDef>.GetNamed("RJW_Fertility");

		//rjw Body parts
		public static readonly BodyPartDef genitalsDef = DefDatabase<BodyPartDef>.GetNamed("Genitals");
		public static readonly BodyPartDef breastsDef = DefDatabase<BodyPartDef>.GetNamed("Chest");

		public static readonly BodyPartDef flankDef = DefDatabase<BodyPartDef>.GetNamed("Flank");
		public static readonly BodyPartDef anusDef = DefDatabase<BodyPartDef>.GetNamed("Anus");

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

		public static readonly JobDef Masturbate = DefDatabase<JobDef>.GetNamed("RJW_Masturbate");
		public static readonly JobDef casual_sex = DefDatabase<JobDef>.GetNamed("JoinInBed");
		public static readonly JobDef knotted = DefDatabase<JobDef>.GetNamed("RJW_Knotted");
		public static readonly JobDef gettin_loved = DefDatabase<JobDef>.GetNamed("GettinLoved");
		public static readonly JobDef gettin_raped = DefDatabase<JobDef>.GetNamed("GettinRaped");
		public static readonly JobDef gettin_bred = DefDatabase<JobDef>.GetNamed("GettinBred");
		public static readonly JobDef RapeCP = DefDatabase<JobDef>.GetNamed("RapeComfortPawn");
		public static readonly JobDef RapeEnemy = DefDatabase<JobDef>.GetNamed("RapeEnemy");
		public static readonly JobDef RapeRandom = DefDatabase<JobDef>.GetNamed("RandomRape");
		public static readonly JobDef RapeCorpse = DefDatabase<JobDef>.GetNamed("ViolateCorpse");
		public static readonly JobDef bestiality = DefDatabase<JobDef>.GetNamed("Bestiality");
		public static readonly JobDef bestialityForFemale = DefDatabase<JobDef>.GetNamed("BestialityForFemale");
		public static readonly JobDef whore_inviting_visitors = DefDatabase<JobDef>.GetNamedSilentFail("WhoreInvitingVisitors");
		public static readonly JobDef whore_is_serving_visitors = DefDatabase<JobDef>.GetNamedSilentFail("WhoreIsServingVisitors");
		public static readonly JobDef animalMate = DefDatabase<JobDef>.GetNamed("RJW_Mate");
		public static readonly JobDef animalBreed = DefDatabase<JobDef>.GetNamed("Breed");
		public static readonly JobDef quick_sex = DefDatabase<JobDef>.GetNamed("Quickie");
		public static readonly JobDef getting_quickie = DefDatabase<JobDef>.GetNamed("GettingQuickie");
		public static readonly JobDef struggle_in_BondageGear = DefDatabase<JobDef>.GetNamed("StruggleInBondageGear");
		public static readonly JobDef unlock_BondageGear = DefDatabase<JobDef>.GetNamed("UnlockBondageGear");
		public static readonly JobDef give_BondageGear = DefDatabase<JobDef>.GetNamed("GiveBondageGear");

		public static readonly FleckDef mote_noheart = DefDatabase<FleckDef>.GetNamed("Mote_NoHeart");

		public static readonly StatDef sex_satisfaction = DefDatabase<StatDef>.GetNamed("SexSatisfaction");
		public static readonly StatDef vulnerability_stat = DefDatabase<StatDef>.GetNamed("Vulnerability");
		public static readonly StatDef sex_drive_stat = DefDatabase<StatDef>.GetNamed("SexFrequency");

		public static readonly RecordDef GetRapedAsComfortPawn = DefDatabase<RecordDef>.GetNamed("GetRapedAsComfortPrisoner");
		public static readonly RecordDef CountOfFappin = DefDatabase<RecordDef>.GetNamed("CountOfFappin");
		public static readonly RecordDef CountOfSex = DefDatabase<RecordDef>.GetNamed("CountOfSex");
		public static readonly RecordDef CountOfSexWithHumanlikes = DefDatabase<RecordDef>.GetNamed("CountOfSexWithHumanlikes");
		public static readonly RecordDef CountOfSexWithAnimals = DefDatabase<RecordDef>.GetNamed("CountOfSexWithAnimals");
		public static readonly RecordDef CountOfSexWithInsects = DefDatabase<RecordDef>.GetNamed("CountOfSexWithInsects");
		public static readonly RecordDef CountOfSexWithOthers = DefDatabase<RecordDef>.GetNamed("CountOfSexWithOthers");
		public static readonly RecordDef CountOfSexWithCorpse = DefDatabase<RecordDef>.GetNamed("CountOfSexWithCorpse");
		public static readonly RecordDef CountOfRapedHumanlikes = DefDatabase<RecordDef>.GetNamed("CountOfRapedHumanlikes");
		public static readonly RecordDef CountOfBeenRapedByHumanlikes = DefDatabase<RecordDef>.GetNamed("CountOfBeenRapedByHumanlikes");
		public static readonly RecordDef CountOfRapedAnimals = DefDatabase<RecordDef>.GetNamed("CountOfRapedAnimals");
		public static readonly RecordDef CountOfBeenRapedByAnimals = DefDatabase<RecordDef>.GetNamed("CountOfBeenRapedByAnimals");
		public static readonly RecordDef CountOfRapedInsects = DefDatabase<RecordDef>.GetNamed("CountOfRapedInsects");
		public static readonly RecordDef CountOfBeenRapedByInsects = DefDatabase<RecordDef>.GetNamed("CountOfBeenRapedByInsects");
		public static readonly RecordDef CountOfRapedOthers = DefDatabase<RecordDef>.GetNamed("CountOfRapedOthers");
		public static readonly RecordDef CountOfBeenRapedByOthers = DefDatabase<RecordDef>.GetNamed("CountOfBeenRapedByOthers");
		public static readonly RecordDef CountOfBirthHuman = DefDatabase<RecordDef>.GetNamed("CountOfBirthHuman");
		public static readonly RecordDef CountOfBirthAnimal = DefDatabase<RecordDef>.GetNamed("CountOfBirthAnimal");
		public static readonly RecordDef CountOfBirthEgg = DefDatabase<RecordDef>.GetNamed("CountOfBirthEgg");

		public static readonly MeditationFocusDef SexMeditationFocus = DefDatabase<MeditationFocusDef>.GetNamed("Sex");
		public enum rjwSextype { None, Vaginal, Anal, Oral, Masturbation, DoublePenetration, Boobjob, Handjob, Footjob, Fingering, Scissoring, MutualMasturbation, Fisting, MechImplant, Rimming, Fellatio, Cunnilingus, Sixtynine }
		public static readonly ReadOnlyCollection <rjwSextype> rjwSextypeCollection
			= Array.AsReadOnly((rjwSextype[])Enum.GetValues(typeof(rjwSextype)));
		public static void bootstrap(Map m)
		{
			if (m.GetComponent<MapCom_Injector>() == null)
				m.components.Add(new MapCom_Injector(m));
		}

		//<Summary>Simple method that quickly checks for match from a list.</Summary>
		public static bool ContainsAny(this string haystack, params string[] needles) { return needles.Any(haystack.Contains); }

		public static bool has_traits(Pawn pawn)
		{
			return pawn?.story?.traits != null;
		}

		public static bool has_quirk(Pawn pawn, string quirk)
		{
			return pawn != null && is_human(pawn) && CompRJW.Comp(pawn).quirks.ToString().Contains(quirk);
		}

		[SyncMethod]
		public static string random_pick_a_trait(this Pawn pawn)
		{
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			return has_traits(pawn) ? pawn.story.traits.allTraits.RandomElement().def.defName : null;
		}

		public static bool is_psychopath(Pawn pawn)
		{
			return has_traits(pawn) && pawn.story.traits.HasTrait(TraitDefOf.Psychopath);
		}

		public static bool is_ascetic(Pawn pawn)
		{
			return has_traits(pawn) && pawn.story.traits.HasTrait(TraitDefOf.Ascetic);
		}

		public static bool is_bloodlust(Pawn pawn)
		{
			return has_traits(pawn) && pawn.story.traits.HasTrait(TraitDefOf.Bloodlust);
		}

		public static bool is_brawler(Pawn pawn)
		{
			return has_traits(pawn) && pawn.story.traits.HasTrait(TraitDefOf.Brawler);
		}

		public static bool is_kind(Pawn pawn)
		{
			return has_traits(pawn) && pawn.story.traits.HasTrait(TraitDefOf.Kind);
		}

		public static bool is_rapist(Pawn pawn)
		{
			return has_traits(pawn) && pawn.story.traits.HasTrait(rapist);
		}

		public static bool is_necrophiliac(Pawn pawn)
		{
			return has_traits(pawn) && pawn.story.traits.HasTrait(necrophiliac);
		}

		public static bool is_zoophile(Pawn pawn)
		{
			return has_traits(pawn) && pawn.story.traits.HasTrait(zoophile);
		}

		public static bool is_masochist(Pawn pawn)
		{
			return has_traits(pawn) && pawn.story.traits.HasTrait(TraitDef.Named("Masochist"));
		}

		/// <summary>
		/// Returns true if the given pawn has the nymphomaniac trait.
		/// This may or may not be a nymph pawnKind.
		/// </summary>
		public static bool is_nympho(Pawn pawn)
		{
			return has_traits(pawn) && pawn.story.traits.HasTrait(nymphomaniac);
		}

		public static bool is_slime(Pawn pawn)
		{
			string racename = pawn.kindDef.race.defName.ToLower();
			//if (Prefs.DevMode) ModLog.Message(" is_slime " + xxx.get_pawnname(pawn) + " " + racename.Contains("slime"));

			return racename.Contains("slime");
		}

		public static bool is_demon(Pawn pawn)
		{
			string racename = pawn.kindDef.race.defName.ToLower();
			//if (Prefs.DevMode) ModLog.Message(" is_demon " + xxx.get_pawnname(pawn) + " " + racename.Contains("demon"));

			return racename.Contains("demon");
		}

		public static bool is_asexual(Pawn pawn) => CompRJW.Comp(pawn).orientation == Orientation.Asexual;
		public static bool is_bisexual(Pawn pawn) => CompRJW.Comp(pawn).orientation == Orientation.Bisexual;
		public static bool is_homosexual(Pawn pawn) => (CompRJW.Comp(pawn).orientation == Orientation.Homosexual || CompRJW.Comp(pawn).orientation == Orientation.MostlyHomosexual);
		public static bool is_heterosexual(Pawn pawn) => (CompRJW.Comp(pawn).orientation == Orientation.Heterosexual || CompRJW.Comp(pawn).orientation == Orientation.MostlyHeterosexual);
		public static bool is_pansexual(Pawn pawn) => CompRJW.Comp(pawn).orientation == Orientation.Pansexual;

		public static bool is_slave(Pawn pawn)
		{
			if (is_vanillaslave(pawn) || is_modslave(pawn))
				return true;
			else
				return false;
		}
		public static bool is_vanillaslave(Pawn pawn)
		{
			if (pawn.IsSlave)//1.3
				return true;
			else
				return false;
		}
		public static bool is_modslave(Pawn pawn)
		{
			if (SimpleSlaveryIsActive)
				return pawn?.health.hediffSet.HasHediff(xxx.Enslaved) ?? false;
			else
				return false;
		}

		public static bool is_nympho_or_rapist_or_zoophile(Pawn pawn)
		{
			if (!has_traits(pawn)) { return false; }
			return (is_rapist(pawn) || is_nympho(pawn) || is_zoophile(pawn));
		}

		//Humanoid Alien Framework traits
		public static bool is_xenophile(Pawn pawn)
		{
			if (!has_traits(pawn) || !AlienFrameworkIsActive) { return false; }
			return pawn.story.traits.DegreeOfTrait(xenophobia) == -1;
		}

		public static bool is_xenophobe(Pawn pawn)
		{
			if (!has_traits(pawn) || !AlienFrameworkIsActive) { return false; }
			return pawn.story.traits.DegreeOfTrait(xenophobia) == 1;
		}

		public static bool is_whore(Pawn pawn)
		{
			if (!has_traits(pawn)) { return false; }
			return pawn != null && pawn.IsDesignatedService() && !pawn.story.traits.HasTrait(TraitDefOf.Asexual);
			//return (pawn != null && pawn.ownership != null && pawn.ownership.OwnedBed is Building_WhoreBed && (!xxx.RomanceDiversifiedIsActive || !pawn.story.traits.HasTrait(xxx.asexual)));
		}

		public static bool is_lecher(Pawn pawn)
		{
			if (!has_traits(pawn)) { return false; }
			return RomanceDiversifiedIsActive && pawn.story.traits.HasTrait(philanderer) || PsychologyIsActive && pawn.story.traits.HasTrait(lecher);
		}

		public static bool is_prude(Pawn pawn)
		{
			if (!has_traits(pawn)) { return false; }
			return RomanceDiversifiedIsActive && pawn.story.traits.HasTrait(faithful) || PsychologyIsActive && pawn.story.traits.HasTrait(prude);
		}

		public static bool is_animal(Pawn pawn)
		{
			return pawn?.RaceProps?.Animal ?? false;
		}

		public static bool is_insect(Pawn pawn)
		{
			if (pawn == null) return false;
			bool isit = pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid
						|| pawn.RaceProps.FleshType.corpseCategory.ToString().Contains("CorpsesInsect")
						//genetic rim
						|| pawn.RaceProps.FleshType.defName.Contains("GR_Insectoid");
			//Log.Message("is_insect " + get_pawnname(pawn) + " - " + isit);
			return isit;
		}

		public static bool is_mechanoid(Pawn pawn)
		{
			if (pawn == null) return false;
			if (AndroidsCompatibility.IsAndroid(pawn)) return false;

			bool isit = pawn.RaceProps.IsMechanoid
						|| pawn.RaceProps.FleshType == FleshTypeDefOf.Mechanoid
						|| pawn.RaceProps.FleshType.corpseCategory.ToString().Contains("CorpsesMechanoid")
						//genetic rim
						|| pawn.RaceProps.FleshType.defName.Contains("GR_Mechanoid")
						//android tiers
						|| pawn.RaceProps.FleshType.defName.Contains("MechanisedInfantry")
						|| pawn.RaceProps.FleshType.defName.Contains("Android")
						;
			//Log.Message("is_mechanoid " + get_pawnname(pawn) + " - " + isit);
			return isit;
		}

		public static bool is_tooluser(Pawn pawn)
		{
			return pawn.RaceProps.ToolUser;
		}

		public static bool is_human(Pawn pawn)
		{
			if (pawn == null) return false;
			return pawn.RaceProps.Humanlike;
		}

		public static bool is_female(Pawn pawn)
		{
			return pawn.gender == Gender.Female;
		}
		public static bool is_male(Pawn pawn)
		{
			return pawn.gender == Gender.Male;
		}

		public static bool is_healthy(Pawn pawn)
		{
			return !pawn.Dead &&
				pawn.health.capacities.CanBeAwake &&
				pawn.health.hediffSet.BleedRateTotal <= 0.0f &&
				pawn.health.hediffSet.PainTotal < config.significant_pain_threshold;
		}

		/// <summary>
		/// not going to die soon
		/// </summary>
		/// <param name="pawn"></param>
		/// <returns></returns>
		public static bool is_healthy_enough(Pawn pawn)
		{
			return !pawn.Dead &&
				pawn.health.capacities.CanBeAwake &&
				pawn.health.hediffSet.BleedRateTotal <= 0.1f;
		}

		/// <summary>
		/// pawn can initiate action or respond - whoring, etc
		/// </summary>
		public static bool IsTargetPawnOkay(Pawn pawn)
		{
			return xxx.is_healthy(pawn) && !pawn.Downed && !pawn.Suspended;
		}

		public static bool is_not_dying(Pawn pawn)
		{
			return !pawn.Dead &&
				pawn.health.hediffSet.BleedRateTotal <= 0.3f;
		}

		public static bool is_starved(Pawn pawn)
		{
			return pawn?.needs?.food != null &&
				pawn.needs.food.Starving;
		}
		public static float bleedingRate(Pawn pawn)
		{
			return pawn?.health?.hediffSet?.BleedRateTotal ?? 0f;
		}

		public static string get_pawnname(Pawn who)
		{
			//ModLog.Message("xxx::get_pawnname is "+ who.KindLabelDefinite());
			//ModLog.Message("xxx::get_pawnname is "+ who.KindLabelIndefinite());
			if (who == null) return "null";

			string name = who.Label;
			if (name != null)
			{
				if (who.Name?.ToStringShort != null)
					name = who.Name.ToStringShort;
			}
			else
				name = "noname";

			return name;
		}

		public static bool is_gettin_rapedNow(Pawn pawn)
		{
			if (pawn?.jobs?.curDriver != null)
			{
				return pawn.jobs.curDriver.GetType() == typeof(JobDriver_SexBaseRecieverRaped);
			}
			return false;
		}

		//cells checks are cheap, pathing is expensive. Do pathing check last.)

		public static float need_some_sex(Pawn pawn)
		{
			// 3=> always horny for non humanlikes
			float horniness_degree = 3f;
			Need_Sex need_sex = pawn.needs.TryGetNeed<Need_Sex>();
			if (need_sex == null) return horniness_degree;
			if (need_sex.CurLevel < need_sex.thresh_frustrated()) horniness_degree = 3f;
			else if (need_sex.CurLevel < need_sex.thresh_horny()) horniness_degree = 2f;
			else if (need_sex.CurLevel < need_sex.thresh_satisfied()) horniness_degree = 1f;
			else horniness_degree = 0f;
			return horniness_degree;
		}
		public enum SexNeed
		{
			Frustrated,
			Horny,
			Neutral,
			Satisfied
		};

		public static SexNeed need_sex(Pawn pawn)
		{
			// 3=> always horny for non humanlikes, since they dont have need
			Need_Sex need_sex = pawn.needs.TryGetNeed<Need_Sex>();
			if (need_sex == null) return SexNeed.Frustrated;

			if (RJWSettings.sexneed_fix)
				return need_sex_fixed(pawn);
			else
				return need_sex_broken(pawn);
		}

		/// <summary>
		/// Original rjw threshholds for sex
		/// </summary>
		/// <param name="pawn"></param>
		/// <returns></returns>
		private static SexNeed need_sex_broken(Pawn pawn)
		{
			Need_Sex need_sex = pawn.needs.TryGetNeed<Need_Sex>();
			if (need_sex.CurLevel >= need_sex.thresh_satisfied())
				return SexNeed.Satisfied;
			else if (need_sex.CurLevel >= need_sex.thresh_neutral())
				return SexNeed.Neutral;
			else if (need_sex.CurLevel >= need_sex.thresh_horny())
				return SexNeed.Horny;
			else
				return SexNeed.Frustrated;
		}

		private static SexNeed need_sex_fixed(Pawn pawn)
		{
			// 3=> always horny for non humanlikes
			Need_Sex need_sex = pawn.needs.TryGetNeed<Need_Sex>();
			if (need_sex.CurLevel <= need_sex.thresh_frustrated())
				return SexNeed.Frustrated;
			else if (need_sex.CurLevel <= need_sex.thresh_horny())
				return SexNeed.Horny;
			else if (need_sex.CurLevel <= need_sex.thresh_neutral())
				return SexNeed.Neutral;
			else
				return SexNeed.Satisfied;
		}

		public static bool is_frustrated(Pawn pawn)
		{
			return need_sex(pawn) == SexNeed.Frustrated;
		}

		public static bool is_horny(Pawn pawn)
		{
			return need_sex(pawn) == SexNeed.Horny;
		}

		public static bool is_hornyorfrustrated(Pawn pawn)
		{
			return (need_sex(pawn) == SexNeed.Horny || need_sex(pawn) == SexNeed.Frustrated);
		}

		public static bool is_neutral(Pawn pawn)
		{
			return need_sex(pawn) == SexNeed.Neutral;
		}
		public static bool is_satisfied(Pawn pawn)
		{
			return need_sex(pawn) == SexNeed.Satisfied;
		}

		/// <summary> Checks to see if the pawn has any partners who don't have a Polyamorous/Polygamous trait; aka someone who'd get mad about sleeping around. </summary>
		/// <returns> True if the pawn has at least one romantic partner who does not have a poly trait. False if no partners or all partners are poly. </returns>
		public static bool HasNonPolyPartner(Pawn pawn, bool OnCurrentMap = false)
		{
			if (pawn.relations == null) // probably droids or who knows what modded abomination
				return false;

			// If they don't have a partner at all we can bail right away.
			if (!LovePartnerRelationUtility.HasAnyLovePartner(pawn))
				return false;

			// They have a partner and a mod that adds a poly trait, so check each partner to see if they're poly.
			foreach (DirectPawnRelation relation in pawn.relations.DirectRelations)
			{
				if (relation.def != PawnRelationDefOf.Lover &&
					relation.def != PawnRelationDefOf.Fiance &&
					relation.def != PawnRelationDefOf.Spouse)
				{
					continue;
				}

				// Dead partners don't count.  And stasis'd partners will never find out!
				if (relation.otherPawn.Dead || relation.otherPawn.Suspended)
					continue;

				// Neither does anyone on another map because cheating away from home is obviously never ever discovered
				if (OnCurrentMap) // check only on Current Map
					if (pawn.Map == null || relation.otherPawn.Map == null || relation.otherPawn.Map != pawn.Map)
						continue;

				if ((AnyPolyamoryModIsActive && relation.otherPawn.story.traits.HasTrait(polyamorous)) ||
					(AnyPolygamyModIsActive && relation.otherPawn.story.traits.HasTrait(polygamous)))
				{
					// We have a partner who has the poly trait!  But they could have multiple partners so keep checking.
					continue;
				}

				// We found a partner who doesn't have a poly trait.
				return true;
			}

			// If we got here then we checked every partner and all of them had a poly trait, so they don't have a non-poly partner.
			return false;
		}

		public static Gender opposite_gender(Gender g)
		{
			switch (g)
			{
				case Gender.Male:
					return Gender.Female;
				case Gender.Female:
					return Gender.Male;
				default:
					return Gender.None;
			}
		}

		public static float get_sex_satisfaction(Pawn pawn)
		{
			try
			{
				return pawn.GetStatValue(xxx.sex_satisfaction, false);
			}
			catch (NullReferenceException)
			//not seeded with stats, error for non humanlikes/corpses
			//this and below should probably be rewritten to do calculations here
			{
				//Log.Warning(e.ToString());
				return 1f;
			}
		}

		public static float get_vulnerability(Pawn pawn)
		{
			try
			{
				return pawn.GetStatValue(vulnerability_stat, false);
			}
			catch (NullReferenceException)
			//not seeded with stats, error for non humanlikes/corpses
			{
				//Log.Warning(e.ToString());
				return 1f;
			}
		}

		public static float get_sex_drive(Pawn pawn)
		{
			try
			{
				return pawn.GetStatValue(sex_drive_stat, false) * (pawn.GetRJWPawnData().raceSexDrive);
			}
			catch (NullReferenceException)
			//not seeded with stats, error for non humanlikes/corpses
			{
				//Log.Warning(e.ToString());
				return 1f;
			}
		}

		public static bool isSingleOrPartnerNotHere(Pawn pawn)
		{
			return LovePartnerRelationUtility.ExistingLovePartner(pawn) == null || LovePartnerRelationUtility.ExistingLovePartner(pawn).Map != pawn.Map;
		}

		//base check
		public static bool can_do_loving(Pawn pawn)
		{
			if (is_mechanoid(pawn))
				return false;

			if (is_human(pawn))
			{
				int age = pawn.ageTracker.AgeBiologicalYears;
				var t = 13;
				if (pawn.GetRJWPawnData().RaceSupportDef?.teenAge != null && pawn.GetRJWPawnData().RaceSupportDef?.teenAge != 0)
				{
					t = pawn.GetRJWPawnData().RaceSupportDef.teenAge;
				}
				else if (!pawn.ageTracker.CurLifeStage.reproductive)
					if (pawn.ageTracker.Growth < 1)
						return false;

				if (age < t)
					return false;

				if (!pawn.apparel.WornApparel.NullOrEmpty())
					if (pawn.apparel.WornApparel.Where(x => x.def.defName.ToLower().Contains("warcasket")).Any())
						return false;

				return true;
			}
			if (is_animal(pawn))
			{
				if (pawn.GetRJWPawnData().RaceSupportDef?.teenAge != null && pawn.GetRJWPawnData().RaceSupportDef?.teenAge != 0)
				{
					int age = pawn.ageTracker.AgeBiologicalYears;
					int t = pawn.GetRJWPawnData().RaceSupportDef.teenAge;
					if (age < t)
						return false;
				}
				//CurLifeStageIndex/Growth for insects since they are not reproductive
				else if (!pawn.ageTracker.CurLifeStage.reproductive)
					if (pawn.ageTracker.Growth < 1)
						return false;

				return true;
			}
			return false;
		}

		public static bool can_do_animalsex(Pawn pawn1, Pawn pawn2)
		{
			bool v = false;
			if (xxx.is_animal(pawn1) && xxx.is_animal(pawn2))
				if (RJWSettings.animal_on_animal_enabled)
					v = true;
			else if (RJWSettings.bestiality_enabled)
					v = true;

			return v;
		}

		public static bool can_masturbate(Pawn pawn)
		{
			if (!RJWPreferenceSettings.FapInArmbinders)
				if (Genital_Helper.hands_blocked(pawn))
					return false;
			if (!RJWPreferenceSettings.FapInBelts)
				if (Genital_Helper.genitals_blocked(pawn))
					return false;

			if (!xxx.can_be_fucked(pawn) && !xxx.can_fuck(pawn)) //TODO: should improve this someday
				return false;

			return true;
		}

		// Penetrative organ check.
		public static bool can_fuck(Pawn pawn)
		{
			//this may cause problems with human mechanoids, like misc. bots or other custom race mechanoids
			if (is_mechanoid(pawn))
				return true;

			if (!can_do_loving(pawn))
				return false;

			var parts = pawn.GetGenitalsList();

			if (Genital_Helper.penis_blocked(pawn) || (!Genital_Helper.has_penis_fertile(pawn, parts) && !Genital_Helper.has_penis_infertile(pawn, parts) && !Genital_Helper.has_ovipositorF(pawn, parts))) return false;

			return true;
		}
		
		// Orifice check.
		public static bool can_be_fucked(Pawn pawn)
		{
			if (is_mechanoid(pawn))
				return false;

			if (!can_do_loving(pawn))
				return false;

			if (Genital_Helper.has_anus(pawn) && !Genital_Helper.anus_blocked(pawn))
				return true;
			if (Genital_Helper.has_vagina(pawn) && !Genital_Helper.vagina_blocked(pawn))
				return true;
			if (Genital_Helper.has_mouth(pawn) && !Genital_Helper.oral_blocked(pawn))
				return true;
			//not sure about below, when female rape male, need to check all code so meh
			//if ((Genital_Helper.has_penis(pawn) || Genital_Helper.has_penis_infertile(pawn) || Genital_Helper.has_ovipositorF(pawn)) && !Genital_Helper.penis_blocked(pawn))
			//	return true;
			//if (Genital_Helper.has_breasts(pawn) && !Genital_Helper.breasts_blocked(pawn))
			//	return true;
			//if (pawn.health.hediffSet.GetNotMissingParts().Any(part => part.IsInGroup(BodyPartGroupDefOf.RightHand) || part.IsInGroup(BodyPartGroupDefOf.LeftHand)) && !Genital_Helper.hands_blocked(pawn))
			//	return true;

			return false;
		}

		public static bool can_rape(Pawn pawn, bool forced = false)
		{
			if (!RJWSettings.rape_enabled)
				return false;

			if (is_mechanoid(pawn))
				return true;

			if (!(can_fuck(pawn) ||
				(!is_male(pawn) && get_vulnerability(pawn) < RJWSettings.nonFutaWomenRaping_MaxVulnerability && can_be_fucked(pawn))))
				return false;

			if (is_human(pawn))
			{
				if (pawn.ageTracker.Growth < 1 && !pawn.ageTracker.CurLifeStage.reproductive)
					return false;

				if (RJWSettings.WildMode || forced)
					return true;

				return need_some_sex(pawn) > 0;
			}

			return true;
		}

		public static bool can_get_raped(Pawn pawn)
		{
			if (!RJWSettings.rape_enabled)
				return false;

			if (!can_be_fucked(pawn))
				return false;

			if (is_human(pawn))
			{
				if (pawn.ageTracker.Growth < 1 && !pawn.ageTracker.CurLifeStage.reproductive)
					return false;

				if (RJWSettings.WildMode)
					return true;

				if (!(RJWSettings.rapee_MinVulnerability_human >= 0 && get_vulnerability(pawn) > RJWSettings.rapee_MinVulnerability_human))
					return false;
			}

			return true;
		}

		public static bool is_Virgin(Pawn pawn)
		{
			//if (RJWSettings.DevMode) ModLog.Message("xxx::is_Virgin check:" +get_pawnname(pawn));
			if (pawn.relations != null)
				if (pawn.relations.ChildrenCount > 0)
				{
					//if (RJWSettings.DevMode) ModLog.Message("xxx::is_Virgin " + " ChildrenCount " + pawn.relations.ChildrenCount);
					return false;
				}

			if (!(
				pawn.records.GetValue(GetRapedAsComfortPawn) == 0 &&
				pawn.records.GetValue(CountOfSex) == 0 &&
				pawn.records.GetValue(CountOfSexWithHumanlikes) == 0 &&
				pawn.records.GetValue(CountOfSexWithAnimals) == 0 &&
				pawn.records.GetValue(CountOfSexWithInsects) == 0 &&
				pawn.records.GetValue(CountOfSexWithOthers) == 0 &&
				pawn.records.GetValue(CountOfSexWithCorpse) == 0 &&
				//pawn.records.GetValue(CountOfWhore) == 0 &&
				pawn.records.GetValue(CountOfRapedHumanlikes) == 0 &&
				pawn.records.GetValue(CountOfBeenRapedByHumanlikes) == 0 &&
				pawn.records.GetValue(CountOfRapedAnimals) == 0 &&
				pawn.records.GetValue(CountOfBeenRapedByAnimals) == 0 &&
				pawn.records.GetValue(CountOfRapedInsects) == 0 &&
				pawn.records.GetValue(CountOfBeenRapedByInsects) == 0 &&
				pawn.records.GetValue(CountOfRapedOthers) == 0 &&
				pawn.records.GetValue(CountOfBeenRapedByOthers) == 0 &&
				pawn.records.GetAsInt(xxx.CountOfBirthHuman) == 0 &&
				pawn.records.GetAsInt(xxx.CountOfBirthAnimal) == 0 &&
				pawn.records.GetAsInt(xxx.CountOfBirthEgg) == 0
				))
			{
				//if (RJWSettings.DevMode) ModLog.Message("xxx::is_Virgin " + "records check fail");
				return false;
			}
			return true;
		}
	}
}
