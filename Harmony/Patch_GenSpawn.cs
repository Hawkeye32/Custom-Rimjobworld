using HarmonyLib;
using Verse;
using System;
using RimWorld;

/// <summary>
/// patches GenSpawn to:
/// check if spawned thing is pawn, and add sexualize it if parts missing
/// </summary>
namespace rjw
{
	[HarmonyPatch(typeof(GenSpawn), "Spawn", new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool) })]
	static class Patch_GenSpawn_Spawn
	{
		[HarmonyPostfix]

		static void Sexualize_GenSpawn_Spawn(ref Thing __result)
		{
			if (__result != null)
				if (__result is Pawn)
				{
					//ModLog.Message("Sexualize_GenSpawn_Spawn:: " + xxx.get_pawnname(__result));
					Pawn pawn = __result as Pawn;
					if (pawn.kindDef.race.defName.Contains("AIRobot") // No genitalia/sexuality for roombas.
						|| pawn.kindDef.race.defName.Contains("AIPawn") // ...nor MAI.
						|| pawn.kindDef.race.defName.Contains("RPP_Bot")
						|| pawn.kindDef.race.defName.Contains("PRFDrone") // Project RimFactory Revived drones
						) return;

					if (!Genital_Helper.has_genitals(pawn))
					{
						Sexualizer.sexualize_pawn(pawn);
					}
				}
		}
	}
}
