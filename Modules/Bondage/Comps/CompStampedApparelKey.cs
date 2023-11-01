using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

// Adds unlock options to right-click menu for holokeys.
namespace rjw
{
	public class CompStampedApparelKey : CompUsable
	{
		protected string make_label(Pawn pawn, Pawn other)
		{
			return FloatMenuOptionLabel(pawn) + " on " + ((other == null) ? "self" : xxx.get_pawnname(other));
		}

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
		{
			if (!pawn.CanReserve(parent))
				yield return new FloatMenuOption(FloatMenuOptionLabel(pawn) + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.DisabledOption);
			else if (pawn.CanReach(parent, PathEndMode.Touch, Danger.Some))
			{
				// Option for the pawn to use the key on themself
				if (!pawn.is_wearing_locked_apparel())
					yield return new FloatMenuOption("Not wearing locked apparel", null, MenuOptionPriority.DisabledOption);
				else
					yield return this.make_option(make_label(pawn, null), pawn, null, null);

				if ((pawn.Map != null) && (pawn.Map == Find.CurrentMap))
				{
					// Options for use on colonists
					foreach (var other in pawn.Map.mapPawns.FreeColonists)
						if ((other != pawn) && other.is_wearing_locked_apparel())
							yield return this.make_option(make_label(pawn, other), pawn, other, null);

					// Options for use on prisoners
					foreach (var prisoner in pawn.Map.mapPawns.PrisonersOfColony)
						if (prisoner.is_wearing_locked_apparel())
							yield return this.make_option(make_label(pawn, prisoner), pawn, prisoner, WorkTypeDefOf.Warden);

					// Options for use on corpses
					foreach (var q in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse))
					{
						var corpse = q as Corpse;
						if (corpse.InnerPawn.is_wearing_locked_apparel())
							yield return this.make_option(make_label(pawn, corpse.InnerPawn), pawn, corpse, null);
					}
				}
			}
		}
	}
}