using HarmonyLib;
using RimWorld;
using System;
using Verse;


namespace rjw
{
	///<summary>
	///RJW Designators checks/update
	///update designators only for selected pawn, once, instead of every tick(60 times per sec)
	///</summary>
	[HarmonyPatch(typeof(Selector), "Select")]
	[StaticConstructorOnStartup]
	static class PawnSelect
	{
		[HarmonyPrefix]
		private static bool Update_Designators_Permissions(Selector __instance, ref object obj)
		{
			if (obj is Pawn)
			{
				//ModLog.Message("Selector patch");
				Pawn pawn = (Pawn)obj;
				//ModLog.Message("pawn: " + xxx.get_pawnname(pawn));
				pawn.UpdatePermissions();
			}
			return true;
		}
	}

	//[HarmonyPatch(typeof(Dialog_InfoCard), "Setup")]
	////[HarmonyPatch(typeof(Dialog_InfoCard), "Dialog_InfoCard", new Type[] {typeof(Def)})]
	//[StaticConstructorOnStartup]
	//static class Button
	//{
	//	[HarmonyPostfix]
	//	public static bool Postfix()
	//	{
	//		ModLog.Message("InfoCardButton");
	//		return true;
	//	}
	//}
}