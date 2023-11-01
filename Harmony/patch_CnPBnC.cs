using System.Reflection;
using Harmony;
using RimWorld;
using Verse;
using System;

namespace rjw
{
	/// <summary>
	/// Conditional patching class that only does patching if CnP is active
	/// </summary>
	public class patch_CnPBnC
	{
		private readonly static Type Hediff_Baby = AccessTools.TypeByName("Hediff_Baby");
		Notify_TraitChanged()
			//CnP (or BnC) try to address the joy need of a child, prisoners don't have that. 
								//Considering how you can arrest children without rjw, it is more of a bug of that mod than ours.
		[HarmonyPatch(typeof(HediffGiver_Birthday), "GrowUpTo")]
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
	}
}
