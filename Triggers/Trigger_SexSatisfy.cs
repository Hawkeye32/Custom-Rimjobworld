using Verse;
using Verse.AI.Group;

namespace rjw
{
	public class Trigger_SexSatisfy : TriggerFilter
	{
		private const int CheckInterval = 120;
		private const int TickTimeout = 900;
		private int currentTick = 0;
		public float targetValue = 0.3f;

		public Trigger_SexSatisfy(float t)
		{
			this.targetValue = t;
			currentTick = 0;
		}

		public override bool AllowActivation(Lord lord, TriggerSignal signal)
		{
			currentTick++;
			if (signal.type == TriggerSignalType.Tick && Find.TickManager.TicksGame % CheckInterval == 0)
			{
				float? avgValue = null;
				foreach (var pawn in lord.ownedPawns)
				{
					/*foreach(Pawn p in lord.Map.mapPawns.PawnsInFaction(Faction.OfPlayer))
					{
					}*/
					Need_Sex n = pawn.needs.TryGetNeed<Need_Sex>();
					//if (n != null && pawn.gender == Gender.Male && !pawn.Downed)
					if(xxx.can_rape(pawn) && (xxx.is_healthy_enough(pawn) && xxx.IsTargetPawnOkay(pawn) || !xxx.is_human(pawn)) && Find.TickManager.TicksGame > pawn.mindState.canLovinTick)
					{
						avgValue = (avgValue == null) ? n.CurLevel : (avgValue + n.CurLevel) / 2f;
					}
				}
				//--Log.Message("[ABF]Trigger_SexSatisfy::ActivateOn Checked value :" + avgValue + "/" + targetValue);
				return avgValue == null || avgValue >= targetValue;
			}
			return currentTick >= TickTimeout;
		}
	}
}