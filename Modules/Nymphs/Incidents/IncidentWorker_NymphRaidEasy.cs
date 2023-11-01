using Multiplayer.API;
using RimWorld;
using rjw.Modules.Shared.Logs;
using UnityEngine;
using Verse;

namespace rjw.Modules.Nymphs.Incidents
{
	public class IncidentWorker_NymphRaidEasy : IncidentWorker_BaseNymphRaid
	{
		private static ILog _log = LogManager.GetLogger<IncidentWorker_NymphRaidEasy, NymphLogProvider>();

		[SyncMethod]
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			//The event is disabled, don't fire !
			if (RJWSettings.NymphRaidEasy == false)
			{
				_log.Debug("The incident can't fire as it is disabled in RJW settings");
				return false;
			}

			return base.CanFireNowSub(parms);
		}
		protected override int GetNymphCount(IncidentParms parms, Map map)
		{
			int count;

			//Calculating nymphs manyness
			if (RJWSettings.NymphRaidRP)
			{
				count = base.GetNymphCount(parms, map);
			}
			else
			{
				count = (Find.World.worldPawns.AllPawnsAlive.Count + map.mapPawns.FreeColonistsAndPrisonersSpawnedCount);
				//Cap the max
				count = Mathf.Min(count, 100);
				//Cap the min
				count = Mathf.Max(count, 1);
			}

			return count;
		}
	}
}
