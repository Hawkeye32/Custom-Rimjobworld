using HarmonyLib;
using RimWorld;
using System;
using Verse;


namespace rjw
{
	///<summary>
	///unset designators on GuestRelease
	///until pawn leaves map, pawn is still guest/slave/prisoner, so they can actually be changed back
	///</summary>
	[HarmonyPatch(typeof(GenGuest), "GuestRelease")]
	[StaticConstructorOnStartup]
	static class PATCH_GenGuest_GuestRelease
	{
		[HarmonyPrefix]
		private static void GuestRelease_Update_Designators(Pawn p)
		{
			if (p != null)
			{
				//ModLog.Message("GenGuest GuestRelease");
				//ModLog.Message("pawn: " + xxx.get_pawnname(p));
				p.UnDesignateComfort();
				p.UnDesignateService();
				p.UnDesignateBreeding();
				p.UnDesignateBreedingAnimal();
				p.UnDesignateMilking();
				p.UnDesignateHero();
				//ModLog.Message(p.IsDesignatedComfort().ToString());
			}
		}
	}

	///<summary>
	///unset designators on Pawn_ExitMap
	///now pawn actually leaves map and get their factions reset
	///</summary>
	[HarmonyPatch(typeof(Pawn), "ExitMap")]
	[StaticConstructorOnStartup]
	static class PATCH_Pawn_ExitMap
	{
		[HarmonyPrefix]
		private static void Pawn_ExitMap_Update_Designators(Pawn __instance, bool allowedToJoinOrCreateCaravan, Rot4 exitDir)
		{
			Pawn p = __instance;
			if (p != null)
				if (!p.IsColonist)
				{
					//ModLog.Message("Pawn ExitMap");
					//ModLog.Message("pawn: " + xxx.get_pawnname(p));
					p.UnDesignateComfort();
					p.UnDesignateService();
					p.UnDesignateBreeding();
					p.UnDesignateBreedingAnimal();
					p.UnDesignateMilking();
					p.UnDesignateHero();
					//ModLog.Message(p.IsDesignatedComfort().ToString());
				}
		}
	}
}