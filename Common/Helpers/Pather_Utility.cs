using Verse;
using Verse.AI;

namespace rjw
{
	public static class Pather_Utility
	{
		public static bool cells_to_target_casual(Pawn pawn, IntVec3 Position)
		{
			//less taxing, ignores walls
			return pawn.Position.DistanceTo(Position) < RJWSettings.maxDistanceCellsCasual;
		}

		public static bool cells_to_target_rape(Pawn pawn, IntVec3 Position)
		{
			//less taxing, ignores walls
			return pawn.Position.DistanceTo(Position) < RJWSettings.maxDistanceCellsRape;
		}

		public static bool can_path_to_target(Pawn pawn, IntVec3 Position)
		{
			//more taxing, using real pathing
			bool canit = true;
			if (RJWSettings.maxDistancePathCost > 0)
			{
				PawnPath pawnPath = pawn.Map.pathFinder.FindPath(pawn.Position, Position, pawn);
				if (pawnPath.TotalCost > RJWSettings.maxDistancePathCost)
					canit = false;// too far
				pawnPath.Dispose();
			}
			return canit;
		}
	}
}