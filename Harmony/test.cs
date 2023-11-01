using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace rjw
{
	class StatsReportUtilityPatch
	{
		[HarmonyPatch(typeof(StatsReportUtility))]
		[StaticConstructorOnStartup]
		public static class Patch_StatsReportUtility
		{
			private static StatDrawEntry DescriptionEntry(Hediff thing)
			{
				return new StatDrawEntry(StatCategoryDefOf.BasicsImportant, "Description".Translate(), "", thing.DescriptionFlavor, 99999, null, Dialog_InfoCard.DefsToHyperlinks(thing.def.descriptionHyperlinks), false);
			}
		}
	}

	class HediffPatch
	{
		[HarmonyPatch(typeof(Hediff))]
		[StaticConstructorOnStartup]
		public static class Patch_Hediff
		{
			public virtual string DescriptionFlavor
			{
				get
				{
					return this.def.description;
				}
			}
		}
	}
}