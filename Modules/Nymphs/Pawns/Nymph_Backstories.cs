using System.Collections.Generic;
using RimWorld;
using Verse;
using Multiplayer.API;
using System.Linq;
using System.Linq.Expressions;
using HarmonyLib;

namespace rjw
{
	public struct nymph_passion_chances
	{
		public float major;
		public float minor;

		public nymph_passion_chances(float maj, float min)
		{
			major = maj;
			minor = min;
		}
	}

	public static class nymph_backstories
	{

		public static nymph_passion_chances get_passion_chances(BackstoryDef child_bs, BackstoryDef adult_bs, SkillDef skill_def)
		{
			var maj = 0.0f;
			var min = 0.0f;

			if (adult_bs.defName.Contains("feisty"))
			{
				if (skill_def == SkillDefOf.Melee) { maj = 0.50f; min = 1.00f; }
				else if (skill_def == SkillDefOf.Shooting) { maj = 0.25f; min = 0.75f; }
				else if (skill_def == SkillDefOf.Social) { maj = 0.10f; min = 0.67f; }
			}
			else if (adult_bs.defName.Contains("curious"))
			{
				if (skill_def == SkillDefOf.Construction) { maj = 0.15f; min = 0.40f; }
				else if (skill_def == SkillDefOf.Crafting) { maj = 0.50f; min = 1.00f; }
				else if (skill_def == SkillDefOf.Social) { maj = 0.20f; min = 1.00f; }
			}
			else if (adult_bs.defName.Contains("tender"))
			{
				if (skill_def == SkillDefOf.Medicine) { maj = 0.20f; min = 0.60f; }
				else if (skill_def == SkillDefOf.Social) { maj = 0.50f; min = 1.00f; }
			}
			else if (adult_bs.defName.Contains("chatty"))
			{
				if (skill_def == SkillDefOf.Social) { maj = 1.00f; min = 1.00f; }
			}
			else if (adult_bs.defName.Contains("broken"))
			{
				if (skill_def == SkillDefOf.Artistic) { maj = 0.50f; min = 1.00f; }
				else if (skill_def == SkillDefOf.Social) { maj = 0.00f; min = 0.33f; }
			}
			else if (adult_bs.defName.Contains("homekeeper"))
			{
				if (skill_def == SkillDefOf.Cooking) { maj = 0.50f; min = 1.00f; }
				else if (skill_def == SkillDefOf.Social) { maj = 0.00f; min = 0.33f; }
			}

			return new nymph_passion_chances(maj, min);
		}

		// Randomly chooses backstories and traits for a nymph
		[SyncMethod]
		public static void generate(Pawn pawn)
		{
			var tr = pawn.story;
			//reset stories
			tr.Childhood = DefDatabase<BackstoryDef>.AllDefsListForReading.Where(x => x.slot == BackstorySlot.Childhood && x.spawnCategories.Contains("rjw_nymphsCategory")).RandomElement();
			tr.Adulthood = DefDatabase<BackstoryDef>.AllDefsListForReading.Where(x => x.slot == BackstorySlot.Adulthood && x.spawnCategories.Contains("rjw_nymphsCategory")).RandomElement();

			pawn.story.traits.allTraits.AddDistinct(new Trait(xxx.nymphomaniac, 0));

			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			var beauty = 0;
			var rv2 = Rand.Value;

			if (tr.Adulthood.defName.Contains("feisty"))
			{
				beauty = Rand.RangeInclusive(0, 2);
				if (rv2 < 0.33)
					pawn.story.traits.allTraits.AddDistinct(new Trait(TraitDefOf.Brawler));
				else if (rv2 < 0.67)
					pawn.story.traits.allTraits.AddDistinct(new Trait(TraitDefOf.Bloodlust));
				else
					pawn.story.traits.allTraits.AddDistinct(new Trait(xxx.rapist));
			}
			else if (tr.Adulthood.defName.Contains("curious"))
			{
				beauty = Rand.RangeInclusive(0, 2);
				if (rv2 < 0.33)
					pawn.story.traits.allTraits.AddDistinct(new Trait(TraitDefOf.Transhumanist));
			}
			else if (tr.Adulthood.defName.Contains("tender"))
			{
				beauty = Rand.RangeInclusive(1, 2);
				if (rv2 < 0.50)
					pawn.story.traits.allTraits.AddDistinct(new Trait(TraitDefOf.Kind));
			}
			else if (tr.Adulthood.defName.Contains("chatty"))
			{
				beauty = 2;
				if (rv2 < 0.33)
					pawn.story.traits.allTraits.AddDistinct(new Trait(TraitDefOf.Greedy));
			}
			else if (tr.Adulthood.defName.Contains("broken"))
			{
				beauty = Rand.RangeInclusive(0, 2);
				if (rv2 < 0.33)
					pawn.story.traits.allTraits.AddDistinct(new Trait(TraitDefOf.DrugDesire, 1));
				else if (rv2 < 0.67)
					pawn.story.traits.allTraits.AddDistinct(new Trait(TraitDefOf.DrugDesire, 2));
			}
			else if (tr.Adulthood.defName.Contains("homekeeper"))
			{
				beauty = Rand.RangeInclusive(1, 2);
				if (rv2 < 0.33)
					pawn.story.traits.allTraits.AddDistinct(new Trait(TraitDefOf.Kind));
			}

			if (beauty > 0)
				pawn.story.traits.allTraits.AddDistinct(new Trait(TraitDefOf.Beauty, beauty, false));
		}
	}
}