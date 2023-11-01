using Verse;
using Multiplayer.API;

namespace rjw
{
	public static class PawnDesignations_Comfort
	{
		public static bool UpdateCanDesignateComfort(this Pawn pawn)
		{
			//rape disabled
			if (!RJWSettings.rape_enabled)
				return pawn.GetRJWPawnData().CanDesignateComfort = false;

			//no permission to change designation for NON prisoner hero/ other player
			if (!pawn.CanChangeDesignationPrisoner() && !pawn.CanChangeDesignationColonist())
				return pawn.GetRJWPawnData().CanDesignateComfort = false;

			//no permission to change designation for prisoner hero/ self
			if (!pawn.CanChangeDesignationPrisoner())
				return pawn.GetRJWPawnData().CanDesignateComfort = false;

			//cant sex
			if (!(xxx.can_fuck(pawn) || xxx.can_be_fucked(pawn)))
				return pawn.GetRJWPawnData().CanDesignateComfort = false;

			if (!pawn.IsDesignatedHero())
			{
				if ((xxx.is_masochist(pawn) || (RJWSettings.override_RJW_designation_checks && !MP.IsInMultiplayer)) && pawn.IsColonist)
					return pawn.GetRJWPawnData().CanDesignateComfort = true;
			}
			else if (pawn.IsHeroOwner())
				return pawn.GetRJWPawnData().CanDesignateComfort = true;

			if (pawn.IsPrisonerOfColony || xxx.is_slave(pawn))
				return pawn.GetRJWPawnData().CanDesignateComfort = true;

			return pawn.GetRJWPawnData().CanDesignateComfort = false;
		}
		public static bool CanDesignateComfort(this Pawn pawn)
		{
			return pawn.GetRJWPawnData().CanDesignateComfort;
		}
		public static void ToggleComfort(this Pawn pawn)
		{
			pawn.UpdateCanDesignateComfort();
			if (pawn.CanDesignateComfort())
			{
				if (!pawn.IsDesignatedComfort())
					DesignateComfort(pawn);
				else
					UnDesignateComfort(pawn);
			}
		}
		public static bool IsDesignatedComfort(this Pawn pawn)
		{
			if (pawn.GetRJWPawnData().Comfort)
			{
				if (!pawn.IsDesignatedHero())
				{
					if (!pawn.IsPrisonerOfColony)
						if (!(xxx.is_masochist(pawn) || xxx.is_slave(pawn)))
						{
							if (!pawn.IsColonist)
								UnDesignateComfort(pawn);
							else if (!(RJWSettings.WildMode || (RJWSettings.override_RJW_designation_checks && !MP.IsInMultiplayer)))
								UnDesignateComfort(pawn);
						}
				}

				if (pawn.Dead)
					pawn.UnDesignateComfort();
			}

			return pawn.GetRJWPawnData().Comfort;
		}
		[SyncMethod]
		public static void DesignateComfort(this Pawn pawn)
		{
			DesignatorsData.rjwComfort.AddDistinct(pawn);
			pawn.GetRJWPawnData().Comfort = true;
		}
		[SyncMethod]
		public static void UnDesignateComfort(this Pawn pawn)
		{
			DesignatorsData.rjwComfort.Remove(pawn);
			pawn.GetRJWPawnData().Comfort = false;
		}
	}
}
