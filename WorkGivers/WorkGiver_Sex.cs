using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Assigns a pawn to have sex with
	/// </summary>
	public class WorkGiver_Sex : WorkGiver_Sexchecks
	{
		public override bool MoreChecks(Pawn pawn, Thing t, bool forced = false)
		{
			//ModLog.Message("" + this.GetType().ToString() + " base checks: pass");

			Pawn target = t as Pawn;
			if (target == pawn)
			{
				//JobFailReason.Is("no self sex", null);
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
					//check initiator
					//fail for:
					//satisfied non nymph pawns
					if (xxx.need_some_sex(pawn) <= 1f && !xxx.is_nympho(pawn))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("not horny enough");
						return false;
					}
					if (!xxx.IsTargetPawnOkay(target))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("target not healthy enough");
						return false;
					}
					if (!xxx.is_lecher(pawn) && !xxx.is_psychopath(pawn) && !xxx.is_nympho(pawn))
						if (!xxx.isSingleOrPartnerNotHere(pawn))
							if (!LovePartnerRelationUtility.LovePartnerRelationExists(pawn, target))
							{
								if (RJWSettings.DevMode) JobFailReason.Is("cannot have sex while partner around");
								return false;
							}

					float relations = 0;
					float attraction = 0;
					if (!xxx.is_animal(target))
					{
						relations = pawn.relations.OpinionOf(target);
						if (relations < RJWHookupSettings.MinimumRelationshipToHookup)
						{
							if (!(relations > 0 && xxx.is_nympho(pawn)))
							{
								if (RJWSettings.DevMode) JobFailReason.Is($"i don't like them:({relations})");
								return false;
							}
						}

						attraction = pawn.relations.SecondaryRomanceChanceFactor(target);
						if (attraction < RJWHookupSettings.MinimumAttractivenessToHookup)
						{
							if (!(attraction > 0 && xxx.is_nympho(pawn)))
							{
								if (RJWSettings.DevMode) JobFailReason.Is($"i don't find them attractive:({attraction})");
								return false;
							}
						}
					}
					//ModLog.Message("WorkGiver_Sex::" + SexAppraiser.would_fuck(pawn, target));
					if (SexAppraiser.would_fuck(pawn, target) < 0.1f)
					{
						return false;
					}

					if (!xxx.is_animal(target))
					{
						//check partner
						if (xxx.need_some_sex(target) <= 1f && !xxx.is_nympho(target))
						{
							if (RJWSettings.DevMode) JobFailReason.Is("partner not horny enough");
							return false;
						}
						if (!xxx.is_lecher(target) && !xxx.is_psychopath(target) && !xxx.is_nympho(target))
							if (!xxx.isSingleOrPartnerNotHere(target))
								if (!LovePartnerRelationUtility.LovePartnerRelationExists(pawn, target))
								{
									if (RJWSettings.DevMode) JobFailReason.Is("partner cannot have sex while their partner around");
									return false;
								}

						relations = target.relations.OpinionOf(pawn);
						if (relations <= RJWHookupSettings.MinimumRelationshipToHookup)
						{
							if (!(relations > 0 && xxx.is_nympho(target)))
							{
								if (RJWSettings.DevMode) JobFailReason.Is($"don't like me:({relations})");
								return false;
							}
						}

						attraction = target.relations.SecondaryRomanceChanceFactor(pawn);
						if (attraction <= RJWHookupSettings.MinimumAttractivenessToHookup)
						{
							if (!(attraction > 0 && xxx.is_nympho(target)))
							{
								if (RJWSettings.DevMode) JobFailReason.Is($"doesn't find me attractive:({attraction})");
								return false;
							}
						}
					}

					//ModLog.Message("WorkGiver_Sex::" + SexAppraiser.would_fuck(target, pawn));
					if (SexAppraiser.would_fuck(target, pawn) < 0.1f)
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
			//TODO:: fix bed stealing during join other pawn
			//Building_Bed bed = pawn.ownership.OwnedBed;
			//if (bed == null)
			//	bed = (t as Pawn).ownership.OwnedBed;
			
			Building_Bed bed = (t as Pawn).CurrentBed();
			
			if (bed == null)
				return null;
			
			//if (pawn.CurrentBed() != (t as Pawn).CurrentBed())
			//	return null;
			
			return JobMaker.MakeJob(xxx.casual_sex, t as Pawn, bed);
		}
	}
}