using System;
using System.Linq;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using System.Text;
using Verse.AI.Group;
using Multiplayer.API;

namespace rjw
{
	public class Hediff_InsectEgg : HediffWithComps
	{
		/*
		 * Dev Note: To easily test egg-birth, set the p_end_tick to p_start_tick + 10 in the SaveFile. 
		 */

		public float p_start_tick = 0;
		public float p_end_tick = 0;
		public float lastTick = 0;

		private HediffDef_InsectEgg HediffDefOfEgg => (HediffDef_InsectEgg)def;

		public string parentDef => HediffDefOfEgg.parentDef;

		public List<string> parentDefs => HediffDefOfEgg.parentDefs;
		public List<GeneDef> eggGenes;

		// TODO: I'd like to change "father" to "fertilizer" but that would be a breaking change.
		public Pawn father;					//can be parentkind defined in egg
		public Pawn implanter;					//can be any pawn
		public bool canbefertilized = true;
		public bool fertilized => father != null;
		public float eggssize = 0.1f;
		protected List<Pawn> babies;
		float Gestation = 0f;

		///Contractions duration, effectively additional hediff stage, a dirty hack to make birthing process notable
		//protected const int TicksPerHour = 2500;
		protected int contractions = 0;

		public override string LabelBase
		{
			get
			{
				if (Prefs.DevMode && implanter != null)
				{
						return implanter.kindDef.race.label + " egg";
				}

				if (eggssize <= 0.10f)
					return "Small egg";
				if (eggssize <= 0.3f)
					return "Medium egg";
				else if (eggssize <= 0.5f)
					return "Big egg";
				else
					return "Huge egg";
			}
		}

		public override string LabelInBrackets
		{
			get
			{
				if (Prefs.DevMode)
				{
					if (fertilized)
						return "Fertilized";
					else
						return "Unfertilized";
				}
				return null;
			}
		}

		public float GestationProgress
		{
			get => Gestation;
			set => Gestation = value;
		}

		public override bool TryMergeWith(Hediff other)
		{
			return false;
		}

		public override void Tick()
		{
			var thisTick = Find.TickManager.TicksGame;
			GestationProgress = (1 + thisTick - p_start_tick) / (p_end_tick - p_start_tick);

			if ((thisTick - lastTick) >= 1000)
			{
				lastTick = thisTick;
				//birthing takes an hour
				if (GestationProgress >= 1 && contractions == 0 && !(pawn.jobs.curDriver is JobDriver_Sex))
				{
					if (PawnUtility.ShouldSendNotificationAbout(pawn) && (pawn.IsColonist || pawn.IsPrisonerOfColony))
					{
						string key = "RJW_EggContractions";
						string text = TranslatorFormattedStringExtensions.Translate(key, pawn.LabelIndefinite());
						Messages.Message(text, pawn, MessageTypeDefOf.NeutralEvent);
					}
					contractions++;
					if (!Genital_Helper.has_ovipositorF(pawn))
						pawn.health.AddHediff(xxx.submitting);
				}
				else if (GestationProgress >= 1 && contractions != 0 && (pawn.CarriedBy == null || pawn.CarriedByCaravan()))
				{
					if (PawnUtility.ShouldSendNotificationAbout(pawn) && (pawn.IsColonist || pawn.IsPrisonerOfColony))
					{
						string key1 = "RJW_GaveBirthEggTitle";
						string message_title = TranslatorFormattedStringExtensions.Translate(key1, pawn.LabelIndefinite());
						string key2 = "RJW_GaveBirthEggText";
						string message_text = TranslatorFormattedStringExtensions.Translate(key2, pawn.LabelIndefinite());

						Messages.Message(message_text, pawn, MessageTypeDefOf.SituationResolved);
					}
					GiveBirth();
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref father, "father", true);
			Scribe_References.Look(ref implanter, "implanter", true);
			Scribe_Values.Look(ref Gestation, "Gestation");
			Scribe_Values.Look(ref p_start_tick, "p_start_tick", 0);
			Scribe_Values.Look(ref p_end_tick, "p_end_tick", 0);
			Scribe_Values.Look(ref lastTick, "lastTick", 0);
			Scribe_Values.Look(ref eggssize, "eggssize", 0.1f);
			Scribe_Values.Look(ref canbefertilized, "canbefertilized", true);
			Scribe_Collections.Look(ref eggGenes, label: "genes", lookMode: LookMode.Def);
			Scribe_Collections.Look(ref babies, saveDestroyedThings: true, label: "babies", lookMode: LookMode.Deep, ctorArgs: new object[0]);
		}
		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			GiveBirth();
		}

		//should someday remake into birth eggs and then within few ticks hatch them
		[SyncMethod]
		public void GiveBirth()
		{
			Pawn mother = pawn;
			Pawn baby = null;

			if (RJWSettings.DevMode) ModLog.Message($"{pawn} is giving birth to an InsectEgg");
			if (fertilized)
			{
				if (RJWSettings.DevMode) ModLog.Message($"Egg was fertilized - Fertilizer {father} and Implanter {implanter}");
				PawnKindDef spawn_kind_def = implanter.kindDef;

				Faction spawn_faction = DetermineSpawnFaction(mother);
				spawn_kind_def = AdjustSpawnKindDef(mother, spawn_kind_def);

				if (!spawn_kind_def.RaceProps.Humanlike)
				{
					BirthFertilizedAnimalEgg(mother, spawn_kind_def);
				}
				else //humanlike baby
				{
					baby = ProcessHumanLikeInsectEgg(mother, spawn_kind_def, spawn_faction);
				}
				UpdateBirtherRecords(mother);
			}
			else
			{
				BirthUnfertilizedEggs(mother);
			}
			// Post birth
			ProcessPostBirth(mother, baby);

			mother.health.RemoveHediff(this);
		}

		/// <summary>
		/// Bumps the Birth-Statistic for eggs and maybe adds fitting quirks for the mother.
		/// </summary>
		/// <param name="mother">The pawn that birthed a new egg</param>
		private static void UpdateBirtherRecords(Pawn mother)
		{

			if (RJWSettings.DevMode) ModLog.Message($"Updating {mother}s records after birthing an egg"); 
			
			mother.records.AddTo(xxx.CountOfBirthEgg, 1);
			if (mother.records.GetAsInt(xxx.CountOfBirthEgg) > 100)
			{
				mother.Add(Quirk.Incubator);
				mother.Add(Quirk.ImpregnationFetish);
			}
		}

		/// <summary>
		/// Adjusts the Pawn_Kind_Def if it is a Nymph. 
		/// Issue is that later processing cannot nicely deal with Nymphs.
		/// </summary>
		/// <param name="mother">The mother, for potential back-up lookup</param>
		/// <param name="spawn_kind_def">The current spawn_kind_def</param>
		/// <returns></returns>
		private PawnKindDef AdjustSpawnKindDef(Pawn mother, PawnKindDef spawn_kind_def)
		{
			if (spawn_kind_def.defName.Contains("Nymph"))
			{
				//child is nymph, try to find other PawnKindDef
				var spawn_kind_def_list = new List<PawnKindDef>();
				spawn_kind_def_list.AddRange(DefDatabase<PawnKindDef>.AllDefs.Where(x => x.race == spawn_kind_def.race && !x.defName.Contains("Nymph")));
				//no other PawnKindDef found try mother
				if (spawn_kind_def_list.NullOrEmpty())
					spawn_kind_def_list.AddRange(DefDatabase<PawnKindDef>.AllDefs.Where(x => x.race == mother.kindDef.race && !x.defName.Contains("Nymph")));
				//no other PawnKindDef found try implanter
				if (spawn_kind_def_list.NullOrEmpty() && implanter != null)
					spawn_kind_def_list.AddRange(DefDatabase<PawnKindDef>.AllDefs.Where(x => x.race == implanter.kindDef.race && !x.defName.Contains("Nymph")));
				//no other PawnKindDef found fallback to generic colonist
				if (spawn_kind_def_list.NullOrEmpty())
					spawn_kind_def = PawnKindDefOf.Colonist;

				spawn_kind_def = spawn_kind_def_list.RandomElement();
			}

			return spawn_kind_def;
		}

		private void ProcessPostBirth(Pawn mother, Pawn baby)
		{
			if (mother.Spawned)
			{
				// Spawn guck
				if (mother.caller != null)
				{
					mother.caller.DoCall();
				}
				if (baby != null)
				{
					if (baby.caller != null)
					{
						baby.caller.DoCall();
					}
				}
				FilthMaker.TryMakeFilth(mother.Position, mother.Map, ThingDefOf.Filth_AmnioticFluid, mother.LabelIndefinite(), 5);
				int howmuch = xxx.has_quirk(mother, "Incubator") ? Rand.Range(1, 3) * 2 : Rand.Range(1, 3);
				int i = 0;
				if (xxx.is_insect(baby) || xxx.is_insect(mother) || xxx.is_insect(implanter) || xxx.is_insect(father))
				{
					while (i++ < howmuch)
					{
						Thing jelly = ThingMaker.MakeThing(ThingDefOf.InsectJelly);
						if (mother.MapHeld?.areaManager?.Home[mother.PositionHeld] == false)
							jelly.SetForbidden(true, false);
						GenPlace.TryPlaceThing(jelly, mother.PositionHeld, mother.MapHeld, ThingPlaceMode.Near);
					}
				}
			}
		}

		/// <summary>
		/// Pops out eggs that are not fertilized.
		/// </summary>
		/// <param name="mother"></param>
		private void BirthUnfertilizedEggs(Pawn mother)
		{
			if (RJWSettings.DevMode) ModLog.Message($"{mother} is giving 'birth' to unfertilized Insect-Eggs.");

			if (PawnUtility.ShouldSendNotificationAbout(pawn) && (pawn.IsColonist || pawn.IsPrisonerOfColony))
			{
				string key = "EggDead";
				string text = TranslatorFormattedStringExtensions.Translate(key, pawn.LabelIndefinite()).CapitalizeFirst();
				Messages.Message(text, pawn, MessageTypeDefOf.SituationResolved);
			}
			Thing egg;
			var UEgg = DefDatabase<ThingDef>.GetNamedSilentFail(HediffDefOfEgg.UnFertEggDef);
			if (UEgg == null)
				UEgg = (DefDatabase<ThingDef>.GetNamedSilentFail("RJW_EggInsectUnfertilized"));
			egg = ThingMaker.MakeThing(UEgg);

			if (mother.MapHeld?.areaManager?.Home[mother.PositionHeld] == false)
				egg.SetForbidden(true, false);
			GenPlace.TryPlaceThing(egg, mother.PositionHeld, mother.MapHeld, ThingPlaceMode.Near);
		}

		private Pawn ProcessHumanLikeInsectEgg(Pawn mother, PawnKindDef spawn_kind_def, Faction spawn_faction)
		{
			ModLog.Message("Hediff_InsectEgg::BirthBaby() " + spawn_kind_def + " of " + spawn_faction + " in " + (int)(50 * implanter.GetStatValue(StatDefOf.PsychicSensitivity)) + " chance!");

			PawnGenerationRequest request = new PawnGenerationRequest(
				kind: spawn_kind_def,
				faction: spawn_faction,
				forceGenerateNewPawn: true,
				developmentalStages: DevelopmentalStage.Newborn,
				allowDowned: true,
				canGeneratePawnRelations: false,
				colonistRelationChanceFactor: 0,
				allowFood: false,
				allowAddictions: false,
				relationWithExtraPawnChanceFactor: 0,
				//forceNoIdeo: true,
				forbidAnyTitle: true,
				forceNoBackstory: true);

			Pawn baby = PawnGenerator.GeneratePawn(request);
			// If we have genes, and the baby has not set any, add the ones stored from fertilization as endogenes
			if (RJWPregnancySettings.egg_pregnancy_genes && eggGenes != null && eggGenes.Count > 0)
			{
				if (RJWSettings.DevMode) ModLog.Message($"Setting {eggGenes.Count} genes for {baby}");
				foreach (var gene in baby.genes.GenesListForReading)
					baby.genes.RemoveGene(gene);
				foreach (var gene in eggGenes)
					baby.genes.AddGene(gene, false);
			} else
			{
				if (RJWSettings.DevMode) ModLog.Message($"Did not find genes for {baby}");
			}

			DetermineIfBabyIsPrisonerOrSlave(mother, baby);

			SetBabyRelations(baby,implanter,father, mother);

			PawnUtility.TrySpawnHatchedOrBornPawn(baby, mother);

			if (spawn_faction == Faction.OfInsects || (spawn_faction != null && (spawn_faction.def.defName.Contains("insect") || spawn_faction == implanter.Faction)))
			{
				Lord lord = implanter.GetLord();
				if (lord != null)
				{
					lord.AddPawn(baby);
				}
			}

			Hediff_BasePregnancy.BabyPostBirth(mother, father, baby);
			Sexualizer.sexualize_pawn(baby);

			// Move the baby in front of the mother, rather than on top
			if (mother.Spawned)
				if (mother.CurrentBed() != null)
				{
					baby.Position = baby.Position + new IntVec3(0, 0, 1).RotatedBy(mother.CurrentBed().Rotation);
				}

			return baby;
		}

		private void SetBabyRelations(Pawn Baby, Pawn EggProducer, Pawn Fertilizer, Pawn Host)
		{
			if (RJWSettings.DevMode) ModLog.Message($"Setting Relations for {Baby}: Egg-Mother={EggProducer}, Egg-Fertilizer={Fertilizer}, Egg-Host={Host}");
            if (!RJWSettings.Disable_egg_pregnancy_relations)
				if (Baby.RaceProps.IsFlesh)
				{
					if (EggProducer != null)
						Baby.relations.AddDirectRelation(PawnRelationDefOf.Parent, EggProducer);
					// Note: Depending on the Sex of the Fertilizer, it's labelled as "Mother" too.
					if (Fertilizer != null)
						Baby.relations.AddDirectRelation(PawnRelationDefOf.Parent, Fertilizer);
					if (Host != null && Host != EggProducer)
						Baby.relations.AddDirectRelation(PawnRelationDefOf.ParentBirth, Host);
				}
		}

		private static void DetermineIfBabyIsPrisonerOrSlave(Pawn mother, Pawn baby)
		{
			if (mother.IsSlaveOfColony)
			{
				if (mother.SlaveFaction != null)
					baby.SetFaction(mother.SlaveFaction);
				else if (mother.HomeFaction != null)
					baby.SetFaction(mother.HomeFaction);
				else if (mother.Faction != null)
					baby.SetFaction(mother.Faction);
				else
					baby.SetFaction(Faction.OfPlayer);
				baby.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Slave);
			}
			else if (mother.IsPrisonerOfColony)
			{
				if (mother.HomeFaction != null)
					baby.SetFaction(mother.HomeFaction);
				baby.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Prisoner);
			}
		}

		private void BirthFertilizedAnimalEgg(Pawn mother, PawnKindDef spawn_kind_def)
		{
			if (RJWSettings.DevMode) ModLog.Message($"{mother} is birthing an fertilized egg of {spawn_kind_def} ");

			Thing egg;
			ThingDef EggDef;
			string childrendef = "";
			PawnKindDef children = null;

			//make egg
			EggDef = DefDatabase<ThingDef>.GetNamedSilentFail(HediffDefOfEgg.FertEggDef);			//try to find predefined
			if (EggDef == null)                                                                     //fail, use generic
				EggDef = (DefDatabase<ThingDef>.GetNamedSilentFail("RJW_EggInsectFertilized"));
			egg = ThingMaker.MakeThing(EggDef);

			//make child
			List<string> childlist = new List<string>();
			if (!HediffDefOfEgg.childrenDefs.NullOrEmpty())
			{
				foreach (var child in HediffDefOfEgg.childrenDefs)
				{
					if (DefDatabase<PawnKindDef>.GetNamedSilentFail(child) != null)
						childlist.AddDistinct(child);
				}
				childrendef = childlist.RandomElement();                                                    //try to find predefined
			}
			if (!childrendef.NullOrEmpty())
				children = DefDatabase<PawnKindDef>.GetNamedSilentFail(childrendef);

			if (children == null)                                                                           //use fatherDef
				children = spawn_kind_def;

			//put child into egg
			if (children != null)
			{
				var t = egg.TryGetComp<CompHatcher>();
				t.Props.hatcherPawn = children;
				t.hatcheeParent = implanter;
				t.otherParent = father;
				t.hatcheeFaction = implanter.Faction;
			}

			if (mother.MapHeld?.areaManager?.Home[mother.PositionHeld] == false)
				egg.SetForbidden(true, false);

			//poop egg
			GenPlace.TryPlaceThing(egg, mother.PositionHeld, mother.MapHeld, ThingPlaceMode.Near);
		}

		private Faction DetermineSpawnFaction(Pawn mother)
		{
			Faction spawn_faction;

			//core Hive Insects... probably
			if (implanter.Faction == Faction.OfInsects || father.Faction == Faction.OfInsects || mother.Faction == Faction.OfInsects)
			{
				spawn_faction = Faction.OfInsects;
				int chance = 5;

				//random chance to make insect neutral/tamable
				if (father.Faction == Faction.OfInsects)
					chance = 5;
				if (father.Faction != Faction.OfInsects)
					chance = 10;
				if (father.Faction == Faction.OfPlayer)
					chance = 25;
				if (implanter.Faction == Faction.OfPlayer)
					chance += 25;
				if (implanter.Faction == Faction.OfPlayer && xxx.is_human(implanter))
					chance += (int)(25 * implanter.GetStatValue(StatDefOf.PsychicSensitivity));
				if (Rand.Range(0, 100) <= chance)
					spawn_faction = null;

				//chance tame insect on birth 
				if (spawn_faction == null)
					if (implanter.Faction == Faction.OfPlayer && xxx.is_human(implanter))
						if (Rand.Range(0, 100) <= (int)(50 * implanter.GetStatValue(StatDefOf.PsychicSensitivity)))
							spawn_faction = Faction.OfPlayer;
			}
			//humanlikes
			else if (xxx.is_human(implanter))
			{
				spawn_faction = implanter.Faction;
			}
			//animal, spawn implanter faction (if not player faction/not tamed)
			else if (!xxx.is_human(implanter) && !(implanter.Faction?.IsPlayer ?? false))
			{
				spawn_faction = implanter.Faction;
			}
			//spawn factionless(tamable, probably)
			else
			{
				spawn_faction = null;
			}

			return spawn_faction;
		}

		/// <summary>
		/// Sets the Father of an egg. 
		/// If possible, the genes of the baby are determined too and stored in the egg-pregnancy.
		/// Exception is for androids or immortals (mod content).
		/// </summary>
		/// <param name="Pawn"> Pawn to be set as Father </param>
		public void Fertilize(Pawn Pawn)
		{
			if (implanter == null)
			{
				return;
			}
			if (xxx.ImmortalsIsActive && (Pawn.health.hediffSet.HasHediff(xxx.IH_Immortal) || pawn.health.hediffSet.HasHediff(xxx.IH_Immortal)))
			{
				return;
			}
			if (AndroidsCompatibility.IsAndroid(pawn))
			{
				return;
			}

			if (!fertilized && canbefertilized && GestationProgress < 0.5)
			{
				if (RJWSettings.DevMode) ModLog.Message($"{xxx.get_pawnname(pawn)} had eggs inside fertilized (Fertilized by {xxx.get_pawnname(father)}): {this}");
				father = Pawn;
				ChangeEgg(implanter);
			}

			// If both parents are human / have genes, set the genes of the offspring
			if (RJWPregnancySettings.egg_pregnancy_genes && father != null && implanter != null && father.genes != null && implanter.genes != null)
			{
				if (RJWSettings.DevMode) ModLog.Message($"Setting InsectEgg-Genes for {this}");
				DetermineChildGenes();
			} else
			{
				if (RJWSettings.DevMode) ModLog.Message($"Could not set Genes for {this}");
			}
		}

		/// <summary>
		/// Sets the genes of the child if both Implanter and Fertilizer have genes.
		/// Intentionally in it's own method for easier access with Harmony Patches.
		/// </summary>
		private void DetermineChildGenes()
		{
			eggGenes = PregnancyUtility.GetInheritedGenes(father, implanter);
			if (RJWSettings.DevMode) ModLog.Message($"Set Genes for {this} from {father} & {implanter} - total of {eggGenes.Count} genes set.");
		}

		/// <summary>
		/// Sets the Implanter and thus the base-egg type. 
		/// If the egg is Self-Fertilized, the egg also gets fertilized (with Implanter as father, too). 
		/// </summary>
		/// <param name="Pawn">Pawn to be implanter.</param>
		public void Implanter(Pawn Pawn)
		{
			if (implanter == null)
			{
				if (RJWSettings.DevMode) ModLog.Message("Hediff_InsectEgg:: set implanter:" + xxx.get_pawnname(Pawn));
				implanter = Pawn;
				ChangeEgg(implanter);

				if (!implanter.health.hediffSet.HasHediff(xxx.sterilized))
				{
					if (HediffDefOfEgg.selffertilized)
						Fertilize(implanter);
				}
				else
					canbefertilized = false;
			}
		}

		//Change egg type after implanting/fertilizing
		public void ChangeEgg(Pawn Pawn)
		{
			if (Pawn != null)
			{
				float p_end_tick_mods = 1;

				if (Pawn.RaceProps?.gestationPeriodDays < 1)
				{
					p_end_tick_mods = GenDate.TicksPerDay;
				}
				else
				{
					p_end_tick_mods = Pawn.RaceProps.gestationPeriodDays * GenDate.TicksPerDay;
				}

				if (xxx.has_quirk(pawn, "Incubator") || pawn.health.hediffSet.HasHediff(HediffDef.Named("FertilityEnhancer")))
					p_end_tick_mods /= 2f;

				p_end_tick_mods *= RJWPregnancySettings.egg_pregnancy_duration;

				p_start_tick = Find.TickManager.TicksGame;
				p_end_tick = p_start_tick + p_end_tick_mods;
				lastTick = p_start_tick;

				eggssize = Pawn.RaceProps.baseBodySize / 5;
				if (!Genital_Helper.has_ovipositorF(pawn)) // non ovi egg full size
					Severity = eggssize;
				else if (eggssize > 0.1f) // cap egg size in ovi to 10%
					Severity = 0.1f;
				else
					Severity = eggssize;
			}
		}

		//for setting implanter/fertilize eggs
		public bool IsParent(Pawn parent)
		{
			//anyone can fertilize
			if (RJWPregnancySettings.egg_pregnancy_fertilize_anyone) return true;

			//only set egg parent or implanter can fertilize
			else return parentDef == parent.kindDef.defName
						|| parentDefs.Contains(parent.kindDef.defName)
						|| implanter.kindDef == parent.kindDef; // unknown eggs
		}

		public override string DebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.DebugString());
			stringBuilder.AppendLine(" Gestation progress: " + GestationProgress.ToStringPercent());
			if (RJWSettings.DevMode) stringBuilder.AppendLine(" Implanter: " + xxx.get_pawnname(implanter));
			if (RJWSettings.DevMode) stringBuilder.AppendLine(" Father: " + xxx.get_pawnname(father));
			return stringBuilder.ToString();
		}
	}
}