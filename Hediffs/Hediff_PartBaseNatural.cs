using System.Linq;
using Verse;
using RimWorld;
using System.Text;
using Multiplayer.API;
using UnityEngine;
using System.Collections.Generic;

namespace rjw
{
	public class Hediff_PartBaseNatural : HediffWithComps
	{
		public override bool ShouldRemove => false;

		public bool discovered = false;

		// Used for ovipositors.
		public int nextEggTick = -1;
		public float lastsize = -1;
		public float oviTick = 0;
		public float dayTick = 0;

		public override void ExposeData()
		{
			base.ExposeData();

			Scribe_Values.Look(ref this.nextEggTick, "nextEggTick");
			Scribe_Values.Look(ref this.lastsize, "lastsize");
			Scribe_Values.Look(ref this.oviTick, "oviTick");
			Scribe_Values.Look(ref this.dayTick, "dayTick");
			//			Scribe_Values.Look(ref this.produceEggs, "produceEggs");
			//			Scribe_Defs.Look(ref this.pawnKindDefOverride, "pawnKindDefOverride");
			//			Scribe_Values.Look(ref this.genitalType, "genitalType");
		}


		public override string LabelBase
		{
			get
			{
				/*
				 * make patch to make/save capmods?
				if (CapMods.Count < 5)
				{
					PawnCapacityModifier pawnCapacityModifier = new PawnCapacityModifier();
					pawnCapacityModifier.capacity = PawnCapacityDefOf.Moving;
					pawnCapacityModifier.offset += 0.5f;
					CapMods.Add(pawnCapacityModifier);
				}
				*/

				//name/kind
				return this.def.label;
			}
		}

		//public override string LabelInBrackets
		//{
		//	get
		//	{
		//		string size = "on fire!";
		//		size = (this.comps.Find(x => x is CompHediffBodyPart) as CompHediffBodyPart).Size;
		//		return size;

		//		//vanilla
		//		//return (this.CurStage != null && !this.CurStage.label.NullOrEmpty()) ? this.CurStage.label : null;
		//	}
		//}

		//overrides comps
		//public override string TipStringExtra
		//{
		//	get
		//	{
		//		StringBuilder stringBuilder = new StringBuilder();
		//		foreach (StatDrawEntry current in HediffStatsUtility.SpecialDisplayStats(this.CurStage, this))
		//		{
		//			if (current.ShouldDisplay)
		//			{
		//				stringBuilder.AppendLine(current.LabelCap + ": " + current.ValueString);
		//			}
		//		}
		//		//stringBuilder.AppendLine("Size: " + this.TryGetComp<CompHediffBodyPart>.Size);
		//		//stringBuilder.AppendLine("1");// size?
		//		//stringBuilder.AppendLine("2");// erm something?
		//		return stringBuilder.ToString();
		//	}
		//}

		/// <summary>
		/// stack hediff in health tab?
		/// </summary>
		public override int UIGroupKey
		{
			get
			{
				return loadID;
			}
		}

		/// <summary>
		/// do not merge same rjw parts into one
		/// </summary>
		public override bool TryMergeWith(Hediff other)
		{
			return false;
		}

		/// <summary>
		/// show rjw parts in health tab or not
		/// </summary>
		public override bool Visible
		{
			get
			{
				if (RJWSettings.ShowRjwParts == RJWSettings.ShowParts.Hide)
				{
					discovered = false;
				}
				else if (!discovered)
				{
					if (RJWSettings.ShowRjwParts != RJWSettings.ShowParts.Hide)
					{
						discovered = true;
						return discovered;
					}

					//show at game start
					if (Current.ProgramState != ProgramState.Playing && Prefs.DevMode)
						return true;

					//show for hero
					if (pawn.IsDesignatedHero() && pawn.IsHeroOwner())
					{
						discovered = true;
						return discovered;
					}

					//show if no clothes
					if (pawn.apparel != null)// animals?
					{
						bool hasPants;
						bool hasShirt;
						pawn.apparel.HasBasicApparel(out hasPants, out hasShirt);// naked?

						if (!hasPants)
						{
							bool flag3 = false;
							foreach (BodyPartRecord current in this.pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null))
							{
								if (current.IsInGroup(BodyPartGroupDefOf.Legs))
								{
									flag3 = true;
									break;
								}
							}
							if (!flag3)
							{
								hasPants = true;
							}
						}

						if (this.def.defName.ToLower().Contains("breast") || this.def.defName.ToLower().Contains("chest"))
							discovered = !hasShirt;
						else
							discovered = !hasPants;

					}
				}

				return discovered;
			}
		}

		/// <summary>
		/// egg production ticks, part growning for childrens
		/// </summary>
		public override void Tick()
		{
			var thisTick = Find.TickManager.TicksGame;

			//change size for growing pawn/child
			if ((thisTick - dayTick) >= GenDate.TicksPerDay)
			{
				//change pawn parts sizes for kids(?)
				//update size if pawn bodysize changed from last check
				if (pawn.BodySize != lastsize)
				{
					var t = this.TryGetComp<CompHediffBodyPart>();
					if (t != null)
					{
						t.updatesize(); // update/reset part size
						lastsize = pawn.BodySize;
					}
				}
				dayTick = thisTick;
			}

			if ((thisTick - oviTick) >= 10000)
			{
				var partBase = def as HediffDef_PartBase;
				if (partBase != null)
				{
					if (partBase.produceEggs)
					{
						//Log.Message("genital ovi tick");
						//Log.Message("pawn " + pawn.Label);
						//Log.Message("id " + pawn.ThingID);
						var IsPlayerFaction = pawn.Faction?.IsPlayer ?? false; //colonists/animals
						var IsPlayerHome = pawn.Map?.IsPlayerHome ?? false;

						if (IsPlayerHome || IsPlayerFaction || pawn.IsPrisonerOfColony)
						{
							//Log.Message("-2 ");
							if (pawn.health.capacities.GetLevel(PawnCapacityDefOf.Moving) <= 0.5)
							{
								return;
							}

							//Log.Message("-3 ");
							if (thisTick >= nextEggTick)
							{
								float maxEggsSize = (pawn.BodySize / 5) * (xxx.has_quirk(pawn, "Incubator") ? 2f : 1f)
									* (Genital_Helper.has_ovipositorF(pawn)
									? 2f * RJWPregnancySettings.egg_pregnancy_ovipositor_capacity_factor
									: 0.5f);
								float eggedsize = 0;
								//Log.Message("-4 ");

								List<Hediff_InsectEgg> ownEggs = new();
								pawn.health.hediffSet.GetHediffs(ref ownEggs);
								foreach (var ownEgg in ownEggs)
								{
									if (ownEgg.father != null)
										eggedsize += ownEgg.father.RaceProps.baseBodySize / 5;
									else if (ownEgg.implanter != null)
										eggedsize += ownEgg.implanter.RaceProps.baseBodySize / 5;
									else //something fucked up, father/implanter null / immortal pawn reborn /egg is broken?
										eggedsize += ownEgg.eggssize;
								}

								//Log.Message("-5 ");
								if (RJWSettings.DevMode) ModLog.Message($"{xxx.get_pawnname(pawn)} filled with {eggedsize} out of max capacity of {maxEggsSize} eggs.");
								if (eggedsize < maxEggsSize)
								{
									HediffDef_InsectEgg egg = null;
									string defname = "";

									//Log.Message("-6 ");
									while (egg == null)
									{
										if (defname == "")
										{
											if (RJWSettings.DevMode) ModLog.Message(" trying to find " + pawn.kindDef.defName + " egg");
											defname = pawn.kindDef.defName;
										}
										else
										{

											if (RJWSettings.DevMode) ModLog.Message(" no " + defname + " egg found, defaulting to Unknown egg");
											defname = "Unknown";
										}

										//Rand.PopState();
										//Rand.PushState(RJW_Multiplayer.PredictableSeed());
										//Log.Message("-7 ");
										egg = TryGetEgg(defname);
									}

									//Log.Message("-8 ");
									if (RJWSettings.DevMode) ModLog.Message("I choose you " + egg + "!");

									//Log.Message("-9 ");
									var genitals = Genital_Helper.get_genitalsBPR(pawn);
									if (genitals != null)
									{
										// Calculator amount of eggs added
										int neweggs = 1;
										int mineggs = 1;
										int maxeggs = 1;
										mineggs = Genital_Helper.min_EggsProduced(pawn);
										maxeggs = Genital_Helper.max_EggsProduced(pawn);
										neweggs = (int)(mineggs + (Rand.Value * (maxeggs - mineggs)) + 0.5);
										if (RJWSettings.DevMode)
										{
											ModLog.Message("Min Egg amount : " + mineggs);
											ModLog.Message("Max Egg amount : " + maxeggs);
											ModLog.Message("Actual Egg amount : " + neweggs);
										}

										//Log.Message("-10 ");
										//Log.Message("genitals bpr: " + genitals);
										for (int i = 0; i < neweggs; i++)
										{
											var addedEgg = pawn.health.AddHediff(egg, genitals) as Hediff_InsectEgg;

											// ModLog.Message(" i = " + i);
											addedEgg?.Implanter(pawn);
										}
									}
									//Log.Message("-12 ");
								}

								// Reset for next egg.
								nextEggTick = TryGetnextEggTick();
							}
						}
					}
				}
				oviTick = thisTick;
			}
		}

		[SyncMethod]
		private int TryGetnextEggTick()
		{
			var partBase = def as HediffDef_PartBase;
			return Find.TickManager.TicksGame + Rand.Range(partBase.minEggTick, partBase.maxEggTick);
		}

		[SyncMethod]
		private HediffDef_InsectEgg TryGetEgg(string defname)
		{
			return (from x in DefDatabase<HediffDef_InsectEgg>.AllDefs where x.IsParent(defname) select x).RandomElement();
		}
	}
}
