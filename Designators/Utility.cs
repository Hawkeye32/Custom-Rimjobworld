using Verse;
using System.Diagnostics;
using RimWorld;

namespace rjw
{
	public static class PawnDesignations_Utility
	{
		public static bool UpdatePermissions(this Pawn pawn)
		{
			pawn.UpdateCanChangeDesignationPrisoner();
			pawn.UpdateCanChangeDesignationColonist();
			pawn.UpdateCanDesignateService();
			pawn.UpdateCanDesignateComfort();
			pawn.UpdateCanDesignateBreedingAnimal();
			pawn.UpdateCanDesignateBreeding();
			pawn.UpdateCanDesignateHero();
			return true;
		}
		public static bool UpdateCanChangeDesignationColonist(this Pawn pawn)
		{
			//check if pawn is a hero of other player
			//if yes - limit widget access in mp for this pawn

			if ((pawn.IsDesignatedHero() && !pawn.IsHeroOwner()))
			{
				//Log.Warning("CanChangeDesignationColonist:: Pawn:" + xxx.get_pawnname(pawn));
				//Log.Warning("CanChangeDesignationColonist:: IsDesignatedHero:" + pawn.IsDesignatedHero());
				//Log.Warning("CanChangeDesignationColonist:: IsHeroOwner:" + pawn.IsHeroOwner());
				return pawn.GetRJWPawnData().CanChangeDesignationColonist = false;
			}

			return pawn.GetRJWPawnData().CanChangeDesignationColonist = true;
		}
		public static bool CanChangeDesignationColonist(this Pawn pawn)
		{
			return pawn.GetRJWPawnData().CanChangeDesignationColonist;
		}
		public static bool UpdateCanChangeDesignationPrisoner(this Pawn pawn)
		{
			//check if player hero is a slave/prisoner
			//if yes - limit widget access in mp for all widgets

			//Stopwatch sw = new Stopwatch();
			//sw.Start();

			//Log.Warning("rjwHero:: Count " + DesignatorsData.rjwHero.Count);
			//Log.Warning("rjwComfort:: Count " + DesignatorsData.rjwComfort.Count);
			//Log.Warning("rjwService:: Count " + DesignatorsData.rjwService.Count);
			//Log.Warning("rjwMilking:: Count " + DesignatorsData.rjwMilking.Count);
			//Log.Warning("rjwBreeding:: Count " + DesignatorsData.rjwBreeding.Count);
			//Log.Warning("rjwBreedingAnimal:: Count " + DesignatorsData.rjwBreedingAnimal.Count);

			foreach (Pawn item in DesignatorsData.rjwHero)
			{
				//Log.Warning("CanChangeDesignationPrisoner:: Pawn:" + xxx.get_pawnname(item));
				//Log.Warning("CanChangeDesignationPrisoner:: IsHeroOwner:" + item.IsHeroOwner());
				if (item.IsHeroOwner())
				{
					if (item.IsPrisonerOfColony || item.IsPrisoner || xxx.is_slave(item))
					{
						//Log.Warning("CanChangeDesignationPrisoner:: Pawn:" + xxx.get_pawnname(item));
						//Log.Warning("CanChangeDesignationPrisoner:: Hero of " + MP.PlayerName);
						//Log.Warning("CanChangeDesignationPrisoner:: is prisoner(colony):" + item.IsPrisonerOfColony);
						//Log.Warning("CanChangeDesignationPrisoner:: is prisoner:" + item.IsPrisoner);
						//Log.Warning("CanChangeDesignationPrisoner:: is slave:" + xxx.is_slave(item));
						return pawn.GetRJWPawnData().CanChangeDesignationPrisoner = false;
					}
				}
			}
			//sw.Stop();
			//Log.Warning("Elapsed={0}" + sw.Elapsed);

			return pawn.GetRJWPawnData().CanChangeDesignationPrisoner = true;
		}
		public static bool CanChangeDesignationPrisoner(this Pawn pawn)
		{
			return pawn.GetRJWPawnData().CanChangeDesignationPrisoner;
		}
	}
}
