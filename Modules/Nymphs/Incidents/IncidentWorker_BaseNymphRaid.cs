using Multiplayer.API;
using RimWorld;
using rjw.Modules.Nymphs.Implementation;
using rjw.Modules.Shared.Logs;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace rjw.Modules.Nymphs.Incidents
{
	public abstract class IncidentWorker_BaseNymphRaid : IncidentWorker_NeutralGroup
	{
		private static ILog _log = LogManager.GetLogger<IncidentWorker_BaseNymphRaid, NymphLogProvider>();

		protected static readonly INymphService _nymphGeneratorService;

		static IncidentWorker_BaseNymphRaid()
		{
			_nymphGeneratorService = NymphService.Instance;
		}

		protected virtual float ThreatPointMultiplier => 1f;

		[SyncMethod]
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			//No multiplayer support ... I guess
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

		protected virtual int GetNymphCount(IncidentParms parms, Map map)
		{
			//Calculating nymphs manyness
			int count;
			count = Mathf.RoundToInt(parms.points / parms.pawnKind.combatPower * ThreatPointMultiplier);
			//Cap the min
			count = Mathf.Max(count, 1);

			return count;
		}

		[SyncMethod]
		protected override void ResolveParmsPoints(IncidentParms parms)
		{
			if (parms.points <= 0)
			{
				parms.points = StorytellerUtility.DefaultThreatPointsNow(parms.target) * ThreatPointMultiplier;
			}
			_log.Debug($"Incident generated with {parms.points} points");
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

			//Find PawnKind
			parms.pawnKind = Nymph_Generator.GetFixedNymphPawnKindDef();

			if(parms.pawnKind == null)
			{
				_log.Debug($"Incident failed to fire, no Pawn Kind for nymphs");
				return false;
			}

			//Calculating nymphs manyness
			int count = GetNymphCount(parms, (Map)parms.target);

			_log.Debug($"Will generate {count} nymphs");

			List<Pawn> nymphs = GenerateNymphs(parms.target as Map, count);

			parms.raidArrivalMode.Worker.Arrive(nymphs, parms);

			SetManhunters(nymphs);

			Find.LetterStack.ReceiveLetter(
				"RJW_nymph_incident_raid_title".Translate(), 
				"RJW_nymph_incident_raid_description".Translate(),
				LetterDefOf.ThreatBig,
				nymphs);

			return true;
		}


		protected List<Pawn> GenerateNymphs(Map map, int count)
		{
			List<Pawn> result = new List<Pawn>();

			PawnKindDef nymphKind = _nymphGeneratorService.RandomNymphKind();

			for (int i = 1; i <= count; ++i)
			{
				Pawn nymph = _nymphGeneratorService.GenerateNymph(map, nymphKind);

				//Set it wild
				nymph.ChangeKind(PawnKindDefOf.WildMan);

				result.Add(nymph);
			}

			return result;
		}

		protected void SetManhunters(List<Pawn> nymphs)
		{
			foreach (var nymph in nymphs)
			{
				_nymphGeneratorService.SetManhunter(nymph);
			}
		}
	}
}
