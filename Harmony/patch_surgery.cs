namespace rjw
{
	/// <summary>
	/// Patch:
	/// recipes
	/// </summary>

	//TODO: inject rjw recipes
	//[HarmonyPatch(typeof(HealthCardUtility), "GenerateSurgeryOption")]
	//internal static class PATCH_HealthCardUtility_recipes
	//{
	//	//private static FloatMenuOption GenerateSurgeryOption(Pawn pawn, Thing thingForMedBills, RecipeDef recipe, IEnumerable<ThingDef> missingIngredients, BodyPartRecord part = null)
	//	//public FloatMenuOption(string label, Action action, MenuOptionPriority priority = MenuOptionPriority.Default, Action mouseoverGuiAction = null, Thing revalidateClickTarget = null, float extraPartWidth = 0, Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null);
	//	//floatMenuOption = new FloatMenuOption(text, action, MenuOptionPriority.Default, null, null, 0f, null, null);
	//	[HarmonyPostfix]
	//	private static void Postfix(ref FloatMenuOption __result, ref Pawn pawn)
	//	{
			
	//		ModLog.Message("PATCH_HealthCardUtility_recipes");
	//		ModLog.Message("PATCH_HealthCardUtility_recipes list: " + __result);
	//		//foreach (FloatMenuOption recipe in recipeOptionsMaker)
	//		//	{
	//		//		ModLog.Message("PATCH_HealthCardUtility_recipes: " + recipe);
	//		//	}

	//		return;
	//	}
	//}
	
	//erm.. idk ?
	//[HarmonyPatch(typeof(HealthCardUtility), "GetTooltip")]
	//internal static class PATCH_HealthCardUtility_GetTooltip
	//{
	//	[HarmonyPostfix]
	//	private static void Postfix(Pawn pawn)
	//	{
	//		ModLog.Message("GetTooltip");
	//		//ModLog.Message("PATCH_HealthCardUtility_recipes list: " + floatMenuOption);
	//		//foreach (FloatMenuOption recipe in recipeOptionsMaker)
	//		//	{
	//		//		ModLog.Message("PATCH_HealthCardUtility_recipes: " + recipe);
	//		//	}
	//		return;
	//	}
	//}
	
	//TODO: make toggle/floatmenu to parts switching
	//[HarmonyPatch(typeof(HealthCardUtility), "EntryClicked")]
	//internal static class PATCH_HealthCardUtility_EntryClicked
	//{
	//	[HarmonyPostfix]
	//	private static void Postfix(Pawn pawn)
	//	{
	//		ModLog.Message("EntryClicked");
	//		//ModLog.Message("PATCH_HealthCardUtility_recipes list: " + floatMenuOption);
	//		//foreach (FloatMenuOption recipe in recipeOptionsMaker)
	//		//	{
	//		//		ModLog.Message("PATCH_HealthCardUtility_recipes: " + recipe);
	//		//	}
	//		return;
	//	}
	//}
}
