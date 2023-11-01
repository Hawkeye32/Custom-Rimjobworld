using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Assigns a pawn to rape a comfort prisoner
	/// </summary>
	public class WorkGiver_BestialityM : WorkGiver_RJW_Sexchecks
	{
		public override bool MoreChecks(Pawn pawn, Thing t, bool forced = false)
		{
			if (!RJWSettings.rape_enabled) return false;

			Pawn target = t as Pawn;
			if (!RJWSettings.WildMode)
			{
				if (xxx.is_kind(pawn))
				{
					JobFailReason.Is("refuses to rape");
					return false;
				}
				//satisfied pawns
				//horny non rapists
				if ((xxx.need_some_sex(pawn) <= 1f)
					|| (xxx.need_some_sex(pawn) <= 2f && !(xxx.is_rapist(pawn) || xxx.is_psychopath(pawn) || xxx.is_nympho(pawn))))
					{
						JobFailReason.Is("not horny enough");
						return false;
					}
				if (!target.IsDesignatedComfort())
				{
					//JobFailReason.Is("not designated as CP", null);
					return false;
				}
				if (!xxx.is_healthy_enough(target)
					|| !xxx.is_not_dying(target) && (xxx.is_bloodlust(pawn) || xxx.is_psychopath(pawn) || xxx.is_rapist(pawn) || xxx.has_quirk(pawn, "Somnophile")))
				{
					//--Log.Message("[RJW]WorkGiver_RapeCP::HasJobOnThing called0 - target isn't healthy enough or is in a forbidden position.");
					JobFailReason.Is("target not healthy enough");
					return false;
				}
				if (pawn.relations.OpinionOf(target) > 50 && !xxx.is_rapist(pawn) && !xxx.is_psychopath(pawn) && !xxx.is_masochist(target))
				{
					JobFailReason.Is("refuses to rape a friend");
					return false;
				}
				if (!xxx.can_rape(pawn))
				{
					//--Log.Message("[RJW]WorkGiver_RapeCP::HasJobOnThing called1 - pawn don't need sex or is not healthy, or cannot rape");
					JobFailReason.Is("cannot rape target (vulnerability too low, or age mismatch)");
					return false;
				}
				if (!xxx.isSingleOrPartnerNotHere(pawn) && !xxx.is_lecher(pawn) && !xxx.is_psychopath(pawn) && !xxx.is_nympho(pawn))
					if (!LovePartnerRelationUtility.LovePartnerRelationExists(pawn, target))
					{
						//--Log.Message("[RJW]WorkGiver_RapeCP::HasJobOnThing called2 - pawn is not single or has partner around");
						JobFailReason.Is("cannot rape while in stable relationship");
						return false;
					}
			}

			if (!pawn.CanReserve(target, xxx.max_rapists_per_prisoner, 0))
				return false;

			//Log.Message("[RJW]" + this.GetType().ToString() + " extended checks: can start sex");
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			//--Log.Message("[RJW]WorkGiver_RapeCP::JobOnThing(" + xxx.get_pawnname(pawn) + "," + t.ToStringSafe() + ") is called.");
			return new Job(xxx.comfort_prisoner_rapin, t);
		}
	}
}