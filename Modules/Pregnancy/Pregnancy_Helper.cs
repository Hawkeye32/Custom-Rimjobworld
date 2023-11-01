using RimWorld;
using Verse;
using System;
using System.Linq;
using System.Collections.Generic;
using Multiplayer.API;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Extensions;
using static rjw.Hediff_BasePregnancy;

///RimWorldChildren pregnancy:
//using RimWorldChildren;

namespace rjw
{
	/// <summary>
	/// This handles pregnancy chosing between different types of pregnancy awailable to it
	/// 1a:RimWorldChildren pregnancy for humanlikes
	/// 1b:RJW pregnancy for humanlikes
	/// 2:RJW pregnancy for bestiality
	/// 3:RJW pregnancy for insects
	/// 4:RJW pregnancy for mechanoids
	/// </summary>

	public static class PregnancyHelper
	{
		// TODO: Move this and all HediifDef.Named calls to a proper DefOf class
		private static HediffDef RJW_IUD = HediffDef.Named("RJW_IUD");

		//called by aftersex (including rape, breed, etc)
		//called by mcevent

		//pawn - "father"; partner = mother

		//TODO: this needs rewrite to account receiver group sex (props?)
		public static void impregnate(SexProps props)
		{

			if (RJWSettings.DevMode) ModLog.Message("Rimjobworld::impregnate(" + props.sexType + "):: " + xxx.get_pawnname(props.pawn) + " + " + xxx.get_pawnname(props.partner) + ":");

			//"mech" pregnancy
			if (props.sexType == xxx.rjwSextype.MechImplant)
			{
				if (RJWPregnancySettings.mechanoid_pregnancy_enabled && xxx.is_mechanoid(props.pawn))
				{
					// removing old pregnancies
					var p = GetPregnancies(props.partner);
					if (!p.NullOrEmpty())
					{
						var i = p.Count;
						while (i > 0)
						{
							i -= 1;
							var h = GetPregnancies(props.partner);
							if (h[i] is Hediff_MechanoidPregnancy)
							{
								if (RJWSettings.DevMode) ModLog.Message(" already pregnant by mechanoid");
							}
							else if (h[i] is Hediff_BasePregnancy)
							{
								if (RJWSettings.DevMode) ModLog.Message(" removing rjw normal pregnancy");
								(h[i] as Hediff_BasePregnancy).Kill();
							}
							else
							{
								if (RJWSettings.DevMode) ModLog.Message(" removing vanilla or other mod pregnancy");
								props.partner.health.RemoveHediff(h[i]);
							}
						}
					}

					// new pregnancy
					if (RJWSettings.DevMode) ModLog.Message(" mechanoid pregnancy started");
					Hediff_MechanoidPregnancy hediff = Hediff_BasePregnancy.Create<Hediff_MechanoidPregnancy>(props.partner, props.pawn);
					return;

					/*
					// Not an actual pregnancy. This implants mechanoid tech into the target.
					//may lead to pregnancy
					//old "chip pregnancies", maybe integrate them somehow?
					//Rand.PopState();
					//Rand.PushState(RJW_Multiplayer.PredictableSeed());
					HediffDef_MechImplants egg = (from x in DefDatabase<HediffDef_MechImplants>.AllDefs	select x).RandomElement();
					if (egg != null)
					{
						if (RJWSettings.DevMode) Log.Message(" planting MechImplants:" + egg.ToString());
						PlantSomething(egg, partner, !Genital_Helper.has_vagina(partner), 1);
						return;
					}
					else
					{
						if (RJWSettings.DevMode) Log.Message(" no mech implant found");
					}*/
				}
				return;
			}

			//"ovi" pregnancy/egglaying
			var AnalOk = props.sexType == xxx.rjwSextype.Anal && RJWPregnancySettings.insect_anal_pregnancy_enabled;
			var OralOk = props.sexType == xxx.rjwSextype.Oral && RJWPregnancySettings.insect_oral_pregnancy_enabled;
			// Sextype can result in pregnancy.
			if (!(props.sexType == xxx.rjwSextype.Vaginal || props.sexType == xxx.rjwSextype.DoublePenetration ||
				AnalOk || OralOk))
				return;

			Pawn giver = props.pawn; // orgasmer
			Pawn receiver = props.partner;
			List<Hediff> pawnparts = giver.GetGenitalsList();
			List<Hediff> partnerparts = receiver.GetGenitalsList();
			var interaction = Modules.Interactions.Helpers.InteractionHelper.GetWithExtension(props.dictionaryKey);

			//ModLog.Message(" RaceImplantEggs()" + pawn.RaceImplantEggs());
			//"insect" pregnancy
			//straight, female (partner) recives egg insertion from other/sex starter (pawn)

			if (CouldBeEgging(props, giver, receiver, pawnparts, partnerparts))
			{
				//TODO: add widget toggle for bind all/neutral/hostile pawns
				if (!props.isReceiver)
					if (CanCocoon(giver))
						if (giver.HostileTo(receiver) || receiver.IsPrisonerOfColony || receiver.health.hediffSet.HasHediff(xxx.submitting))
							if (!receiver.health.hediffSet.HasHediff(HediffDef.Named("RJW_Cocoon")))
							{
								receiver.health.AddHediff(HediffDef.Named("RJW_Cocoon"));
							}

				DoEgg(props);
				return;
			}

			if (!(props.sexType == xxx.rjwSextype.Vaginal || props.sexType == xxx.rjwSextype.DoublePenetration))
				return;

			//"normal" and "beastial" pregnancy
			if (RJWSettings.DevMode) ModLog.Message(" 'normal' pregnancy checks");

			//interaction stuff if for handling futa/see who penetrates who in interaction
			if (!props.isReceiver &&
				interaction.DominantHasTag(GenitalTag.CanPenetrate) &&
				interaction.SubmissiveHasFamily(GenitalFamily.Vagina))
			{
				if (RJWSettings.DevMode) ModLog.Message(" impregnate - by initiator");
			}
			else if (props.isReceiver && props.isRevese &&
				interaction.DominantHasFamily(GenitalFamily.Vagina) &&
				interaction.SubmissiveHasTag(GenitalTag.CanPenetrate))
			{
				if (RJWSettings.DevMode) ModLog.Message(" impregnate - by receiver (reverse)");
			}
			else
			{
				if (RJWSettings.DevMode) ModLog.Message(" no valid interaction tags/family");
				return;
			}

			if (!Modules.Interactions.Helpers.PartHelper.FindParts(giver, GenitalTag.CanFertilize).Any())
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(giver) + " has no parts to Fertilize with");
				return;
			}
			if (!Modules.Interactions.Helpers.PartHelper.FindParts(receiver, GenitalTag.CanBeFertilized).Any())
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(receiver) + " has no parts to be Fertilized");
				return;
			}

			if (CanImpregnate(giver, receiver, props.sexType))
			{
				DoImpregnate(giver, receiver);
			}
		}

		private static bool CouldBeEgging(SexProps props, Pawn giver, Pawn reciever, List<Hediff> pawnparts, List<Hediff> partnerparts)
		{
			List<Hediff_InsectEgg> eggs = new();
			reciever.health.hediffSet.GetHediffs(ref eggs);
			//no ovipositor or fertilization possible
			if ((Genital_Helper.has_ovipositorF(giver, pawnparts) ||
				Genital_Helper.has_ovipositorM(giver, pawnparts) ||
				(Genital_Helper.has_penis_fertile(giver, pawnparts) && (giver.RaceImplantEggs() || eggs.Any()))
				) == false)
			{
				return false;
			}

			if ((props.sexType == xxx.rjwSextype.Vaginal || props.sexType == xxx.rjwSextype.DoublePenetration) &&
				Genital_Helper.has_vagina(reciever, partnerparts))
			{
				return true;
			}

			if ((props.sexType == xxx.rjwSextype.Anal || props.sexType == xxx.rjwSextype.DoublePenetration) &&
				Genital_Helper.has_anus(reciever) &&
				RJWPregnancySettings.insect_anal_pregnancy_enabled)
			{
				return true;
			}

			if (props.sexType == xxx.rjwSextype.Oral &&
				RJWPregnancySettings.insect_oral_pregnancy_enabled)
			{
				return true;
			}

			return false;
		}

		private static bool CanCocoon(Pawn pawn)
		{
			return xxx.is_insect(pawn);
		}

		/// <summary>
		/// Gets the pregnancy that is likely to complete first, when there are multiple
		/// pregnancies, such as from egg implants.
		/// </summary>
		/// <param name="pawn">The pawn to inspect.</param>
		/// <returns>The pregnancy likely to complete first.</returns>
		public static Hediff GetPregnancy(Pawn pawn)
		{
			var allPregnancies = GetPregnancies(pawn);
			if (allPregnancies.Count == 0) return null;

			allPregnancies.SortBy((preg) =>
			{
				(var ticksCompleted, var ticksToBirth) = GetProgressTicks(preg);
				return ticksToBirth - ticksCompleted;
			});
			return allPregnancies[0];
		}

		/// <summary>
		/// Gets the gestation progress of a pregnancy.
		/// </summary>
		/// <param name="hediff">The pregnancy hediff.</param>
		/// <returns>The current progress, a value between 0 and 1.</returns>
		public static float GetProgress(Hediff hediff) => hediff switch
		{
			Hediff_BasePregnancy rjwPreg => rjwPreg.GestationProgress,
			Hediff_Pregnant vanillaPreg => vanillaPreg.GestationProgress,
			_ => 0f
		};

		/// <summary>
		/// <para>Gets gestation ticks completed and the total needed for birth.  Both
		/// of these values are approximate, as things like malnutrition can affect
		/// the gestation rate.</para>
		/// </summary>
		/// <param name="hediff">The pregnancy hediff.</param>
		/// <returns>A tuple of ticks.</returns>
		public static (int ticksCompleted, int ticksToBirth) GetProgressTicks(Hediff hediff)
		{
			if (hediff is Hediff_BasePregnancy rjwPreg)
			{
				// RJW pregnancies maintain these numbers internally, so they don't
				// need to be inferred.
				var ticksToBirth = rjwPreg.p_end_tick - rjwPreg.p_start_tick;
				return (rjwPreg.ageTicks, Convert.ToInt32(ticksToBirth));
			}
			else
			{
				var progress = GetProgress(hediff);
				var raceProps = GetGestatingRace(hediff);
				var ticksToBirth = (int)(raceProps.gestationPeriodDays * GenDate.TicksPerDay);
				var ticksCompleted = (int)(progress * ticksToBirth);
				return (ticksCompleted, ticksToBirth);
			}
		}

		/// <summary>
		/// <para>Gets the race-props that are providing the gestation rate of a pregnancy.</para>
		/// <para>This may or may not be accurate, depending on how the pregnancy is ultimately
		/// implemented.</para>
		/// </summary>
		/// <param name="hediff">The pregnancy hediff.</param>
		/// <returns>The properties of the assumed gestating race.</returns>
		public static RaceProperties GetGestatingRace(Hediff hediff) => hediff switch
		{
			// Insect eggs use the implanter, if it is set.  Otherwise, the egg is probably
			// still inside the genetic mother, so we can use the carrier instead.
			Hediff_InsectEgg eggPreg when eggPreg.implanter is { } implanter =>
				implanter.RaceProps,
			// Some RJW pregnancies will store the gestating babies; the first pawn in
			// the list is treated as the gestation source.
			Hediff_BasePregnancy rjwPreg when rjwPreg.babies is { } babies && babies.Count > 0 =>
				babies[0].RaceProps,
			// For anything else, assume the carrying pawn is the source.
			_ => hediff.pawn.RaceProps
		};

		/// <summary>
		/// Gets all pregnancies of the pawn as a list.  This may include both RJW
		/// and vanilla pregnancies.
		/// </summary>
		/// <param name="pawn">The pawn to inspect.</param>
		/// <returns>A list of pregnancy hediffs.</returns>
		public static List<Hediff> GetPregnancies(Pawn pawn) =>
			pawn.health.hediffSet.hediffs
				.Where(x => x is Hediff_BasePregnancy or Hediff_Pregnant)
				.ToList();

		///<summary>Can get preg with above conditions, do impregnation.</summary>

		[SyncMethod]
		public static void DoEgg(SexProps props)
		{
			if (RJWPregnancySettings.insect_pregnancy_enabled)
			{
				if (RJWSettings.DevMode) ModLog.Message(" insect pregnancy");

				//female "insect" plant eggs
				//futa "insect" 50% plant eggs
				if ((Genital_Helper.has_ovipositorF(props.pawn) && !Genital_Helper.has_penis_fertile(props.pawn)) ||
					(Rand.Value > 0.5f && Genital_Helper.has_ovipositorF(props.pawn)))
					//penis eggs someday?
					//(Rand.Value > 0.5f && (Genital_Helper.has_ovipositorF(pawn) || Genital_Helper.has_penis_fertile(pawn) && pawn.RaceImplantEggs())))
				{
					float maxeggssize = (props.partner.BodySize / 5) * (xxx.has_quirk(props.partner, "Incubator") ? 2f : 1f)
						* (Genital_Helper.has_ovipositorF(props.partner)
						? 2f * RJWPregnancySettings.egg_pregnancy_ovipositor_capacity_factor
						: 1f);
					float eggedsize = 0;

					List<Hediff_InsectEgg> eggs = new();
					props.partner.health.hediffSet.GetHediffs(ref eggs);

					foreach (Hediff_InsectEgg egg in eggs)
					{
						if (egg.father != null)
							eggedsize += egg.father.RaceProps.baseBodySize / 5 * (egg.def as HediffDef_InsectEgg).eggsize * RJWPregnancySettings.egg_pregnancy_eggs_size;
						else
							eggedsize += egg.implanter.RaceProps.baseBodySize / 5 * (egg.def as HediffDef_InsectEgg).eggsize * RJWPregnancySettings.egg_pregnancy_eggs_size;
					}
					if (RJWSettings.DevMode) ModLog.Message(" determine " + xxx.get_pawnname(props.partner) + " size of eggs inside: " + eggedsize + ", max: " + maxeggssize);

					props.pawn.health.hediffSet.GetHediffs(ref eggs);
					BodyPartRecord partnerGenitals = null;

					if (props.sexType == xxx.rjwSextype.Anal)
						partnerGenitals = Genital_Helper.get_anusBPR(props.partner);
					else if (props.sexType == xxx.rjwSextype.Oral)
						partnerGenitals = Genital_Helper.get_stomachBPR(props.partner);
					else if (props.sexType == xxx.rjwSextype.DoublePenetration && Rand.Value > 0.5f && RJWPregnancySettings.insect_anal_pregnancy_enabled)
						partnerGenitals = Genital_Helper.get_anusBPR(props.partner);
					else
						partnerGenitals = Genital_Helper.get_genitalsBPR(props.partner);

					while (eggs.Any() && eggedsize < maxeggssize)
					{
						if (props.sexType == xxx.rjwSextype.Vaginal)
						{
							// removing old pregnancies
							var p = GetPregnancies(props.partner);
							if (!p.NullOrEmpty())
							{
								var i = p.Count;
								while (i > 0)
								{
									i -= 1;
									var h = GetPregnancies(props.partner);
									if (h[i] is Hediff_MechanoidPregnancy)
									{
										if (RJWSettings.DevMode) ModLog.Message(" egging - pregnant by mechanoid, skip");
									}
									else if (h[i] is Hediff_BasePregnancy)
									{
										if (RJWSettings.DevMode) ModLog.Message(" egging - removing rjw normal pregnancy");
										(h[i] as Hediff_BasePregnancy).Kill();
									}
									else
									{
										if (RJWSettings.DevMode) ModLog.Message(" egging - removing vanilla or other mod pregnancy");
										props.partner.health.RemoveHediff(h[i]);
									}
								}
							}
						}

						var egg = eggs.First();
						eggs.Remove(egg);

						props.pawn.health.RemoveHediff(egg);
						props.partner.health.AddHediff(egg, partnerGenitals);

						egg.Implanter(props.pawn);

						eggedsize += egg.eggssize * (egg.def as HediffDef_InsectEgg).eggsize * RJWPregnancySettings.egg_pregnancy_eggs_size;
					}
				}
				//male or futa fertilize eggs
				else if (!props.pawn.health.hediffSet.HasHediff(xxx.sterilized))
				{
					if (Genital_Helper.has_penis_fertile(props.pawn))
						if ((Genital_Helper.has_ovipositorF(props.pawn) || Genital_Helper.has_ovipositorM(props.pawn)) || (props.pawn.health.capacities.GetLevel(xxx.reproduction) > 0))
						{
							List<Hediff_InsectEgg> eggs = new();
							props.partner.health.hediffSet.GetHediffs(ref eggs);
							foreach (var egg in eggs)
								egg.Fertilize(props.pawn);
						}
				}
				return;
			}
		}

		[SyncMethod]
		public static void DoImpregnate(Pawn pawn, Pawn partner)
		{
			if (RJWSettings.DevMode) ModLog.Message(" Doimpregnate " + xxx.get_pawnname(pawn) + " is a father, " + xxx.get_pawnname(partner) + " is a mother");

			if (AndroidsCompatibility.IsAndroid(pawn) && !AndroidsCompatibility.AndroidPenisFertility(pawn))
			{
				if (RJWSettings.DevMode) ModLog.Message(" Father is android with no arcotech penis, abort");
				return;
			}
			if (AndroidsCompatibility.IsAndroid(partner) && !AndroidsCompatibility.AndroidVaginaFertility(partner))
			{
				if (RJWSettings.DevMode) ModLog.Message(" Mother is android with no arcotech vagina, abort");
				return;
			}

			// fertility check
			float basePregnancyChance = RJWPregnancySettings.humanlike_impregnation_chance / 100f;
			if (xxx.is_animal(partner))
				basePregnancyChance = RJWPregnancySettings.animal_impregnation_chance / 100f;

			// Interspecies modifier
			if (pawn.def.defName != partner.def.defName)
			{
				if (RJWPregnancySettings.complex_interspecies)
					basePregnancyChance *= SexUtility.BodySimilarity(pawn, partner);
				else
					basePregnancyChance *= RJWPregnancySettings.interspecies_impregnation_modifier;
			}
			else
			{
				//Egg fertility check
				CompEggLayer compEggLayer = partner.TryGetComp<CompEggLayer>();
				if (compEggLayer != null)
					basePregnancyChance = 1.0f;
			}

			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			float fertility = Math.Min(pawn.health.capacities.GetLevel(xxx.reproduction), partner.health.capacities.GetLevel(xxx.reproduction));

			if (partner.health.hediffSet.HasHediff(RJW_IUD))
			{
				fertility /= 99f;
			}

			float pregnancyChance = basePregnancyChance * fertility;

			if (!Rand.Chance(pregnancyChance))
			{
				if (RJWSettings.DevMode) ModLog.Message(" Impregnation failed. Chance: " + pregnancyChance.ToStringPercent());
				return;
			}
			if (RJWSettings.DevMode) ModLog.Message(" Impregnation succeeded. Chance: " + pregnancyChance.ToStringPercent());

			AddPregnancyHediff(partner, pawn);
		}


		///<summary>For checking normal pregnancy, should not for egg implantion or such.</summary>
		public static bool CanImpregnate(Pawn fucker, Pawn fucked, xxx.rjwSextype sexType = xxx.rjwSextype.Vaginal)
		{
			if (fucker == null || fucked == null) return false;

			if (RJWSettings.DevMode) ModLog.Message("Rimjobworld::CanImpregnate checks (" + sexType + "):: " + xxx.get_pawnname(fucker) + " + " + xxx.get_pawnname(fucked) + ":");

			if (sexType == xxx.rjwSextype.MechImplant && !RJWPregnancySettings.mechanoid_pregnancy_enabled)
			{
				if (RJWSettings.DevMode) ModLog.Message(" mechanoid 'pregnancy' disabled");
				return false;
			}
			if (!(sexType == xxx.rjwSextype.Vaginal || sexType == xxx.rjwSextype.DoublePenetration))
			{
				if (RJWSettings.DevMode) ModLog.Message(" sextype cannot result in pregnancy");
				return false;
			}

			if (AndroidsCompatibility.IsAndroid(fucker) && AndroidsCompatibility.IsAndroid(fucked))
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(fucked) + " androids cant breed/reproduce androids");
				return false;
			}

			if ((fucker.IsUnsexyRobot() || fucked.IsUnsexyRobot()) && !(sexType == xxx.rjwSextype.MechImplant))
			{
				if (RJWSettings.DevMode) ModLog.Message(" unsexy robot cant be pregnant");
				return false;
			}

			if (!fucker.RaceHasPregnancy())
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(fucked) + " filtered race that cant be pregnant");
				return false;
			}

			if (!fucked.RaceHasPregnancy())
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(fucker) + " filtered race that cant impregnate");
				return false;
			}


			if (fucked.IsPregnant())
			{
				if (RJWSettings.DevMode) ModLog.Message(" already pregnant.");
				return false;
			}

			List<Hediff_InsectEgg> eggs = new();
			fucked.health.hediffSet.GetHediffs(ref eggs);
			if ((from x in eggs where x.def == DefDatabase<HediffDef_InsectEgg>.GetNamed(x.def.defName) select x).Any())
			{
				if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(fucked) + " cant get pregnant while eggs inside");
				return false;
			}

			var pawnparts = fucker.GetGenitalsList();
			var partnerparts = fucked.GetGenitalsList();
			if (!(Genital_Helper.has_penis_fertile(fucker, pawnparts) && Genital_Helper.has_vagina(fucked, partnerparts)) && !(Genital_Helper.has_penis_fertile(fucked, partnerparts) && Genital_Helper.has_vagina(fucker, pawnparts)))
			{
				if (RJWSettings.DevMode) ModLog.Message(" missing genitals for impregnation");
				return false;
			}

			if (fucker.health.capacities.GetLevel(xxx.reproduction) <= 0 || fucked.health.capacities.GetLevel(xxx.reproduction) <= 0)
			{
				if (RJWSettings.DevMode) ModLog.Message(" one (or both) pawn(s) infertile");
				return false;
			}

			if (xxx.is_human(fucked) && xxx.is_human(fucker) && (RJWPregnancySettings.humanlike_impregnation_chance == 0 || !RJWPregnancySettings.humanlike_pregnancy_enabled))
			{
				if (RJWSettings.DevMode) ModLog.Message(" human pregnancy chance set to 0% or pregnancy disabled.");
				return false;
			}
			else if (((xxx.is_animal(fucker) && xxx.is_human(fucked)) || (xxx.is_human(fucker) && xxx.is_animal(fucked))) && !RJWPregnancySettings.bestial_pregnancy_enabled)
			{
				if (RJWSettings.DevMode) ModLog.Message(" bestiality pregnancy chance set to 0% or pregnancy disabled.");
				return false;
			}
			else if (xxx.is_animal(fucked) && xxx.is_animal(fucker) && (RJWPregnancySettings.animal_impregnation_chance == 0 || !RJWPregnancySettings.animal_pregnancy_enabled))
			{
				if (RJWSettings.DevMode) ModLog.Message(" animal-animal pregnancy chance set to 0% or pregnancy disabled.");
				return false;
			}
			else if (fucker.def.defName != fucked.def.defName && (RJWPregnancySettings.interspecies_impregnation_modifier <= 0.0f && !RJWPregnancySettings.complex_interspecies))
			{
				if (RJWSettings.DevMode) ModLog.Message(" interspecies pregnancy disabled.");
				return false;
			}

			if (!(fucked.RaceProps.gestationPeriodDays > 0))
			{
				CompEggLayer compEggLayer = fucked.TryGetComp<CompEggLayer>();
				if (compEggLayer == null)
				{
					if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(fucked) + " mother.RaceProps.gestationPeriodDays is " + fucked.RaceProps.gestationPeriodDays + " cant impregnate");
					return false;
				}
			}

			return true;
		}

		//Plant babies for human/bestiality pregnancy
		public static void AddPregnancyHediff(Pawn mother, Pawn father)
		{
			//human-human
			if (RJWPregnancySettings.humanlike_pregnancy_enabled && xxx.is_human(mother) && xxx.is_human(father))
			{
				CompEggLayer compEggLayer = mother.TryGetComp<CompEggLayer>();
				if (compEggLayer != null)
				{
					// fertilize eggs of humanlikes ?!
					if (!compEggLayer.FullyFertilized)
					{
						compEggLayer.Fertilize(father);
						//if (!mother.kindDef.defName.Contains("Chicken"))
						//	if (compEggLayer.Props.eggFertilizedDef.defName.Contains("RJW"))
					}
				}
				else
				{
					if (RJWPregnancySettings.UseVanillaPregnancy)
					{
						// vanilla 1.4 human pregnancy hediff
						ModLog.Message("preg hediffdefof PregnantHuman " + HediffDefOf.PregnantHuman);
						StartVanillaPregnancy(mother, father);
					}
					else
					{
						// use RJW hediff
						Hediff_BasePregnancy.Create<Hediff_HumanlikePregnancy>(mother, father);
					}
				}
			}
			//human-animal
			//maybe make separate option for human males vs female animals???
			else if (RJWPregnancySettings.bestial_pregnancy_enabled && (xxx.is_human(mother) ^ xxx.is_human(father)))
			{
				CompEggLayer compEggLayer = mother.TryGetComp<CompEggLayer>();
				if (compEggLayer != null)
				{
					if (!compEggLayer.FullyFertilized)
						compEggLayer.Fertilize(father);
				}
				else
				{
					DnaGivingParent dnaGivingParent = Hediff_BasePregnancy.SelectDnaGivingParent(mother, father);
					if (RJWPregnancySettings.UseVanillaPregnancy && xxx.is_human(mother) && dnaGivingParent == DnaGivingParent.Mother)
					{
						// vanilla 1.4 human pregnancy hediff
						ModLog.Message("preg hediffdefof PregnantHuman " + HediffDefOf.PregnantHuman);
						StartVanillaPregnancy(mother, father);
					}
					else
					{
						Hediff_BasePregnancy.Create<Hediff_BestialPregnancy>(mother, father, dnaGivingParent);
					}
				}
			}
			//animal-animal
			else if (xxx.is_animal(mother) && xxx.is_animal(father))
			{
				CompEggLayer compEggLayer = mother.TryGetComp<CompEggLayer>();
				if (compEggLayer != null)
				{
					// fertilize eggs of same species
					if (!compEggLayer.FullyFertilized)
						if (mother.kindDef == father.kindDef)
							compEggLayer.Fertilize(father);
				}
				else if (RJWPregnancySettings.animal_pregnancy_enabled)
				{
					Hediff_BasePregnancy.Create<Hediff_BestialPregnancy>(mother, father);
				}
			}
		}

		public static void StartVanillaPregnancy(Pawn mother, Pawn father, Pawn geneticMother = null, HediffDef def = null)
		{
			def ??= HediffDefOf.PregnantHuman;
			Hediff_Pregnant pregnancy = (Hediff_Pregnant) HediffMaker.MakeHediff(def, mother);
			pregnancy.SetParents(geneticMother, father, PregnancyUtility.GetInheritedGeneSet(father, mother));
			mother.health.AddHediff(pregnancy);
		}

		//Plant Insect eggs/mech chips/other preg mod hediff?
		public static bool PlantSomething(HediffDef def, Pawn target, bool isToAnal = false, int amount = 1)
		{
			if (def == null)
				return false;
			if (!isToAnal && !Genital_Helper.has_vagina(target))
				return false;
			if (isToAnal && !Genital_Helper.has_anus(target))
				return false;

			BodyPartRecord Part = (isToAnal) ? Genital_Helper.get_anusBPR(target) : Genital_Helper.get_genitalsBPR(target);
			if (Part != null || Part.parts.Count != 0)
			{
				//killoff normal preg
				if (!isToAnal)
				{
					if (RJWSettings.DevMode) ModLog.Message(" removing other pregnancies");
					var p = GetPregnancies(target);
					if (!p.NullOrEmpty())
					{
						foreach (var x in p)
						{
							if (x is Hediff_BasePregnancy)
							{
								var preg = x as Hediff_BasePregnancy;
								preg.Kill();
							}
							else
							{
								target.health.RemoveHediff(x);
							}
						}
					}
				}

				for (int i = 0; i < amount; i++)
				{

					if (RJWSettings.DevMode) ModLog.Message(" planting something weird");
					target.health.AddHediff(def, Part);
				}

				return true;
			}
			return false;
		}

		/// <summary>
		/// Remove CnP Pregnancy, that is added without passing rjw checks
		/// </summary>
		public static void cleanup_CnP(Pawn pawn)
		{
			//They do subpar probability checks and disrespect our settings, but I fail to just prevent their doloving override.
			//probably needs harmonypatch
			//So I remove the hediff if it is created and recreate it if needed in our handler later

			if (RJWSettings.DevMode) ModLog.Message(" cleanup_CnP after love check");

			var h = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("HumanPregnancy"));
			if (h != null && h.ageTicks < 100)
			{
				pawn.health.RemoveHediff(h);
				if (RJWSettings.DevMode) ModLog.Message(" removed hediff from " + xxx.get_pawnname(pawn));
			}
		}

		/// <summary>
		/// Remove Vanilla Pregnancy
		/// </summary>
		public static void cleanup_vanilla(Pawn pawn)
		{
			if (RJWSettings.DevMode) ModLog.Message(" cleanup_vanilla after love check");

			var h = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant);
			if (h != null && h.ageTicks < 100)
			{
				pawn.health.RemoveHediff(h);
				if (RJWSettings.DevMode) ModLog.Message(" removed hediff from " + xxx.get_pawnname(pawn));
			}
		}

		/// <summary>
		/// Below is stuff for RimWorldChildren
		/// its not used, we rely only on our own pregnancies
		/// </summary>

		/// <summary>
		/// This function tries to call Children and pregnancy utilities to see if that mod could handle the pregnancy
		/// </summary>
		/// <returns>true if cnp pregnancy will work, false if rjw one should be used instead</returns>
		public static bool CnP_WillAccept(Pawn mother)
		{
			//if (!xxx.RimWorldChildrenIsActive)
				return false;

			//return RimWorldChildren.ChildrenUtility.RaceUsesChildren(mother);
		}

		/// <summary>
		/// This funtcion tries to call Children and Pregnancy to create humanlike pregnancy implemented by the said mod.
		/// </summary>
		public static void CnP_DoPreg(Pawn mother, Pawn father)
		{
			if (!xxx.RimWorldChildrenIsActive)
				return;

			//RimWorldChildren.Hediff_HumanPregnancy.Create(mother, father);
		}

	}
}