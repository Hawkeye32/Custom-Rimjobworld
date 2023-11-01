using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using System;
using Multiplayer.API;
using System.Linq;

namespace rjw
{
	public static class Nymph_Generator
	{
		/// <summary>
		/// Returns true if the given pawnGenerationRequest is for a nymph pawnKind.
		/// </summary>
		public static bool IsNymph(PawnGenerationRequest pawnGenerationRequest)
		{
			return pawnGenerationRequest.KindDef != null && pawnGenerationRequest.KindDef.defName.Contains("Nymph");
		}

		private static bool is_trait_conflicting_or_duplicate(Pawn pawn, Trait t)
		{
			foreach (var existing in pawn.story.traits.allTraits)
				if ((existing.def == t.def) || (t.def.ConflictsWith(existing)))
					return true;
			return false;
		}

		public static bool IsNymphBodyType(Pawn pawn)
		{
			return pawn.story.bodyType == BodyTypeDefOf.Female || pawn.story.bodyType == BodyTypeDefOf.Thin;
		}

		[SyncMethod]
		public static Gender RandomNymphGender()
		{
			//with males 100% its still 99%, coz im  to lazy to fix it
			//float rnd = Rand.Value;
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			float chance = RJWSettings.male_nymph_chance;

			Gender Pawngender = Rand.Chance(chance) ? Gender.Male: Gender.Female;
			//ModLog.Message(" setnymphsex: " + (rnd < chance) + " rnd:" + rnd + " chance:" + chance);
			//ModLog.Message(" setnymphsex: " + Pawngender);
			return Pawngender;
		}

		/// <summary>
		/// Replaces a pawn's randomly generated backstory and traits to turn it into propper a nymph
		/// </summary>
		[SyncMethod]
		public static void set_story(Pawn pawn)
		{
			//C# rjw story
			nymph_backstories.generate(pawn);

			//The mod More Trait Slots will adjust the max number of traits pawn can get, and therefore,
			//I need to collect pawns' traits and assign other_traits back to the pawn after adding the nymph_story traits.

			//backup generated + rjw traits
			Stack<Trait> other_traits = new Stack<Trait>();
			int numberOfTotalTraits = 0;
			if (!pawn.story.traits.allTraits.NullOrEmpty())
			{
				foreach (Trait t in pawn.story.traits.allTraits)
				{
					other_traits.Push(t);
					++numberOfTotalTraits;
				}
			}
			pawn.story.traits.allTraits.Clear();

			//xml nymph story forced traits
			var trait_count = 0;
			foreach (var t in pawn.story.Adulthood.forcedTraits)
			{
				pawn.story.traits.GainTrait(new Trait(t.def, t.degree));
				++trait_count;
			}

			//xml story + rjw backup + w/e generator generated backup
			while (trait_count < numberOfTotalTraits)
			{
				Trait t = other_traits.Pop();
				if (!is_trait_conflicting_or_duplicate(pawn, t))
					pawn.story.traits.GainTrait(t);
				++trait_count;
			}

			// add broken body hediff to broken nymph
			if (pawn.story.Adulthood.defName.Contains("broken"))
			{
				pawn.health.AddHediff(xxx.feelingBroken);
				(pawn.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken)).Severity = Rand.Range(0.4f, 1.0f);
			}

			//reset disabled skills/works
			pawn.skills.Notify_SkillDisablesChanged();
			pawn.Notify_DisabledWorkTypesChanged();
		}

		[SyncMethod]
		private static int sum_previous_gains(SkillDef def, Pawn_StoryTracker sto, Pawn_AgeTracker age)
		{
			int total_gain = 0;
			int gain;

			// Gains from backstories
			if (sto.Childhood.skillGains.TryGetValue(def, out gain))
				total_gain += gain;
			if (sto.Adulthood.skillGains.TryGetValue(def, out gain))
				total_gain += gain;

			// Gains from traits
			foreach (var trait in sto.traits.allTraits)
				if (trait.CurrentData.skillGains.TryGetValue(def, out gain))
					total_gain += gain;

			// Gains from age
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			var rgain = Rand.Value * (float)total_gain * 0.35f;
			var age_factor = Mathf.Clamp01((age.AgeBiologicalYearsFloat - 17.0f) / 10.0f); // Assume nymphs are 17~27
			total_gain += (int)(age_factor * rgain);

			return Mathf.Clamp(total_gain, 0, 20);
		}

		/// <summary>
		/// Set a nymph's initial skills & passions from backstory, traits, and age 
		/// </summary>
		[SyncMethod]
		public static void set_skills(Pawn pawn)
		{
			foreach (var skill_def in DefDatabase<SkillDef>.AllDefsListForReading)
			{
				var rec = pawn.skills.GetSkill(skill_def);
				if (!rec.TotallyDisabled)
				{
					//Rand.PopState();
					//Rand.PushState(RJW_Multiplayer.PredictableSeed());
					rec.Level = sum_previous_gains(skill_def, pawn.story, pawn.ageTracker);
					rec.xpSinceLastLevel = rec.XpRequiredForLevelUp * Rand.Range(0.10f, 0.90f);

					var pas_cha = nymph_backstories.get_passion_chances(pawn.story.Childhood, pawn.story.Adulthood, skill_def);
					var rv = Rand.Value;
					if (rv < pas_cha.major) rec.passion = Passion.Major;
					else if (rv < pas_cha.minor) rec.passion = Passion.Minor;
					else rec.passion = Passion.None;
				}
				else
					rec.passion = Passion.None;
			}
		}

		[SyncMethod]
		public static PawnKindDef GetFixedNymphPawnKindDef()
		{
			var def = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Nymph")).RandomElement();
			//var def = PawnKindDef.Named("Nymph");
			// This is 18 in the xml but something is overwriting it to 5.
			//def.minGenerationAge = 18;
			return def;
		}

		[SyncMethod]
		public static Pawn GenerateNymph(Faction faction = null)
		{
			// Most of the special properties of nymphs are in harmony patches to PawnGenerator.
			PawnGenerationRequest request = new PawnGenerationRequest(
				kind: GetFixedNymphPawnKindDef(),
				faction: faction,
				forceGenerateNewPawn: true,
				canGeneratePawnRelations: true,
				colonistRelationChanceFactor: 0.0f,
				relationWithExtraPawnChanceFactor: 0
				);

			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			Pawn pawn = PawnGenerator.GeneratePawn(request);
			//Log.Message(""+ pawn.Faction);
			//IntVec3 spawn_loc = CellFinder.RandomClosewalkCellNear(around_loc, map, 6);//RandomSpawnCellForPawnNear could be an alternative
			//GenSpawn.Spawn(pawn, spawn_loc, map);
			return pawn;
		}
	}
}