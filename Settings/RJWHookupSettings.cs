using System;
using UnityEngine;
using Verse;

namespace rjw
{
	public class RJWHookupSettings : ModSettings
	{
		public static bool HookupsEnabled = true;
		public static bool QuickHookupsEnabled = true;
		public static bool NoHookupsDuringWorkHours = true;
		public static bool ColonistsCanHookup = true;
		public static bool ColonistsCanHookupWithVisitor = false;
		public static bool CanHookupWithPrisoner = false;
		public static bool VisitorsCanHookupWithColonists = false;
		public static bool VisitorsCanHookupWithVisitors = true;
		public static bool PrisonersCanHookupWithNonPrisoner = false;
		public static bool PrisonersCanHookupWithPrisoner = true;
		public static float HookupChanceForNonNymphos = 0.3f;
		public static float MinimumFuckabilityToHookup = 0.1f;
		public static float MinimumAttractivenessToHookup = 0.5f;
		public static float MinimumRelationshipToHookup = 20f;

		//public static bool NymphosCanPickAnyone = true;
		public static bool NymphosCanCheat = true;
		public static bool NymphosCanHomewreck = true;
		public static bool NymphosCanHomewreckReverse = true;

		private static Vector2 scrollPosition;
		private static float height_modifier = 0f;

		public static void DoWindowContents(Rect inRect)
		{
			MinimumFuckabilityToHookup = Mathf.Clamp(MinimumFuckabilityToHookup, 0.1f, 1f);
			MinimumAttractivenessToHookup = Mathf.Clamp(MinimumAttractivenessToHookup, 0.0f, 1f);
			MinimumRelationshipToHookup = Mathf.Clamp(MinimumRelationshipToHookup, -100, 100);

			Rect outRect = new Rect(0f, 30f, inRect.width, inRect.height - 30f);
			Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + height_modifier);

			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect); // scroll

			Listing_Standard listingStandard = new Listing_Standard();
			listingStandard.maxOneColumn = true;
			listingStandard.ColumnWidth = viewRect.width / 2.05f;
			listingStandard.Begin(viewRect);
			listingStandard.Gap(4f);

			// Casual sex settings
			listingStandard.CheckboxLabeled("SettingHookupsEnabled".Translate(), ref HookupsEnabled, "SettingHookupsEnabled_desc".Translate());
			if(HookupsEnabled)
				listingStandard.CheckboxLabeled("SettingQuickHookupsEnabled".Translate(), ref QuickHookupsEnabled, "SettingQuickHookupsEnabled_desc".Translate());
			listingStandard.CheckboxLabeled("SettingNoHookupsDuringWorkHours".Translate(), ref NoHookupsDuringWorkHours, "SettingNoHookupsDuringWorkHours_desc".Translate());

			listingStandard.Gap(10f);
			listingStandard.CheckboxLabeled("SettingColonistsCanHookup".Translate(), ref ColonistsCanHookup, "SettingColonistsCanHookup_desc".Translate());
			listingStandard.CheckboxLabeled("SettingColonistsCanHookupWithVisitor".Translate(), ref ColonistsCanHookupWithVisitor, "SettingColonistsCanHookupWithVisitor_desc".Translate());
			listingStandard.CheckboxLabeled("SettingVisitorsCanHookupWithColonists".Translate(), ref VisitorsCanHookupWithColonists, "SettingVisitorsCanHookupWithColonists_desc".Translate());
			listingStandard.CheckboxLabeled("SettingVisitorsCanHookupWithVisitors".Translate(), ref VisitorsCanHookupWithVisitors, "SettingVisitorsCanHookupWithVisitors_desc".Translate());

			listingStandard.Gap(10f);
			listingStandard.CheckboxLabeled("SettingPrisonersCanHookupWithNonPrisoner".Translate(), ref PrisonersCanHookupWithNonPrisoner, "SettingPrisonersCanHookupWithNonPrisoner_desc".Translate());
			listingStandard.CheckboxLabeled("SettingPrisonersCanHookupWithPrisoner".Translate(), ref PrisonersCanHookupWithPrisoner, "SettingPrisonersCanHookupWithPrisoner_desc".Translate());
			listingStandard.CheckboxLabeled("SettingCanHookupWithPrisoner".Translate(), ref CanHookupWithPrisoner, "SettingCanHookupWithPrisoner_desc".Translate());

			listingStandard.Gap(10f);
			//listingStandard.CheckboxLabeled("SettingNymphosCanPickAnyone".Translate(), ref NymphosCanPickAnyone, "SettingNymphosCanPickAnyone_desc".Translate());
			listingStandard.CheckboxLabeled("SettingNymphosCanCheat".Translate(), ref NymphosCanCheat, "SettingNymphosCanCheat_desc".Translate());
			listingStandard.CheckboxLabeled("SettingNymphosCanHomewreck".Translate(), ref NymphosCanHomewreck, "SettingNymphosCanHomewreck_desc".Translate());
			listingStandard.CheckboxLabeled("SettingNymphosCanHomewreckReverse".Translate(), ref NymphosCanHomewreckReverse, "SettingNymphosCanHomewreckReverse_desc".Translate());

			listingStandard.Gap(10f);
			listingStandard.Label("SettingHookupChanceForNonNymphos".Translate() + ": " + (int)(HookupChanceForNonNymphos * 100) + "%", -1f, "SettingHookupChanceForNonNymphos_desc".Translate());
			HookupChanceForNonNymphos = listingStandard.Slider(HookupChanceForNonNymphos, 0.0f, 1.0f);
			listingStandard.Label("SettingMinimumFuckabilityToHookup".Translate() + ": " + (int)(MinimumFuckabilityToHookup * 100) + "%", -1f, "SettingMinimumFuckabilityToHookup_desc".Translate());
			MinimumFuckabilityToHookup = listingStandard.Slider(MinimumFuckabilityToHookup, 0.1f, 1.0f); // Minimum must be above 0.0 to avoid breaking SexAppraiser.would_fuck()'s hard-failure cases that return 0f
			listingStandard.Label("SettingMinimumAttractivenessToHookup".Translate() + ": " + (int)(MinimumAttractivenessToHookup * 100) + "%", -1f, "SettingMinimumAttractivenessToHookup_desc".Translate());
			MinimumAttractivenessToHookup = listingStandard.Slider(MinimumAttractivenessToHookup, 0.0f, 1.0f);
			listingStandard.Label("SettingMinimumRelationshipToHookup".Translate() + ": " + (MinimumRelationshipToHookup), -1f, "SettingMinimumRelationshipToHookup_desc".Translate());
			MinimumRelationshipToHookup = listingStandard.Slider((int)MinimumRelationshipToHookup, -100f, 100f);

			listingStandard.NewColumn();
			listingStandard.Gap(4f);

			listingStandard.End();
			height_modifier = listingStandard.CurHeight;
			Widgets.EndScrollView();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref HookupsEnabled, "SettingHookupsEnabled");
			Scribe_Values.Look(ref QuickHookupsEnabled, "SettingQuickHookupsEnabled");
			Scribe_Values.Look(ref NoHookupsDuringWorkHours, "NoHookupsDuringWorkHours");
			Scribe_Values.Look(ref ColonistsCanHookup, "SettingColonistsCanHookup");
			Scribe_Values.Look(ref ColonistsCanHookupWithVisitor, "SettingColonistsCanHookupWithVisitor");
			Scribe_Values.Look(ref VisitorsCanHookupWithColonists, "SettingVisitorsCanHookupWithColonists");
			Scribe_Values.Look(ref VisitorsCanHookupWithVisitors, "SettingVisitorsCanHookupWithVisitors");

			// Prisoner settings
			Scribe_Values.Look(ref CanHookupWithPrisoner, "SettingCanHookupWithPrisoner");
			Scribe_Values.Look(ref PrisonersCanHookupWithNonPrisoner, "SettingPrisonersCanHookupWithNonPrisoner");
			Scribe_Values.Look(ref PrisonersCanHookupWithPrisoner, "SettingPrisonersCanHookupWithPrisoner");

			// Nympho settings
			//Scribe_Values.Look(ref NymphosCanPickAnyone, "SettingNymphosCanPickAnyone");
			Scribe_Values.Look(ref NymphosCanCheat, "SettingNymphosCanCheat");
			Scribe_Values.Look(ref NymphosCanHomewreck, "SettingNymphosCanHomewreck");
			Scribe_Values.Look(ref NymphosCanHomewreckReverse, "SettingNymphosCanHomewreckReverse");

			Scribe_Values.Look(ref HookupChanceForNonNymphos, "SettingHookupChanceForNonNymphos");
			Scribe_Values.Look(ref MinimumFuckabilityToHookup, "SettingMinimumFuckabilityToHookup");
			Scribe_Values.Look(ref MinimumAttractivenessToHookup, "SettingMinimumAttractivenessToHookup");
			Scribe_Values.Look(ref MinimumRelationshipToHookup, "SettingMinimumRelationshipToHookup");
		}
	}
}
