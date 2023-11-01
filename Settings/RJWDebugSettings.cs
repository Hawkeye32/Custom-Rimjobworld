using System;
using UnityEngine;
using Verse;
using System.Collections.Generic;

namespace rjw
{
	public class RJWDebugSettings : ModSettings
	{
		private static Vector2 scrollPosition;
		private static float height_modifier = 0f;

		public static void DoWindowContents(Rect inRect)
		{
			Rect outRect = new Rect(0f, 30f, inRect.width, inRect.height - 30f);
			Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + height_modifier);

			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect); // scroll

			Listing_Standard listingStandard = new Listing_Standard();
			listingStandard.maxOneColumn = true;
			listingStandard.ColumnWidth = viewRect.width / 2.05f;
			listingStandard.Begin(viewRect);
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("submit_button_enabled".Translate(), ref RJWSettings.submit_button_enabled, "submit_button_enabled_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("RJW_designation_box".Translate(), ref RJWSettings.show_RJW_designation_box, "RJW_designation_box_desc".Translate());
			listingStandard.Gap(5f);
			if (listingStandard.ButtonText("Rjw Parts " + RJWSettings.ShowRjwParts))
			{
				Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
					{
					  new FloatMenuOption("Extended", (() => RJWSettings.ShowRjwParts = RJWSettings.ShowParts.Extended)),
					  new FloatMenuOption("Show", (() => RJWSettings.ShowRjwParts = RJWSettings.ShowParts.Show)),
					  //new FloatMenuOption("Known".Translate(), (() => RJWSettings.ShowRjwParts = RJWSettings.ShowParts.Known)),
					  new FloatMenuOption("Hide", (() => RJWSettings.ShowRjwParts = RJWSettings.ShowParts.Hide))
					}));
			}
			listingStandard.Gap(30f);

			GUI.contentColor = Color.yellow;
			listingStandard.Label("YOU PATHETIC CHEATER ");
			GUI.contentColor = Color.white;
			listingStandard.CheckboxLabeled("override_RJW_designation_checks_name".Translate(), ref RJWSettings.override_RJW_designation_checks, "override_RJW_designation_checks_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("override_control".Translate(), ref RJWSettings.override_control, "override_control_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("AddTrait_Rapist".Translate(), ref RJWSettings.AddTrait_Rapist, "AddTrait_Rapist_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("AddTrait_Masocist".Translate(), ref RJWSettings.AddTrait_Masocist, "AddTrait_Masocist_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("AddTrait_Nymphomaniac".Translate(), ref RJWSettings.AddTrait_Nymphomaniac, "AddTrait_Nymphomaniac_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("AddTrait_Necrophiliac".Translate(), ref RJWSettings.AddTrait_Necrophiliac, "AddTrait_Necrophiliac_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("AddTrait_Nerves".Translate(), ref RJWSettings.AddTrait_Nerves, "AddTrait_Nerves_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("AddTrait_Zoophiliac".Translate(), ref RJWSettings.AddTrait_Zoophiliac, "AddTrait_Zoophiliac_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("AddTrait_FootSlut".Translate(), ref RJWSettings.AddTrait_FootSlut, "AddTrait_FootSlut_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("AddTrait_CumSlut".Translate(), ref RJWSettings.AddTrait_CumSlut, "AddTrait_CumSlut_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("AddTrait_ButtSlut".Translate(), ref RJWSettings.AddTrait_ButtSlut, "AddTrait_ButtSlut_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("Allow_RMB_DeepTalk".Translate(), ref RJWSettings.Allow_RMB_DeepTalk, "Allow_RMB_DeepTalk_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("Disable_bestiality_pregnancy_relations".Translate(), ref RJWSettings.Disable_bestiality_pregnancy_relations, "Disable_bestiality_pregnancy_relations_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("Disable_egg_pregnancy_relations".Translate(), ref RJWSettings.Disable_egg_pregnancy_relations, "Disable_egg_pregnancy_relations_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("Disable_MeditationFocusDrain".Translate(), ref RJWSettings.Disable_MeditationFocusDrain, "Disable_MeditationFocusDrain_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("Disable_RecreationDrain".Translate(), ref RJWSettings.Disable_RecreationDrain, "Disable_RecreationDrain_desc".Translate());
			listingStandard.Gap(5f);

			listingStandard.NewColumn();
			listingStandard.Gap(4f);
			GUI.contentColor = Color.yellow;
			listingStandard.CheckboxLabeled("designated_freewill".Translate(), ref RJWSettings.designated_freewill, "designated_freewill_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("override_lovin".Translate(), ref RJWSettings.override_lovin, "override_lovin_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("override_matin".Translate(), ref RJWSettings.override_matin, "override_matin_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("matin_crossbreed".Translate(), ref RJWSettings.matin_crossbreed, "matin_crossbreed_desc".Translate());
			listingStandard.Gap(5f);
			listingStandard.CheckboxLabeled("DevMode_name".Translate(), ref RJWSettings.DevMode, "DevMode_desc".Translate());
			listingStandard.Gap(5f);
			if (RJWSettings.DevMode)
			{
				listingStandard.CheckboxLabeled("WildMode_name".Translate(), ref RJWSettings.WildMode, "WildMode_desc".Translate());
				listingStandard.Gap(5f);
				listingStandard.CheckboxLabeled("HippieMode_name".Translate(), ref RJWSettings.HippieMode, "HippieMode_desc".Translate());
				listingStandard.Gap(5f);
				listingStandard.CheckboxLabeled("DebugLogJoinInBed".Translate(), ref RJWSettings.DebugLogJoinInBed, "DebugLogJoinInBed_desc".Translate());
				listingStandard.Gap(5f);
				listingStandard.CheckboxLabeled("DebugRape".Translate(), ref RJWSettings.DebugRape, "DebugRape_desc".Translate());
				listingStandard.Gap(5f);
				listingStandard.CheckboxLabeled("DebugInteraction".Translate(), ref RJWSettings.DebugInteraction, "DebugInteraction_desc".Translate());
				listingStandard.Gap(5f);
				listingStandard.CheckboxLabeled("DebugNymph".Translate(), ref RJWSettings.DebugNymph, "DebugNymph_desc".Translate());
				listingStandard.Gap(5f);
			}
			else
			{
				RJWSettings.DebugLogJoinInBed = false;
				RJWSettings.DebugRape = false;
			}
			listingStandard.CheckboxLabeled("GenderlessAsFuta_name".Translate(), ref RJWSettings.GenderlessAsFuta, "GenderlessAsFuta_desc".Translate());
			listingStandard.Gap(5f);
			GUI.contentColor = Color.white;

			listingStandard.Gap(30f);
			listingStandard.Label("maxDistanceCellsCasual_name".Translate() + ": " + (RJWSettings.maxDistanceCellsCasual), -1f, "maxDistanceCellsCasual_desc".Translate());
			RJWSettings.maxDistanceCellsCasual = listingStandard.Slider((int)RJWSettings.maxDistanceCellsCasual, 0, 10000);
			listingStandard.Label("maxDistanceCellsRape_name".Translate() + ": " + (RJWSettings.maxDistanceCellsRape), -1f, "maxDistanceCellsRape_desc".Translate());
			RJWSettings.maxDistanceCellsRape = listingStandard.Slider((int)RJWSettings.maxDistanceCellsRape, 0, 10000);
			listingStandard.Label("maxDistancePathCost_name".Translate() + ": " + (RJWSettings.maxDistancePathCost), -1f, "maxDistancePathCost_desc".Translate());
			RJWSettings.maxDistancePathCost = listingStandard.Slider((int)RJWSettings.maxDistancePathCost, 0, 5000);

			listingStandard.End();
			height_modifier = listingStandard.CurHeight;
			Widgets.EndScrollView();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref RJWSettings.submit_button_enabled, "submit_button_enabled", RJWSettings.submit_button_enabled, true);
			Scribe_Values.Look(ref RJWSettings.show_RJW_designation_box, "show_RJW_designation_box", RJWSettings.show_RJW_designation_box, true);
			Scribe_Values.Look(ref RJWSettings.ShowRjwParts, "ShowRjwParts", RJWSettings.ShowRjwParts, true);
			Scribe_Values.Look(ref RJWSettings.maxDistanceCellsCasual, "maxDistanceCellsCasual", RJWSettings.maxDistanceCellsCasual, true);
			Scribe_Values.Look(ref RJWSettings.maxDistanceCellsRape, "maxDistanceCellsRape", RJWSettings.maxDistanceCellsRape, true);
			Scribe_Values.Look(ref RJWSettings.maxDistancePathCost, "maxDistancePathCost", RJWSettings.maxDistancePathCost, true);

			Scribe_Values.Look(ref RJWSettings.AddTrait_Rapist, "AddTrait_Rapist", RJWSettings.AddTrait_Rapist, true);
			Scribe_Values.Look(ref RJWSettings.AddTrait_Masocist, "AddTrait_Masocist", RJWSettings.AddTrait_Masocist, true);
			Scribe_Values.Look(ref RJWSettings.AddTrait_Nymphomaniac, "AddTrait_Nymphomaniac", RJWSettings.AddTrait_Nymphomaniac, true);
			Scribe_Values.Look(ref RJWSettings.AddTrait_Necrophiliac, "AddTrait_Necrophiliac", RJWSettings.AddTrait_Necrophiliac, true);
			Scribe_Values.Look(ref RJWSettings.AddTrait_Nerves, "AddTrait_Nerves", RJWSettings.AddTrait_Nerves, true);
			Scribe_Values.Look(ref RJWSettings.AddTrait_Zoophiliac, "AddTrait_Zoophiliac", RJWSettings.AddTrait_Zoophiliac, true);
			Scribe_Values.Look(ref RJWSettings.AddTrait_FootSlut, "AddTrait_FootSlut", RJWSettings.AddTrait_FootSlut, true);
			Scribe_Values.Look(ref RJWSettings.AddTrait_CumSlut, "AddTrait_CumSlut", RJWSettings.AddTrait_CumSlut, true);
			Scribe_Values.Look(ref RJWSettings.AddTrait_ButtSlut, "AddTrait_ButtSlut", RJWSettings.AddTrait_ButtSlut, true);

			Scribe_Values.Look(ref RJWSettings.Allow_RMB_DeepTalk, "Allow_RMB_DeepTalk", RJWSettings.Allow_RMB_DeepTalk, true);
			Scribe_Values.Look(ref RJWSettings.Allow_RMB_DeepTalk, "Allow_RMB_DeepTalk", RJWSettings.Allow_RMB_DeepTalk, true);
			Scribe_Values.Look(ref RJWSettings.Disable_bestiality_pregnancy_relations, "Disable_bestiality_pregnancy_relations", RJWSettings.Disable_bestiality_pregnancy_relations, true);
			Scribe_Values.Look(ref RJWSettings.Disable_egg_pregnancy_relations, "Disable_egg_pregnancy_relations", RJWSettings.Disable_egg_pregnancy_relations, true);
			Scribe_Values.Look(ref RJWSettings.Disable_MeditationFocusDrain, "Disable_MeditationFocusDrain", RJWSettings.Disable_MeditationFocusDrain, true);
			Scribe_Values.Look(ref RJWSettings.Disable_RecreationDrain, "Disable_RecreationDrain", RJWSettings.Disable_RecreationDrain, true);

			Scribe_Values.Look(ref RJWSettings.GenderlessAsFuta, "GenderlessAsFuta", RJWSettings.GenderlessAsFuta, true);
			Scribe_Values.Look(ref RJWSettings.override_lovin, "override_lovin", RJWSettings.override_lovin, true);
			Scribe_Values.Look(ref RJWSettings.override_matin, "override_matin", RJWSettings.override_matin, true);
			Scribe_Values.Look(ref RJWSettings.matin_crossbreed, "matin_crossbreed", RJWSettings.matin_crossbreed, true);
			Scribe_Values.Look(ref RJWSettings.WildMode, "Wildmode", RJWSettings.WildMode, true);
			Scribe_Values.Look(ref RJWSettings.HippieMode, "Hippiemode", RJWSettings.HippieMode, true);
			Scribe_Values.Look(ref RJWSettings.override_RJW_designation_checks, "override_RJW_designation_checks", RJWSettings.override_RJW_designation_checks, true);
			Scribe_Values.Look(ref RJWSettings.override_control, "override_control", RJWSettings.override_control, true);
			Scribe_Values.Look(ref RJWSettings.DevMode, "DevMode", RJWSettings.DevMode, true);
			Scribe_Values.Look(ref RJWSettings.DebugLogJoinInBed, "DebugLogJoinInBed", RJWSettings.DebugLogJoinInBed, true);
			Scribe_Values.Look(ref RJWSettings.DebugRape, "DebugRape", RJWSettings.DebugRape, true);
			Scribe_Values.Look(ref RJWSettings.DebugInteraction, "DebugInteraction", RJWSettings.DebugInteraction, true);
			Scribe_Values.Look(ref RJWSettings.DebugNymph, "DebugNymph", RJWSettings.DebugNymph, true);
		}
	}
}
