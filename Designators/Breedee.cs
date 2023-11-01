using Verse;
using RimWorld;
using Multiplayer.API;

namespace rjw
{
	public static class PawnDesignations_Breedee
	{
		public static bool UpdateCanDesignateBreeding(this Pawn pawn)
		{
			//no permission to change designation for NON prisoner hero/ other player
			if (!pawn.CanChangeDesignationPrisoner() && !pawn.CanChangeDesignationColonist())
				return pawn.GetRJWPawnData().CanDesignateBreeding = false;

			//no permission to change designation for prisoner hero/ self
			if (!pawn.CanChangeDesignationPrisoner())
				return pawn.GetRJWPawnData().CanDesignateBreeding = false;

			//cant have penetrative sex
			if (!xxx.can_be_fucked(pawn))
				return pawn.GetRJWPawnData().CanDesignateBreeding = false;


			if (RJWSettings.bestiality_enabled && xxx.is_human(pawn))
			{
				if (!pawn.IsDesignatedHero())
				{
					if ((xxx.is_zoophile(pawn) || (RJWSettings.override_RJW_designation_checks && !MP.IsInMultiplayer)) && pawn.IsColonist)
						return pawn.GetRJWPawnData().CanDesignateBreeding = true;
				}
				else if (pawn.IsHeroOwner())
					return pawn.GetRJWPawnData().CanDesignateBreeding = true;

				if (pawn.IsPrisonerOfColony || xxx.is_slave(pawn))
					return pawn.GetRJWPawnData().CanDesignateBreeding = true;
			}

			if (RJWSettings.animal_on_animal_enabled && xxx.is_animal(pawn)
				&& pawn.Faction == Faction.OfPlayer)
				return pawn.GetRJWPawnData().CanDesignateBreeding = true;

			return pawn.GetRJWPawnData().CanDesignateBreeding = false;
		}
		public static bool CanDesignateBreeding(this Pawn pawn)
		{
			return pawn.GetRJWPawnData().CanDesignateBreeding;
		}
		public static void ToggleBreeding(this Pawn pawn)
		{
			pawn.UpdateCanDesignateBreeding();
			if (pawn.CanDesignateBreeding())
			{
				if (!pawn.IsDesignatedBreeding())
					DesignateBreeding(pawn);
				else
					UnDesignateBreeding(pawn);
			}
		}
		public static bool IsDesignatedBreeding(this Pawn pawn)
		{
			if (pawn.GetRJWPawnData().Breeding)
			{
				if (!xxx.is_animal(pawn))
				{
					if (!RJWSettings.bestiality_enabled)
						UnDesignateBreeding(pawn);

					else if (!pawn.IsDesignatedHero())
						if (!(xxx.is_zoophile(pawn) || pawn.IsPrisonerOfColony || xxx.is_slave(pawn)))
							if (!(RJWSettings.WildMode || (RJWSettings.override_RJW_designation_checks && !MP.IsInMultiplayer)))
								UnDesignateBreeding(pawn);
				}
				else
				{
					if (!RJWSettings.animal_on_animal_enabled)
						UnDesignateBreeding(pawn);

					else if (!pawn.Faction?.IsPlayer ?? false)
						UnDesignateBreeding(pawn);
				}
				if (pawn.Dead)
					pawn.UnDesignateBreeding();
			}

			return pawn.GetRJWPawnData().Breeding;
		}
		[SyncMethod]
		public static void DesignateBreeding(this Pawn pawn)
		{
			DesignatorsData.rjwBreeding.AddDistinct(pawn);
			pawn.GetRJWPawnData().Breeding = true;
		}
		[SyncMethod]
		public static void UnDesignateBreeding(this Pawn pawn)
		{
			DesignatorsData.rjwBreeding.Remove(pawn);
			pawn.GetRJWPawnData().Breeding = false;
		}
	}
}
