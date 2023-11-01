using System;
using UnityEngine;
using Verse;

namespace rjw
{
	public class RJWSettings : ModSettings
	{
		public static bool animal_on_animal_enabled = false;
		public static bool bestiality_enabled = false;
		public static bool necrophilia_enabled = false;
		private static bool overdrive = false;

		public static bool rape_enabled = false;
		public static bool designated_freewill = true;
		public static bool animal_CP_rape = false;
		public static bool visitor_CP_rape = false;
		public static bool colonist_CP_rape = false;
		public static bool rape_beating = false;
		public static bool gentle_rape_beating = false;
		public static bool rape_stripping = false;

		public static bool cum_filth = true;
		public static bool sounds_enabled = true;
		public static float sounds_sex_volume = 1.0f;
		public static float sounds_cum_volume = 1.0f;
		public static float sounds_voice_volume = 1.0f;
		public static float sounds_orgasm_volume = 1.0f;
		public static float sounds_animal_on_animal_volume = 0.5f;

		public static bool NymphTamed = false;
		public static bool NymphWild = true;
		public static bool NymphRaidEasy = false;
		public static bool NymphRaidHard = false;
		public static bool NymphRaidRP = false;
		public static bool NymphPermanentManhunter = false;
		public static bool NymphSappers = true;
		public static bool FemaleFuta = false;
		public static bool MaleTrap = false;
		public static float male_nymph_chance = 0.0f;
		public static float futa_nymph_chance = 0.0f;
		public static float futa_natives_chance = 0.0f;
		public static float futa_spacers_chance = 0.5f;

		public static bool sexneed_fix = true;
		public static float sexneed_decay_rate = 1.0f;
		public static int Animal_mating_cooldown = 0;

		public static float nonFutaWomenRaping_MaxVulnerability = 0.8f;
		public static float rapee_MinVulnerability_human = 1.2f;

		public static bool RPG_hero_control = false;
		public static bool RPG_hero_control_HC = false;
		public static bool RPG_hero_control_Ironman = false;

		public static bool submit_button_enabled = true;
		public static bool show_RJW_designation_box = false;
		public static ShowParts ShowRjwParts = ShowParts.Show;
		public static float maxDistanceCellsCasual = 100;
		public static float maxDistanceCellsRape = 100;
		public static float maxDistancePathCost = 1000;

		public static bool WildMode = false;
		public static bool HippieMode = false;
		public static bool override_RJW_designation_checks = false;
		public static bool override_control = false;
		public static bool override_lovin = true;
		public static bool override_matin = true;
		public static bool matin_crossbreed = false;
		public static bool GenderlessAsFuta = false;
		public static bool DevMode = false;
		public static bool DebugLogJoinInBed = false;
		public static bool DebugRape = false; 
		public static bool DebugInteraction = false;
		public static bool DebugNymph = false;
		public static bool AddTrait_Rapist = true;
		public static bool AddTrait_Masocist = true;
		public static bool AddTrait_Nymphomaniac = true;
		public static bool AddTrait_Necrophiliac = true;
		public static bool AddTrait_Nerves = true;
		public static bool AddTrait_Zoophiliac = true;
		public static bool AddTrait_FootSlut = true;
		public static bool AddTrait_CumSlut = true;
		public static bool AddTrait_ButtSlut = true;
		public static bool Allow_RMB_DeepTalk = false;
		public static bool Disable_bestiality_pregnancy_relations = false;
		public static bool Disable_egg_pregnancy_relations = false;
		public static bool Disable_MeditationFocusDrain = false;
		public static bool Disable_RecreationDrain = false;
		public static bool UseAdvancedAgeScaling = true;
		public static bool AllowYouthSex = true;
		public static bool sendTraitGainLetters = true;

		private static Vector2 scrollPosition;
		private static float height_modifier = 300f;

		public enum ShowParts
		{
			Extended,
			Show,
			Known,
			Hide
		};

		public static void DoWindowContents(Rect inRect)
		{
			sexneed_decay_rate = Mathf.Clamp(sexneed_decay_rate, 0.0f, 10000.0f);
			nonFutaWomenRaping_MaxVulnerability = Mathf.Clamp(nonFutaWomenRaping_MaxVulnerability, 0.0f, 3.0f);
			rapee_MinVulnerability_human = Mathf.Clamp(rapee_MinVulnerability_human, 0.0f, 3.0f);

			Rect outRect = new Rect(0f, 30f, inRect.width, inRect.height - 30f);
			Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + height_modifier);

			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect); // scroll

			Listing_Standard listingStandard = new Listing_Standard();
			listingStandard.maxOneColumn = true;
			listingStandard.ColumnWidth = viewRect.width / 2.05f;
			listingStandard.Begin(viewRect);
			listingStandard.Gap(4f);
			listingStandard.CheckboxLabeled("animal_on_animal_enabled".Translate(), ref animal_on_animal_enabled, "animal_on_animal_enabled_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("bestiality_enabled".Translate(), ref bestiality_enabled, "bestiality_enabled_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("necrophilia_enabled".Translate(), ref necrophilia_enabled, "necrophilia_enabled_desc".Translate());
			listingStandard.Gap(10f);
			listingStandard.CheckboxLabeled("rape_enabled".Translate(), ref rape_enabled, "rape_enabled_desc".Translate());
			if (rape_enabled)
			{
				listingStandard.Gap(3f);
				listingStandard.CheckboxLabeled("  " + "rape_stripping".Translate(), ref rape_stripping, "rape_stripping_desc".Translate());
				listingStandard.Gap(3f);
				listingStandard.CheckboxLabeled("  " + "ColonistCanCP".Translate(), ref colonist_CP_rape, "ColonistCanCP_desc".Translate());
				listingStandard.Gap(3f);
				listingStandard.CheckboxLabeled("  " + "VisitorsCanCP".Translate(), ref visitor_CP_rape, "VisitorsCanCP_desc".Translate());
				listingStandard.Gap(3f);
				//if (!bestiality_enabled)
				//{
				//	GUI.contentColor = Color.grey;
				//	animal_CP_rape = false;
				//}
				//listingStandard.CheckboxLabeled("  " + "AnimalsCanCP".Translate(), ref animal_CP_rape, "AnimalsCanCP_desc".Translate());
				//if (!bestiality_enabled)
				//	GUI.contentColor = Color.white;
				listingStandard.Gap(3f);
				listingStandard.CheckboxLabeled("  " + "PrisonersBeating".Translate(), ref rape_beating, "PrisonersBeating_desc".Translate());
				listingStandard.Gap(3f);
				listingStandard.CheckboxLabeled("   " + "GentlePrisonersBeating".Translate(), ref gentle_rape_beating, "GentlePrisonersBeating_desc".Translate());
			}
			else
			{
				colonist_CP_rape = false;
				visitor_CP_rape = false;
				animal_CP_rape = false;
				rape_beating = false;
			}
			listingStandard.Gap(10f);
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("cum_filth".Translate(), ref cum_filth, "cum_filth_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("sounds_enabled".Translate(), ref sounds_enabled, "sounds_enabled_desc".Translate());
			if (sounds_enabled)
			{
				listingStandard.Label("sounds_sex_volume".Translate() + ": " + Math.Round(sounds_sex_volume * 100f, 0) + "%", -1f, "sounds_sex_volume_desc".Translate());
				sounds_sex_volume = listingStandard.Slider(sounds_sex_volume, 0f, 2f);
				listingStandard.Label("sounds_cum_volume".Translate() + ": " + Math.Round(sounds_cum_volume * 100f, 0) + "%", -1f, "sounds_cum_volume_desc".Translate());
				sounds_cum_volume = listingStandard.Slider(sounds_cum_volume, 0f, 2f);
				listingStandard.Label("sounds_voice_volume".Translate() + ": " + Math.Round(sounds_voice_volume * 100f, 0) + "%", -1f, "sounds_voice_volume_desc".Translate());
				sounds_voice_volume = listingStandard.Slider(sounds_voice_volume, 0f, 2f);
				listingStandard.Label("sounds_orgasm_volume".Translate() + ": " + Math.Round(sounds_orgasm_volume * 100f, 0) + "%", -1f, "sounds_orgasm_volume_desc".Translate());
				sounds_orgasm_volume = listingStandard.Slider(sounds_orgasm_volume, 0f, 2f);
				listingStandard.Label("sounds_animal_on_animal_volume".Translate() + ": " + Math.Round(sounds_animal_on_animal_volume * 100f, 0) + "%", -1f, "sounds_animal_on_animal_volume_desc".Translate());
				sounds_animal_on_animal_volume = listingStandard.Slider(sounds_animal_on_animal_volume, 0f, 2f);
			}
			listingStandard.Gap(10f);
			listingStandard.CheckboxLabeled("RPG_hero_control_name".Translate(), ref RPG_hero_control, "RPG_hero_control_desc".Translate());
			listingStandard.Gap(5f);
			if (RPG_hero_control)
			{
				listingStandard.CheckboxLabeled("RPG_hero_control_HC_name".Translate(), ref RPG_hero_control_HC, "RPG_hero_control_HC_desc".Translate());
				listingStandard.Gap(5f);
				listingStandard.CheckboxLabeled("RPG_hero_control_Ironman_name".Translate(), ref RPG_hero_control_Ironman, "RPG_hero_control_Ironman_desc".Translate());
				listingStandard.Gap(5f);
			}
			else
			{
				RPG_hero_control_HC = false;
				RPG_hero_control_Ironman = false;
			}

			listingStandard.Gap(10f);
			listingStandard.CheckboxLabeled("UseAdvancedAgeScaling".Translate(), ref UseAdvancedAgeScaling, "UseAdvancedAgeScaling_desc".Translate());
			listingStandard.CheckboxLabeled("AllowYouthSex".Translate(), ref AllowYouthSex, "AllowYouthSex_desc".Translate());

			listingStandard.NewColumn();
			listingStandard.Gap(4f);
			GUI.contentColor = Color.white;
			if (sexneed_decay_rate < 2.5f)
			{
				overdrive = false;
				listingStandard.Label("sexneed_decay_rate_name".Translate() + ": " + Math.Round(sexneed_decay_rate * 100f, 0) + "%", -1f, "sexneed_decay_rate_desc".Translate());
				sexneed_decay_rate = listingStandard.Slider(sexneed_decay_rate, 0.0f, 5.0f);
			}
			else if (sexneed_decay_rate <= 5.0f && !overdrive)
			{
				GUI.contentColor = Color.yellow;
				listingStandard.Label("sexneed_decay_rate_name".Translate() + ": " + Math.Round(sexneed_decay_rate * 100f, 0) + "% [Not recommended]", -1f, "sexneed_decay_rate_desc".Translate());
				sexneed_decay_rate = listingStandard.Slider(sexneed_decay_rate, 0.0f, 5.0f);
				if (sexneed_decay_rate == 5.0f)
				{
					GUI.contentColor = Color.red;
					if (listingStandard.ButtonText("OVERDRIVE"))
						overdrive = true;
				}
				GUI.contentColor = Color.white;
			}
			else
			{
				GUI.contentColor = Color.red;
				listingStandard.Label("sexneed_decay_rate_name".Translate() + ": " + Math.Round(sexneed_decay_rate * 100f, 0) + "% [WARNING: UNSAFE]", -1f, "sexneed_decay_rate_desc".Translate());
				GUI.contentColor = Color.white;
				sexneed_decay_rate = listingStandard.Slider(sexneed_decay_rate, 0.0f, 10000.0f);
			}
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("sexneed_fix_name".Translate(), ref sexneed_fix, "sexneed_fix_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.Label("Animal_mating_cooldown".Translate() + ": " + Animal_mating_cooldown + "h", -1f, "Animal_mating_cooldown_desc".Translate());
			Animal_mating_cooldown = (int)listingStandard.Slider(Animal_mating_cooldown, 0, 100);
			listingStandard.Gap(5f);
			if (rape_enabled)
			{
				listingStandard.Label("NonFutaWomenRaping_MaxVulnerability".Translate() + ": " + (int)(nonFutaWomenRaping_MaxVulnerability * 100), -1f, "NonFutaWomenRaping_MaxVulnerability_desc".Translate());
				nonFutaWomenRaping_MaxVulnerability = listingStandard.Slider(nonFutaWomenRaping_MaxVulnerability, 0.0f, 3.0f);
				listingStandard.Label("Rapee_MinVulnerability_human".Translate() + ": " + (int)(rapee_MinVulnerability_human * 100), -1f, "Rapee_MinVulnerability_human_desc".Translate());
				rapee_MinVulnerability_human = listingStandard.Slider(rapee_MinVulnerability_human, 0.0f, 3.0f);
			}
			listingStandard.Gap(20f);
			listingStandard.CheckboxLabeled("NymphTamed".Translate(), ref NymphTamed, "NymphTamed_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("NymphWild".Translate(), ref NymphWild, "NymphWild_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("NymphRaidEasy".Translate(), ref NymphRaidEasy, "NymphRaidEasy_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("NymphRaidHard".Translate(), ref NymphRaidHard, "NymphRaidHard_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("NymphRaidRP".Translate(), ref NymphRaidRP, "NymphRaidRP_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("NymphPermanentManhunter".Translate(), ref NymphPermanentManhunter, "NymphPermanentManhunter_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("NymphSappers".Translate(), ref NymphSappers, "NymphSappers_desc".Translate());
			listingStandard.Gap(5f);

			// Save compatibility check for 1.9.7
			// This can probably be safely removed at a later date, I doubt many players use old saves for long.
			if (male_nymph_chance > 1.0f || futa_nymph_chance > 1.0f || futa_natives_chance > 1.0f || futa_spacers_chance > 1.0f)
			{
				male_nymph_chance = 0.0f;
				futa_nymph_chance = 0.0f;
				futa_natives_chance = 0.0f;
				futa_spacers_chance = 0.0f;
			}

			listingStandard.CheckboxLabeled("FemaleFuta".Translate(), ref FemaleFuta, "FemaleFuta_desc".Translate());
			listingStandard.CheckboxLabeled("MaleTrap".Translate(), ref MaleTrap, "MaleTrap_desc".Translate());
			listingStandard.Label("male_nymph_chance".Translate() + ": " + (int)(male_nymph_chance * 100) + "%", -1f, "male_nymph_chance_desc".Translate());
			male_nymph_chance = listingStandard.Slider(male_nymph_chance, 0.0f, 1.0f);
			if (FemaleFuta || MaleTrap)
			{
				listingStandard.Label("futa_nymph_chance".Translate() + ": " + (int)(futa_nymph_chance * 100) + "%", -1f, "futa_nymph_chance_desc".Translate());
				futa_nymph_chance = listingStandard.Slider(futa_nymph_chance, 0.0f, 1.0f);
			}
			if (FemaleFuta || MaleTrap)
			{
				listingStandard.Label("futa_natives_chance".Translate() + ": " + (int)(futa_natives_chance * 100) + "%", -1f, "futa_natives_chance_desc".Translate());
				futa_natives_chance = listingStandard.Slider(futa_natives_chance, 0.0f, 1.0f);
				listingStandard.Label("futa_spacers_chance".Translate() + ": " + (int)(futa_spacers_chance * 100) + "%", -1f, "futa_spacers_chance_desc".Translate());
				futa_spacers_chance = listingStandard.Slider(futa_spacers_chance, 0.0f, 1.0f);
			}

			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("SendTraitGainLetters".Translate(), ref RJWSettings.sendTraitGainLetters, "SendTraitGainLetters_desc".Translate());


			listingStandard.End();
			height_modifier = listingStandard.CurHeight;
			Widgets.EndScrollView();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref animal_on_animal_enabled, "animal_on_animal_enabled", animal_on_animal_enabled, true);
			Scribe_Values.Look(ref bestiality_enabled, "bestiality_enabled", bestiality_enabled, true);
			Scribe_Values.Look(ref necrophilia_enabled, "necrophilia_enabled", necrophilia_enabled, true);
			Scribe_Values.Look(ref designated_freewill, "designated_freewill", designated_freewill, true);
			Scribe_Values.Look(ref rape_enabled, "rape_enabled", rape_enabled, true);
			Scribe_Values.Look(ref colonist_CP_rape, "colonist_CP_rape", colonist_CP_rape, true);
			Scribe_Values.Look(ref visitor_CP_rape, "visitor_CP_rape", visitor_CP_rape, true);
			Scribe_Values.Look(ref animal_CP_rape, "animal_CP_rape", animal_CP_rape, true);
			Scribe_Values.Look(ref rape_beating, "rape_beating", rape_beating, true);
			Scribe_Values.Look(ref gentle_rape_beating, "gentle_rape_beating", gentle_rape_beating, true);
			Scribe_Values.Look(ref rape_stripping, "rape_stripping", rape_stripping, true);
			Scribe_Values.Look(ref NymphTamed, "NymphTamed", NymphTamed, true);
			Scribe_Values.Look(ref NymphWild, "NymphWild", NymphWild, true);
			Scribe_Values.Look(ref NymphRaidEasy, "NymphRaidEasy", NymphRaidEasy, true);
			Scribe_Values.Look(ref NymphRaidHard, "NymphRaidHard", NymphRaidHard, true);
			Scribe_Values.Look(ref NymphRaidRP, "NymphRaidRP", NymphRaidRP, true);
			Scribe_Values.Look(ref NymphPermanentManhunter, "NymphPermanentManhunter", NymphPermanentManhunter, true);
			Scribe_Values.Look(ref NymphSappers, "NymphSappers", NymphSappers, true);
			Scribe_Values.Look(ref FemaleFuta, "FemaleFuta", FemaleFuta, true);
			Scribe_Values.Look(ref MaleTrap, "MaleTrap", MaleTrap, true);
			Scribe_Values.Look(ref sounds_enabled, "sounds_enabled", sounds_enabled, true);
			Scribe_Values.Look(ref sounds_sex_volume, "sounds_sexvolume", sounds_sex_volume, true);
			Scribe_Values.Look(ref sounds_cum_volume, "sounds_cumvolume", sounds_cum_volume, true);
			Scribe_Values.Look(ref sounds_voice_volume, "sounds_voicevolume", sounds_voice_volume, true);
			Scribe_Values.Look(ref sounds_orgasm_volume, "sounds_orgasmvolume", sounds_orgasm_volume, true);
			Scribe_Values.Look(ref sounds_animal_on_animal_volume, "sounds_animal_on_animalvolume", sounds_animal_on_animal_volume, true);
			Scribe_Values.Look(ref cum_filth, "cum_filth", cum_filth, true);
			Scribe_Values.Look(ref sexneed_fix, "sexneed_fix", sexneed_fix, true);
			Scribe_Values.Look(ref sexneed_decay_rate, "sexneed_decay_rate", sexneed_decay_rate, true);
			Scribe_Values.Look(ref Animal_mating_cooldown, "Animal_mating_cooldown", Animal_mating_cooldown, true);
			Scribe_Values.Look(ref nonFutaWomenRaping_MaxVulnerability, "nonFutaWomenRaping_MaxVulnerability", nonFutaWomenRaping_MaxVulnerability, true);
			Scribe_Values.Look(ref rapee_MinVulnerability_human, "rapee_MinVulnerability_human", rapee_MinVulnerability_human, true);
			Scribe_Values.Look(ref male_nymph_chance, "male_nymph_chance", male_nymph_chance, true);
			Scribe_Values.Look(ref futa_nymph_chance, "futa_nymph_chance", futa_nymph_chance, true);
			Scribe_Values.Look(ref futa_natives_chance, "futa_natives_chance", futa_natives_chance, true);
			Scribe_Values.Look(ref futa_spacers_chance, "futa_spacers_chance", futa_spacers_chance, true);
			Scribe_Values.Look(ref RPG_hero_control, "RPG_hero_control", RPG_hero_control, true);
			Scribe_Values.Look(ref RPG_hero_control_HC, "RPG_hero_control_HC", RPG_hero_control_HC, true);
			Scribe_Values.Look(ref RPG_hero_control_Ironman, "RPG_hero_control_Ironman", RPG_hero_control_Ironman, true);
			Scribe_Values.Look(ref UseAdvancedAgeScaling, "UseAdvancedAgeScaling", UseAdvancedAgeScaling, true);
			Scribe_Values.Look(ref AllowYouthSex, "AllowTeenSex", AllowYouthSex, true);
			Scribe_Values.Look(ref sendTraitGainLetters, "sendTraitGainLetters", sendTraitGainLetters, true);
		}
	}
}
