using Verse;
using Multiplayer.API;

namespace rjw
{
	public static class PawnDesignations_Service
	{
		public static bool UpdateCanDesignateService(this Pawn pawn)
		{
			//no permission to change designation for NON prisoner hero/ other player
			if (!pawn.CanChangeDesignationPrisoner() && !pawn.CanChangeDesignationColonist())
				return pawn.GetRJWPawnData().CanDesignateService = false;

			//no permission to change designation for prisoner hero/ self
			if (!pawn.CanChangeDesignationPrisoner())
				return pawn.GetRJWPawnData().CanDesignateService = false;

			//cant sex
			if (!(xxx.can_fuck(pawn) || xxx.can_be_fucked(pawn)))
				return pawn.GetRJWPawnData().CanDesignateService = false;

			if (!pawn.IsDesignatedHero())
			{
				if (pawn.IsColonist)
					return pawn.GetRJWPawnData().CanDesignateService = true;
			}
			else if (pawn.IsHeroOwner())
				return pawn.GetRJWPawnData().CanDesignateService = true;

			if (pawn.IsPrisonerOfColony || xxx.is_slave(pawn))
				return pawn.GetRJWPawnData().CanDesignateService = true;

			return pawn.GetRJWPawnData().CanDesignateService = false;
		}
		public static bool CanDesignateService(this Pawn pawn)
		{
			return SaveStorage.DataStore.GetPawnData(pawn).CanDesignateService;
		}
		public static void ToggleService(this Pawn pawn)
		{
			pawn.UpdateCanDesignateService();
			if (pawn.CanDesignateService())
			{
				if (!pawn.IsDesignatedService())
					DesignateService(pawn);
				else
					UnDesignateService(pawn);
			}
		}
		public static bool IsDesignatedService(this Pawn pawn)
		{
			if (SaveStorage.DataStore.GetPawnData(pawn).Service)
			{
				if (!pawn.IsDesignatedHero())
					if (!(pawn.IsColonist || pawn.IsPrisonerOfColony || xxx.is_slave(pawn)))
						UnDesignateService(pawn);

				if (pawn.Dead)
					pawn.UnDesignateService();
			}

			return SaveStorage.DataStore.GetPawnData(pawn).Service;
		}
		[SyncMethod]
		public static void DesignateService(this Pawn pawn)
		{
			DesignatorsData.rjwService.AddDistinct(pawn);
			SaveStorage.DataStore.GetPawnData(pawn).Service = true;
		}
		[SyncMethod]
		public static void UnDesignateService(this Pawn pawn)
		{
			DesignatorsData.rjwService.Remove(pawn);
			SaveStorage.DataStore.GetPawnData(pawn).Service = false;
		}
	}
}
