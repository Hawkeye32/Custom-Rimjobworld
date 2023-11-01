using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Multiplayer.API;
using rjw.Modules.Interactions;
using rjw.Modules.Interactions.Implementation;

namespace rjw
{
	[StaticConstructorOnStartup]
	static class RMB_Masturbate
	{

		static RMB_Masturbate()
		{
			Harmony harmony = new Harmony("rjw");
			//start sex options
			harmony.Patch(AccessTools.Method(typeof(RMB_Menu), "GenerateRMBOptions"), prefix: null,
				postfix: new HarmonyMethod(typeof(RMB_Masturbate), nameof(ChoicesAtFor)));
		}

		public static List<FloatMenuOption> ChoicesAtFor(List<FloatMenuOption> __instance, Pawn pawn, LocalTargetInfo target) 
		{
			FloatMenuOption(pawn, ref __instance, ref target);
			return __instance;
		}
		public static void FloatMenuOption(Pawn pawn, ref List<FloatMenuOption> opts, ref LocalTargetInfo target)
		{
			opts.AddRange(GenerateRMBOptions(pawn, target).Where(x => x.action != null));
		}

		public static List<FloatMenuOption> GenerateRMBOptions(Pawn pawn, LocalTargetInfo target)
		{
			List<FloatMenuOption> opts = new List<FloatMenuOption>();
			FloatMenuOption option = null;

			if (target.Pawn == pawn)
			{
				option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_Masturbate".Translate(), delegate ()
				{
					FloatMenuUtility.MakeMenu(GenerateSoloSexRoleOptions(pawn, target).Where(x => x.action != null), (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
				}, MenuOptionPriority.High), pawn, target);
				opts.AddDistinct(option);
			}
			return opts;
		}

		//this options probably can be merged into 1, but for now/testing keep it separate
		public static List<FloatMenuOption> GenerateSoloSexRoleOptions(Pawn pawn, LocalTargetInfo target)
		{
			List<FloatMenuOption> opts = new List<FloatMenuOption>();
			FloatMenuOption option = null;

			option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_Masturbate_Bed".Translate(), delegate ()
			{
				Find.Targeter.BeginTargeting(TargetParemetersMasturbationChairOrBed(target), (LocalTargetInfo targetThing) =>
				{
					FloatMenuUtility.MakeMenu(GenerateSoloSexPoseOptions(pawn, targetThing).Where(x => x.action != null), (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
				});
			}, MenuOptionPriority.High), pawn, target);
			opts.AddDistinct(option);

			option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_Masturbate_At".Translate(), delegate ()
			{
				Find.Targeter.BeginTargeting(TargetParemetersMasturbationLoc(target), (LocalTargetInfo targetThing) =>
				{
					FloatMenuUtility.MakeMenu(GenerateSoloSexPoseOptions(pawn, targetThing).Where(x => x.action != null), (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
				});
			}, MenuOptionPriority.High), pawn, target);
			opts.AddDistinct(option);

			option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_Masturbate_Here".Translate(), delegate ()
			{
				FloatMenuUtility.MakeMenu(GenerateSoloSexPoseOptions(pawn, target).Where(x => x.action != null), (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
			}, MenuOptionPriority.High), pawn, target);
			opts.AddDistinct(option);
			return opts;
		}

		public static TargetingParameters TargetParemetersMasturbationChairOrBed(LocalTargetInfo target)
		{
			return new TargetingParameters()
			{
				canTargetBuildings = true,
				mapObjectTargetsMustBeAutoAttackable = false,
				validator = (TargetInfo target) =>
				{
					if (!target.HasThing)
						return false;
					Building building = target.Thing as Building;
					if (building == null)
						return false;
					if (building.def.building.isSittable)
						return true;
					if (building is Building_Bed)
						return true;
					return false;
				}
			};
		}
		public static TargetingParameters TargetParemetersMasturbationLoc(LocalTargetInfo target)
		{
			return new TargetingParameters()
			{
				canTargetLocations = true,
				mapObjectTargetsMustBeAutoAttackable = false,
				validator = (TargetInfo target) =>
				{
					if (!target.HasThing)
						return true;
					return false;
				}
			};
		}
		public static List<FloatMenuOption> GenerateSoloSexPoseOptions(Pawn pawn, LocalTargetInfo target)
		{
			List<FloatMenuOption> opts = new List<FloatMenuOption>();
			FloatMenuOption option = null;
			var job = xxx.Masturbate;

			List<InteractionDef> validintdefs = new List<InteractionDef>();
			foreach (InteractionDef d in SexUtility.SexInterractions)
			{
				var interaction = Modules.Interactions.Helpers.InteractionHelper.GetWithExtension(d);
				if (interaction.Extension.rjwSextype != xxx.rjwSextype.Masturbation.ToStringSafe())
					continue;

				ILewdInteractionValidatorService service = LewdInteractionValidatorService.Instance;
				if (service.IsValid(d, pawn, pawn))
					validintdefs.Add(d);
			}

			foreach (InteractionDef dictionaryKey in validintdefs)
			{
				var interaction = Modules.Interactions.Helpers.InteractionHelper.GetWithExtension(dictionaryKey);
				var dev = "";
				if (RJWSettings.DevMode)
					dev = " ( defName: " + dictionaryKey.defName + ")";

				var label = interaction.Extension.RMBLabel.CapitalizeFirst() + dev;
				option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, delegate ()
				{
					RMB_Menu.HaveSex(pawn, job, target, dictionaryKey);
				}, MenuOptionPriority.High), pawn, target);
				opts.AddDistinct(option);
			}

			if (opts.NullOrEmpty())
				opts.AddDistinct(new FloatMenuOption("none", null));

			return opts;
		}
	}
}