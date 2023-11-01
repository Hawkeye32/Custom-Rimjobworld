using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Assigns a pawn to rape
	/// </summary>
	public class WorkGiver_Rape : WorkGiver_Sexchecks
	{
		public override bool MoreChecks(Pawn pawn, Thing t, bool forced = false)
		{
			if (RJWSettings.DebugRape) ModLog.Message("" + this.GetType().ToString() + " base checks: pass");
			if (!RJWSettings.rape_enabled) return false;

			Pawn target = t as Pawn;
			if (target == pawn)
			{
				//JobFailReason.Is("no self rape", null);
				return false;
			}
			if (!WorkGiverChecks(pawn, t, forced))
				return false;

			if (!xxx.is_human(target))
			{
				return false;
			}
			if (!pawn.CanReserve(target, xxx.max_rapists_per_prisoner, 0))
				return false;

			if (!(pawn.IsDesignatedHero() || RJWSettings.override_control))
				if (!RJWSettings.WildMode)
				{
					if (xxx.is_kind(pawn) || xxx.is_masochist(pawn))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("refuses to rape");
						return false;
					}
					if (pawn.relations.OpinionOf(target) > 50 && !xxx.is_rapist(pawn) && !xxx.is_psychopath(pawn) && !xxx.is_masochist(target))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("refuses to rape a friend");
						return false;
					}
					if (!xxx.can_get_raped(target))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("cannot rape target");
						return false;
					}
					//fail for:
					//satisfied pawns
					//horny non rapists
					if ((xxx.need_some_sex(pawn) <= 1f)
						|| (xxx.need_some_sex(pawn) <= 2f && !(xxx.is_rapist(pawn) || xxx.is_psychopath(pawn) || xxx.is_nympho(pawn))))
						{
						if (RJWSettings.DevMode) JobFailReason.Is("not horny enough");
							return false;
						}
					if (!xxx.can_rape(pawn))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("cannot rape");
						return false;
					}
					if (!xxx.is_healthy_enough(target)
						|| !xxx.is_not_dying(target) && (xxx.is_bloodlust(pawn) || xxx.is_psychopath(pawn) || xxx.is_rapist(pawn) || xxx.has_quirk(pawn, "Somnophile")))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("target not healthy enough");
						return false;
					}
					if (!xxx.is_lecher(pawn) && !xxx.is_psychopath(pawn) && !xxx.is_nympho(pawn))
						if (!xxx.isSingleOrPartnerNotHere(pawn))
							if (!LovePartnerRelationUtility.LovePartnerRelationExists(pawn, target))
							{
								if (RJWSettings.DevMode) JobFailReason.Is("cannot rape while partner around");
								return false;
							}
					//ModLog.Message("WorkGiver_RapeCP::" + SexAppraiser.would_fuck(pawn, target));
					if (SexAppraiser.would_fuck(pawn, target) < 0.1f)
					{
						return false;
					}
				}

			//ModLog.Message("" + this.GetType().ToString() + " extended checks: can start sex");
			return true;
		}

		public override bool WorkGiverChecks(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn target = t as Pawn;
			if (pawn.HostileTo(target) || target.IsDesignatedComfort())
			{
				return false;
			}
			return true;
		}
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(xxx.RapeRandom, t);
		}
	}
}