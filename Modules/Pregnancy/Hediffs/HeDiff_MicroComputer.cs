using System.Collections.Generic;
using Verse;
using Multiplayer.API;
using RimWorld;

namespace rjw
{
	//MicroComputer
	internal class Hediff_MicroComputer : Hediff_MechImplants
	{
		protected int nextEventTick = GenDate.TicksPerDay;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.nextEventTick, "nextEventTick", GenDate.TicksPerDay, false);
		}

		[SyncMethod]
		public override void PostMake()
		{
			base.PostMake();
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			nextEventTick = Rand.Range(mcDef.minEventInterval, mcDef.maxEventInterval);
		}

		[SyncMethod]
		public override void Tick()
		{
			base.Tick();
			if (this.pawn.IsHashIntervalTick(1000))
			{
				if (this.ageTicks >= nextEventTick)
				{
					HediffDef randomEffectDef = DefDatabase<HediffDef>.GetNamed(randomEffect);
					if (randomEffectDef != null)
					{
						pawn.health.AddHediff(randomEffectDef);
					}
					else
					{
						//--ModLog.Message("" + this.GetType().ToString() + "::Tick() - There is no Random Effect");
					}
					this.ageTicks = 0;
				}
			}
		}

		protected HediffDef_MechImplants mcDef
		{
			get
			{
				return ((HediffDef_MechImplants)def);
			}
		}

		protected List<string> randomEffects
		{
			get
			{
				return mcDef.randomHediffDefs;
			}
		}

		protected string randomEffect
		{
			get
			{
				//Rand.PopState();
				//Rand.PushState(RJW_Multiplayer.PredictableSeed());
				return randomEffects.RandomElement<string>();
			}
		}
	}
}