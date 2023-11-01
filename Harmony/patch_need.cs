namespace rjw
{
	/*
	[HarmonyPatch(typeof(Pawn_NeedsTracker))]
	[HarmonyPatch("ShouldHaveNeed")]
	static class patches_need
	{
		[HarmonyPostfix]
		static void on_postfix(Pawn_NeedsTracker __instance, NeedDef nd, ref bool __result){
			Pawn p=(Pawn)(typeof(Pawn_NeedsTracker).GetField("pawn", xxx.ins_public_or_no).GetValue(__instance));
			__result = __result && (nd.defName != "Sex" || (p!=null && p.Map!=null));
		}
	}
	*/
}