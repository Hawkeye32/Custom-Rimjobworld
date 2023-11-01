using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using Multiplayer.API;
using Verse.AI.Group;

namespace rjw
{
	//TODO: check slime stuff working in mp
	//TODO: separate menus in sub scripts
	//TODO: add demon switch parts
	//TODO: figure out and make interface?
	//TODO: add vibration toggle for bionics(+50% partner satisfaction?)
	public class Dialog_Sexcard : Window
	{
		private readonly Pawn pawn;

		public Dialog_Sexcard(Pawn editFor)
		{
			pawn = editFor;
		}

		public static breasts Breasts;
		public enum breasts
		{
			selectone,
			none,
			featureless_chest,
			flat_breasts,
			small_breasts,
			average_breasts,
			large_breasts,
			huge_breasts,
			slime_breasts,
		};

		public static anuses Anuses;
		public enum anuses
		{
			selectone,
			none,
			micro_anus,
			tight_anus,
			average_anus,
			loose_anus,
			gaping_anus,
			slime_anus,
		};

		public static vaginas Vaginas;
		public enum vaginas
		{
			selectone,
			none,
			micro_vagina,
			tight_vagina,
			average_vagina,
			loose_vagina,
			gaping_vagina,
			slime_vagina,
			feline_vagina,
			canine_vagina,
			equine_vagina,
			dragon_vagina,
		};

		public static penises Penises;
		public enum penises
		{
			selectone,
			none,
			micro_penis,
			small_penis,
			average_penis,
			big_penis,
			huge_penis,
			slime_penis,
			feline_penis,
			canine_penis,
			equine_penis,
			dragon_penis,
			raccoon_penis,
			hemipenis,
			crocodilian_penis,
		};

		public void SexualityCard(Rect rect, Pawn pawn)
		{
			CompRJW comp = pawn.TryGetComp<CompRJW>();
			if (pawn == null || comp == null) return;

			Text.Font = GameFont.Medium;
			Rect rect1 = new Rect(8f, 4f, rect.width - 8f, rect.height - 20f);
			Widgets.Label(rect1, "RJW");//rjw

			Text.Font = GameFont.Tiny;
			float num = rect1.y + 40f;
			Rect row1 = new Rect(10f, num, rect.width - 8f, 24f);//sexuality
			Rect row2 = new Rect(10f, num + 24, rect.width - 8f, 24f);//quirks
			Rect row3 = new Rect(10f, num + 48, rect.width - 8f, 24f);//whore price

			//Rect sexuality_button = new Rect(10f, rect1.height - 0f, rect.width - 8f, 24f);//change sex pref
			Rect button1 = new Rect(10f, rect1.height - 10f, rect.width - 8f, 24f);//re sexualize
			Rect button2 = new Rect(10f, rect1.height - 34f, rect.width - 8f, 24f);//archtech toggle
			Rect button3 = new Rect(10f, rect1.height - 58f, rect.width - 8f, 24f);//breast
			Rect button4 = new Rect(10f, rect1.height - 82f, rect.width - 8f, 24f);//anus
			Rect button5 = new Rect(10f, rect1.height - 106f, rect.width - 8f, 24f);//vagina
			Rect button6 = new Rect(10f, rect1.height - 130f, rect.width - 8f, 24f);//penis 1
			Rect button7 = new Rect(10f, rect1.height - 154f, rect.width - 8f, 24f);//penis 2
			Rect button8 = new Rect(10f, rect1.height + 14f, rect.width/2 - 8f, 24f);//show Would_fuck
			Rect button9 = new Rect(10f + rect.width / 2, rect1.height + 14f, rect.width/2 - 8f, 24f);//show Would_fuck table

			DrawSexuality(pawn, row1);
			DrawQuirks(pawn, row2);
			//DrawWhoring(pawn, row3);

			// TODO: Add translations. or not
			if (RJWSettings.DevMode || Current.ProgramState != ProgramState.Playing)
			{
				if (Widgets.ButtonText(button1, Current.ProgramState != ProgramState.Playing ? "Reroll" : "[DEV] Reroll"))
				{
					Re_sexualize(pawn);
				}
			}
			if (RJWSettings.DevMode && Current.ProgramState == ProgramState.Playing)
			{
				if (Widgets.ButtonText(button8, "Would_fuck chances"))
				{
					Would_fuck(pawn);
				}
				if (Widgets.ButtonText(button9, "Would_fuck chances (CSV Table)"))
				{
					Would_fuckT(pawn);
				}
			}
			//Archotech genitals FertilityToggle
			List<Hediff> parts = new List<Hediff>();
			var GenitalBPR = Genital_Helper.get_genitalsBPR(pawn);
			parts = Genital_Helper.get_PartsHediffList(pawn, GenitalBPR);

			if (!parts.NullOrEmpty())
			{
				foreach (var p in parts)
				{
					if (PartProps.TryGetProps(p, out List<string> l) && l.Contains("FertilityToggle"))
					{
						if (pawn.health.hediffSet.HasHediff(HediffDef.Named("ImpregnationBlocker")))
						{
							if (Widgets.ButtonText(button2, "Enable reproduction"))
							{
								Change_Fertility(pawn);
							}
						}
						else if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("FertilityEnhancer")))
						{
							if (Widgets.ButtonText(button2, "Enchance fertility"))
							{
								Change_Fertility(pawn);
							}
						}
						else
						{
							if (Widgets.ButtonText(button2, "Disable reproduction"))
							{
								Change_Fertility(pawn);
							}
						}
						break;
					}
				}
			}

			// TODO: add mp synchronizers
			// TODO: clean that mess
			// TODO: add demon toggles
			if (MP.IsInMultiplayer)
				return;

			//List<String> Parts = null;
			//if (xxx.is_slime(pawn))
			//	Parts = new List<string>(DefDatabase<StringListDef>.GetNamed("SlimeMorphFilters").strings);
			//if (xxx.is_demon(pawn))
			//	Parts = new List<string>(DefDatabase<StringListDef>.GetNamed("DemonMorphFilters").strings);

			//if (Parts.Any() && (pawn.IsColonistPlayerControlled || pawn.IsPrisonerOfColony || pawn.IsSlaveOfColony))
				//if (xxx.is_slime(pawn) && (pawn.IsColonistPlayerControlled || pawn.IsPrisonerOfColony || pawn.IsSlaveOfColony))
			if (pawn.IsColonistPlayerControlled || pawn.IsPrisonerOfColony || pawn.IsSlaveOfColony)
			{
				BodyPartRecord bpr_genitalia = Genital_Helper.get_genitalsBPR(pawn);
				BodyPartRecord bpr_breasts = Genital_Helper.get_breastsBPR(pawn);
				BodyPartRecord bpr_anus = Genital_Helper.get_anusBPR(pawn);
				BodyPartRecord partBPR = null;
				Rect Button = new Rect();

				Button = button3;
				partBPR = bpr_breasts;
				Make_button(Button, pawn, partBPR);

				Button = button4;
				partBPR = bpr_anus;
				Make_button(Button, pawn, partBPR);

				Button = button5;
				partBPR = bpr_genitalia;
				Make_button(Button, pawn, partBPR, "p");

				Button = button6;
				partBPR = bpr_genitalia;
				Make_button(Button, pawn, partBPR, "v");
			}
		}

		static void DrawSexuality(Pawn pawn, Rect row)
		{
			string sexuality;

			if (RJWPreferenceSettings.sexuality_distribution == RJWPreferenceSettings.Rjw_sexuality.Vanilla)
				CompRJW.VanillaTraitCheck(pawn);
			if (RJWPreferenceSettings.sexuality_distribution == RJWPreferenceSettings.Rjw_sexuality.SYRIndividuality)
				CompRJW.CopyIndividualitySexuality(pawn);
			if (RJWPreferenceSettings.sexuality_distribution == RJWPreferenceSettings.Rjw_sexuality.Psychology)
				CompRJW.CopyPsychologySexuality(pawn);

			switch (CompRJW.Comp(pawn).orientation)
			{
				case Orientation.Asexual:
					sexuality = "Asexual";
					break;
				case Orientation.Bisexual:
					sexuality = "Bisexual";
					break;
				case Orientation.Heterosexual:
					sexuality = "Hetero";
					break;
				case Orientation.Homosexual:
					sexuality = "Gay";
					break;
				case Orientation.LeaningHeterosexual:
					sexuality = "Bisexual, leaning hetero";
					break;
				case Orientation.LeaningHomosexual:
					sexuality = "Bisexual, leaning gay";
					break;
				case Orientation.MostlyHeterosexual:
					sexuality = "Mostly hetero";
					break;
				case Orientation.MostlyHomosexual:
					sexuality = "Mostly gay";
					break;
				case Orientation.Pansexual:
					sexuality = "Pansexual";
					break;
				default:
					sexuality = "None";
					break;
			}
			
			//allow to change pawn sexuality for:
			//own hero, game start
			if (RJWPreferenceSettings.sexuality_distribution == RJWPreferenceSettings.Rjw_sexuality.RimJobWorld &&
				(((Current.ProgramState == ProgramState.Playing &&
				pawn.IsDesignatedHero() && pawn.IsHeroOwner()) ||
				Prefs.DevMode) ||
				Current.ProgramState == ProgramState.Entry))

			{
				if (Widgets.ButtonText(row, "Sexuality: " + sexuality, false))
				{
					Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()			//this needs fixing in 1.1 with vanilla orientation traits
					{
						new FloatMenuOption("Asexual", (() => Change_orientation(pawn, Orientation.Asexual)), MenuOptionPriority.Default),
						new FloatMenuOption("Pansexual", (() => Change_orientation(pawn, Orientation.Pansexual)), MenuOptionPriority.Default),
						new FloatMenuOption("Heterosexual", (() => Change_orientation(pawn, Orientation.Heterosexual)), MenuOptionPriority.Default),
						new FloatMenuOption("MostlyHeterosexual", (() => Change_orientation(pawn, Orientation.MostlyHeterosexual)), MenuOptionPriority.Default),
						new FloatMenuOption("LeaningHeterosexual", (() => Change_orientation(pawn, Orientation.LeaningHeterosexual)), MenuOptionPriority.Default),
						new FloatMenuOption("Bisexual", (() => Change_orientation(pawn, Orientation.Bisexual)), MenuOptionPriority.Default),
						new FloatMenuOption("LeaningHomosexual", (() => Change_orientation(pawn, Orientation.LeaningHomosexual)), MenuOptionPriority.Default),
						new FloatMenuOption("MostlyHomosexual", (() => Change_orientation(pawn, Orientation.MostlyHomosexual)), MenuOptionPriority.Default),
						new FloatMenuOption("Homosexual", (() => Change_orientation(pawn, Orientation.Homosexual)), MenuOptionPriority.Default),
					}));
				}
			}
			else
			{
				Widgets.Label(row, "Sexuality: " + sexuality);
				if (Mouse.IsOver(row))
					Widgets.DrawHighlight(row);
			}
		}

		static void DrawQuirks(Pawn pawn, Rect row)
		{

			var quirks = Quirk.All
				.Where(quirk => pawn.Has(quirk))
				.OrderBy(quirk => quirk.Key)
				.ToList();

			// Not actually localized.
			var quirkString = quirks.Select(quirk => quirk.Key).ToCommaList();
			
			if ((Current.ProgramState == ProgramState.Playing &&
				pawn.IsDesignatedHero() && pawn.IsHeroOwner() ||
				Prefs.DevMode) ||
				Current.ProgramState == ProgramState.Entry)

			{
				var quirksAll = Quirk.All
								.OrderBy(quirk => quirk.Key)
								.ToList();

				if (!RJWSettings.DevMode)
				{
					quirksAll.Remove(Quirk.Breeder);
					quirksAll.Remove(Quirk.Incubator);
				}

				if (xxx.is_insect(pawn))
					quirksAll.Add(Quirk.Incubator);

				if (Widgets.ButtonText(row, "Quirks".Translate() + quirkString, false))
				{
					var list = new List<FloatMenuOption>();
					list.Add(new FloatMenuOption("Reset", (() => QuirkAdder.Clear(pawn)), MenuOptionPriority.Default));
					foreach (Quirk quirk in quirksAll)
					{
						if (!pawn.Has(quirk))
							list.Add(new FloatMenuOption(quirk.Key, (() => QuirkAdder.Add(pawn, quirk, warnOnFail: true))));
						//else
						//	list.Add(new FloatMenuOption(quirk.Key, (() => QuirkAdder.Remove(pawn, quirk))));
						//TODO: fix quirk description box in 1.1 menus
						//list.Add(new FloatMenuOption(quirk.Key, (() => QuirkAdder.Add(pawn, quirk)), MenuOptionPriority.Default, delegate 
						//{
						//	TooltipHandler.TipRegion(row, quirk.LocaliztionKey.Translate(pawn.Named("pawn")));
						//}
						//));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			}
			else
			{
				// TODO: long quirk list support
				// This can be too long and line wrap.
				// Should probably be a vertical list like traits.
				Widgets.Label(row, "Quirks".Translate() + quirkString);

			}

			if (!Mouse.IsOver(row)) return;
			
			Widgets.DrawHighlight(row);
			if (quirks.NullOrEmpty())
			{
				TooltipHandler.TipRegion(row, "NoQuirks".Translate());
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (var q in quirks)
				{
					stringBuilder.AppendLine(q.Key.Colorize(Color.yellow));
					stringBuilder.AppendLine(q.LocaliztionKey.Translate(pawn.Named("pawn")).AdjustedFor(pawn).Resolve());
					stringBuilder.AppendLine("");
				}
				string str = stringBuilder.ToString().TrimEndNewlines();
				TooltipHandler.TipRegion(row, str);
			}
		}

		//static string DrawWhoring(Pawn pawn, Rect row)
		//{
			//string price;
			//if (RJWSettings.sex_age_legacymode && pawn.ageTracker.AgeBiologicalYears < RJWSettings.sex_minimum_age)
			//	price = "Inapplicable (too young)";
			//else if (!RJWSettings.sex_age_legacymode && pawn.ageTracker.Growth < 1 && !pawn.ageTracker.CurLifeStage.reproductive)
			//	price = "Inapplicable (too young)";
			//else if (pawn.ownership.OwnedRoom == null)
			//{
			//	if (Current.ProgramState == ProgramState.Playing)
			//		price = WhoringHelper.WhoreMinPrice(pawn) + " - " + WhoringHelper.WhoreMaxPrice(pawn) + " (base, needs suitable bedroom)";
			//	else
			//		price = WhoringHelper.WhoreMinPrice(pawn) + " - " + WhoringHelper.WhoreMaxPrice(pawn) + " (base, modified by bedroom quality)";
			//}
			//else if (xxx.is_animal(pawn))
			//	price = "Incapable of whoring";
			//else
			//	price = WhoringHelper.WhoreMinPrice(pawn) + " - " + WhoringHelper.WhoreMaxPrice(pawn);

			//Widgets.Label(row, "WhorePrice".Translate() + price);
			//if (Mouse.IsOver(row))
			//	Widgets.DrawHighlight(row);
			//return price;
		//}


		[SyncMethod]
		static void Change_orientation(Pawn pawn, Orientation orientation)
		{
			CompRJW.Comp(pawn).orientation = orientation;
		}

		[SyncMethod]
		static void Change_Fertility(Pawn pawn)
		{
			BodyPartRecord genitalia = Genital_Helper.get_genitalsBPR(pawn);
			Hediff blocker = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("ImpregnationBlocker"));
			Hediff enhancer = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("FertilityEnhancer"));

			if (blocker != null)
			{
				pawn.health.RemoveHediff(blocker);
			}
			else if (enhancer == null)
			{
				pawn.health.AddHediff(HediffDef.Named("FertilityEnhancer"), genitalia);
			}
			else 
			{
				if (enhancer != null)
					pawn.health.RemoveHediff(enhancer);
				pawn.health.AddHediff(HediffDef.Named("ImpregnationBlocker"), genitalia);
			}
		}

		[SyncMethod]
		static void Re_sexualize(Pawn pawn)
		{
			CompRJW.Comp(pawn).Sexualize(pawn, true);
		}

		static void Would_fuck(Pawn pawn)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("canFuck: " + xxx.can_fuck(pawn) + ", canBeFucked: " + xxx.can_be_fucked(pawn) + ", canDoLoving: " + xxx.can_do_loving(pawn));
			stringBuilder.AppendLine("canRape: " + xxx.can_rape(pawn) + ", canBeRaped: " + xxx.can_get_raped(pawn));

			if (!pawn.IsColonist)
				if (pawn.Faction != null)
				{
					int pawnAmountOfSilvers = pawn.inventory.innerContainer.TotalStackCountOfDef(ThingDefOf.Silver);
					int caravanAmountOfSilvers = 0;
					Lord lord = pawn.GetLord();
					List<Pawn> caravanMembers = pawn.Map.mapPawns.PawnsInFaction(pawn.Faction).Where(x => x.GetLord() == lord && x.inventory?.innerContainer?.TotalStackCountOfDef(ThingDefOf.Silver) > 0).ToList();

					caravanAmountOfSilvers += caravanMembers.Sum(member => member.inventory.innerContainer.TotalStackCountOfDef(ThingDefOf.Silver));

					stringBuilder.AppendLine("pawnSilvers: " + pawnAmountOfSilvers + ", caravanSilvers: " + caravanAmountOfSilvers);
				}
			stringBuilder.AppendLine();

			stringBuilder.AppendLine("Humans - Colonists:");
			List<Pawn> pawns = pawn.Map.mapPawns.AllPawnsSpawned.Where(x => x != pawn && x.RaceProps.Humanlike && x.IsColonist).OrderBy(x => xxx.get_pawnname(x)).ToList();
			foreach (Pawn partner in pawns)
			{
				stringBuilder.AppendLine(partner.LabelShort + " (" + partner.gender.GetLabel() +
					", age: " + partner.ageTracker.AgeBiologicalYears +
					", " + CompRJW.Comp(partner).orientation +
					"): (would fuck) " + SexAppraiser.would_fuck(pawn, partner).ToString("F3") +
					"): (be fucked) " + SexAppraiser.would_fuck(partner, pawn).ToString("F3") +
					": (rape) " + SexAppraiser.would_rape(pawn, partner));
			}
			stringBuilder.AppendLine();

			stringBuilder.AppendLine("Humans - Prisoners:");
			pawns = pawn.Map.mapPawns.AllPawnsSpawned.Where(x => x != pawn && x.RaceProps.Humanlike && !x.IsColonist && x.IsPrisonerOfColony).OrderBy(x => xxx.get_pawnname(x)).ToList();
			foreach (Pawn partner in pawns)
			{
				stringBuilder.AppendLine(partner.LabelShort + " (" + partner.gender.GetLabel() +
					", age: " + partner.ageTracker.AgeBiologicalYears +
					", " + CompRJW.Comp(partner).orientation +
					"): (would fuck) " + SexAppraiser.would_fuck(pawn, partner).ToString("F3") +
					"): (be fucked) " + SexAppraiser.would_fuck(partner, pawn).ToString("F3") +
					": (rape) " + SexAppraiser.would_rape(pawn, partner));
			}
			stringBuilder.AppendLine();

			stringBuilder.AppendLine("Humans - Slaves:");
			pawns = pawn.Map.mapPawns.AllPawnsSpawned.Where(x => x != pawn && x.RaceProps.Humanlike && !x.IsColonist && x.IsSlaveOfColony).OrderBy(x => xxx.get_pawnname(x)).ToList();
			foreach (Pawn partner in pawns)
			{
				stringBuilder.AppendLine(partner.LabelShort + " (" + partner.gender.GetLabel() +
					", age: " + partner.ageTracker.AgeBiologicalYears +
					", " + CompRJW.Comp(partner).orientation +
					"): (would fuck) " + SexAppraiser.would_fuck(pawn, partner).ToString("F3") +
					"): (be fucked) " + SexAppraiser.would_fuck(partner, pawn).ToString("F3") +
					": (rape) " + SexAppraiser.would_rape(pawn, partner));
			}
			stringBuilder.AppendLine();

			stringBuilder.AppendLine("Humans - non Colonists:");
			pawns = pawn.Map.mapPawns.AllPawnsSpawned.Where(x => x != pawn && x.RaceProps.Humanlike && !x.IsColonist && !x.IsPrisonerOfColony && !x.IsSlaveOfColony).OrderBy(x => xxx.get_pawnname(x)).ToList();
			foreach (Pawn partner in pawns)
			{
				stringBuilder.AppendLine(partner.LabelShort + " (" + partner.gender.GetLabel() +
					", age: " + partner.ageTracker.AgeBiologicalYears +
					", " + CompRJW.Comp(partner).orientation +
					"): (would fuck) " + SexAppraiser.would_fuck(pawn, partner).ToString("F3") +
					"): (be fucked) " + SexAppraiser.would_fuck(partner, pawn).ToString("F3") +
					": (rape) " + SexAppraiser.would_rape(pawn, partner));
			}
			stringBuilder.AppendLine();

			stringBuilder.AppendLine("Animals - Colony:");
			pawns = pawn.Map.mapPawns.AllPawnsSpawned.Where(x => x != pawn && x.RaceProps.Animal && x.Faction == Faction.OfPlayer).OrderBy(x => xxx.get_pawnname(x)).ToList();
			foreach (Pawn partner in pawns)
			{
				stringBuilder.AppendLine(partner.LabelShort + " (" + partner.gender.GetLabel() +
					", age: " + partner.ageTracker.AgeBiologicalYears +
					//", " + CompRJW.Comp(partner).orientation +
					"): (would fuck) " + SexAppraiser.would_fuck(pawn, partner).ToString("F3") +
					"): (be fucked) " + SexAppraiser.would_fuck(partner, pawn).ToString("F3") +
					": (rape) " + SexAppraiser.would_rape(pawn, partner));
			}
			stringBuilder.AppendLine();

			stringBuilder.AppendLine("Animals - non Colony:");
			pawns = pawn.Map.mapPawns.AllPawnsSpawned.Where(x => x != pawn && x.RaceProps.Animal && x.Faction != Faction.OfPlayer).OrderBy(x => xxx.get_pawnname(x)).ToList();
			foreach (Pawn partner in pawns)
			{
				stringBuilder.AppendLine(partner.LabelShort + " (" + partner.gender.GetLabel() +
					", age: " + partner.ageTracker.AgeBiologicalYears +
					//", " + CompRJW.Comp(partner).orientation +
					"): (would fuck) " + SexAppraiser.would_fuck(pawn, partner).ToString("F3") +
					"): (be fucked) " + SexAppraiser.would_fuck(partner, pawn).ToString("F3") +
					": (rape) " + SexAppraiser.would_rape(pawn, partner));
			}
			Find.WindowStack.Add(new Dialog_MessageBox(stringBuilder.ToString(), null, null, null, null, null, false, null, null));
		}
		static void Would_fuckT(Pawn pawn)
		{
			{
				DebugTables.MakeTablesDialog(pawn.Map.mapPawns.AllPawnsSpawned.Where(x => x != null & x != pawn),
					new TableDataGetter<Pawn>("Name", (Pawn p) => p.NameFullColored),
					new TableDataGetter<Pawn>("Age", (Pawn p) => p.ageTracker.AgeBiologicalYears),
					new TableDataGetter<Pawn>("Gender", (Pawn p) => p.gender.GetLabel()),
					new TableDataGetter<Pawn>("isAnimal", (Pawn p) => p.RaceProps.Animal),
					new TableDataGetter<Pawn>("isAnimalOfColony", (Pawn p) => p.RaceProps.Animal & p.Faction == Faction.OfPlayer),
					new TableDataGetter<Pawn>("IsColonist", (Pawn p) => p.IsColonist),
					new TableDataGetter<Pawn>("IsPrisonerOfColony", (Pawn p) => p.IsPrisonerOfColony),
					new TableDataGetter<Pawn>("IsSlaveOfColony", (Pawn p) => p.IsSlaveOfColony),
					new TableDataGetter<Pawn>("would fuck score", (Pawn p) => SexAppraiser.would_fuck(pawn, p).ToStringPercent()),
					new TableDataGetter<Pawn>("be fucked score", (Pawn p) => SexAppraiser.would_fuck(p, pawn).ToStringPercent()),
					new TableDataGetter<Pawn>("would rape", (Pawn p) => SexAppraiser.would_rape(pawn, p)),
					new TableDataGetter<Pawn>("be raped", (Pawn p) => SexAppraiser.would_rape(p, pawn))
				);
			}
		}

		static void Make_button(Rect Button, Pawn pawn, BodyPartRecord partBPR, string gen = "")
		{
			string ButtonLabel = "nothing here";
			bool resizable = RJWSettings.DevMode;
			List<Hediff> parts = new List<Hediff>();
			List<Hediff> parts1 = new List<Hediff>();
			Hediff hed = null;

			parts = Genital_Helper.get_PartsHediffList(pawn, partBPR);
			if (!parts.NullOrEmpty())
			{
				if (gen == "p")
					parts1.AddRange(parts.Where(x => Genital_Helper.is_penis(x)));
				if (gen == "v")
					parts1.AddRange(parts.Where(x => Genital_Helper.is_vagina(x)));
				if (gen != "")
					parts = parts1;

				if (!parts.NullOrEmpty())
					hed = parts.FirstOrDefault();

				if (hed != null)
				{
					//ButtonLabel = "Morph breasts";
					ButtonLabel = "Change " + hed.Label + " size";

					if (!resizable)
						foreach (var p in parts)
						{
							if (PartProps.TryGetProps(p, out List<string> l) && l.Contains("Resizable"))
								resizable = true;
						}

					if (resizable)
						if (hed is Hediff_PartBaseNatural)
						{
							Make_resize_button(Button, ButtonLabel, hed, parts);
						}
						else if (hed is Hediff_PartBaseArtifical)
						{
							Make_resize_button(Button, ButtonLabel, hed, parts);
						}
				}
			}
		}

		static void Make_resize_button(Rect Button, string ButtonLabel, Hediff hed, List<Hediff> parts)
		{
			if (Widgets.ButtonText(Button, ButtonLabel))
			{
				var stages = hed.def.stages;
				var opts = new List<FloatMenuOption>();

				foreach (var s in stages)
				{
					opts.Add(new FloatMenuOption(hed.def.label.CapitalizeFirst() + "(" + s.label.CapitalizeFirst() + ")", () =>
					{
						foreach (var p in parts)
						{
							if (RJWSettings.DevMode ||
							(PartProps.TryGetProps(p, out List<string> l) && l.Contains("Resizable")))
								p.Severity = s.minSeverity;
						}
					}, MenuOptionPriority.Default));
				}

				Find.WindowStack.Add(new FloatMenu(opts));
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			bool flag = false;
			soundClose = SoundDefOf.InfoCard_Close;
			closeOnClickedOutside = true;
			absorbInputAroundWindow = false;
			forcePause = true;
			preventCameraMotion = false;
			if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Escape))
			{
				flag = true;
				Event.current.Use();
			}
			Rect windowRect = inRect.ContractedBy(17f);
			Rect mainRect = new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height - 20f);
			Rect okRect = new Rect(inRect.width / 3, mainRect.yMax + 10f, inRect.width / 3f, 30f);
			SexualityCard(mainRect, pawn);
			if (Widgets.ButtonText(okRect, "CloseButton".Translate()) || flag)
			{
				Close();
			}
		}
	}
}