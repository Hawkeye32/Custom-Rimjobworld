using Verse;
using Multiplayer.API;

namespace rjw
{
	public static class PawnDesignations_Milking
	{
		public static bool UpdateCanDesignateMilking(this Pawn pawn)
		{
			return pawn.GetRJWPawnData().CanDesignateMilking = false;
		}
		public static bool CanDesignateMilking(this Pawn pawn)
		{
			return pawn.GetRJWPawnData().CanDesignateMilking = false;
		}
		public static void ToggleMilking(this Pawn pawn)
		{
			if (pawn.CanDesignateMilking())
			{
				if (!pawn.IsDesignatedMilking())
					DesignateMilking(pawn);
				else
					UnDesignateMilking(pawn);
			}
		}
		public static bool IsDesignatedMilking(this Pawn pawn)
		{
			if (pawn.GetRJWPawnData().Milking)
			{
				if (!pawn.IsDesignatedHero())
					if (!(pawn.IsColonist || pawn.IsPrisonerOfColony || xxx.is_slave(pawn)))
						UnDesignateMilking(pawn);

				if (pawn.Dead)
					pawn.UnDesignateMilking();
			}

			return pawn.GetRJWPawnData().Milking;
		}
		[SyncMethod]
		public static void DesignateMilking(this Pawn pawn)
		{
			DesignatorsData.rjwMilking.AddDistinct(pawn);
			pawn.GetRJWPawnData().Milking = true;
		}
		[SyncMethod]
		public static void UnDesignateMilking(this Pawn pawn)
		{
			DesignatorsData.rjwMilking.Remove(pawn);
			pawn.GetRJWPawnData().Milking = false;
		}
	}
}
