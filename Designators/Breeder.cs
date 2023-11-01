using Verse;
using RimWorld;
using Multiplayer.API;

namespace rjw
{
	public static class PawnDesignations_Breeder
	{
		public static bool UpdateCanDesignateBreedingAnimal(this Pawn pawn)
		{
			//no permission to change designation for NON prisoner hero/ other player
			if (!pawn.CanChangeDesignationPrisoner() && !pawn.CanChangeDesignationColonist())
				return pawn.GetRJWPawnData().CanDesignateBreedingAnimal = false;

			//no permission to change designation for prisoner hero/ self
			if (!pawn.CanChangeDesignationPrisoner())
				return pawn.GetRJWPawnData().CanDesignateBreedingAnimal = false;

			//Log.Message("CanDesignateAnimal for " + xxx.get_pawnname(pawn) + " " + SaveStorage.bestiality_enabled);
			//Log.Message("checking animal props " + (pawn.Faction?.IsPlayer.ToString()?? "tynanfag") + xxx.is_animal(pawn) + xxx.can_rape(pawn));
			if ((RJWSettings.bestiality_enabled || RJWSettings.animal_on_animal_enabled)
				&& xxx.is_animal(pawn)
				&& xxx.can_fuck(pawn)
				&& pawn.Faction == Faction.OfPlayer)
				return pawn.GetRJWPawnData().CanDesignateBreedingAnimal = true;

			return pawn.GetRJWPawnData().CanDesignateBreedingAnimal = false;
		}
		public static bool CanDesignateBreedingAnimal(this Pawn pawn)
		{
			return pawn.GetRJWPawnData().CanDesignateBreedingAnimal;
		}
		public static void ToggleBreedingAnimal(this Pawn pawn)
		{
			pawn.UpdateCanDesignateBreedingAnimal();
			if (pawn.CanDesignateBreedingAnimal())
			{
				if (!pawn.IsDesignatedBreedingAnimal())
					DesignateBreedingAnimal(pawn);
				else
					UnDesignateBreedingAnimal(pawn);
			}
		}
		public static bool IsDesignatedBreedingAnimal(this Pawn pawn)
		{
			if (pawn.GetRJWPawnData().BreedingAnimal)
			{
				if (!pawn.Faction?.IsPlayer ?? false)
					UnDesignateBreedingAnimal(pawn);

				if (pawn.Dead)
					pawn.UnDesignateBreedingAnimal();
			}

			return pawn.GetRJWPawnData().BreedingAnimal;
		}
		[SyncMethod]
		public static void DesignateBreedingAnimal(this Pawn pawn)
		{
			DesignatorsData.rjwBreedingAnimal.AddDistinct(pawn);
			pawn.GetRJWPawnData().BreedingAnimal = true;
		}
		[SyncMethod]
		public static void UnDesignateBreedingAnimal(this Pawn pawn)
		{
			DesignatorsData.rjwBreedingAnimal.Remove(pawn);
			pawn.GetRJWPawnData().BreedingAnimal = false;
		}
	}
}
