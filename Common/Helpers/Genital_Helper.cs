using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using rjw.Modules.Testing;

using GenitalFamily = rjw.Modules.Interactions.Enums.GenitalFamily;
using GenitalTag = rjw.Modules.Interactions.Enums.GenitalTag;
using GenitalPartExtension = rjw.Modules.Interactions.DefModExtensions.GenitalPartExtension;

namespace rjw
{
	public static class Genital_Helper
	{
		public static HediffDef generic_anus = HediffDef.Named("GenericAnus");
		public static HediffDef generic_penis = HediffDef.Named("GenericPenis");
		public static HediffDef generic_vagina = HediffDef.Named("GenericVagina");
		public static HediffDef generic_breasts = HediffDef.Named("GenericBreasts");

		public static HediffDef average_penis = HediffDef.Named("Penis");
		public static HediffDef hydraulic_penis = HediffDef.Named("HydraulicPenis");
		public static HediffDef bionic_penis = HediffDef.Named("BionicPenis");
		public static HediffDef archotech_penis = HediffDef.Named("ArchotechPenis");

		public static HediffDef average_vagina = HediffDef.Named("Vagina");
		public static HediffDef hydraulic_vagina = HediffDef.Named("HydraulicVagina");
		public static HediffDef bionic_vagina = HediffDef.Named("BionicVagina");
		public static HediffDef archotech_vagina = HediffDef.Named("ArchotechVagina");

		public static HediffDef average_breasts = HediffDef.Named("Breasts");
		public static HediffDef hydraulic_breasts = HediffDef.Named("HydraulicBreasts");
		public static HediffDef bionic_breasts = HediffDef.Named("BionicBreasts");
		public static HediffDef archotech_breasts = HediffDef.Named("ArchotechBreasts");
		public static HediffDef featureless_chest = HediffDef.Named("FeaturelessChest");
		public static HediffDef udder_breasts = HediffDef.Named("UdderBreasts");

		public static HediffDef average_anus = HediffDef.Named("Anus");
		public static HediffDef hydraulic_anus = HediffDef.Named("HydraulicAnus");
		public static HediffDef bionic_anus = HediffDef.Named("BionicAnus");
		public static HediffDef archotech_anus = HediffDef.Named("ArchotechAnus");

		public static HediffDef peg_penis = HediffDef.Named("PegDick");

		public static HediffDef insect_anus = HediffDef.Named("InsectAnus");
		public static HediffDef ovipositorM = HediffDef.Named("OvipositorM");
		public static HediffDef ovipositorF = HediffDef.Named("OvipositorF");

		public static HediffDef demonT_penis = HediffDef.Named("DemonTentaclePenis");
		public static HediffDef demon_penis = HediffDef.Named("DemonPenis");
		public static HediffDef demon_vagina = HediffDef.Named("DemonVagina");
		public static HediffDef demon_anus = HediffDef.Named("DemonAnus");

		public static HediffDef slime_breasts = HediffDef.Named("SlimeBreasts");
		public static HediffDef slime_penis = HediffDef.Named("SlimeTentacles");
		public static HediffDef slime_vagina = HediffDef.Named("SlimeVagina");
		public static HediffDef slime_anus = HediffDef.Named("SlimeAnus");

		public static HediffDef feline_penis = HediffDef.Named("CatPenis");
		public static HediffDef feline_vagina = HediffDef.Named("CatVagina");

		public static HediffDef canine_penis = HediffDef.Named("DogPenis");
		public static HediffDef canine_vagina = HediffDef.Named("DogVagina");

		public static HediffDef equine_penis = HediffDef.Named("HorsePenis");
		public static HediffDef equine_vagina = HediffDef.Named("HorseVagina");

		public static HediffDef dragon_penis = HediffDef.Named("DragonPenis");
		public static HediffDef dragon_vagina = HediffDef.Named("DragonVagina");

		public static HediffDef raccoon_penis = HediffDef.Named("RaccoonPenis");

		public static HediffDef hemipenis = HediffDef.Named("HemiPenis");

		public static HediffDef crocodilian_penis = HediffDef.Named("CrocodilianPenis");

		// I'm not a big fan of looking for defs through string matching, but
		// I suppose it handles the odd rebelious race mod.

		public static readonly BodyPartTagDef BodyPartTagDefEatingSource = DefDatabase<BodyPartTagDef>.GetNamed("EatingSource");
		public static readonly BodyPartTagDef BodyPartTagDefMetabolismSource = DefDatabase<BodyPartTagDef>.GetNamed("MetabolismSource");
		public static readonly BodyPartTagDef BodyPartTagDefTongue = DefDatabase<BodyPartTagDef>.GetNamed("Tongue");

		private static readonly HashSet<BodyPartDef> mouthBPs =
			DefDatabase<BodyPartDef>.AllDefsListForReading
				.Where((def) => def.defName.ToLower().ContainsAny("mouth", "teeth", "jaw", "beak") || def.tags.Contains(BodyPartTagDefEatingSource))
				.ToHashSet();

		private static readonly HashSet<BodyPartDef> tongueBPs =
			DefDatabase<BodyPartDef>.AllDefsListForReading
				.Where((def) => def.defName.ToLower().Contains("tongue") || def.tags.Contains(BodyPartTagDefTongue))
				.ToHashSet();

		private static readonly HashSet<BodyPartDef> stomachBPs =
			DefDatabase<BodyPartDef>.AllDefsListForReading
				.Where((def) => def.defName.ToLower().Contains("stomach") || def.tags.Contains(BodyPartTagDefMetabolismSource))
				.ToHashSet();

		private static readonly HashSet<BodyPartDef> torsoBPs =
			DefDatabase<BodyPartDef>.AllDefsListForReading
				.Where((def) => def.defName.ToLower().Contains("torso"))
				.ToHashSet();

		private static readonly HashSet<BodyPartDef> tailBPs =
			DefDatabase<BodyPartDef>.AllDefsListForReading
				.Where((def) => def.defName.ToLower().Contains("tail"))
				.ToHashSet();

		private static readonly HashSet<BodyPartDef> bodyBPs =
			DefDatabase<BodyPartDef>.AllDefsListForReading
				.Where((def) => def.defName.ToLower().Contains("body"))
				.ToHashSet();

		private static readonly BodyPartTagDef bodyPartTagDef_tongue =
			DefDatabase<BodyPartTagDef>.GetNamed("Tongue");

		// These BPRs are added by RJW.  They must have some sex part attached
		// to count as being present.

		public static BodyPartRecord get_genitalsBPR(Pawn pawn)
		{
			//--Log.Message("Genital_Helper::get_genitals( " + xxx.get_pawnname(pawn) + " ) called");
			var bpr = pawn?.RaceProps.body.AllParts.Find(bpr => bpr.def == xxx.genitalsDef);
			if (bpr is not null) return bpr;

			//--ModLog.Message(" get_genitals( " + xxx.get_pawnname(pawn) + " ) - Part is null");
			return null;
		}

		public static BodyPartRecord get_anusBPR(Pawn pawn)
		{
			//--ModLog.Message(" get_anus( " + xxx.get_pawnname(pawn) + " ) called");
			var bpr = pawn?.RaceProps.body.AllParts.Find(bpr => bpr.def == xxx.anusDef);
			if (bpr is not null) return bpr;

			//--ModLog.Message(" get_anus( " + xxx.get_pawnname(pawn) + " ) - Part is null");
			return null;
		}

		public static BodyPartRecord get_breastsBPR(Pawn pawn)
		{
			//--ModLog.Message(" get_breasts( " + xxx.get_pawnname(pawn) + " ) called");
			var bpr = pawn?.RaceProps.body.AllParts.Find(bpr => bpr.def == xxx.breastsDef);
			if (bpr is not null) return bpr;

			//--ModLog.Message(" get_breasts( " + xxx.get_pawnname(pawn) + " ) - Part is null");
			return null;
		}

		public static BodyPartRecord get_uddersBPR(Pawn pawn)
		{
			//--ModLog.Message(" get_udder( " + xxx.get_pawnname(pawn) + " ) called");
			var bpr = pawn?.RaceProps.body.AllParts.Find(bpr => bpr.def == xxx.flankDef);
			if (bpr is not null) return bpr;

			//--ModLog.Message(" get_udders( " + xxx.get_pawnname(pawn) + " ) - Part is null");
			return null;
		}

		// These BPRs are either pre-existing or added by other mods.  They are considered
		// present as long as they have no `Hediff_PartRemoved` on them.  Use the `has_bpr`
		// method to check that they're not chopped off, blown off, smashed off, or whatever
		// other kinds of terrible things can befall a poor pawn.

		public static BodyPartRecord get_mouthBPR(Pawn pawn)
		{
			//--ModLog.Message(" get_mouth( " + xxx.get_pawnname(pawn) + " ) called");
			var bpr = pawn?.RaceProps.body.AllParts.Find((bpr) => mouthBPs.Contains(bpr.def));
			if (bpr is not null) return bpr;

			//--ModLog.Message(" get_mouth( " + xxx.get_pawnname(pawn) + " ) - Part is null");
			return null;
		}

		public static BodyPartRecord get_tongueBPR(Pawn pawn)
		{
			//--ModLog.Message(" get_tongue( " + xxx.get_pawnname(pawn) + " ) called");
			if (pawn?.RaceProps.body.AllParts is { } allParts)
			{
				foreach (var bpr in allParts)
				{
					if (tongueBPs.Contains(bpr.def)) return bpr;
					if (bpr.def.tags.Contains(bodyPartTagDef_tongue)) return bpr;
				}
			}

			//--ModLog.Message(" get_tongue( " + xxx.get_pawnname(pawn) + " ) - Part is null");
			return null;
		}

		public static BodyPartRecord get_stomachBPR(Pawn pawn)
		{
			//--ModLog.Message(" get_stomach( " + xxx.get_pawnname(pawn) + " ) called");
			if (pawn?.RaceProps.body.AllParts is { } allParts)
			{
				var bpr = allParts.Find((bpr) => stomachBPs.Contains(bpr.def));
				if (bpr is not null) return bpr;

				//--ModLog.Message(" get_stomach( " + xxx.get_pawnname(pawn) + " ) - Part is null, trying to get torso...");
				bpr = allParts.Find((bpr) => torsoBPs.Contains(bpr.def));
				if (bpr is not null) return bpr;
			}

			//--ModLog.Message(" get_stomach( " + xxx.get_pawnname(pawn) + " ) - Part is null, no stomach or torso.");
			return null;
		}

		public static BodyPartRecord get_tailBPR(Pawn pawn)
		{
			//should probably make scale race check or something
			//--ModLog.Message(" get_tail( " + xxx.get_pawnname(pawn) + " ) called");
			var bpr = pawn?.RaceProps.body.AllParts.Find(bpr => tailBPs.Contains(bpr.def));
			if (bpr is not null) return bpr;

			//--ModLog.Message(" get_tail( " + xxx.get_pawnname(pawn) + " ) - Part is null");
			return null;
		}

		public static BodyPartRecord get_torsoBPR(Pawn pawn)
		{
			//--ModLog.Message(" get_torsoBPR( " + xxx.get_pawnname(pawn) + " ) called");
			if (pawn?.RaceProps.body.AllParts is { } allParts)
			{
				var bpr = allParts.Find((bpr) => torsoBPs.Contains(bpr.def));
				if (bpr is not null) return bpr;

				//--ModLog.Message(" get_torsoBPR( " + xxx.get_pawnname(pawn) + " ) - Part is null, trying to get body...");
				bpr = allParts.Find((bpr) => bodyBPs.Contains(bpr.def));
				if (bpr is not null) return bpr;
			}

			//--ModLog.Message(" get_torsoBPR( " + xxx.get_pawnname(pawn) + " ) - Part is null, no torso or body");
			return null;
		}

		public static bool breasts_blocked(Pawn pawn)
		{
			if (pawn.apparel?.WornApparel is not { } wornApparel) return false;
			return wornApparel.ToBondageGear().Any((def) => def.blocks_breasts);
		}

		public static bool udder_blocked(Pawn pawn)
		{
			if (pawn.apparel?.WornApparel is not { } wornApparel) return false;
			return wornApparel.ToBondageGear().Any((def) => def.blocks_udder);
		}

		public static bool anus_blocked(Pawn pawn)
		{
			if (pawn.apparel?.WornApparel is not { } wornApparel) return false;
			return wornApparel.ToBondageGear().Any((def) => def.blocks_anus);
		}

		public static bool genitals_blocked(Pawn pawn)
		{
			if (pawn.apparel?.WornApparel is not { } wornApparel) return false;
			return wornApparel.ToBondageGear().Any((def) => def.blocks_penis || def.blocks_vagina);
		}

		public static bool hands_blocked(Pawn pawn)
		{
			if (pawn.apparel?.WornApparel is not { } wornApparel) return false;
			return wornApparel.ToBondageGear().Any((def) => def.blocks_hands);
		}

		public static bool penis_blocked(Pawn pawn)
		{
			if (pawn.apparel?.WornApparel is not { } wornApparel) return false;
			return wornApparel.ToBondageGear().Any((def) => def.blocks_penis);
		}

		public static bool oral_blocked(Pawn pawn)
		{
			if (pawn.apparel?.WornApparel is not { } wornApparel) return false;
			return wornApparel.ToBondageGear().Any((def) => def.blocks_oral);
		}

		public static bool vagina_blocked(Pawn pawn)
		{
			if (pawn.apparel?.WornApparel is not { } wornApparel) return false;
			return wornApparel.ToBondageGear().Any((def) => def.blocks_vagina);
		}

		private static IEnumerable<bondage_gear_def> ToBondageGear(this List<Apparel> wornApparel) =>
			wornApparel.Select((app) => app.def).OfType<bondage_gear_def>();

		public static bool is_sex_part(Hediff hed) =>
			hed is Hediff_PartBaseNatural || hed is Hediff_PartBaseArtifical;

		public static List<Hediff> get_PartsHediffList(Pawn pawn, BodyPartRecord Part) =>
			pawn.health.hediffSet.hediffs.FindAll((hed) => is_sex_part(hed) && hed.Part == Part);

		public static List<Hediff> get_AllPartsHediffList(Pawn pawn) =>
			pawn.health.hediffSet.hediffs.FindAll(is_sex_part);

		public static bool has_genitals(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			return !parts.NullOrEmpty();
		}

		public static bool has_breasts(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetBreastList();
			if (parts.NullOrEmpty()) return false;

			return parts.Any((hed) => hed.def != featureless_chest);
		}

		public static bool has_udder(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetUdderList();
			return !parts.NullOrEmpty();
		}

		public static bool has_male_breasts(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetBreastList();
			if (parts.NullOrEmpty()) return false;

			return parts.Any((hed) => is_sex_part(hed) && hed.CurStageIndex == 0);
		}

		/// <summary>
		/// Can do breastjob if breasts are average or bigger
		/// </summary>
		/// <param name="pawn"></param>
		/// <returns></returns>
		public static bool can_do_breastjob(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetBreastList();
			if (parts.NullOrEmpty()) return false;

			return parts.Any((hed) => is_sex_part(hed) && hed.CurStageIndex > 1);
		}

		public static bool has_tail(Pawn pawn) =>
			has_bpr(pawn, get_tailBPR(pawn));

		public static bool has_mouth(Pawn pawn) =>
			has_bpr(pawn, get_mouthBPR(pawn));

		public static bool has_tongue(Pawn pawn) =>
			has_bpr(pawn, get_tongueBPR(pawn));

		/// <summary>
		/// <para>Checks to see if a body part is actually attached.</para>
		/// <para>This applies to BPRs that are not added by RJW.  For those,
		/// we count the part as missing if they do not have an accompanying
		/// sex-part.</para>
		/// </summary>
		/// <param name="pawn">The pawn to check.</param>
		/// <param name="bpr">The `BodyPartRecord` indicating the body part.</param>
		/// <returns>Whether the part is missing.</returns>
		public static bool has_bpr(Pawn pawn, BodyPartRecord bpr)
		{
			if (bpr is null) return false;
			return !pawn.health.hediffSet.hediffs
				.Where((hed) => hed.Part == bpr)
				.Any((hed) => hed is Hediff_MissingPart);
		}

		public static bool has_anus(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetAnusList();
			if (parts.NullOrEmpty()) return false;

			for (int i = 0; i < parts.Count; i++)
				if (is_anus(parts[i]))
					return true;
			return false;
		}

		public static bool is_anus(Hediff hed)
		{
			if (!GenitalPartExtension.TryGet(hed, out var ext)) return false;
			return ext.family == GenitalFamily.Anus;
		}

		/// <summary>
		/// Insertable, this is both vagina and ovipositorf
		/// </summary>
		/// <param name="pawn"></param>
		/// <param name="parts"></param>
		/// <returns></returns>
		public static bool has_vagina(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			if (parts.NullOrEmpty()) return false;

			for (int i = 0; i < parts.Count; i++)
				if (is_vagina(parts[i]))
					return true;
			return false;
		}

		private static bool is_vagina_family(GenitalFamily family) => family switch
		{
			GenitalFamily.Vagina => true,
			GenitalFamily.FemaleOvipositor => true,
			_ => false
		};

		public static bool is_vagina(Hediff hed) =>
			GenitalPartExtension.TryGet(hed, out var ext) && is_vagina_family(ext.family);

		/// <summary>
		/// <para>Checks to see if a pawn has a penis, whether fertile or not.</para>
		/// <para>This includes male ovipositors, but not female ovipositors (that is
		/// regarded more like a vagina).</para>
		/// </summary>
		/// <param name="pawn">The pawn to inspect.</param>
		/// <param name="parts">The pre-obtained parts, if available.</param>
		public static bool has_male_bits(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			if (parts.NullOrEmpty()) return false;

			return parts.Any(is_penis);
		}

		public static bool has_penis_fertile(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			if (parts.NullOrEmpty()) return false;

			for (int i = 0; i < parts.Count; i++)
				if (is_fertile_penis(parts[i]))
					return true;
			return false;
		}

		public static bool has_penis_infertile(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			if (parts.NullOrEmpty()) return false;

			for (int i = 0; i < parts.Count; i++)
				if (is_infertile_penis(parts[i]))
					return true;
			return false;
		}

		private static bool is_penis_family(GenitalFamily family) => family switch
		{
			GenitalFamily.Penis => true,
			GenitalFamily.MaleOvipositor => true,
			_ => false
		};

		private static bool is_fertile_tag(GenitalTag tag) => tag switch
		{
			GenitalTag.CanFertilize => true,
			GenitalTag.CanFertilizeEgg => true,
			_ => false
		};

		public static bool is_penis(Hediff hed) =>
			GenitalPartExtension.TryGet(hed, out var ext) && is_penis_family(ext.family);

		public static bool is_fertile_penis(Hediff hed)
		{
			if (!GenitalPartExtension.TryGet(hed, out var ext)) return false;
			if (!is_penis_family(ext.family)) return false;

			for (int i = 0; i < ext.tags.Count; i++)
				if (is_fertile_tag(ext.tags[i]))
					return true;
			return false;
		}

		public static bool is_infertile_penis(Hediff hed)
		{
			if (!GenitalPartExtension.TryGet(hed, out var ext)) return false;
			if (!is_penis_family(ext.family)) return false;

			for (int i = 0; i < ext.tags.Count; i++)
				if (is_fertile_tag(ext.tags[i]))
					return false;
			return true;
		}

		public static bool has_multipenis(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			if (parts.NullOrEmpty()) return false;

			int count = 0;
			foreach (Hediff hed in parts)
			{
				if (!is_penis(hed)) continue;

				// Matches hemipenis.
				if (PartProps.TryGetProps(hed, out var props))
					if (props.Contains("Multiple"))
						return true;

				count += 1;
				if (count > 1) return true;
			}

			return false;
		}

		public static bool has_ovipositorM(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			if (parts.NullOrEmpty()) return false;

			for (int i = 0; i < parts.Count; i++)
				if (is_male_ovipositor(parts[i]))
					return true;
			return false;

			static bool is_male_ovipositor(Hediff hed)
			{
				if (!GenitalPartExtension.TryGet(hed, out var ext)) return false;
				return ext.family == GenitalFamily.MaleOvipositor;
			}
		}

		public static bool has_ovipositorF(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			if (parts.NullOrEmpty()) return false;

			for (int i = 0; i < parts.Count; i++)
				if (is_female_ovipositor(parts[i]))
					return true;
			return false;

			static bool is_female_ovipositor(Hediff hed)
			{
				if (!GenitalPartExtension.TryGet(hed, out var ext)) return false;
				return ext.family == GenitalFamily.FemaleOvipositor;
			}
		}
		/// <summary>
		/// Can do autofellatio if penis is huge or bigger
		/// </summary>
		/// <param name="pawn"></param>
		/// <returns></returns>
		public static bool can_do_autofelatio(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			if (parts.NullOrEmpty()) return false;

			return parts.Any((hed) => is_penis(hed) && hed.CurStageIndex > 3);
		}

		/// <summary>
		/// Count only fertile penises
		/// </summary>
		/// <param name="pawn"></param>
		/// <returns></returns>
		public static bool is_futa(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			return has_vagina(pawn, parts) && has_penis_fertile(pawn, parts);
		}

		public static int min_EggsProduced(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			// No fitting parts - return 0
			if (parts.NullOrEmpty()) return 0;

			// set min to 1 in case it is not defined in the xml file
			int minEggsProduced = 1;

			var eggParts = parts.Select(part => part.def).OfType<HediffDef_PartBase>();
			foreach (HediffDef_PartBase hed in eggParts)
			{
				//if (hed.minEggAmount)
				if (minEggsProduced > 0)
				{
					minEggsProduced = hed.minEggsProduced;
				}
			}


			return minEggsProduced;
		}

		public static int max_EggsProduced(Pawn pawn, List<Hediff> parts = null)
		{
			parts ??= pawn.GetGenitalsList();
			// No fitting parts - return 0
			if (parts.NullOrEmpty()) return 0;

			// set min to 1 in case it is not defined in the xml file
			int maxEggsProduced = 1;

			var eggParts = parts.Select(part => part.def).OfType<HediffDef_PartBase>();
			foreach (HediffDef_PartBase hed in eggParts)
			{
				//if (hed.minEggAmount)
				if (maxEggsProduced > 0)
				{
					maxEggsProduced = hed.maxEggsProduced;
				}
			}


			return maxEggsProduced;
		}

		/// <summary>
		/// <para>This utility appears in the "Output" tab of the debug menu.</para>
		/// <para>It generates every kind of pawn in the game and runs all these methods
		/// against them.  You can use this to validate changes to these utilities.</para>
		/// <para>Just run this before your changes, then after, and use some kind of diff
		/// utility to see if anything is ...different.</para>
		/// <para>Be aware that sometimes pawns can just generate in an imperfect way,
		/// like an iguana whose tail is currently severed off will change how `has_tail`
		/// might generate its output.</para>
		/// </summary>
		[DebugOutput("RJW", onlyWhenPlaying = true)]
		public static void GenitalsOfAllPawnKinds()
		{
			var artificialParts = DefDatabase<RecipeDef>.AllDefsListForReading
				.Where((r) => r.workerClass == typeof(Recipe_InstallGenitals))
				.Where((r) => typeof(Hediff_PartBaseArtifical).IsAssignableFrom(r.addsHediff.hediffClass))
				.ToArray();

			var dataSources = DefDatabase<PawnKindDef>.AllDefsListForReading
				.Where((d) => d.race != null)
				.OrderBy((d) => d.defName)
				.SelectMany(CreatePawns)
				.ToArray();

			var table = new TableDataGetter<(Pawn pawn, string label)>[]
			{
				new("kind", (d) => d.pawn.kindDef.defName),
				new("sex", (d) => GenderHelper.GetSex(d.pawn).ToString().CapitalizeFirst()),
				new("gender", (d) => d.pawn.gender),
				new("label", (d) => d.label),
				new("genitalsBPR", (d) => get_genitalsBPR(d.pawn)?.def.defName),
				new("breastsBPR", (d) => get_breastsBPR(d.pawn)?.def.defName),
				new("uddersBPR", (d) => get_uddersBPR(d.pawn)?.def.defName),
				new("mouthBPR", (d) => get_mouthBPR(d.pawn)?.def.defName),
				new("tongueBPR", (d) => get_tongueBPR(d.pawn)?.def.defName),
				new("stomachBPR", (d) => get_stomachBPR(d.pawn)?.def.defName),
				new("tailBPR", (d) => get_tailBPR(d.pawn)?.def.defName),
				new("anusBPR", (d) => get_anusBPR(d.pawn)?.def.defName),
				new("torsoBPR", (d) => get_torsoBPR(d.pawn)?.def.defName),
				new("has_genitals", (d) => has_genitals(d.pawn)),
				new("has_breasts", (d) => has_breasts(d.pawn)),
				new("has_udder", (d) => has_udder(d.pawn)),
				new("has_male_breasts", (d) => has_male_breasts(d.pawn)),
				new("can_do_breastjob", (d) => can_do_breastjob(d.pawn)),
				new("has_tail", (d) => has_tail(d.pawn)),
				new("has_mouth", (d) => has_mouth(d.pawn)),
				new("has_tongue", (d) => has_tongue(d.pawn)),
				new("has_anus", (d) => has_anus(d.pawn)),
				new("has_vagina", (d) => has_vagina(d.pawn)),
				new("has_male_bits", (d) => has_male_bits(d.pawn)),
				new("has_penis_fertile", (d) => has_penis_fertile(d.pawn)),
				new("has_penis_infertile", (d) => has_penis_infertile(d.pawn)),
				new("has_multipenis", (d) => has_multipenis(d.pawn)),
				new("has_ovipositorM", (d) => has_ovipositorM(d.pawn)),
				new("has_ovipositorF", (d) => has_ovipositorF(d.pawn)),
				new("is_futa", (d) => is_futa(d.pawn)),
				new("min_EggsProduced", (d) => min_EggsProduced(d.pawn)),
				new("max_EggsProduced", (d) => max_EggsProduced(d.pawn))
			};

			DebugTables.MakeTablesDialog(dataSources, table);

			IEnumerable<PawnGenerationRequest> CreateRequests(PawnKindDef def)
			{
				if (!def.RaceProps.hasGenders)
				{
					yield return new PawnGenerationRequest(kind: def)
						.RequestDefaults();
				}
				else
				{
					yield return new PawnGenerationRequest(kind: def, fixedGender: Gender.Male)
						.RequestDefaults();

					yield return new PawnGenerationRequest(kind: def, fixedGender: Gender.Female)
						.RequestDefaults();
				}
			}

			IEnumerable<(Pawn pawn, string label)> CreatePawns(PawnKindDef def)
			{
				foreach (var request in CreateRequests(def))
					if (TestHelper.GenerateSeededPawn(request) is { } pawn)
						yield return (pawn, "natural");

				// Only going to test artificial parts and complex sexes on colonists.
				if (def.defName != "Colonist") yield break;

				// Producing fertile and infertile futa.
				foreach (var request in CreateRequests(def))
				{
					if (request.FixedGender == Gender.Female)
					{
						if (TestHelper.GenerateSeededPawn(request) is { } fertileFemale)
							if (TestHelper.MakeIntoFuta(fertileFemale))
								yield return (fertileFemale, "as fertile female futa");

						if (TestHelper.GenerateSeededPawn(request) is { } infertileFemale)
							if (TestHelper.MakeIntoFuta(infertileFemale, true))
								yield return (infertileFemale, "as infertile female futa");
					}
					else if (request.FixedGender == Gender.Male)
					{
						if (TestHelper.GenerateSeededPawn(request) is { } fertileMale)
							if (TestHelper.MakeIntoFuta(fertileMale))
								yield return (fertileMale, "as fertile male futa");

						if (TestHelper.GenerateSeededPawn(request) is { } infertileMale)
							if (TestHelper.MakeIntoFuta(infertileMale))
								yield return (infertileMale, "as infertile male futa");
					}
				}

				// Producing a trap.
				foreach (var request in CreateRequests(def))
				{
					if (request.FixedGender != Gender.Male) continue;

					if (TestHelper.GenerateSeededPawn(request) is { } malePawn)
						if (TestHelper.MakeIntoTrap(malePawn))
							yield return (malePawn, "as trap");
				}

				// Testing artifical parts.
				foreach (var recipe in artificialParts)
				{
					foreach (var request in CreateRequests(def))
					{
						var pawn = TestHelper.GenerateSeededPawn(request);
						if (pawn is null) continue;
						if (!TestHelper.ApplyPartToPawn(pawn, recipe)) continue;

						yield return (pawn, $"has {recipe.addsHediff.defName}");
					}
				}
			}
		}
	}

}
