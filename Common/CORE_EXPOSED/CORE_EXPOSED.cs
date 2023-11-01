/*
Core versions of scripts addapted for Greater Good
*/

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using rjw;


namespace rjw_CORE_EXPOSED
{
	// used in Vulnerability StatDef calculation
	public class SkillNeed_BaseBonus : SkillNeed
	{
		private float baseValue = 0.5f;

		private float bonusPerLevel = 0.05f;

		public override float ValueFor(Pawn pawn)
		{
			if (pawn.skills == null)
			{
				return 1f;
			}
			int level = pawn.skills.GetSkill(this.skill).Level;
			// remove melee bonus for pawns: downed, sleeping/resting/lying, wearing armbinder
			if (pawn.Downed || pawn.GetPosture() != PawnPosture.Standing || 
				(xxx.RjwExIsActive && pawn.health.hediffSet.HasHediff(xxx.RjwEx_Armbinder)))  // Simple verification on RJW side, but ideally this method should just be patched in RJW-EX to return valueAtLevel(0) if Armbinder active
				level = 0;
			return this.ValueAtLevel(level);
		}

		private float ValueAtLevel(int level)
		{
			return this.baseValue + this.bonusPerLevel * (float)level;
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string error in base.ConfigErrors())
			{
				yield return error;
			}
			for (int i = 1; i <= 20; i++)
			{
				float factor = this.ValueAtLevel(i);
				if (factor <= 0f)
				{
					yield return "SkillNeed yields factor < 0 at skill level " + i;
				}
			}
		}
	}

	public static class LovePartnerRelationUtility
	{
		public static float LovinMtbSinglePawnFactor(Pawn pawn)
		{
			float num = 1f;
			num /= 1f - pawn.health.hediffSet.PainTotal;
			float efficiency = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness);
			if (efficiency < 0.5f)
			{
				num /= efficiency * 2f;
			}

			if (!pawn.RaceProps.Humanlike)
			{
				return num * 4f;
			}
			if (RimWorld.LovePartnerRelationUtility.ExistingLovePartner(pawn) != null)
			{
				num *= 2f; //This is a factor which makes pawns with love partners less likely to do fappin/random raping/rapingCP/bestiality/necro.
			}
			else if (pawn.gender == Gender.Male)
			{
				num /= 1.25f; //This accounts for single men
			}
			return num / GenMath.FlatHill(0.0001f, 8f, 13f, 28f, 50f, 0.15f, pawn.ageTracker.AgeBiologicalYearsFloat);//this needs to be changed
		}
	}

	public static class MedicalRecipesUtility
	{
		public static bool IsCleanAndDroppable(Pawn pawn, BodyPartRecord part)
		{
			return IsClean(pawn, part) && part.def.spawnThingOnRemoved != null;
		}
		//TODO: add check if hediff are bad?
		public static bool IsClean(Pawn pawn, BodyPartRecord part)
		{
			//in vanilla Hediff = always bad?
			return !pawn.Dead && !(from x in pawn.health.hediffSet.hediffs
									where (x.Part == part)
									select x).Any<Hediff>();
		}

		public static bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
		{
			return pawn.Faction != billDoerFaction || HealthUtility.PartRemovalIntent(pawn, part) == BodyPartRemovalIntent.Harvest;
		}

		public static void RestorePartAndSpawnAllPreviousParts(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
		{
			SpawnNaturalPartIfClean(pawn, part, pos, map);
			SpawnThingsFromHediffs(pawn, part, pos, map);
			pawn.health.RestorePart(part, null, true);
		}

		// always false since rjw uses hediffs for genitals, thus its always dirty
		public static Thing SpawnNaturalPartIfClean(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
		{
			if (MedicalRecipesUtility.IsCleanAndDroppable(pawn, part))
			{
				//normal/rimworld parts
				return GenSpawn.Spawn(part.def.spawnThingOnRemoved, pos, map);
			}
			return null;
		}

		/// <summary>
		/// spawn rjw parts after operation
		/// </summary>
		/// <param name="pawn"></param>
		/// <param name="part"></param>
		/// <param name="pos"></param>
		/// <param name="map"></param>
		public static void SpawnThingsFromHediffs(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
		{
			if (!pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined).Contains(part))
			{
				return;
			}
			if (pawn.Dead)
			{
				return;
			}
			IEnumerable<Hediff> enumerable = from x in pawn.health.hediffSet.hediffs
											 where x.Part == part
											 select x;
			//RJW parts and other implants?
			foreach (Hediff current in enumerable)
			{
				if (current.def.spawnThingOnRemoved != null)
				{
					//Thing thing = GenSpawn.Spawn(current.def.spawnThingOnRemoved, pos, map);

					//spawn thing
					GenSpawn.Spawn(SexPartAdder.recipePartRemover(current), pos, map);
				}
			}
			//spawn sub parts? not sure why would it call itself but w/e
			for (int i = 0; i < part.parts.Count; i++)
			{
				MedicalRecipesUtility.SpawnThingsFromHediffs(pawn, part.parts[i], pos, map);
			}
		}
	}

	public class Recipe_RemoveBodyPart : Recipe_Surgery
	{
		public virtual bool blocked(Pawn p)
		{
			return false;
		}
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			if (!blocked(pawn))
			{
				IEnumerable<BodyPartRecord> parts = pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined);
				foreach (BodyPartRecord part in parts)
				{
					if (pawn.health.hediffSet.HasDirectlyAddedPartFor(part))
					{
						yield return part;
					}
					if (MedicalRecipesUtility.IsCleanAndDroppable(pawn, part))
					{
						yield return part;
					}
					if (part != pawn.RaceProps.body.corePart && part.def.canSuggestAmputation && pawn.health.hediffSet.hediffs.Any((Hediff d) => !(d is Hediff_Injury) && d.def.isBad && d.Visible && d.Part == part))
					{
						yield return part;
					}
				}
			}
		}

		private const int ViolationGoodwillImpact = -15;

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			bool flag = MedicalRecipesUtility.IsClean(pawn, part);
			bool flag2 = MedicalRecipesUtility.IsViolationOnPawn(pawn, part, Faction.OfPlayer);
			if (billDoer != null)
			{
				if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
				{
					return;
				}
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
				{
					billDoer,
					pawn
				});
				MedicalRecipesUtility.SpawnNaturalPartIfClean(pawn, part, billDoer.Position, billDoer.Map);
				MedicalRecipesUtility.SpawnThingsFromHediffs(pawn, part, billDoer.Position, billDoer.Map);
			}
			DamageDef surgicalCut = DamageDefOf.SurgicalCut;
			float amount = 99999f;
			float armorPenetration = 999f;
			pawn.TakeDamage(new DamageInfo(surgicalCut, amount, armorPenetration, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
			if (flag)
			{
				if (pawn.Dead)
				{
					ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, billDoer, PawnExecutionKind.OrganHarvesting);
				}
				ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn, billDoer);
			}
			if (flag2 && pawn.Faction != null && billDoer != null && billDoer.Faction != null)
			{
				Faction faction = pawn.Faction;
				Faction faction2 = billDoer.Faction;
				HistoryEventDef reason = DefDatabase<HistoryEventDef>.GetNamedSilentFail("GoodwillChangedReason_RemovedBodyPart");
				//string reason = "GoodwillChangedReason_RemovedBodyPart".Translate(part.LabelShort);
				GlobalTargetInfo? lookTarget = pawn;
				faction.TryAffectGoodwillWith(faction2, ViolationGoodwillImpact, true, true, reason, lookTarget);
			}
		}

		public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part)
		{
			if (pawn.RaceProps.IsMechanoid || pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(part))
			{
				return RecipeDefOf.RemoveBodyPart.LabelCap;
			}
			BodyPartRemovalIntent bodyPartRemovalIntent = HealthUtility.PartRemovalIntent(pawn, part);
			if (bodyPartRemovalIntent == BodyPartRemovalIntent.Harvest)
			{
				return "Harvest".Translate();
			}
			if (bodyPartRemovalIntent != BodyPartRemovalIntent.Amputate)
			{
				throw new InvalidOperationException();
			}
			if (part.depth == BodyPartDepth.Inside)
			{
				return "RemoveOrgan".Translate();
			}
			return "Amputate".Translate();
		}
	}

	public class Recipe_InstallOrReplaceBodyPart : Recipe_Surgery
	{
		public virtual bool blocked(Pawn p)
		{
			return false;
		}

		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			if (!blocked(pawn))
			{
				//ModLog.Message("Recipe_InstallOrReplaceBodyPart Recipe: " + recipe.defName);

				// For all parts listed in the recipe
				for (int i = 0; i < recipe.appliedOnFixedBodyParts.Count; i++)
				{
					// Grab the recipe part
					BodyPartDef part = recipe.appliedOnFixedBodyParts[i];

					// For all parts on the pawn
					List<BodyPartRecord> bpList = pawn.RaceProps.body.AllParts;
					for (int j = 0; j < bpList.Count; j++)
					{
						// Grab the pawn part
						BodyPartRecord record = bpList[j];

						// If the recipe part and pawn part match
						if (record.def == part)
						{
							// Grab all pawn hediffs for natural and artificial parts that are on the current pawn body part
							IEnumerable<Hediff> diffs = from x in pawn.health.hediffSet.hediffs
														where x.Part == record && (x is Hediff_PartBaseNatural || x is Hediff_PartBaseArtifical)
														select x;

							//ModLog.Message("Recipe_InstallOrReplaceBodyPart GetPartsToApplyOn: Pawn=" + pawn.Name + " Part=" + record.Label + ", diffs.Count=" + diffs.Count<Hediff>() + ", First Diff=" + diffs.First<Hediff>().def.defName + " recipe.addsHediff=" + recipe.addsHediff.defName);

							// If the pawn doesn't have exactly 1 diff for that part OR the first diff found is not the one being added by the recipe
							//if (diffs.Count<Hediff>() != 1 || diffs.First<Hediff>().def != recipe.addsHediff)
							{
								//ModLog.Message("Diff Matched.  Recipe parent=" + record.parent?.Label);

								// If this is the base body OR the pawn has the parent body part of the recipe part
								if (record.parent == null || pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Contains(record.parent))
								{
									//ModLog.Message("Parent Found.  PartOrAnyAncestorHasDirectlyAddedParts=" + pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) + ", HasDirectlyAddedPartFor=" + pawn.health.hediffSet.HasDirectlyAddedPartFor(record));

									if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) || pawn.health.hediffSet.HasDirectlyAddedPartFor(record))
									{
										//ModLog.Message("Recipe: " + recipe.defName + "Applies to: " + record.Label + " of " + pawn.Name);
										yield return record;
									}
								}
							}
						}
					}
				}
			}
		}

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (billDoer != null)
			{
				if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
				{
					DamageDef surgicalCut = DamageDefOf.SurgicalCut;
					float amount = 99999f;
					float armorPenetration = 999f;
					pawn.TakeDamage(new DamageInfo(surgicalCut, amount, armorPenetration, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
					return;
				}
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
				{
					billDoer,
					pawn
				});
				MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, billDoer.Position, billDoer.Map);
			}
			//pawn.health.AddHediff(recipe.addsHediff, part, null, null);

			pawn.health.AddHediff(SexPartAdder.recipePartAdder(recipe, pawn, part, ingredients), part, null, null);
		}
	}

	public class Recipe_AddBodyPart : Recipe_Surgery
	{
		public virtual bool blocked(Pawn p)
		{
			return false;
		}
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			if (!blocked(pawn))
			{
				for (int i = 0; i < recipe.appliedOnFixedBodyParts.Count; i++)
				{
					BodyPartDef part = recipe.appliedOnFixedBodyParts[i];
					List<BodyPartRecord> bpList = pawn.RaceProps.body.AllParts;
					for (int j = 0; j < bpList.Count; j++)
					{
						BodyPartRecord record = bpList[j];
						if (record.def == part)
						{
							IEnumerable<Hediff> diffs = from x in pawn.health.hediffSet.hediffs
														where x.Part == record && (x is Hediff_PartBaseNatural || x is Hediff_PartBaseArtifical)
														select x;

							// If the pawn doesn't have exactly 1 diff for that part OR the first diff found is not the one being added by the recipe
							//if (diffs.Count<Hediff>() != 1 || diffs.First<Hediff>().def != recipe.addsHediff)
							{
								if (record.parent == null || pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Contains(record.parent))
								{
									if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) || pawn.health.hediffSet.HasDirectlyAddedPartFor(record))
									{
										yield return record;
									}
								}
							}
						}
					}
				}
			}
		}

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (billDoer != null)
			{
				if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
				{
					DamageDef surgicalCut = DamageDefOf.SurgicalCut;
					float amount = 99999f;
					float armorPenetration = 999f;
					pawn.TakeDamage(new DamageInfo(surgicalCut, amount, armorPenetration, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
					return;
				}
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
				{
					billDoer,
					pawn
				});
				//pawn.health.AddHediff(recipe.addsHediff, part, null, null);

				pawn.health.AddHediff(SexPartAdder.recipePartAdder(recipe, pawn, part, ingredients), part, null, null);
			}
		}
	}
	public class Recipe_RemovePart : Recipe_Surgery
	{
		public virtual bool blocked(Pawn p)
		{
			return false;
		}
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			if (!blocked(pawn))
			{
				for (int i = 0; i < recipe.appliedOnFixedBodyParts.Count; i++)
				{
					BodyPartDef part = recipe.appliedOnFixedBodyParts[i];
					List<BodyPartRecord> bpList = pawn.RaceProps.body.AllParts;
					for (int j = 0; j < bpList.Count; j++)
					{
						BodyPartRecord record = bpList[j];
						if (record.def == part)
						{
							IEnumerable<Hediff> diffs = from x in pawn.health.hediffSet.hediffs
														where x.Part == record && (x is Hediff_PartBaseNatural || x is Hediff_PartBaseArtifical)
														select x;

							// If the pawn doesn't have exactly 1 diff for that part OR the first diff found is not the one being added by the recipe
							//if (diffs.Count<Hediff>() != 1 || diffs.First<Hediff>().def != recipe.addsHediff)
							{
								if (record.parent == null || pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Contains(record.parent))
								{
									if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) || pawn.health.hediffSet.HasDirectlyAddedPartFor(record))
									{
										yield return record;
									}
								}
							}
						}
					}
				}
			}
		}

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (billDoer != null)
			{
				if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
				{
					DamageDef surgicalCut = DamageDefOf.SurgicalCut;
					float amount = 99999f;
					float armorPenetration = 999f;
					pawn.TakeDamage(new DamageInfo(surgicalCut, amount, armorPenetration, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
					return;
				}
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
				{
					billDoer,
					pawn
				});
				//pawn.health.AddHediff(recipe.addsHediff, part, null, null);

				pawn.health.AddHediff(SexPartAdder.recipePartAdder(recipe, pawn, part, ingredients), part, null, null);
			}
		}
	}
}