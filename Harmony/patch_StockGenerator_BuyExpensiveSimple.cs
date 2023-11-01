using System;
using RimWorld;
using Verse;
using HarmonyLib;

namespace rjw
{
	///<summary>
	///Disable vanilla(?) traders from buying/selling natural rjw parts
	///</summary>
	[HarmonyPatch(typeof(StockGenerator_BuyExpensiveSimple), "HandlesThingDef")]
	[StaticConstructorOnStartup]
	static class PATCH_StockGenerator_BuyExpensiveSimple_HandlesThingDef
	{
		[HarmonyPostfix]
		static void remove_RJW_stuff_fromtraderBUY(ref bool __result, ThingDef thingDef)
		{
			if (thingDef == null)
				return;
			if (thingDef.tradeTags.NullOrEmpty()) 
				return;
			if (thingDef.tradeTags.Contains("RJW_NoBuy")) 
				__result = false;
		}
	}
}
