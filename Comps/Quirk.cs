using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{
	public class Quirk
	{
		public static List<Quirk> All = new List<Quirk>();

		public static readonly Quirk Breeder = new Quirk(
			"Breeder",
			"BreederQuirk");
		public static readonly Quirk Endytophile = new Quirk(
			"Endytophile",
			"EndytophileQuirk",
			(pawn, partner) => !partner.apparel.PsychologicallyNude,
			sexProps => !sexProps.pawn.apparel.PsychologicallyNude
			);
		public static readonly Quirk Exhibitionist = new Quirk(
			"Exhibitionist",
			"ExhibitionistQuirk",
			null,
			SatisfiesExhibitionist
			);
		public static readonly Quirk Fertile = new Quirk(
			"Fertile",
			"FertileQuirk");
		public static readonly Quirk Gerontophile = new Quirk(
			"Gerontophile",
			"GerontophileQuirk",
			(pawn, partner) => SexUtility.ScaleToHumanAge(partner) > 55,
			sexProps => sexProps.hasPartner() && SexUtility.ScaleToHumanAge(sexProps.partner) > 55
			);
		public static readonly Quirk ImpregnationFetish = new Quirk(
			"Impregnation fetish",
			"ImpregnationFetishQuirk",
			(pawn, partner) =>
				PregnancyHelper.CanImpregnate(pawn, partner)
				|| PregnancyHelper.CanImpregnate(partner, pawn),
			sexProps =>
				sexProps.hasPartner()
				&& (PregnancyHelper.CanImpregnate(sexProps.pawn, sexProps.partner, sexProps.sexType)
					|| PregnancyHelper.CanImpregnate(sexProps.partner, sexProps.pawn, sexProps.sexType))
			);
		public static readonly Quirk Incubator = new Quirk(
			"Incubator",
			"IncubatorQuirk");
		public static readonly Quirk Infertile = new Quirk(
			"Infertile",
			"InfertileQuirk");
		public static readonly Quirk Messy = new Quirk(
			"Messy",
			"MessyQuirk");
		public static readonly Quirk Podophile = new Quirk(
			"Podophile",
			"PodophileQuirk",
			null,
			sexProps => sexProps.hasPartner() && sexProps.sexType == xxx.rjwSextype.Footjob && sexProps.IsSubmissive()
			);
		public static readonly Quirk Cumslut = new Quirk(
			"Cumslut",
			"CumslutQuirk",
			null,
			sexProps => sexProps.hasPartner() && sexProps.sexType == xxx.rjwSextype.Fellatio && sexProps.IsSubmissive()
			);
		public static readonly Quirk Buttslut = new Quirk(
			"Buttslut",
			"ButtslutQuirk",
			null,
			sexProps => sexProps.hasPartner() && sexProps.sexType == xxx.rjwSextype.Anal && sexProps.IsSubmissive()
			);
		public static readonly Quirk PregnancyFetish = new Quirk(
			"Pregnancy fetish",
			"PregnancyFetishQuirk",
			(pawn, partner) => partner.IsVisiblyPregnant(),
			sexProps => sexProps.hasPartner() && sexProps.partner.IsVisiblyPregnant()
			);
		public static readonly Quirk Sapiosexual = new Quirk(
			"Sapiosexual",
			"SapiosexualQuirk",
			(pawn, partner) => SatisfiesSapiosexual(partner),
			null
			);
		public static readonly Quirk Somnophile = new Quirk(
			"Somnophile",
			"SomnophileQuirk",
			(pawn, partner) => !partner.Awake(),
			sexProps => sexProps.hasPartner() && !sexProps.partner.Awake()
			);
		public static readonly Quirk Teratophile = new Quirk(
			"Teratophile",
			"TeratophileQuirk",
			(pawn, partner) => SatisfiesTeratophile(partner),
			null
			);
		public static readonly Quirk Vigorous = new Quirk(
			"Vigorous",
			"VigorousQuirk");

		// There might be too many of these I dunno.
		// People have expressed "special interest" in some of them so I thought
		// it would be cool to have them in the game but since people are weird you end up with a lot of fetishes.
		public static readonly Quirk ChitinLover = MakeTagBasedQuirk("Chitin lover", "ChitinLoverQuirk", RaceTag.Chitin);
		public static readonly Quirk DemonLover = MakeTagBasedQuirk("Demon lover", "DemonLoverQuirk", RaceTag.Demon);
		public static readonly Quirk FeatherLover = MakeTagBasedQuirk("Feather lover", "FeatherLoverQuirk", RaceTag.Feathers);
		public static readonly Quirk FurLover = MakeTagBasedQuirk("Fur lover", "FurLoverQuirk", RaceTag.Fur);
		public static readonly Quirk PlantLover = MakeTagBasedQuirk("Plant lover", "PlantLoverQuirk", RaceTag.Plant);
		public static readonly Quirk RobotLover = MakeTagBasedQuirk("Robot lover", "RobotLoverQuirk", RaceTag.Robot);
		public static readonly Quirk ScaleLover = MakeTagBasedQuirk("Scale lover", "ScaleLoverQuirk", RaceTag.Scales);
		public static readonly Quirk SkinLover = MakeTagBasedQuirk("Skin lover", "SkinLoverQuirk", RaceTag.Skin);
		public static readonly Quirk SlimeLover = MakeTagBasedQuirk("Slime lover", "SlimeLoverQuirk", RaceTag.Slime);

		public string Key { get; }
		public string LocaliztionKey { get; }
		// Hack, quirk generation logic should be in the quirk not based on flags on the quirk.
		public RaceTag RaceTag { get; }
		readonly Func<Pawn, Pawn, bool> PawnSatisfiesFunc;
		readonly Func<SexProps, bool> SexSatisfiesFunc;
		readonly Action<Pawn> AfterAddFunc;

		public static Quirk MakeTagBasedQuirk(string key, string localizationKey, RaceTag tag)
		{
			return new Quirk(
				key,
				localizationKey,
				(pawn, partner) => partner.Has(tag),
				sexProps => sexProps.hasPartner() && sexProps.partner.Has(tag),
				null,
				tag);
		}

		public Quirk(
			string key,
			string localizationKey,
			Func<Pawn, Pawn, bool> pawnSatisfies = null,
			Func<SexProps, bool> sexSatisfies = null,
			Action<Pawn> afterAdd = null,
			RaceTag raceTag = null)
		{
			Key = key;
			LocaliztionKey = localizationKey;
			PawnSatisfiesFunc = pawnSatisfies;
			SexSatisfiesFunc = sexSatisfies;
			AfterAddFunc = afterAdd;
			RaceTag = raceTag;
			All.Add(this);
		}

		public bool IsSatisfiedBy(Pawn pawn, Pawn partner)
		{
			return pawn != null
				&& partner != null
				&& pawn.Has(this)
				&& PawnSatisfiesFunc != null
				&& PawnSatisfiesFunc(pawn, partner);
		}

		public static int CountSatisfiedQuirks(SexProps props)
		{
			Pawn pawn = props.pawn;
			Pawn partner = props.partner;
			var satisfies = All.Where(quirk =>
				quirk.SexSatisfiesFunc != null
				&& pawn.Has(quirk)
				&& quirk.SexSatisfiesFunc(props));
			return satisfies.Count();
		}

		public void DoAfterAdd(Pawn pawn)
		{
			AfterAddFunc?.Invoke(pawn);
		}

		public static bool SatisfiesExhibitionist(SexProps sexProps)
		{
			var zoo = xxx.is_zoophile(sexProps.pawn);
			return sexProps.pawn.Map.mapPawns.AllPawnsSpawned.Any(x =>
				x != sexProps.pawn
				&& x != sexProps.partner
				&& !x.Dead
				&& (zoo || !xxx.is_animal(x))
				&& x.CanSee(sexProps.pawn));
		}

		public static bool SatisfiesSapiosexual(Pawn partner)
		{
			if (!xxx.has_traits(partner)) return false;
			return partner.story.traits.HasTrait(TraitDefOf.TooSmart)
				|| (xxx.CTIsActive && partner.story.traits.HasTrait(xxx.RCT_Savant))
				|| (xxx.IndividualityIsActive && partner.story.traits.HasTrait(xxx.SYR_CreativeThinker))
				|| (xxx.CTIsActive && partner.story.traits.HasTrait(xxx.RCT_Inventor))
				|| partner.story.traits.HasTrait(TraitDefOf.GreatMemory)
				|| partner.story.traits.HasTrait(TraitDefOf.Transhumanist)
				|| partner.skills.GetSkill(SkillDefOf.Intellectual).levelInt >= 15;
		}

		public static bool SatisfiesTeratophile(Pawn partner)
		{
			if (partner.story == null) return false;
			var story = partner.story;
			return RelationsUtility.IsDisfigured(partner)
				|| story.bodyType == BodyTypeDefOf.Fat
				|| story.traits.HasTrait(TraitDefOf.CreepyBreathing)
				|| (story.traits.HasTrait(TraitDefOf.Beauty) && story.traits.DegreeOfTrait(TraitDefOf.Beauty) < 0)
				|| partner.GetStatValue(StatDefOf.PawnBeauty) < 0;
		}

		public static void AddThought(Pawn pawn)
		{
			var thoughtDef = DefDatabase<ThoughtDef>.GetNamed("ThatsMyFetish");
			pawn.needs.mood.thoughts.memories.TryGainMemory(thoughtDef);
		}
	}
}
