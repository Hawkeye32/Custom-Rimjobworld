#nullable enable

using System.Collections.Generic;
using Verse;

namespace rjw
{
	[StaticConstructorOnStartup]
	internal class HediffDef_EnemyImplants : HediffDef
	{
		//single parent eggs
		public string parentDef = "";
		//multiparent eggs
		public List<string> parentDefs = new();

		//for implanting eggs
		public bool IsParent(string defName)
		{
			// Predefined parent eggs.
			if (parentDef == defName) return true;
			if (parentDefs.Contains(defName)) return true;
			// Dynamic egg.
			if (parentDef != "Unknown" || defName != "Unknown") return false;
			return RJWPregnancySettings.egg_pregnancy_implant_anyone;
		}
	}

	[StaticConstructorOnStartup]
	internal class HediffDef_InsectEgg : HediffDef_EnemyImplants
	{
		//this is filled from xml
		//1 day = 60000 ticks
		public float eggsize = 1;
		public bool selffertilized = false;

		public List<string> childrenDefs = new();
		public string? UnFertEggDef = null;
		public string? FertEggDef = null;
	}

	[StaticConstructorOnStartup]
	internal class HediffDef_MechImplants : HediffDef_EnemyImplants
	{
		public List<string> randomHediffDefs = new();
		public int minEventInterval = 30000;
		public int maxEventInterval = 90000;

		public List<string> childrenDefs = new();
	}
}