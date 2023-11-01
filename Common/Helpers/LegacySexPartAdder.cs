using Multiplayer.API;
using RimWorld;
using Verse;

namespace rjw
{
	/// <summary>
	/// Old, hardcoded part choosing code. Used as a fallback if no RaceGroupDef is found.
	/// </summary>
	public static class LegacySexPartAdder
	{
		static bool privates_gender(Pawn pawn, Gender gender)
		{
			return SexPartAdder.IsAddingPenis(pawn, gender);
		}

		[SyncMethod]
		public static double GenderTechLevelCheck(Pawn pawn)
		{
			bool lowtechlevel = true;

			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			double value = Rand.Value;

			if (pawn?.Faction != null)
				lowtechlevel = (int)pawn.Faction.def.techLevel < 5;
			else if (pawn == null)
				lowtechlevel = false;

			//--save savages from inventing hydraulic and bionic genitals
			while (lowtechlevel && value >= 0.90)
			{
				value = Rand.Value;
			}
			return value;
		}

		public static void AddBreasts(Pawn pawn, BodyPartRecord bpr, Pawn parent)
		{
			var temppawn = parent ?? pawn;
			HediffDef part;
			double value = GenderTechLevelCheck(pawn);
			string racename = temppawn.kindDef.race.defName.ToLower();

			part = Genital_Helper.generic_breasts;

			if (xxx.is_mechanoid(pawn))
			{
				return;
			}
			if (xxx.is_insect(temppawn))
			{
				// this will probably need override in case there are humanoid insect race
				//--ModLog.Message(" add_breasts( " + xxx.get_pawnname(pawn) + " ) - is insect,doesn't need breasts");
				return;
			}
			//alien races - MoreMonstergirls
			else if (racename.Contains("slime"))
			{
				//slimes are always females, and idc what anyone else say!
				part = Genital_Helper.slime_breasts;
			}
			else
			{
				if (pawn.RaceProps.Humanlike)
				{
					//alien races - ChjDroid, ChjAndroid
					if (racename.ContainsAny("mantis", "rockman", "slug", "zoltan", "engie", "sergal", "cutebold", "dodo", "owl", "parrot",
						"penguin", "cassowary", "chicken", "vulture"))
					{
						if (!pawn.health.hediffSet.HasHediff(Genital_Helper.featureless_chest))
							pawn.health.AddHediff(Genital_Helper.featureless_chest, bpr);

						return;
					}
					else if (racename.ContainsAny("avali", "khess"))
					{
						return;
					}
					else if (AndroidsCompatibility.IsAndroid(pawn))
					{
						if (pawn.story.GetBackstory(BackstorySlot.Childhood) != null)
						{
							if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("bishojo"))
								part = Genital_Helper.bionic_breasts;
							else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("pleasure"))
								part = Genital_Helper.bionic_breasts;
							else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("idol"))
								part = Genital_Helper.bionic_breasts;
							else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("social"))
								part = Genital_Helper.hydraulic_breasts;
							else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("substitute"))
								part = Genital_Helper.average_breasts;
							else if (pawn.story.GetBackstory(BackstorySlot.Adulthood) != null)
							{
								if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("courtesan"))
									part = Genital_Helper.bionic_breasts;
								else if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("social"))
									part = Genital_Helper.hydraulic_breasts;
							}
							else
								return;
						}
						else if (pawn.story.GetBackstory(BackstorySlot.Adulthood) != null)
						{
							if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("courtesan"))
								part = Genital_Helper.bionic_breasts;
							else if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("social"))
								part = Genital_Helper.hydraulic_breasts;
						}
						if (part == Genital_Helper.generic_breasts)
							return;
					}
					//alien races - MoreMonstergirls
					else if (racename.Contains("cowgirl"))
					{
						part = Genital_Helper.average_breasts;
						if (value < 0.50 && !pawn.health.hediffSet.HasHediff(Genital_Helper.udder_breasts) && pawn.gender != Gender.Male)
						{
							var bp = Genital_Helper.get_uddersBPR(pawn);
							pawn.health.AddHediff(SexPartAdder.MakePart(Genital_Helper.udder_breasts, pawn, bp), bp);
						}
					}
					else
					{
						if (value < 0.90 || (pawn.ageTracker.AgeBiologicalYears < 2))
							part = Genital_Helper.average_breasts;
						else if (value < 0.95)
							part = Genital_Helper.hydraulic_breasts;
						else
							part = Genital_Helper.bionic_breasts;
					}
				}
				else if (racename.ContainsAny("mammoth", "elasmotherium", "chalicotherium", "megaloceros", "sivatherium", "deinotherium",
					"aurochs", "zygolophodon", "uintatherium", "gazelle", "ffalo", "boomalope", "cow", "miltank", "elk", "reek", "nerf",
					"bantha", "tauntaun", "caribou", "deer", "ibex", "dromedary", "alpaca", "llama", "goat", "moose", "horse", "donkey", "yak"))
				{
					if (!pawn.health.hediffSet.HasHediff(Genital_Helper.udder_breasts) && pawn.gender != Gender.Male)
					{
						bpr = Genital_Helper.get_uddersBPR(pawn);
						part = Genital_Helper.udder_breasts;
					}
					else 
						return;
				}
				else if (racename.ContainsAny("cassowary", "emu", "dinornis", "ostrich", "turkey", "chicken", "duck", "murkroW", "bustard", "palaeeudyptes",
					"goose", "tukiri", "porg", "yi", "kiwi", "penguin", "quail", "ptarmigan", "doduo", "flamingo", "plup", "empoleon", "meadow ave") && !racename.ContainsAny("duck-billed"))
				{
					return;  // Separate list for birds, to make it easier to add cloaca at some later date.
				}   // Other breastless creatures.
				else if (racename.ContainsAny("titanis", "titanoboa", "guan", "tortoise", "turt", "aerofleet", "quinkana", "megalochelys",
					"purussaurus", "cobra", "dewback", "rancor", "frog", "onyx", "flommel", "lapras", "aron", "chinchou",
					"squirtle", "wartortle", "blastoise", "totodile", "croconaw", "feraligatr", "litwick", "pumpkaboo", "shuppet", "haunter",
					"gastly", "oddish", "hoppip", "tropius", "budew", "roselia", "bellsprout", "drifloon", "chikorita", "bayleef", "meganium",
					"char", "drago", "dratini", "saur", "tyrannus", "carnotaurus", "baryonyx", "minmi", "diplodocus", "phodon", "indominus",
					"raptor", "caihong", "coelophysis", "cephale", "compsognathus", "mimus", "troodon", "dactyl", "tanystropheus", "geosternbergia",
					"deino", "suchus", "dracorex", "cephalus", "trodon", "quetzalcoatlus", "pteranodon", "antarctopelta", "stygimoloch", "rhabdodon",
					"rhamphorhynchus", "ceratops", "ceratus", "zalmoxes", "mochlodon", "gigantophis", "crab", "pulmonoscorpius", "manipulator",
					"meganeura", "euphoberia", "holcorobeus", "protosolpuga", "barbslinger", "blizzarisk", "frostmite", "devourer", "hyperweaver",
					"macrelcana", "acklay", "elemental", "megalania", "gecko", "gator", "komodo", "scolipede", "shuckle", "combee", "shedinja",
					"caterpie", "wurmple", "lockjaw", "needlepost", "needleroll", "squid", "slug", "gila", "pleura"))
				{
					return;
				}
				pawn.health.AddHediff(SexPartAdder.MakePart(part, pawn, bpr), bpr);
			}
		}

		public static void AddGenitals(Pawn pawn, Pawn parent, Gender gender, BodyPartRecord bpr, HediffDef part)
		{
			var temppawn = parent ?? pawn;
			double value = GenderTechLevelCheck(pawn);
			string racename = temppawn.kindDef.race.defName.ToLower();

			//Log.Message("Genital_Helper::add_genitals( " + xxx.get_pawnname(pawn));
			//Log.Message("Genital_Helper::add_genitals( " + pawn.kindDef.race.defName);
			//Log.Message("Genital_Helper::is male( " + privates_gender(pawn, gender));
			//Log.Message("Genital_Helper::is male1( " + pawn.gender);
			//Log.Message("Genital_Helper::is male2( " + gender);
			if (xxx.is_mechanoid(pawn))
			{
				return;
			}
			//insects
			else if (xxx.is_insect(temppawn)
				 || racename.Contains("apini")
				 || racename.Contains("mantodean")
				 || racename.Contains("insect")
				 || racename.Contains("bug"))
			{
				part = (privates_gender(pawn, gender)) ? Genital_Helper.ovipositorM : Genital_Helper.ovipositorF;
				//override for Better infestations, since queen is male at creation
				if (racename.Contains("Queen"))
					part = Genital_Helper.ovipositorF;
			}
			//space cats pawns
			else if ((racename.Contains("orassan") || racename.Contains("neko")) && !racename.ContainsAny("akaneko"))
			{
				if ((value < 0.70) || (pawn.ageTracker.AgeBiologicalYears < 2) || !pawn.RaceProps.Humanlike)
					part = (privates_gender(pawn, gender)) ? Genital_Helper.feline_penis : Genital_Helper.feline_vagina;
				else if (value < 0.90)
					part = (privates_gender(pawn, gender)) ? Genital_Helper.hydraulic_penis : Genital_Helper.hydraulic_vagina;
				else
					part = (privates_gender(pawn, gender)) ? Genital_Helper.bionic_penis : Genital_Helper.bionic_vagina;
			}
			//space dog pawns
			else if (racename.Contains("fennex")
				 || racename.Contains("xenn")
				 || racename.Contains("leeani")
				 || racename.Contains("ferian")
				 || racename.Contains("callistan"))
			{
				if ((value < 0.70) || (pawn.ageTracker.AgeBiologicalYears < 2) || !pawn.RaceProps.Humanlike)
					part = (privates_gender(pawn, gender)) ? Genital_Helper.canine_penis : Genital_Helper.canine_vagina;
				else if (value < 0.90)
					part = (privates_gender(pawn, gender)) ? Genital_Helper.hydraulic_penis : Genital_Helper.hydraulic_vagina;
				else
					part = (privates_gender(pawn, gender)) ? Genital_Helper.bionic_penis : Genital_Helper.bionic_vagina;
			}
			//space horse pawns
			else if (racename.Contains("equium"))
			{
				if ((value < 0.70) || (pawn.ageTracker.AgeBiologicalYears < 2) || !pawn.RaceProps.Humanlike)
					part = (privates_gender(pawn, gender)) ? Genital_Helper.equine_penis : Genital_Helper.equine_vagina;
				else if (value < 0.90)
					part = (privates_gender(pawn, gender)) ? Genital_Helper.hydraulic_penis : Genital_Helper.hydraulic_vagina;
				else
					part = (privates_gender(pawn, gender)) ? Genital_Helper.bionic_penis : Genital_Helper.bionic_vagina;
			}
			//space raccoon pawns
			else if (racename.Contains("racc") && !racename.Contains("raccoon"))
			{
				if ((value < 0.70) || (pawn.ageTracker.AgeBiologicalYears < 2) || !pawn.RaceProps.Humanlike)
					part = (privates_gender(pawn, gender)) ? Genital_Helper.raccoon_penis : Genital_Helper.generic_vagina;
				else if (value < 0.90)
					part = (privates_gender(pawn, gender)) ? Genital_Helper.hydraulic_penis : Genital_Helper.hydraulic_vagina;
				else
					part = (privates_gender(pawn, gender)) ? Genital_Helper.bionic_penis : Genital_Helper.bionic_vagina;
			}
			//alien races - ChjDroid, ChjAndroid
			else if (AndroidsCompatibility.IsAndroid(pawn))
			{
				if (pawn.story.GetBackstory(BackstorySlot.Childhood) != null)
				{
					if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("bishojo"))
						part = (privates_gender(pawn, gender)) ? Genital_Helper.bionic_penis : Genital_Helper.bionic_vagina;
					else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("pleasure"))
						part = (privates_gender(pawn, gender)) ? Genital_Helper.bionic_penis : Genital_Helper.bionic_vagina;
					else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("idol"))
						part = (privates_gender(pawn, gender)) ? Genital_Helper.bionic_penis : Genital_Helper.bionic_vagina;
					else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("social"))
						part = (privates_gender(pawn, gender)) ? Genital_Helper.hydraulic_penis : Genital_Helper.hydraulic_vagina;
					else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("substitute"))
						part = (privates_gender(pawn, gender)) ? Genital_Helper.average_penis : Genital_Helper.average_vagina;
					else if (pawn.story.GetBackstory(BackstorySlot.Adulthood) != null)
					{
						if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("courtesan"))
							part = (privates_gender(pawn, gender)) ? Genital_Helper.bionic_penis : Genital_Helper.bionic_vagina;
						else if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("social"))
							part = (privates_gender(pawn, gender)) ? Genital_Helper.hydraulic_penis : Genital_Helper.hydraulic_vagina;
					}
					else
						return;
				}
				else if (pawn.story.GetBackstory(BackstorySlot.Adulthood) != null)
				{
					if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("courtesan"))
						part = (privates_gender(pawn, gender)) ? Genital_Helper.bionic_penis : Genital_Helper.bionic_vagina;
					else if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("social"))
						part = (privates_gender(pawn, gender)) ? Genital_Helper.hydraulic_penis : Genital_Helper.hydraulic_vagina;
				}
				if (part == Genital_Helper.generic_penis || part == Genital_Helper.generic_vagina)
					return;
			}
			//animal cats
			else if (racename.ContainsAny("cat", "cougar", "lion", "leopard", "cheetah", "panther", "tiger", "lynx", "smilodon", "akaneko"))
			{
				part = (privates_gender(pawn, gender)) ? Genital_Helper.feline_penis : Genital_Helper.feline_vagina;
			}
			//animal canine/dogs
			else if (racename.ContainsAny("husky", "warg", "terrier", "collie", "hound", "retriever", "mastiff", "wolf", "fox",
				"vulptex", "dachshund", "schnauzer", "corgi", "pug", "doberman", "chowchow", "borzoi", "saintbernard", "newfoundland",
				"poodle", "dog", "coyote"))
			{
				part = (privates_gender(pawn, gender)) ? Genital_Helper.canine_penis : Genital_Helper.canine_vagina;
			}
			//animal horse - MoreMonstergirls
			else if (racename.ContainsAny("horse", "centaur", "zebra", "donkey", "dryad"))
			{
				part = (privates_gender(pawn, gender)) ? Genital_Helper.equine_penis : Genital_Helper.equine_vagina;
			}
			//animal raccoon
			else if (racename.Contains("racc"))
			{
				part = (privates_gender(pawn, gender)) ? Genital_Helper.raccoon_penis : Genital_Helper.generic_vagina;
			}
			//animal crocodilian (alligator, crocodile, etc)
			else if (racename.ContainsAny("alligator", "crocodile", "caiman", "totodile", "croconaw", "feraligatr", "quinkana", "purussaurus", "kaprosuchus", "sarcosuchus"))
			{
				part = (privates_gender(pawn, gender)) ? Genital_Helper.crocodilian_penis : Genital_Helper.generic_vagina;
			}
			//hemipenes - mostly reptiles and snakes
			else if (racename.ContainsAny("guana", "cobra", "gecko", "snake", "boa", "quinkana", "megalania", "gila", "gigantophis", "komodo", "basilisk", "thorny", "onix", "lizard", "slither") && !racename.ContainsAny("boar"))
			{
				part = (privates_gender(pawn, gender)) ? Genital_Helper.hemipenis : Genital_Helper.generic_vagina;
			}
			//animal dragon - MoreMonstergirls
			else if (racename.ContainsAny("dragon", "thrumbo", "drake", "charizard", "saurus"))
			{
				part = (privates_gender(pawn, gender)) ? Genital_Helper.dragon_penis : Genital_Helper.dragon_vagina;
			}
			//animal slime - MoreMonstergirls
			else if (racename.Contains("slime"))
			{
				// slime always futa
				pawn.health.AddHediff(SexPartAdder.MakePart(privates_gender(pawn, gender) ? Genital_Helper.slime_penis : Genital_Helper.slime_vagina, pawn, bpr), bpr);
				pawn.health.AddHediff(SexPartAdder.MakePart(privates_gender(pawn, gender) ? Genital_Helper.slime_vagina : Genital_Helper.slime_penis, pawn, bpr), bpr);
				return;
			}
			//animal demons - MoreMonstergirls
			else if (racename.Contains("impmother") || racename.Contains("demon"))
			{
				// 25% futa
				pawn.health.AddHediff(SexPartAdder.MakePart(privates_gender(pawn, gender) ? Genital_Helper.demon_penis : Genital_Helper.demon_vagina, pawn, bpr), bpr);
				if (Rand.Value < 0.25f)
					pawn.health.AddHediff(SexPartAdder.MakePart(privates_gender(pawn, gender) ? Genital_Helper.demon_penis : Genital_Helper.demonT_penis, pawn, bpr), bpr);
				return;
			}
			//animal demons - MoreMonstergirls
			else if (racename.Contains("baphomet"))
			{
				if (Rand.Value < 0.50f)
					pawn.health.AddHediff(SexPartAdder.MakePart(privates_gender(pawn, gender) ? Genital_Helper.demon_penis : Genital_Helper.demon_vagina, pawn, bpr), bpr);
				else
					pawn.health.AddHediff(SexPartAdder.MakePart(privates_gender(pawn, gender) ? Genital_Helper.equine_penis : Genital_Helper.demon_vagina, pawn, bpr), bpr);
				return;
			}
			else if (pawn.RaceProps.Humanlike)
			{
				//--Log.Message("Genital_Helper::add_genitals( " + xxx.get_pawnname(pawn) + " ) - race is humanlike");
				if (value < 0.90 || (pawn.ageTracker.AgeBiologicalYears < 2))
					part = (privates_gender(pawn, gender)) ? Genital_Helper.average_penis : Genital_Helper.average_vagina;
				else if (value < 0.95)
					part = (privates_gender(pawn, gender)) ? Genital_Helper.hydraulic_penis : Genital_Helper.hydraulic_vagina;
				else
					part = (privates_gender(pawn, gender)) ? Genital_Helper.bionic_penis : Genital_Helper.bionic_vagina;
			}
			//--Log.Message("Genital_Helper::add_genitals final ( " + xxx.get_pawnname(pawn) + " ) " + part.defName);
			var hd = SexPartAdder.MakePart(part, pawn, bpr);
			//Log.Message("Genital_Helper::add_genitals final ( " + xxx.get_pawnname(pawn) + " ) " + hd.def.defName + " sev " + hd.Severity + " bpr " + BPR.def.defName);
			pawn.health.AddHediff(hd, bpr);
			//Log.Message("Genital_Helper::add_genitals final ( " + xxx.get_pawnname(pawn) + " ) " + pawn.health.hediffSet.HasHediff(hd.def));
		}

		public static void AddAnus(Pawn pawn, BodyPartRecord bpr, Pawn parent)
		{
			var temppawn = parent ?? pawn;
			HediffDef part;
			double value = GenderTechLevelCheck(pawn);
			string racename = temppawn.kindDef.race.defName.ToLower();

			part = Genital_Helper.generic_anus;
			
			if (xxx.is_mechanoid(pawn))
			{
				return;
			}
			else if (xxx.is_insect(temppawn))
			{
				part = Genital_Helper.insect_anus;
			}
			//alien races - ChjDroid, ChjAndroid
			else if (AndroidsCompatibility.IsAndroid(pawn))
			{
				if (pawn.story.GetBackstory(BackstorySlot.Childhood) != null)
				{
					if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("bishojo"))
						part = Genital_Helper.bionic_anus;
					else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("pleasure"))
						part = Genital_Helper.bionic_anus;
					else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("idol"))
						part = Genital_Helper.bionic_anus;
					else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("social"))
						part = Genital_Helper.hydraulic_anus;
					else if (pawn.story.Childhood.untranslatedTitleShort.ToLower().Contains("substitute"))
						part = Genital_Helper.average_anus;
					else if (pawn.story.GetBackstory(BackstorySlot.Adulthood) != null)
					{
						if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("courtesan"))
							part = Genital_Helper.bionic_anus;
						else if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("social"))
							part = Genital_Helper.hydraulic_anus;
					}
				}
				else if (pawn.story.GetBackstory(BackstorySlot.Adulthood) != null)
				{
					if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("courtesan"))
						part = Genital_Helper.bionic_anus;
					else if (pawn.story.Adulthood.untranslatedTitleShort.ToLower().Contains("social"))
						part = Genital_Helper.hydraulic_anus;
				}
				if (part == Genital_Helper.generic_anus)
					return;
			}
			else if (racename.Contains("slime"))
			{
				part = Genital_Helper.slime_anus;
			}
			//animal demons - MoreMonstergirls
			else if (racename.Contains("impmother") || racename.Contains("baphomet") || racename.Contains("demon"))
			{
				part = Genital_Helper.demon_anus;
			}
			else if (pawn.RaceProps.Humanlike)
			{
				if (value < 0.90 || (pawn.ageTracker.AgeBiologicalYears < 2))
					part = Genital_Helper.average_anus;
				else if (value < 0.95)
					part = Genital_Helper.hydraulic_anus;
				else
					part = Genital_Helper.bionic_anus;
			}

			pawn.health.AddHediff(SexPartAdder.MakePart(part, pawn, bpr), bpr);
		}
	}
}
