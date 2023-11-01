using Multiplayer.API;
using RimWorld;
using rjw.Modules.Shared.Logs;
using UnityEngine;
using Verse;

namespace rjw.Modules.Nymphs.Incidents
{
	public class IncidentWorker_NymphRaidHard : IncidentWorker_BaseNymphRaid
	{
		private static ILog _log = LogManager.GetLogger<IncidentWorker_NymphRaidHard, NymphLogProvider>();

		protected override float ThreatPointMultiplier => 1.5f;

		[SyncMethod]
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			//The event is disabled, don't fire !
			if (RJWSettings.NymphRaidHard == false)
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
				count = map.mapPawns.AllPawnsSpawnedCount;
				//Cap the max
				count = Mathf.Min(count, 1000);
				//Cap the min
				count = Mathf.Max(count, 1);
			}

			return count;
		}
	}
}
