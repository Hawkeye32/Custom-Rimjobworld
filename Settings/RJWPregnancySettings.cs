using System;
using UnityEngine;
using Verse;

namespace rjw
{
	public class RJWPregnancySettings : ModSettings
	{
		public static bool humanlike_pregnancy_enabled = true;
		public static bool animal_pregnancy_enabled = true;
		public static bool bestial_pregnancy_enabled = true;
		public static bool insect_pregnancy_enabled = true;
		public static bool insect_anal_pregnancy_enabled = false;
		public static bool insect_oral_pregnancy_enabled = false;
		public static bool egg_pregnancy_implant_anyone = true;
		public static bool egg_pregnancy_fertilize_anyone = false;
		public static bool egg_pregnancy_genes = true;
		public static float egg_pregnancy_eggs_size = 1.0f;
		public static float egg_pregnancy_ovipositor_capacity_factor = 1f;
		public static bool safer_mechanoid_pregnancy = false;

		public static bool mechanoid_pregnancy_enabled = true;

		public static bool use_parent_method = true;
		public static float humanlike_DNA_from_mother = 0.5f;
		public static float bestiality_DNA_inheritance = 0.5f; // human/beast slider
		public static float bestial_DNA_from_mother = 1.0f; // mother/father slider

		public static bool complex_interspecies = false;
		public static int animal_impregnation_chance = 25;
		public static int humanlike_impregnation_chance = 25;
		public static float interspecies_impregnation_modifier = 0.2f;
		public static float fertility_endage_male = 1.2f;
		public static float fertility_endage_female_humanlike = 0.58f;
		public static float fertility_endage_female_animal = 0.96f;

		public static bool phantasy_pregnancy = false;
		public static float normal_pregnancy_duration = 1.0f;
		public static float egg_pregnancy_duration = 1.0f;
		public static float max_num_momtraits_inherited = 3.0f;
		public static float max_num_poptraits_inherited = 3.0f;

		private static bool useVanillaPregnancy = true;
		public static bool UseVanillaPregnancy => useVanillaPregnancy && ModsConfig.BiotechActive;

		private static Vector2 scrollPosition;
		private static float height_modifier = 0f;

		public static void DoWindowContents(Rect inRect)
		{
			Rect outRect = new Rect(0f, 30f, inRect.width, inRect.height - 30f);
			Rect viewRect = new Rect(0f, 30f, inRect.width - 16f, inRect.height + height_modifier);

			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

			Listing_Standard listingStandard = new Listing_Standard();
			listingStandard.maxOneColumn = true;
			listingStandard.ColumnWidth = viewRect.width / 2.05f;
			listingStandard.Begin(viewRect);
			listingStandard.Gap(4f);
			listingStandard.CheckboxLabeled("RJWH_pregnancy".Translate(), ref humanlike_pregnancy_enabled, "RJWH_pregnancy_desc".Translate());
			listingStandard.Gap(5f);
			if (ModsConfig.BiotechActive)
			{
				listingStandard.CheckboxLabeled("UseVanillaPregnancy".Translate(), ref useVanillaPregnancy, "UseVanillaPregnancy_desc".Translate());
				listingStandard.Gap(5f);
			}
			listingStandard.CheckboxLabeled("RJWA_pregnancy".Translate(), ref animal_pregnancy_enabled, "RJWA_pregnancy_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("RJWB_pregnancy".Translate(), ref bestial_pregnancy_enabled, "RJWB_pregnancy_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("RJWI_pregnancy".Translate(), ref insect_pregnancy_enabled, "RJWI_pregnancy_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("egg_pregnancy_implant_anyone".Translate(), ref egg_pregnancy_implant_anyone, "egg_pregnancy_implant_anyone_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("egg_pregnancy_fertilize_anyone".Translate(), ref egg_pregnancy_fertilize_anyone, "egg_pregnancy_fertilize_anyone_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("egg_pregnancy_genes".Translate(), ref egg_pregnancy_genes, "egg_pregnancy_genes_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("RJWI_analPregnancy".Translate(), ref insect_anal_pregnancy_enabled, "RJWI_analPregnancy_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("RJWI_oralPregnancy".Translate(), ref insect_oral_pregnancy_enabled, "RJWI_oralPregnancy_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.Gap(5f);

			int eggs_size = (int)(egg_pregnancy_eggs_size * 100);
			listingStandard.Label("egg_pregnancy_eggs_size".Translate() + ": " + eggs_size + "%", -1f, "egg_pregnancy_eggs_size_desc".Translate());
			egg_pregnancy_eggs_size = listingStandard.Slider(egg_pregnancy_eggs_size, 0.0f, 1.0f);
			int ovipositor_capacity_factor_percentage = (int)(egg_pregnancy_ovipositor_capacity_factor * 100);
			listingStandard.Label("egg_pregnancy_ovipositor_capacity_factor".Translate() + ": " + ovipositor_capacity_factor_percentage + "%", -1, "egg_pregnancy_ovipositor_capacity_factor_desc".Translate());
			// Note: Choose the domain any wider and a different input method has to be used or the slider width has to be increased!
			egg_pregnancy_ovipositor_capacity_factor = listingStandard.Slider(egg_pregnancy_ovipositor_capacity_factor, .1f, 5f);

			listingStandard.Gap(12f);

			listingStandard.CheckboxLabeled("UseParentMethod".Translate(), ref use_parent_method, "UseParentMethod_desc".Translate());
			listingStandard.Gap(5f);
			if (use_parent_method)
			{
				if (humanlike_DNA_from_mother == 0.0f)
				{
					listingStandard.Label("  " + "OffspringLookLikeTheirMother".Translate() + ": " + "AlwaysFather".Translate(), -1f, "OffspringLookLikeTheirMother_desc".Translate());
					humanlike_DNA_from_mother = listingStandard.Slider(humanlike_DNA_from_mother, 0.0f, 1.0f);
				}
				else if (humanlike_DNA_from_mother == 1.0f)
				{
					listingStandard.Label("  " + "OffspringLookLikeTheirMother".Translate() + ": " + "AlwaysMother".Translate(), -1f, "OffspringLookLikeTheirMother_desc".Translate());
					humanlike_DNA_from_mother = listingStandard.Slider(humanlike_DNA_from_mother, 0.0f, 1.0f);
				}
				else
				{
					int value = (int)(humanlike_DNA_from_mother * 100);
					listingStandard.Label("  " + "OffspringLookLikeTheirMother".Translate() + ": " + value + "%", -1f, "OffspringLookLikeTheirMother_desc".Translate());
					humanlike_DNA_from_mother = listingStandard.Slider(humanlike_DNA_from_mother, 0.0f, 1.0f);
				}

				if (bestial_DNA_from_mother == 0.0f)
				{
					listingStandard.Label("  " + "OffspringIsHuman".Translate() + ": " + "AlwaysFather".Translate(), -1f, "OffspringIsHuman_desc".Translate());
					bestial_DNA_from_mother = listingStandard.Slider(bestial_DNA_from_mother, 0.0f, 1.0f);
				}
				else if (bestial_DNA_from_mother == 1.0f)
				{
					listingStandard.Label("  " + "OffspringIsHuman".Translate() + ": " + "AlwaysMother".Translate(), -1f, "OffspringIsHuman_desc".Translate());
					bestial_DNA_from_mother = listingStandard.Slider(bestial_DNA_from_mother, 0.0f, 1.0f);
				}
				else
				{
					int value = (int)(bestial_DNA_from_mother * 100);
					listingStandard.Label("  " + "OffspringIsHuman".Translate() + ": " + value + "%", -1f, "OffspringIsHuman_desc".Translate());
					bestial_DNA_from_mother = listingStandard.Slider(bestial_DNA_from_mother, 0.0f, 1.0f);
				}

				if (bestiality_DNA_inheritance == 0.0f)
				{
					listingStandard.Label("  " + "OffspringIsHuman2".Translate() + ": " + "AlwaysBeast".Translate(), -1f, "OffspringIsHuman2_desc".Translate());
					bestiality_DNA_inheritance = listingStandard.Slider(bestiality_DNA_inheritance, 0.0f, 1.0f);
				}
				else if (bestiality_DNA_inheritance == 1.0f)
				{
					listingStandard.Label("  " + "OffspringIsHuman2".Translate() + ": " + "AlwaysHumanlike".Translate(), -1f, "OffspringIsHuman2_desc".Translate());
					bestiality_DNA_inheritance = listingStandard.Slider(bestiality_DNA_inheritance, 0.0f, 1.0f);
				}
				else
				{
					listingStandard.Label("  " + "OffspringIsHuman2".Translate() + ": " + "UsesOffspringIsHuman".Translate(), -1f, "OffspringIsHuman2_desc".Translate());
					bestiality_DNA_inheritance = listingStandard.Slider(bestiality_DNA_inheritance, 0.0f, 1.0f);
				}
			}
			else
				humanlike_DNA_from_mother = 100;

			listingStandard.Gap(5f);
			listingStandard.Label("max_num_momtraits_inherited".Translate() + ": " + (int)(max_num_momtraits_inherited));
			max_num_momtraits_inherited = listingStandard.Slider(max_num_momtraits_inherited, 0.0f, 9.0f);
			listingStandard.Gap(5f);
			listingStandard.Label("max_num_poptraits_inherited".Translate() + ": " + (int)(max_num_poptraits_inherited));
			max_num_poptraits_inherited = listingStandard.Slider(max_num_poptraits_inherited, 0.0f, 9.0f);
			listingStandard.Gap(5f);

			listingStandard.CheckboxLabeled("MechanoidImplanting".Translate(), ref mechanoid_pregnancy_enabled, "MechanoidImplanting_desc".Translate());
			listingStandard.Gap(5f);
			if (mechanoid_pregnancy_enabled)
			{
				listingStandard.CheckboxLabeled("SaferMechanoidImplanting".Translate(), ref safer_mechanoid_pregnancy, "SaferMechanoidImplanting_desc".Translate());
				listingStandard.Gap(5f);
			}
			listingStandard.CheckboxLabeled("ComplexImpregnation".Translate(), ref complex_interspecies, "ComplexImpregnation_desc".Translate());
			listingStandard.Gap(10f);

			GUI.contentColor = Color.cyan;
			listingStandard.Label("Base pregnancy chances:");
			listingStandard.Gap(5f);
			if (humanlike_pregnancy_enabled)
				listingStandard.Label("  Humanlike/Humanlike (same race): " + humanlike_impregnation_chance + "%");
			else
				listingStandard.Label("  Humanlike/Humanlike (same race): -DISABLED-");
			if (humanlike_pregnancy_enabled && !(humanlike_impregnation_chance * interspecies_impregnation_modifier <= 0.0f) && !complex_interspecies)
				listingStandard.Label("  Humanlike/Humanlike (different race): " + Math.Round(humanlike_impregnation_chance * interspecies_impregnation_modifier, 1) + "%");
			else if (complex_interspecies)
				listingStandard.Label("  Humanlike/Humanlike (different race): -DEPENDS ON SPECIES-");
			else
				listingStandard.Label("  Humanlike/Humanlike (different race): -DISABLED-");
			if (animal_pregnancy_enabled)
				listingStandard.Label("  Animal/Animal (same race): " + animal_impregnation_chance + "%");
			else
				listingStandard.Label("  Animal/Animal (same race): -DISABLED-");
			if (animal_pregnancy_enabled && !(animal_impregnation_chance * interspecies_impregnation_modifier <= 0.0f) && !complex_interspecies)
				listingStandard.Label("  Animal/Animal (different race): " + Math.Round(animal_impregnation_chance * interspecies_impregnation_modifier, 1) + "%");
			else if (complex_interspecies)
				listingStandard.Label("  Animal/Animal (different race): -DEPENDS ON SPECIES-");
			else
				listingStandard.Label("  Animal/Animal (different race): -DISABLED-");
			if (RJWSettings.bestiality_enabled && bestial_pregnancy_enabled && !(animal_impregnation_chance * interspecies_impregnation_modifier <= 0.0f) && !complex_interspecies)
				listingStandard.Label("  Humanlike/Animal: " + Math.Round(animal_impregnation_chance * interspecies_impregnation_modifier, 1) + "%");
			else if (complex_interspecies)
				listingStandard.Label("  Humanlike/Animal: -DEPENDS ON SPECIES-");
			else
				listingStandard.Label("  Humanlike/Animal: -DISABLED-");
			if (RJWSettings.bestiality_enabled && bestial_pregnancy_enabled && !(animal_impregnation_chance * interspecies_impregnation_modifier <= 0.0f) && !complex_interspecies)
				listingStandard.Label("  Animal/Humanlike: " + Math.Round(humanlike_impregnation_chance * interspecies_impregnation_modifier, 1) + "%");
			else if (complex_interspecies)
				listingStandard.Label("  Animal/Humanlike: -DEPENDS ON SPECIES-");
			else
				listingStandard.Label("  Animal/Humanlike: -DISABLED-");
			GUI.contentColor = Color.white;

			listingStandard.NewColumn();
			listingStandard.Gap(4f);
			listingStandard.Label("PregnantCoeffecientForHuman".Translate() + ": " + humanlike_impregnation_chance + "%", -1f, "PregnantCoeffecientForHuman_desc".Translate());
			humanlike_impregnation_chance = (int)listingStandard.Slider(humanlike_impregnation_chance, 0.0f, 100f);
			listingStandard.Label("PregnantCoeffecientForAnimals".Translate() + ": " + animal_impregnation_chance + "%", -1f, "PregnantCoeffecientForAnimals_desc".Translate());
			animal_impregnation_chance = (int)listingStandard.Slider(animal_impregnation_chance, 0.0f, 100f);
			if (!complex_interspecies)
			{
				switch (interspecies_impregnation_modifier)
				{
					case 0.0f:
						GUI.contentColor = Color.grey;
						listingStandard.Label("InterspeciesImpregnantionModifier".Translate() + ": " + "InterspeciesDisabled".Translate(), -1f, "InterspeciesImpregnantionModifier_desc".Translate());
						GUI.contentColor = Color.white;
						break;
					case 1.0f:
						GUI.contentColor = Color.cyan;
						listingStandard.Label("InterspeciesImpregnantionModifier".Translate() + ": " + "InterspeciesMaximum".Translate(), -1f, "InterspeciesImpregnantionModifier_desc".Translate());
						GUI.contentColor = Color.white;
						break;
					default:
						listingStandard.Label("InterspeciesImpregnantionModifier".Translate() + ": " + Math.Round(interspecies_impregnation_modifier * 100, 1) + "%", -1f, "InterspeciesImpregnantionModifier_desc".Translate());
						break;
				}
				interspecies_impregnation_modifier = listingStandard.Slider(interspecies_impregnation_modifier, 0.0f, 1.0f);
			}
			listingStandard.Label("RJW_fertility_endAge_male".Translate() + ": " + (int)(fertility_endage_male * 80) + "In_human_years".Translate(), -1f, "RJW_fertility_endAge_male_desc".Translate());
			fertility_endage_male = listingStandard.Slider(fertility_endage_male, 0.1f, 3.0f);
			listingStandard.Label("RJW_fertility_endAge_female_humanlike".Translate() + ": " + (int)(fertility_endage_female_humanlike * 80) + "In_human_years".Translate(), -1f, "RJW_fertility_endAge_female_humanlike_desc".Translate());
			fertility_endage_female_humanlike = listingStandard.Slider(fertility_endage_female_humanlike, 0.1f, 3.0f);
			listingStandard.Label("RJW_fertility_endAge_female_animal".Translate() + ": " + (int)(fertility_endage_female_animal * 100) + "XofLifeExpectancy".Translate(), -1f, "RJW_fertility_endAge_female_animal_desc".Translate());
			fertility_endage_female_animal = listingStandard.Slider(fertility_endage_female_animal, 0.1f, 3.0f);
			listingStandard.Gap(10f);

			listingStandard.CheckboxLabeled("phantasy_pregnancy".Translate(), ref phantasy_pregnancy, "phantasy_pregnancy_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.Label("normal_pregnancy_duration".Translate() + ": " + (int)(normal_pregnancy_duration * 100) + "%", -1f, "normal_pregnancy_duration_desc".Translate());
			normal_pregnancy_duration = listingStandard.Slider(normal_pregnancy_duration, 0.05f, 2.0f);
			listingStandard.Gap(5f);
			listingStandard.Label("egg_pregnancy_duration".Translate() + ": " + (int)(egg_pregnancy_duration * 100) + "%", -1f, "egg_pregnancy_duration_desc".Translate());
			egg_pregnancy_duration = listingStandard.Slider(egg_pregnancy_duration, 0.05f, 2.0f);
			listingStandard.Gap(5f);

			listingStandard.End();
			height_modifier = listingStandard.CurHeight;
			Widgets.EndScrollView();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref humanlike_pregnancy_enabled, "humanlike_pregnancy_enabled", humanlike_pregnancy_enabled, true);
			Scribe_Values.Look(ref useVanillaPregnancy, "useVanillaPregnancy", useVanillaPregnancy, true);
			Scribe_Values.Look(ref animal_pregnancy_enabled, "animal_enabled", animal_pregnancy_enabled, true);
			Scribe_Values.Look(ref bestial_pregnancy_enabled, "bestial_pregnancy_enabled", bestial_pregnancy_enabled, true);
			Scribe_Values.Look(ref insect_pregnancy_enabled, "insect_pregnancy_enabled", insect_pregnancy_enabled, true);
			Scribe_Values.Look(ref insect_anal_pregnancy_enabled, "insect_anal_pregnancy_enabled", insect_anal_pregnancy_enabled, true);
			Scribe_Values.Look(ref insect_oral_pregnancy_enabled, "insect_oral_pregnancy_enabled", insect_oral_pregnancy_enabled, true);
			Scribe_Values.Look(ref egg_pregnancy_implant_anyone, "egg_pregnancy_implant_anyone", egg_pregnancy_implant_anyone, true);
			Scribe_Values.Look(ref egg_pregnancy_fertilize_anyone, "egg_pregnancy_fertilize_anyone", egg_pregnancy_fertilize_anyone, true);
			Scribe_Values.Look(ref egg_pregnancy_genes, "egg_pregnancy_genes", egg_pregnancy_genes, true);
			Scribe_Values.Look(ref egg_pregnancy_eggs_size, "egg_pregnancy_eggs_size", egg_pregnancy_eggs_size, true);
			Scribe_Values.Look(ref egg_pregnancy_ovipositor_capacity_factor, "egg_pregnancy_ovipositor_capacity_factor", egg_pregnancy_ovipositor_capacity_factor, true);
			Scribe_Values.Look(ref mechanoid_pregnancy_enabled, "mechanoid_enabled", mechanoid_pregnancy_enabled, true);
			Scribe_Values.Look(ref safer_mechanoid_pregnancy, "safer_mechanoid_pregnancy", safer_mechanoid_pregnancy, true);
			Scribe_Values.Look(ref use_parent_method, "use_parent_method", use_parent_method, true);
			Scribe_Values.Look(ref humanlike_DNA_from_mother, "humanlike_DNA_from_mother", humanlike_DNA_from_mother, true);
			Scribe_Values.Look(ref bestial_DNA_from_mother, "bestial_DNA_from_mother", bestial_DNA_from_mother, true);
			Scribe_Values.Look(ref bestiality_DNA_inheritance, "bestiality_DNA_inheritance", bestiality_DNA_inheritance, true);
			Scribe_Values.Look(ref humanlike_impregnation_chance, "humanlike_impregnation_chance", humanlike_impregnation_chance, true);
			Scribe_Values.Look(ref animal_impregnation_chance, "animal_impregnation_chance", animal_impregnation_chance, true);
			Scribe_Values.Look(ref interspecies_impregnation_modifier, "interspecies_impregnation_chance", interspecies_impregnation_modifier, true);
			Scribe_Values.Look(ref complex_interspecies, "complex_interspecies", complex_interspecies, true);
			Scribe_Values.Look(ref fertility_endage_male, "RJW_fertility_endAge_male", fertility_endage_male, true);
			Scribe_Values.Look(ref fertility_endage_female_humanlike, "fertility_endage_female_humanlike", fertility_endage_female_humanlike, true);
			Scribe_Values.Look(ref fertility_endage_female_animal, "fertility_endage_female_animal", fertility_endage_female_animal, true);
			Scribe_Values.Look(ref phantasy_pregnancy, "phantasy_pregnancy", phantasy_pregnancy, true);
			Scribe_Values.Look(ref normal_pregnancy_duration, "normal_pregnancy_duration", normal_pregnancy_duration, true);
			Scribe_Values.Look(ref egg_pregnancy_duration, "egg_pregnancy_duration", egg_pregnancy_duration, true);
			Scribe_Values.Look(ref max_num_momtraits_inherited, "max_num_momtraits_inherited", max_num_momtraits_inherited, true);
			Scribe_Values.Look(ref max_num_poptraits_inherited, "max_num_poptraits_inherited", max_num_poptraits_inherited, true);
		}
	}
}