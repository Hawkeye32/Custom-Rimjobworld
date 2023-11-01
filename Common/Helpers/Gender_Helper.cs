using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Linq;
using System;

//This one is helper lib for handling all the trans surgery.
namespace rjw
{
	public static class GenderHelper
	{
		public enum Sex { male, female, trap, futa, none }//there is Verse,Gender but it is shit

		//These would probably be better packed in some enumerable structure, so that functions below weren't if-trees, but I don't know C#, sry.
		public static HediffDef was_boy = HediffDef.Named("hediff_was_boy");
		public static HediffDef was_girl = HediffDef.Named("hediff_was_girl");
		public static HediffDef was_futa = HediffDef.Named("hediff_was_futa");
		public static HediffDef was_trap = HediffDef.Named("hediff_was_trap");

		static List<HediffDef> old_sex_list = new List<HediffDef> { was_boy, was_girl, was_futa, was_trap };
		static Dictionary<Sex, HediffDef> sex_to_old_sex = new Dictionary<Sex, HediffDef>() {
			{Sex.male, was_boy },{Sex.female, was_girl},{Sex.trap, was_trap},{Sex.futa, was_futa}
		};
		static Dictionary<HediffDef, Sex> old_sex_to_sex = sex_to_old_sex.ToDictionary(x => x.Value, x => x.Key);

		public static HediffDef m2t = HediffDef.Named("hediff_male2trap");
		public static HediffDef m2f = HediffDef.Named("hediff_male2female");
		public static HediffDef m2h = HediffDef.Named("hediff_male2futa");

		public static HediffDef f2t = HediffDef.Named("hediff_female2trap");
		public static HediffDef f2m = HediffDef.Named("hediff_female2male");
		public static HediffDef f2h = HediffDef.Named("hediff_female2futa");

		public static HediffDef h2t = HediffDef.Named("hediff_futa2trap");
		public static HediffDef h2m = HediffDef.Named("hediff_futa2male");
		public static HediffDef h2f = HediffDef.Named("hediff_futa2female");

		public static HediffDef t2h = HediffDef.Named("hediff_trap2futa");
		public static HediffDef t2m = HediffDef.Named("hediff_trap2male");
		public static HediffDef t2f = HediffDef.Named("hediff_trap2female");

		static List<HediffDef> SexChangeThoughts = new List<HediffDef> { m2t, m2f, m2h, f2t, f2m, f2h, h2t, h2m, h2f };

		public static Sex GetSex(Pawn pawn)
		{
			var parts = pawn.GetGenitalsList();

			bool has_vagina = Genital_Helper.has_vagina(pawn, parts);
			bool has_penis = Genital_Helper.has_male_bits(pawn, parts);
			bool has_breasts = Genital_Helper.has_breasts(pawn);
			bool has_male_breasts = Genital_Helper.has_male_breasts(pawn);
			//BodyType? bt = pawn.story?.bodyType;
			//if (bt != null)
			//	bt = BodyType.Undefined;

			Sex res;
			if (has_vagina && !has_penis)
				res = Sex.female;
			else if (has_vagina && has_penis)
				res = Sex.futa;
			else if (has_penis && has_breasts && !has_male_breasts)
				res = Sex.trap;
			else if (has_penis) //probably should change this later
				res = Sex.male;
			else if (pawn.gender == Gender.Male)
				res = Sex.male;
			else if (pawn.gender == Gender.Female)
				res = Sex.female;
			else
				res = Sex.none;
			return res;
		}

		/// <summary>
		/// Checks to see if two sexes can do hetero stuff.
		/// </summary>
		public static bool CanBeHetero(Sex pawnSex, Sex partnerSex) => (pawnSex, partnerSex) switch
		{
			// Get un-gendered out of the possibilities.
			(Sex.none, _) or (_, Sex.none) => false,
			// Futa can be straight towards anything remaining.
			(Sex.futa, _) or (_, Sex.futa) => true,
			// Female vs male.
			(Sex.female, Sex.male or Sex.trap) => true,
			// Male vs female.
			(Sex.male or Sex.trap, Sex.female) => true,
			// Otherwise, nah.
			_ => false
		};

		/// <summary>
		/// <para>Checks to see if two pawns can do hetero stuff.</para>
		/// <para>If you are also going to check `CanBeHomo`, get the sexes first.</para>
		/// </summary>
		public static bool CanBeHetero(Pawn pawn, Pawn partner) =>
			CanBeHetero(GetSex(pawn), GetSex(partner));

		/// <summary>
		/// Checks to see if two sexes can do homo stuff.
		/// </summary>
		public static bool CanBeHomo(Sex pawnSex, Sex partnerSex) => (pawnSex, partnerSex) switch
		{
			// Get un-gendered out of the possibilities.
			(Sex.none, _) or (_, Sex.none) => false,
			// Futa can be gay towards anything remaining.
			(Sex.futa, _) or (_, Sex.futa) => true,
			// Male vs male.
			(Sex.male or Sex.trap, Sex.male or Sex.trap) => true,
			// Female vs female.
			(Sex.female, Sex.female) => true,
			// Otherwise, nah.
			_ => false
		};

		/// <summary>
		/// <para>Checks to see if two pawns can do homo stuff.</para>
		/// <para>If you are also going to check `CanBeHetero`, get the sexes first.</para>
		/// </summary>
		public static bool CanBeHomo(Pawn pawn, Pawn partner) =>
			CanBeHomo(GetSex(pawn), GetSex(partner));

		/*
		public static HediffDef GetReactionHediff(Sex before, Sex after)
		{
			if (before == after)
				return null;

			if (before == Sex.male)
				return (after == Sex.female) ? m2f : m2t;
			else if (before == Sex.female)
			{
				if (after == Sex.male)
					return f2m;
				else if (after == Sex.trap)
					return f2t;
				else if (after == Sex.futa)
					return f2h;
				else
					return null;
			}
			else if (before == Sex.futa && (after == Sex.female || after == Sex.none))
				return h2f;
			else//trap to anything, futa to trap; probably won't even be reached ever
				return null;
		}
		*/

		//TODO: fix reactions
		public static HediffDef GetReactionHediff(Sex before, Sex after)
		{
			if (before == after)
				return null;
			else if (before == Sex.male)
			{
				if (after == Sex.female)
					return m2f;
				else if (after == Sex.trap)
					return m2t;
				else if (after == Sex.futa)
					return m2h;
				else
					return null;
			}
			else if (before == Sex.female)
			{
				if (after == Sex.male)
					return f2m;
				else if (after == Sex.trap)
					return f2t;
				else if (after == Sex.futa)
					return f2h;
				else
					return null;
			}
			else if (before == Sex.futa)
			{
				if (after == Sex.male)
					return h2m;
				else if (after == Sex.female)
					return h2f;
				else if (after == Sex.trap)
					return h2t;
				else
					return null;
			}
			else if (before == Sex.trap)
			{
				if (after == Sex.male)
					return t2m;
				else if (after == Sex.female)
					return t2f;
				else if (after == Sex.futa)
					return t2h;
				else
					return null;
			}
			else//unicorns?
				return null;
		}

		public static bool WasThisBefore(Pawn pawn, Sex after)
		{
			Hediff was = null;

			switch (after)
			{
				case Sex.male:
					was = pawn.health.hediffSet.GetFirstHediffOfDef(was_boy);
					break;
				case Sex.female:
					was = pawn.health.hediffSet.GetFirstHediffOfDef(was_girl);
					break;
				case Sex.trap:
					was = pawn.health.hediffSet.GetFirstHediffOfDef(was_trap);
					break;
				case Sex.futa:
					was = pawn.health.hediffSet.GetFirstHediffOfDef(was_futa);
					break;
			}
			return (was != null) ? true : false;
		}

		//Get one of the sexes that were on this pawn before
		public static Sex GetOriginalSex(Pawn pawn)
		{
			foreach (var os in old_sex_list)
			{
				if (pawn.health.hediffSet.GetFirstHediffOfDef(os) != null)
					return old_sex_to_sex[os];
			}
			return Sex.none;//it shouldnt reach here though
		}

		public static Hediff IsInDenial(Pawn pawn)
		{
			Hediff res = null;
			foreach (var h in SexChangeThoughts)
			{
				res = pawn.health.hediffSet.GetFirstHediffOfDef(h);
				if (res != null)
					break;
			}
			return res;
		}

		//roll how much gender fluid the pawn is. 
		//In ideal world this would actually take into account from where to where transition is moving and so on.
		//Same applies to the thought hediffs themselves, but we get what we can get now
		static float RollSexChangeSeverity(Pawn pawn)
		{
			float res = 1;
			if (xxx.is_bisexual(pawn))
				res *= 0.5f;
			if (pawn.story != null && (pawn.story.bodyType == BodyTypeDefOf.Thin || pawn.story.bodyType == BodyTypeDefOf.Fat))
				res *= 0.8f;
			if (!pawn.ageTracker.CurLifeStage.reproductive)
				res *= 0.2f;
			else
				res *= AgeConfigDef.Instance.rigidityByAge.Evaluate(SexUtility.ScaleToHumanAge(pawn));

			return res;
		}

		//Quick hack to check if hediff is adding happiness
		static bool is_happy(this Hediff thought)
		{
			return thought.CurStageIndex == 0;
		}
		static void make_happy(this Hediff thought)
		{
			if (thought.Severity > 0.24f)
				thought.Severity = 0.24f;//this is currently max severity for hediff, that is associated with positive mood
		}
		static void mix_thoughts(this Hediff newer, Hediff older)
		{
			newer.Severity = (newer.Severity + older.Severity) / 2f;
		}

		static void GiveThought(Pawn pawn, HediffDef thought, bool happy = false, Hediff old_thought = null)
		{
			pawn.health.AddHediff(thought);
			var new_thought = pawn.health.hediffSet.GetFirstHediffOfDef(thought);
			if (happy)
			{
				new_thought.make_happy();
				return;
			}
			new_thought.Severity = RollSexChangeSeverity(pawn);
			if (old_thought != null)
			{
				new_thought.Severity = (new_thought.Severity + old_thought.Severity) / 2f;
			}
		}

		/// <summary>
		/// Executes action and then changes sex if necessary.
		/// </summary>
		public static void ChangeSex(Pawn pawn, Action action)
		{
			var before = GetSex(pawn);
			action();
			var after = GetSex(pawn);

			try
			{
				ChangeSex(pawn, before, after);
			}
			catch
			{
				ModLog.Error("ChangeSex error: " + xxx.get_pawnname(pawn));
			}
		}

		public static void ChangeSex(Pawn pawn, Sex oldsex, Sex newsex)
		{
			if (CompRJW.Comp(pawn) == null)
			{
				ModLog.Warning(" ChangeSex error: CompRJW.Comp(pawn) should not be null");
				return;
			}

			//Log.Message("ChangeSex 1" + oldsex);
			//Log.Message("ChangeSex 2" + newsex);
			// Wakeup pawn sexuality if it has none before
			if (!(CompRJW.Comp(pawn).orientation == Orientation.Asexual || CompRJW.Comp(pawn).orientation == Orientation.None))
				if (oldsex == newsex)
					return;

			//update ingame genders
			if (newsex == Sex.none)
				return;
			else if (newsex == Sex.male || newsex == Sex.trap)
				pawn.gender = Gender.Male;
			else
				pawn.gender = Gender.Female;

			// Sexualize pawn after installation of parts if it was "not interested in".
			if (oldsex == Sex.none || CompRJW.Comp(pawn).orientation == Orientation.Asexual || CompRJW.Comp(pawn).orientation == Orientation.None)
				if (pawn.kindDef.race.defName.ToLower().Contains("droid") && !AndroidsCompatibility.IsAndroid(pawn))
				{
					//basic droids,they don't care
					return;
				}
				else
				{
					CompRJW.Comp(pawn).Sexualize(pawn, true);
				}

			var old_thought = IsInDenial(pawn);
			var react = GetReactionHediff(oldsex, newsex);

			if (old_thought == null || old_thought.is_happy())//pawn either liked it or got used already
			{
				//Log.Message("ChangeSex 1 old_thought" + old_thought);
				//Log.Message("ChangeSex 1 react" + react);
				if (react != null)
				{
					// IsDesignatedHero() crash world gen when adding rjw artificial tech hediffs to royalty, assume they are happy with their implants
					try
					{
						GiveThought(pawn, react, pawn.IsDesignatedHero());//give unhappy, if not hero}
					}
					catch
					{
						ModLog.Warning(" ChangeSex error " + xxx.get_pawnname(pawn) + " faction " + pawn.Faction.Name + ". Probably tried to change sex at world gen for royalty implant, skipping");
						GiveThought(pawn, react, happy: true);
					}
				}
				if (old_thought != null)
					pawn.health.RemoveHediff(old_thought);

				//add tracking hediff
				pawn.health.AddHediff(sex_to_old_sex[oldsex]);
			}
			else//pawn was unhappy
			{
				if (WasThisBefore(pawn, newsex))//pawn is happy to be previous self
				{
					GiveThought(pawn, react, happy: true);
					pawn.health.RemoveHediff(old_thought);
				}
				else//pawn is still unhappy mix the unhappiness from two
				{
					react = GetReactionHediff(GetOriginalSex(pawn), newsex);//check reaction from original sex
					if (react != null)
					{
						GiveThought(pawn, react, old_thought: old_thought);
						pawn.health.RemoveHediff(old_thought);
					}
					//else pawn keeps old unhappy thought
				}
			}
		}
	}
}
