using RimWorld;
using Verse;
using Verse.AI;
using Multiplayer.API;

namespace rjw
{
	/// <summary>
	/// Allow pawn to have sex
	/// dunno if this should be used to allow manual sex start or limit it behind sort of "hero" designator for RP purposes, so player can only control 1 pawn directly?
	/// </summary>
	public class WorkGiver_Sexchecks : WorkGiver_Scanner
	{
		public override int MaxRegionsToScanBeforeGlobalSearch => 4;
		public override PathEndMode PathEndMode => PathEndMode.OnCell;
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (!forced)
			//if (!(forced || RJWSettings.WildMode))
			{
				//ModLog.Message("WorkGiver_RJW_Sexchecks::not player interaction, exit:" + forced);
				return false;
			}
			var isHero = RJWSettings.RPG_hero_control && pawn.IsDesignatedHero();
			if (!(RJWSettings.override_control || isHero))
			{
				//ModLog.Message("WorkGiver_RJW_Sexchecks::direct_control disabled or not hero, exit");
				return false;
			}
			//!
			if (!isHero)
			{
				if (!RJWSettings.override_control || MP.IsInMultiplayer)
				{
					//ModLog.Message("WorkGiver_RJW_Sexchecks::direct_control disabled or is in MP , exit");
					return false;
				}
			}
			else if (!pawn.IsHeroOwner())
			{
				//ModLog.Message("WorkGiver_RJW_Sexchecks::not hero owner, exit");
				return false;
			}
			Pawn target = t as Pawn;
			if (t is Corpse)
			{
				Corpse corpse = t as Corpse;
				target = corpse.InnerPawn;
				//ModLog.Message("WorkGiver_RJW_Sexchecks::Pawn(" + xxx.get_pawnname(pawn) + "), Target corpse(" + xxx.get_pawnname(target) + ")");
			}
			else
			{
				//ModLog.Message("WorkGiver_RJW_Sexchecks::Pawn(" + xxx.get_pawnname(pawn) + "), Target pawn(" + xxx.get_pawnname(target) + ")");
			}

			//Log.Message("1");
			if (t == null || t.Map == null)
			{
				return false;
			}
			//Log.Message("2");
			if (!(xxx.can_fuck(pawn) || xxx.can_be_fucked(pawn)))
			{
				//ModLog.Message("WorkGiver_RJW_Sexchecks::JobOnThing(" + xxx.get_pawnname(pawn) + ") is cannot fuck or be fucked.");
				return false;
			}
			//Log.Message("3");
			if (t is Pawn)
				if (!(xxx.can_fuck(target) || xxx.can_be_fucked(target)))
				{
					//ModLog.Message("WorkGiver_RJW_Sexchecks::JobOnThing(" + xxx.get_pawnname(target) + ") is cannot fuck or be fucked.");
					return false;
				}
			//Log.Message("4");

			if (!RJWSettings.bestiality_enabled && xxx.is_animal(target))
			{
				//if (RJWSettings.DevMode) JobFailReason.Is("bestiality disabled");
				return false;
			}
			//investigate AoA, someday
			//move this?
			//if (xxx.is_animal(pawn) && xxx.is_animal(target) && !RJWSettings.animal_on_animal_enabled)
			//{
			//	return false;
			//}
			if (!xxx.is_human(pawn) && !(xxx.RoMIsActive && pawn.health.hediffSet.HasHediff(xxx.TM_ShapeshiftHD)))
			{
				return false;
			}

			//Log.Message("5");
			if (!pawn.CanReach(t, PathEndMode, Danger.Some))
			{
				if (RJWSettings.DevMode) JobFailReason.Is(
					pawn.CanReach(t, PathEndMode, Danger.Deadly)
						? "unable to reach target safely" : "target unreachable");
				return false;
			}
			//Log.Message("6");
			if (t.IsForbidden(pawn))
			{
				if (RJWSettings.DevMode) JobFailReason.Is("target is outside of allowed area");
				return false;
			}
			//Log.Message("7");
			if (!pawn.IsDesignatedHero())
			{
				if (!RJWSettings.WildMode)
				{
					if (pawn.IsDesignatedComfort() || pawn.IsDesignatedBreeding())
					{
						if (RJWSettings.DevMode) JobFailReason.Is("designated pawns cannot initiate sex");
						return false;
					}
					if (!xxx.is_healthy_enough(pawn))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("not healthy enough for sex");
						return false;
					}
					if (xxx.is_asexual(pawn))
					{
						if (RJWSettings.DevMode) JobFailReason.Is("refuses to have sex");
						return false;
					}
				}
			}
			else
			{
				if (!pawn.IsHeroOwner())
				{
					//ModLog.Message("WorkGiver_Sexchecks::player interaction for not owned hero, exit");
					return false;
				}
			}

			if (!MoreChecks(pawn, t, forced))
				return false;

			return true;
		}

		public virtual bool MoreChecks(Pawn pawn, Thing t, bool forced = false)
		{
			return false;
		}

		public virtual bool WorkGiverChecks(Pawn pawn, Thing t, bool forced = false)
		{
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return null;
		}
	}
}