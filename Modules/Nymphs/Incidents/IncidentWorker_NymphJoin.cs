using Multiplayer.API;
using RimWorld;
using rjw.Modules.Nymphs.Implementation;
using rjw.Modules.Shared.Extensions;
using rjw.Modules.Shared.Logs;
using System.Collections.Generic;
using Verse;

namespace rjw.Modules.Nymphs.Incidents
{
	public class IncidentWorker_NymphJoin : IncidentWorker
	{
		private static ILog _log = LogManager.GetLogger<IncidentWorker_NymphJoin, NymphLogProvider>();

		protected static readonly INymphService _nymphGeneratorService;

		static IncidentWorker_NymphJoin()
		{
			_nymphGeneratorService = NymphService.Instance;
		}

		[SyncMethod]
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			//The event is disabled, don't fire !
			if (RJWSettings.NymphTamed == false)
			{
				_log.Debug("The incident can't fire as it is disabled in RJW settings");
				return false;
			}

			//No multiplayer support
			if (MP.IsInMultiplayer)
			{
				_log.Debug("Could not fire because multiplayer api is on");
				return false;
			}

			//No map
			if (parms.target is Map == false)
			{
				_log.Debug("Could not fire because the incident target is not a map");
				return false;
			}

			return base.CanFireNowSub(parms);
		}

		[SyncMethod]
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			_log.Debug($"Generating incident");

			//Walk from the edge
			parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
			if (parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms) == false)
			{
				_log.Debug($"Incident failed to fire, no spawn points available");
				return false;
			}

			Pawn nymph = GenerateNymph(parms.target as Map);

			_log.Debug($"Generated nymph {nymph.GetName()}");

			parms.raidArrivalMode.Worker.Arrive(new List<Pawn>() { nymph }, parms);

			JoinPlayerFaction(nymph);

			Find.LetterStack.ReceiveLetter(
				"RJW_nymph_incident_join_title".Translate(), 
				"RJW_nymph_incident_join_description".Translate(),
				LetterDefOf.PositiveEvent,
				nymph);

			return true;
		}

		protected Pawn GenerateNymph(Map map)
		{
			Pawn nymph = _nymphGeneratorService.GenerateNymph(map);

			nymph.ChangeKind(PawnKindDefOf.WildMan);

			return nymph;
		}

		protected void JoinPlayerFaction(Pawn nymph)
		{
			nymph.SetFaction(Faction.OfPlayer);
		}
	}
}
