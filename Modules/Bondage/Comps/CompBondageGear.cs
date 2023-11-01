using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

// Adds options to the right-click menu for bondage gear to equip the gear on prisoners/downed pawns
namespace rjw
{
	public class CompBondageGear : CompUsable
	{
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
		{
			if ((pawn.Map != null) && (pawn.Map == Find.CurrentMap))// && (pawn.Map.mapPawns.PrisonersOfColonyCount > 0)
			{
				if (!pawn.CanReserve(parent))
					yield return new FloatMenuOption(FloatMenuOptionLabel(pawn) + " on (" + "Reserved".Translate() + ")", null, MenuOptionPriority.DisabledOption);
				else if (pawn.CanReach(parent, PathEndMode.Touch, Danger.Some))
					foreach (Pawn other in pawn.Map.mapPawns.AllPawns)
						if ((other != pawn) && other.Spawned && (other.Downed || other.IsPrisonerOfColony || xxx.is_slave(other)))
							yield return this.make_option(FloatMenuOptionLabel(pawn) + " on " + xxx.get_pawnname(other), pawn, other, (other.IsPrisonerOfColony || xxx.is_slave(other)) ? WorkTypeDefOf.Warden : null);
			}
		}
	}
}