#nullable enable

using Psychology;
using SyrTraits;
using System.Text;
using Verse;
using RimWorld;
using Multiplayer.API;

namespace rjw
{
	public class CompRJW : ThingComp
	{
		/// <summary>
		/// Core comp for genitalia and sexuality tracking.
		/// </summary>
		public CompRJW() { }

		public CompProperties_RJW Props => (CompProperties_RJW)props;
		public Orientation orientation = Orientation.None;
		public StringBuilder quirks = new();
		public string quirksave = ""; // Not the most elegant way to do this, but it minimizes the save bloat.
		public int NextHookupTick;
		private bool BootStrapTriggered = false;
		public Need_Sex? sexNeed;

		/// <summary>
		/// <para>Gets the pawn for this comp.</para>
		/// <para>May return null if the comp's parent is somehow not a pawn.</para>
		/// </summary>
		private Pawn? pawn;
		private Pawn? Pawn => parent is Pawn pawn ? pawn : null;

		// This automatically checks that genitalia has been added to all freshly spawned pawns.
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);

			if (pawn == null)
			{
				pawn = Pawn;
				if (pawn == null) return;
			}

			if (pawn.kindDef.race.defName.Contains("AIRobot") // No genitalia/sexuality for roombas.
				|| pawn.kindDef.race.defName.Contains("AIPawn") // ...nor MAI.
				|| pawn.kindDef.race.defName.Contains("RPP_Bot")
				|| pawn.kindDef.race.defName.Contains("PRFDrone") // Project RimFactory Revived drones
				) return;

			// No genitalia
			//if (!pawn.RaceProps.body.AllParts.Exists(x => x.def == DefDatabase<BodyPartDef>.GetNamed("Genitals")))
			//	return;

			if (Comp(pawn).orientation == Orientation.None)
			{
				Sexualize(pawn);
				Sexualize(pawn, true);
			}

			//Log.Message("PostSpawnSetup for " + pawn?.Name);
		}

		public override void CompTick()
		{
			base.CompTick();

			if (pawn == null)
			{
				pawn = Pawn;
				if (pawn == null) return;
			}

			if (pawn.IsHashIntervalTick(1500) && !pawn.health.Dead)
			// The `NeedInterval` function is called every 150 ticks but was formerly
			// rate limited by a factor of 10.  This will make the upkeep execute at
			// the same interval.
			{
				if (sexNeed == null) 
				{
					sexNeed = pawn.needs?.TryGetNeed<Need_Sex>();
				}
				if (sexNeed != null)
				{
					DoUpkeep(pawn, sexNeed);
				}
			}
		}

		/// <summary>
		/// Performs some upkeep tasks originally handled in `Need_Sex.NeedInterval`.
		/// </summary>
		private void DoUpkeep(Pawn pawn, Need_Sex sexNeed)
		{
			if (pawn.Map == null) return;
			if (xxx.is_asexual(pawn)) return;

			var curLevel = sexNeed.CurLevel;

			// Update psyfocus if the pawn is awake.
			if (!RJWSettings.Disable_MeditationFocusDrain && pawn.Awake() && curLevel < sexNeed.thresh_frustrated())
				SexUtility.OffsetPsyfocus(pawn, -0.01f);
			//if (curLevel < sexNeed.thresh_horny())
			//	SexUtility.OffsetPsyfocus(pawn, -0.01f);
			//if (curLevel < sexNeed.thresh_frustrated() || curLevel > sexNeed.thresh_ahegao())
			//	SexUtility.OffsetPsyfocus(pawn, -0.05f);

			if (curLevel < sexNeed.thresh_horny() && (pawn.mindState.canLovinTick - Find.TickManager.TicksGame > 300))
				pawn.mindState.canLovinTick = Find.TickManager.TicksGame + 300;

			// This can probably all just go, since it's now a noop.
			// But I don't know if some mod is patching `xxx.bootstrap` or something.
			if (!BootStrapTriggered)
			{
				//--ModLog.Message("CompRJW::DoUpkeep::calling boostrap - pawn is " + xxx.get_pawnname(pawn));
				xxx.bootstrap(pawn.Map);
				BootStrapTriggered = true;
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			if (Pawn == null) return;

			// Saves the data.
			Scribe_Values.Look(ref orientation, "RJW_Orientation");
			Scribe_Values.Look(ref quirksave, "RJW_Quirks", "");
			Scribe_Values.Look(ref NextHookupTick, "RJW_NextHookupTick");

			//Log.Message("PostExposeData for " + pawn?.Name);

			// Restore quirk data from the truncated save version.
			quirks = new StringBuilder(quirksave);
		}

		public static CompRJW Comp(Pawn pawn)
		{
			// Call CompRJW.Comp(pawn).<method> to check sexuality, etc.
			return pawn.GetComp<CompRJW>();
		}

		public static void CopyPsychologySexuality(Pawn pawn)
		{
			try
			{
				int kinsey = pawn.TryGetComp<CompPsychology>().Sexuality.kinseyRating;
				//Orientation originalOrientation = Comp(pawn).orientation;

				if (!Genital_Helper.has_genitals(pawn) && (pawn.kindDef.race.defName.ToLower().Contains("droid") || pawn.kindDef.race.defName.ToLower().Contains("drone")))
					Comp(pawn).orientation = Orientation.Asexual;
				else if (kinsey == 0)
					Comp(pawn).orientation = Orientation.Heterosexual;
				else if (kinsey == 1)
					Comp(pawn).orientation = Orientation.MostlyHeterosexual;
				else if (kinsey == 2)
					Comp(pawn).orientation = Orientation.LeaningHeterosexual;
				else if (kinsey == 3)
					Comp(pawn).orientation = Orientation.Bisexual;
				else if (kinsey == 4)
					Comp(pawn).orientation = Orientation.LeaningHomosexual;
				else if (kinsey == 5)
					Comp(pawn).orientation = Orientation.MostlyHomosexual;
				else if (kinsey == 6)
					Comp(pawn).orientation = Orientation.Homosexual;
				else
					Comp(pawn).orientation = Orientation.Asexual;
				/*else
					Log.Error("RJW::ERRROR - unknown kinsey scale value: " + kinsey);/*

				/*if (Comp(pawn).orientation != originalOrientation)
					Log.Message("RJW + Psychology: Inherited pawn " + xxx.get_pawnname(pawn) + " sexuality from Psychology - " + Comp(pawn).orientation);*/
			}
			catch
			{
				if (!pawn.IsAnimal())
					ModLog.Warning("CopyPsychologySexuality " + pawn?.Name + ", def: " + pawn?.def?.defName + ", kindDef: " + pawn?.kindDef?.race.defName);
			}
		}

		public static void CopyIndividualitySexuality(Pawn pawn)
		{
			try
			{
				CompIndividuality.Sexuality individualitySexuality = pawn.TryGetComp<CompIndividuality>().sexuality;
				//Orientation originalOrientation = Comp(pawn).orientation;

				if (individualitySexuality == CompIndividuality.Sexuality.Asexual)
					Comp(pawn).orientation = Orientation.Asexual;
				else if (!Genital_Helper.has_genitals(pawn) && (pawn.kindDef.race.defName.ToLower().Contains("droid") || pawn.kindDef.race.defName.ToLower().Contains("drone")))
					Comp(pawn).orientation = Orientation.Asexual;
				else if (individualitySexuality == CompIndividuality.Sexuality.Straight)
					Comp(pawn).orientation = Orientation.Heterosexual;
				else if (individualitySexuality == CompIndividuality.Sexuality.Bisexual)
					Comp(pawn).orientation = Orientation.Bisexual;
				else if (individualitySexuality == CompIndividuality.Sexuality.Gay)
					Comp(pawn).orientation = Orientation.Homosexual;
				else
					Comp(pawn).orientation = Orientation.Asexual;

				/*if (Comp(pawn).orientation != originalOrientation)
					Log.Message("RJW + [SYR]Individuality: Inherited pawn " + xxx.get_pawnname(pawn) + " sexuality from Individuality - " + Comp(pawn).orientation);*/
			}
			catch
			{
				if (!pawn.IsAnimal())
					ModLog.Warning("CopyIndividualitySexuality " + pawn?.Name + ", def: " + pawn?.def?.defName + ", kindDef: " + pawn?.kindDef?.race.defName);
			}
		}

		public static void VanillaTraitCheck(Pawn pawn)
		{
			//Orientation originalOrientation = Comp(pawn).orientation;
			if (pawn.story.traits.HasTrait(TraitDefOf.Asexual))
				Comp(pawn).orientation = Orientation.Asexual;
			else if (!Genital_Helper.has_genitals(pawn) && (pawn.kindDef.race.defName.ToLower().Contains("droid") || pawn.kindDef.race.defName.ToLower().Contains("drone")))
				Comp(pawn).orientation = Orientation.Asexual;
			else if (pawn.story.traits.HasTrait(TraitDefOf.Gay))
				Comp(pawn).orientation = Orientation.Homosexual;
			else if (pawn.story.traits.HasTrait(TraitDefOf.Bisexual))
				Comp(pawn).orientation = Orientation.Bisexual;
			else
				Comp(pawn).orientation = Orientation.Heterosexual;
		}

		// The main method for adding genitalia and orientation.
		public void Sexualize(Pawn pawn, bool reroll = false)
		{
			if (reroll)
			{
				Comp(pawn).orientation = Orientation.None;

				if (xxx.has_quirk(pawn, "Fertile"))
				{
					Hediff fertility = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("IncreasedFertility"));
					if (fertility != null)
						pawn.health.RemoveHediff(fertility);
				}
				if (xxx.has_quirk(pawn, "Infertile"))
				{
					Hediff fertility = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("DecreasedFertility"));
					if (fertility != null)
						pawn.health.RemoveHediff(fertility);
				}
				quirks = new StringBuilder();
			}
			else if (Comp(pawn).orientation != Orientation.None)
				return;

			//roll random RJW orientation
			Comp(pawn).orientation = xxx.is_animal(pawn) ? RollAnimalOrientation(pawn) : RollOrientation(pawn);

			//Asexual nymp re-roll
			//if (xxx.is_nympho(pawn))
			//	while (Comp(pawn).orientation == Orientation.Asexual)
			//	{
			//		Comp(pawn).orientation = RollOrientation();
			//	}

			//Log.Message("Sexualizing pawn " + pawn?.Name + ", def: " + pawn?.def?.defName);

			if (!reroll)
				Sexualizer.sexualize_pawn(pawn);
			//Log.Message("Orientation for pawn " + pawn?.Name + " is " + orientation);

			if (xxx.has_traits(pawn) && Genital_Helper.has_genitals(pawn) && !(pawn.kindDef.race.defName.ToLower().Contains("droid") && !AndroidsCompatibility.IsAndroid(pawn)))
			{
				if (RJWPreferenceSettings.sexuality_distribution == RJWPreferenceSettings.Rjw_sexuality.Vanilla)
					VanillaTraitCheck(pawn);
				if (RJWPreferenceSettings.sexuality_distribution == RJWPreferenceSettings.Rjw_sexuality.Psychology)
					CopyPsychologySexuality(pawn);
				if (RJWPreferenceSettings.sexuality_distribution == RJWPreferenceSettings.Rjw_sexuality.SYRIndividuality)
					CopyIndividualitySexuality(pawn);
			}
			else if (((pawn.kindDef.race.defName.ToLower().Contains("droid")) && !AndroidsCompatibility.IsAndroid(pawn)) || !Genital_Helper.has_genitals(pawn))
			{
				// Droids with no genitalia are set as asexual.
				// If player later adds genitalia to the droid, the droid 'sexuality' gets rerolled.
				Comp(pawn).orientation = Orientation.Asexual;
			}

			QuirkAdder.Generate(pawn);

			if (quirks.Length == 0)
			{
				quirks.Append("None");
				quirksave = quirks.ToString();
			}
		}

		/// <summary>
		/// check try vanilla traits/mods, check rjw genitals, futa check, some rng rolls
		/// </summary>
		/// <param name="pawn"></param>
		/// <param name="partner"></param>
		/// <returns></returns>
		public static bool CheckPreference(Pawn pawn, Pawn partner)
		{
			if (RJWPreferenceSettings.sexuality_distribution == RJWPreferenceSettings.Rjw_sexuality.Vanilla)
			{
				if (xxx.has_traits(pawn))
					VanillaTraitCheck(pawn);
				if (xxx.has_traits(partner))
					VanillaTraitCheck(partner);
			}
			if (RJWPreferenceSettings.sexuality_distribution == RJWPreferenceSettings.Rjw_sexuality.SYRIndividuality)
			{
				CopyIndividualitySexuality(pawn);
				CopyIndividualitySexuality(partner);
			}
			if (RJWPreferenceSettings.sexuality_distribution == RJWPreferenceSettings.Rjw_sexuality.Psychology)
			{
				if (RJWPreferenceSettings.sexuality_distribution == RJWPreferenceSettings.Rjw_sexuality.Psychology)
					CopyPsychologySexuality(pawn);
				CopyPsychologySexuality(partner);
			}

			//if (xxx.is_mechanoid(pawn))
			//	return false;

			if (Comp(pawn) is CompRJW rjwComp)
			{
				var ori = rjwComp.orientation;

				if (ori == Orientation.Pansexual || ori == Orientation.Bisexual)
					return true;

				if (ori == Orientation.Asexual)
					return false;

				var pawnSex = GenderHelper.GetSex(pawn);
				var partnerSex = GenderHelper.GetSex(partner);

				var isHetero = GenderHelper.CanBeHetero(pawnSex, partnerSex);
				var isHomo = GenderHelper.CanBeHomo(pawnSex, partnerSex);

				// Oh you crazy futas. fuck all cuz horny!
				if (isHetero && isHomo)
					return true;

				//RMB ui lag fixes in mp intead of [SyncMethod]
				if (MP.IsInMultiplayer)
					return RollOriMP(ori, isHetero, isHomo);
				else
					return RollOriSP(ori, isHetero, isHomo);
			}
			else
			{
				//ModLog.Message("Error, pawn:" + pawn + " doesn't have orientation comp, modded race?");
				return false;
			}
		}

		//no rng no lag
		public static bool RollOriMP(Orientation ori, bool isHetero, bool isHomo)
		{
			switch (ori)
			{
				case Orientation.Heterosexual:
					return !isHomo;
				case Orientation.MostlyHeterosexual:
					return (!isHomo);
				case Orientation.LeaningHeterosexual:
					return (!isHomo);
				case Orientation.LeaningHomosexual:
					return (!isHetero);
				case Orientation.MostlyHomosexual:
					return (!isHetero);
				case Orientation.Homosexual:
					return !isHetero;
				default:
					ModLog.Error("ERROR - tried to check preference for undetermined sexuality.");
					return false;
			}
		}

		//slight rng, but heavy ddos lag in MP with [SyncMethod]
		public static bool RollOriSP(Orientation ori, bool isHetero, bool isHomo)
		{
			switch (ori)
			{
				case Orientation.Heterosexual:
					return !isHomo;
				case Orientation.MostlyHeterosexual:
					return (!isHomo || Rand.Chance(0.2f));
				case Orientation.LeaningHeterosexual:
					return (!isHomo || Rand.Chance(0.6f));
				case Orientation.LeaningHomosexual:
					return (!isHetero || Rand.Chance(0.6f));
				case Orientation.MostlyHomosexual:
					return (!isHetero || Rand.Chance(0.2f));
				case Orientation.Homosexual:
					return !isHetero;
				default:
					ModLog.Error("ERROR - tried to check preference for undetermined sexuality.");
					return false;
			}
		}

		[SyncMethod]
		public Orientation RollOrientation(Pawn pawn)
		{
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			float random = Rand.Range(0f, 1f);
			float checkpoint = RJWPreferenceSettings.asexual_ratio / RJWPreferenceSettings.GetTotal();

			float checkpoint_pan = checkpoint + (RJWPreferenceSettings.pansexual_ratio / RJWPreferenceSettings.GetTotal());
			float checkpoint_het = checkpoint_pan + (RJWPreferenceSettings.heterosexual_ratio / RJWPreferenceSettings.GetTotal());
			float checkpoint_bi = checkpoint_het + (RJWPreferenceSettings.bisexual_ratio / RJWPreferenceSettings.GetTotal());
			float checkpoint_gay = checkpoint_bi + (RJWPreferenceSettings.homosexual_ratio / RJWPreferenceSettings.GetTotal());

			if (random < checkpoint || !Genital_Helper.has_genitals(pawn))
				return Orientation.Asexual;
			else if (random < checkpoint_pan)
				return Orientation.Pansexual;
			else if (random < checkpoint_het)
				return Orientation.Heterosexual;
			else if (random < checkpoint_het + ((checkpoint_bi - checkpoint_het) * 0.33f))
				return Orientation.MostlyHeterosexual;
			else if (random < checkpoint_het + ((checkpoint_bi - checkpoint_het) * 0.66f))
				return Orientation.LeaningHeterosexual;
			else if (random < checkpoint_bi)
				return Orientation.Bisexual;
			else if (random < checkpoint_bi + ((checkpoint_gay - checkpoint_bi) * 0.33f))
				return Orientation.LeaningHomosexual;
			else if (random < checkpoint_bi + ((checkpoint_gay - checkpoint_bi) * 0.66f))
				return Orientation.MostlyHomosexual;
			else
				return Orientation.Homosexual;
		}

		// Simpler system for animals, with most of them being heterosexual.
		// Don't want to disturb player breeding projects by adding too many gay animals.
		[SyncMethod]
		public Orientation RollAnimalOrientation(Pawn pawn)
		{
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			float random = Rand.Range(0f, 1f);

			if (random < 0.03f || !Genital_Helper.has_genitals(pawn))
				return Orientation.Asexual;
			else if (random < 0.85f)
				return Orientation.Heterosexual;
			else if (random < 0.96f)
				return Orientation.Bisexual;
			else
				return Orientation.Homosexual;
		}
	}
}
