using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace rjw
{
	/// <summary>
	/// Conditional patching class that only does patching if CnP is active
	/// </summary>
	public class CnPcompatibility
	{
		//CnP (or BnC) try to address the joy need of a child, prisoners don't have that. 
		//Considering how you can arrest children without rjw, it is more of a bug of that mod than ours.
		[HarmonyPatch(typeof(Pawn_NeedsTracker), "BindDirectNeedFields")]
		static class jfpk
		{
			[HarmonyPostfix]
			static void postfix(ref Pawn_NeedsTracker __instance)
			{
				Pawn_NeedsTracker tr = __instance;
				FieldInfo fieldInfo = AccessTools.Field(typeof(Pawn_NeedsTracker), "pawn");
				Pawn pawn = fieldInfo.GetValue(__instance) as Pawn;
				if (xxx.is_human(pawn) && pawn.ageTracker.CurLifeStageIndex < AgeStage.Teenager)
				{
					if (tr.TryGetNeed(NeedDefOf.Joy) == null)
					{
						MethodInfo method = AccessTools.Method(typeof(Pawn_NeedsTracker), "AddNeed");
						method.Invoke(tr, new object[] { NeedDefOf.Joy });
					}
				}
			}
		}

		//For whatever reason, harmony shits itself, when trying to patch non void private fields, below are variants the code that would work nicely, if it didn't
		//[HarmonyPatch]
		//[StaticConstructorOnStartup]
		//internal static class joy_for_prison_kids
		//{
		//    [HarmonyTargetMethod]
		//    static MethodBase CalculateMethod(HarmonyInstance instance)
		//    {
		//        var r = AccessTools.Method(typeof(Pawn_NeedsTracker), "ShouldHaveNeed");

		//        //Log.Message("Found method "+ r.ReturnType + ' ' + r.FullDescription());
		//        //Log.Message("Parameters " + r.GetType() );
		//        //Log.Message("return " + r.ReturnParameter);
		//        return AccessTools.Method(typeof(Pawn_NeedsTracker), "ShouldHaveNeed");
		//    }
		//    [HarmonyPrepare]
		//    static bool should_patch(HarmonyInstance instance)
		//    {
		//        Log.Clear();
		//        Log.Message("Will patch "+ xxx.RimWorldChildrenIsActive);
		//        return xxx.RimWorldChildrenIsActive;
		//    }

		//doesn't work, gets:  System.Reflection.TargetParameterCountException: parameters do not match signature
		//static void Postfix(HarmonyInstance instance)
		//{
		//    Log.Message("FOO");
		//}
		//[HarmonyPrefix]
		//static bool foo(NeedDef nd)
		//{
		//    Log.Message("Prerun ");
		//    return true;
		//}
		//[HarmonyPostfix]
		//static void postfix(ref bool __result, ref Pawn_NeedsTracker __instance, NeedDef nd)
		//{
		//    if (nd != NeedDefOf.Joy) { return; }
		//    FieldInfo fieldInfo = AccessTools.Field(typeof(Pawn_NeedsTracker), "pawn");
		//    Pawn pawn = fieldInfo.GetValue(__instance) as Pawn;
		//    if (pawn.ageTracker.CurLifeStageIndex < AgeStage.Teenager)
		//    {
		//        __result = true;
		//    }
		//}
		//}
		public static void Patch(HarmonyInstance harmony)
		{
			if (!xxx.RimWorldChildrenIsActive) return;
			Doit(harmony);
		}
		private static void Doit(HarmonyInstance harmony)
		{
			var original = typeof(RimWorldChildren.ChildrenUtility).GetMethod("CanBreastfeed");
			var postfix = typeof(CnPcompatibility).GetMethod("CanBreastfeed");
			harmony.Patch(original, null, new HarmonyMethod(postfix));

			original = typeof(Building_Bed).GetMethod("get_AssigningCandidates");
			postfix = typeof(CnPcompatibility).GetMethod("BedCandidates");
			harmony.Patch(original, null, new HarmonyMethod(postfix));

			//doesn't work cannot reflect private class
			//original = typeof(RimWorldChildren.Hediff_UnhappyBaby).GetMethod("IsBabyUnhappy", BindingFlags.Static | BindingFlags.NonPublic);
			//Log.Message("original is nul " + (original == null));
			//var prefix = typeof(CnPcompatibility).GetMethod("IsBabyUnhappy");
			//harmony.Patch(original, new HarmonyMethod(prefix), null);
		}
		private static void CanBreastfeed(ref  bool __result, ref Pawn __instance)//Postfix
		{
			__result = __instance.health.hediffSet.HasHediff(HediffDef.Named("Lactating"));//I'm a simple man
		}
		private static void BedCandidates(ref IEnumerable<Pawn> __result, ref Building_Bed bed)
		{
			if (!RimWorldChildren.BedPatchMethods.IsCrib(bed)) return;
			__result = bed.Map.mapPawns.FreeColonists.Where(x => x.ageTracker.CurLifeStageIndex <= 2 && x.Faction == Faction.OfPlayer);//Basically do all the work second time but with a tweak
		}
		private static bool IsBabyUnhappy(ref bool __result, ref RimWorldChildren.Hediff_UnhappyBaby __instance)
		{
			var pawn = __instance.pawn;
			
			if ((pawn.needs?.joy.CurLevelPercentage??1) < 0.2f)
				__result = true;
			else
				__result =  false;
			return false;
		}
	}
}
