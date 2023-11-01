using Verse;
using Verse.AI;
using RimWorld;

namespace rjw
{
	public class ThinkNode_ConditionalCanRapeCP : ThinkNode_Conditional
	{
		protected override bool Satisfied(Pawn p)
		{
			//ModLog.Message("ThinkNode_ConditionalCanRapeCP " + pawn);

			if (!RJWSettings.rape_enabled)
				return false;

			// Hostiles cannot use CP.
			if (p.HostileTo(Faction.OfPlayer))
				return false;

			// Designated pawns are not allowed to rape other CP.
			if (!RJWSettings.designated_freewill)
				if ((p.IsDesignatedComfort() || p.IsDesignatedBreeding()))
					return false;

			// Animals cannot rape CP if the setting is disabled.
			//if (!(RJWSettings.bestiality_enabled && RJWSettings.animal_CP_rape) && xxx.is_animal(p) )
			//	return false;

			// Animals cannot rape CP
			if (xxx.is_animal(p))
				return false;

			// colonists(humans) cannot rape CP if the setting is disabled.
			if (!RJWSettings.colonist_CP_rape && p.IsColonist && xxx.is_human(p))
				return false;

			// Visitors(humans) cannot rape CP if the setting is disabled.
			if (!RJWSettings.visitor_CP_rape && p.Faction?.IsPlayer == false && xxx.is_human(p))
				return false;

			// Visitors(animals/caravan) cannot rape CP if the setting is disabled.
			if (!RJWSettings.visitor_CP_rape && p.Faction?.IsPlayer == false && p.Faction != Faction.OfInsects && xxx.is_animal(p))
				return false;

			// Wild animals, insects cannot rape CP.
			//if ((p.Faction == null || p.Faction == Faction.OfInsects) && xxx.is_animal(p))
			//	return false;

			return true;
		}
	}
}