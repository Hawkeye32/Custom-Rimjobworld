using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;
using Multiplayer.API;

namespace rjw
{
	public static class AgeStage
	{
		public const int Baby = 0;
		public const int Toddler = 1;
		public const int Child = 2;
		public const int Teenager = 3;
		public const int Adult = 4;
	}

	public class Hediff_SimpleBaby : HediffWithComps
	{
		// Keeps track of what stage the pawn has grown to
		private int grown_to = 0;
		private int stage_completed = 0;
		public float lastTick = 0;
		public float dayTick = 0;

		//Properties
		public int Grown_To
		{
			get
			{
				return grown_to;
			}
		}

		public override string DebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.DebugString());
			stringBuilder.Append("Current CurLifeStageIndex: " + pawn.ageTracker.CurLifeStageIndex + ", grown_to: " + grown_to);
			return stringBuilder.ToString();
		}

		public override void PostMake()
		{
			Severity = Math.Max(0.01f, Severity);
			if (grown_to == 0 && !pawn.health.hediffSet.HasHediff(HediffDef.Named("RJW_NoManipulationFlag")))
			{
				pawn.health.AddHediff(HediffDef.Named("RJW_NoManipulationFlag"), null, null);
			}
			base.PostMake();

			if (!pawn.RaceProps.lifeStageAges.Any(x => x.def.reproductive))
			{
				if (pawn.health.hediffSet.HasHediff(xxx.RJW_NoManipulationFlag))
				{
					pawn.health.hediffSet.hediffs.Remove(pawn.health.hediffSet.GetFirstHediffOfDef(xxx.RJW_NoManipulationFlag));
				}
				pawn.health.RemoveHediff(this);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref grown_to, "grown_to", 0);
			Scribe_Values.Look(ref lastTick, "lastTick", 0);
			Scribe_Values.Look(ref dayTick, "dayTick", 0);
		}

		internal void GrowUpTo(int stage)
		{
			GrowUpTo(stage, true);
		}

		[SyncMethod]
		internal void GrowUpTo(int stage, bool generated)
		{
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			if (grown_to < stage)
				grown_to = stage;

			// Update the Colonist Bar
			PortraitsCache.SetDirty(pawn);
			pawn.Drawer.renderer.graphics.ResolveAllGraphics();

			// At the toddler stage. Now we can move and talk.
			if ((stage >= 1 || pawn.ageTracker.CurLifeStage.reproductive) && stage_completed < 1)
			{
				Severity = Math.Max(0.5f, Severity);
				stage_completed++;
			}
			// Re-enable skills that were locked out from toddlers
			if ((stage >= 2 || pawn.ageTracker.CurLifeStage.reproductive) && stage_completed < 2)
			{
				if (!generated)
				{
					if (xxx.RimWorldChildrenIsActive)
					{
						pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("CustomBackstory_Rimchild");
					}
					// Remove the hidden hediff stopping pawns from manipulating
				}
				if (pawn.health.hediffSet.HasHediff(xxx.RJW_NoManipulationFlag))
				{
					pawn.health.hediffSet.hediffs.Remove(pawn.health.hediffSet.GetFirstHediffOfDef(xxx.RJW_NoManipulationFlag));
				}
				Severity = Math.Max(0.75f, Severity);
				stage_completed++;
			}
			// The child has grown to a teenager so we no longer need this effect
			if ((stage >= 3 || pawn.ageTracker.CurLifeStage.reproductive) && stage_completed < 3)
			{
				if (!generated && pawn.story.Childhood != null && pawn.story.Childhood.title == "Child")
				{
					if (xxx.RimWorldChildrenIsActive)
					{
						pawn.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("CustomBackstory_Rimchild");
					}
				}

				// Gain traits from life experiences
				if (pawn.story.traits.allTraits.Count < 3)
				{
					List<Trait> life_traitpool = new List<Trait>();
					// Try get cannibalism
					if (pawn.needs.mood.thoughts.memories.Memories.Find(x => x.def == ThoughtDefOf.AteHumanlikeMeatAsIngredient) != null)
					{
						life_traitpool.Add(new Trait(TraitDefOf.Cannibal, 0, false));
					}
					// Try to get bloodlust
					if (pawn.records.GetValue(RecordDefOf.KillsHumanlikes) > 0 || pawn.records.GetValue(RecordDefOf.AnimalsSlaughtered) >= 2)
					{
						life_traitpool.Add(new Trait(TraitDefOf.Bloodlust, 0, false));
					}
					// Try to get shooting accuracy
					if (pawn.records.GetValue(RecordDefOf.ShotsFired) > 100 && (int)pawn.records.GetValue(RecordDefOf.PawnsDowned) > 0)
					{
						life_traitpool.Add(new Trait(TraitDef.Named("ShootingAccuracy"), 1, false));
					}
					else if (pawn.records.GetValue(RecordDefOf.ShotsFired) > 100 && (int)pawn.records.GetValue(RecordDefOf.PawnsDowned) == 0)
					{
						life_traitpool.Add(new Trait(TraitDef.Named("ShootingAccuracy"), -1, false));
					}
					// Try to get brawler
					else if (pawn.records.GetValue(RecordDefOf.ShotsFired) < 15 && pawn.records.GetValue(RecordDefOf.PawnsDowned) > 1)
					{
						life_traitpool.Add(new Trait(TraitDefOf.Brawler, 0, false));
					}
					// Try to get necrophiliac
					if (pawn.records.GetValue(RecordDefOf.CorpsesBuried) > 50)
					{
						life_traitpool.Add(new Trait(xxx.necrophiliac, 0, false));
					}
					// Try to get nymphomaniac
					if (pawn.records.GetValue(RecordDefOf.BodiesStripped) > 50)
					{
						life_traitpool.Add(new Trait(xxx.nymphomaniac, 0, false));
					}
					// Try to get rapist
					if (pawn.records.GetValue(RecordDefOf.TimeAsPrisoner) > 300)
					{
						life_traitpool.Add(new Trait(xxx.rapist, 0, false));
					}
					// Try to get Dislikes Men/Women
					int male_rivals = 0;
					int female_rivals = 0;
					foreach (Pawn colinist in Find.AnyPlayerHomeMap.mapPawns.AllPawnsSpawned)
					{
						if (pawn.relations.OpinionOf(colinist) <= -20)
						{
							if (colinist.gender == Gender.Male)
								male_rivals++;
							else
								female_rivals++;
						}
					}
					// Find which gender we hate
					if (male_rivals > 3 || female_rivals > 3)
					{
						if (male_rivals > female_rivals)
							life_traitpool.Add(new Trait(TraitDefOf.DislikesMen, 0, false));
						else if (female_rivals > male_rivals)
							life_traitpool.Add(new Trait(TraitDefOf.DislikesWomen, 0, false));
					}
					// Pyromaniac never put out any fires. Seems kinda stupid
					/*if ((int)pawn.records.GetValue (RecordDefOf.FiresExtinguished) == 0) {
						life_traitpool.Add (new Trait (TraitDefOf.Pyromaniac, 0, false));
					}*/
					// Neurotic
					if (pawn.records.GetValue(RecordDefOf.TimesInMentalState) > 6)
					{
						life_traitpool.Add(new Trait(TraitDef.Named("Neurotic"), 2, false));
					}
					else if (pawn.records.GetValue(RecordDefOf.TimesInMentalState) > 3)
					{
						life_traitpool.Add(new Trait(TraitDef.Named("Neurotic"), 1, false));
					}

					// People(male or female) can turn gay during puberty
					//Rand.PopState();
					//Rand.PushState(RJW_Multiplayer.PredictableSeed());
					if (Rand.Value <= 0.03f && pawn.story.traits.allTraits.Count <= 3)
					{
						pawn.story.traits.GainTrait(new Trait(TraitDefOf.Gay, 0, true));
					}

					// Now let's try to add some life experience traits
					if (life_traitpool.Count > 0)
					{
						int i = 3;
						while (pawn.story.traits.allTraits.Count < 3 && i > 0)
						{
							Trait newtrait = life_traitpool.RandomElement();
							if (pawn.story.traits.HasTrait(newtrait.def) == false)
								pawn.story.traits.GainTrait(newtrait);
							i--;
						}
					}
				}

				stage_completed++;
				pawn.health.RemoveHediff(this);
			}
		}

		//everyday grow 1 year until reproductive
		public void GrowFast()
		{
			if (RJWPregnancySettings.phantasy_pregnancy)
			{
				if (!pawn.ageTracker.CurLifeStage.reproductive || pawn.ageTracker.Growth >= 1)
				{
					pawn.ageTracker.AgeBiologicalTicks = (pawn.ageTracker.AgeBiologicalYears + 1) * GenDate.TicksPerYear + 1;
					pawn.ageTracker.DebugForceBirthdayBiological();
					GrowUpTo(pawn.ageTracker.CurLifeStageIndex, false);
				}
			}
		}

		public void TickRare()
		{
			//--ModLog.Message("Hediff_SimpleBaby::TickRare is called");
			if (pawn.ageTracker.CurLifeStageIndex > Grown_To)
			{
				GrowUpTo(pawn.ageTracker.CurLifeStageIndex, false);
			}

			// Update the graphics set
			//if (pawn.ageTracker.CurLifeStageIndex >= AgeStage.Toddler)
			//	pawn.Drawer.renderer.graphics.ResolveAllGraphics();

			if (xxx.RimWorldChildrenIsActive)
			{
				//if (Prefs.DevMode)
				//	Log.Message("RJW child tick - CnP active");
				//we do not disable our hediff anymore
	//			if (pawn.health.hediffSet.HasHediff(HediffDef.Named("RJW_NoManipulationFlag")))
				//{
				//	pawn.health.hediffSet.hediffs.Remove(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("RJW_NoManipulationFlag")));
				//	pawn.health.AddHediff(HediffDef.Named("NoManipulationFlag"), null, null);
				//}
				//if (pawn.health.hediffSet.HasHediff(HediffDef.Named("RJW_BabyState")))
				//{
				//	pawn.health.hediffSet.hediffs.Remove(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("RJW_BabyState")));
				//	pawn.health.AddHediff(HediffDef.Named("BabyState"), null, null);
	//				if (Prefs.DevMode) Log.Message("RJW_Babystate self-removing");
	//			}
				if (pawn.ageTracker.CurLifeStageIndex <= 1)
				{   //The UnhappBaby feature is not included in RJW, but will
					// Check if the baby is hungry, and if so, add the whiny baby hediff
					var hunger = pawn.needs.food;
					var joy = pawn.needs.joy;
					if ((joy != null)&&(hunger!=null))
					{ //There's no way to patch out the CnP adressing nill fields
						if (hunger.CurLevelPercentage<hunger.PercentageThreshHungry || joy.CurLevelPercentage <0.1)
						{
							if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("UnhappyBaby"))){
								//--Log.Message("Adding unhappy baby hediff");
								pawn.health.AddHediff(HediffDef.Named("UnhappyBaby"), null, null);
							}
						}
					}
				}
			}
		}

		public override void Tick()
		{
			/*
			if (xxx.RimWorldChildrenIsActive)
			{
				if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("RJW_BabyState"))!=null)
				{
					pawn.health.RemoveHediff(this);
				}
				return;
			}
			*/
			//This void call every frame. should not logmes no reason
			//--ModLog.Message("Hediff_SimpleBaby::PostTick is called");

			base.Tick();
			if (pawn.Spawned)
			{
				var thisTick = Find.TickManager.TicksGame;

				if ((thisTick - dayTick) >= GenDate.TicksPerDay)
				{
					GrowFast();
					dayTick = thisTick;
				}
				if ((thisTick - lastTick) >= 250)
				{
					TickRare();
					lastTick = thisTick;
				}
			}
		}

		public override bool Visible
		{
			get { return false; }
		}
	}
}