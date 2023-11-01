using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Multiplayer.API;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions;
using rjw.Modules.Interactions.Implementation;
using rjw.Modules.Interactions.Objects;
using static RimWorld.MechClusterSketch;

namespace rjw
{
	[StaticConstructorOnStartup]
	static class RMB_Menu
	{
		static RMB_Menu()
		{
			Harmony harmony = new Harmony("rjw");
			//start sex options
			harmony.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "ChoicesAtFor"), prefix: null,
				postfix: new HarmonyMethod(typeof(RMB_Menu), nameof(ChoicesAtFor)));
		}

		//show rmb on
		public static TargetingParameters TargetParameters
		{
			get
			{
				if (targetParameters == null)
				{
					targetParameters = new TargetingParameters()
					{
						canTargetHumans = true,
						canTargetAnimals = true,
						canTargetItems = true,
						mapObjectTargetsMustBeAutoAttackable = false,
					};
				}
				return targetParameters;
			}
		}

		private static TargetingParameters targetParameters = null;
		private static Vector3 rjwclickPos;

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

		//TODO: dildo selection for masturbation/sex
		public static TargetingParameters TargetParemetersDildos(LocalTargetInfo target)
		{
			return new TargetingParameters()
			{
				canTargetItems = true,
				mapObjectTargetsMustBeAutoAttackable = false,
				validator = ((TargetInfo target) =>
				{
					if (!target.HasThing)
						return false;
					Thing dildo = target.Thing as Thing;
					if (dildo == null)
						return false;

					return true;
				})
			};
		}

		public static List<FloatMenuOption> ChoicesAtFor(List<FloatMenuOption> __instance, Vector3 clickPos, Pawn pawn, bool suppressAutoTakeableGoto = false) 
		{
			rjwclickPos = clickPos;
			if (SaveStorage.ModId == "RJW")// disable rmb for SJW
				SexFloatMenuOption(pawn, ref __instance);
			return __instance;
		}
		public static void SexFloatMenuOption(Pawn pawn, ref List<FloatMenuOption> opts)
		{
			if (!ShowRMB(pawn)) return;

			//Log.Message("show options");

			// Find valid targets for sex.
			var validtargets = GenUI.TargetsAt(rjwclickPos, TargetParameters);
			//Log.Message("targets count " + validtargets.Count());

			foreach (LocalTargetInfo target in validtargets)
			{
				if (target.Pawn != null && target.Pawn.Drafted)
					continue;
				// Ensure target is reachable.
				if (!pawn.CanReach(target, PathEndMode.ClosestTouch, Danger.Deadly))
				{
					//option = new FloatMenuOption("CannotReach".Translate(target.Thing.LabelCap, target.Thing) + " (" + "NoPath".Translate() + ")", null);
					continue;
				}

				//Log.Message("target " +  target.Label);
				opts.AddRange(GenerateRMBOptions(pawn, target).Where(x => x.action != null));
				//sex-role?-pose ?
				//rjw?-sex(do fuck/rape checks)-role?-pose ?
				//sex-rjwsextype?-interactiondefs?
			}
		}

		public static bool ShowRMB(Pawn pawn)
		{
			// If the pawn in question cannot take jobs, don't bother.
			if (pawn.jobs == null)
				return false;

			// If the pawn is drafted - quit.
			if (pawn.Drafted)
				return false;

			// Getting raped - no control
			if (pawn.jobs.curDriver is JobDriver_SexBaseRecieverRaped)
				return false;

			//is colonist?, is hospitality colonist/guest?, no control for guests
			if (!pawn.IsFreeColonist || pawn.Faction == null || pawn.GetExtraHomeFaction(null) != null)
				return false;

			//not hero mode or override_control - quit
			if (!(RJWSettings.RPG_hero_control || RJWSettings.override_control))
				return false;

			var HeroOK0 = false;    //is hero
			var HeroOK1 = false;    //owned hero?
			var HeroOK2 = false;    //not owned hero? maybe prison check etc in future
									// || xxx.is_slave(pawn)
			if (RJWSettings.RPG_hero_control)
			{
				HeroOK0 = pawn.IsDesignatedHero();
				HeroOK1 = HeroOK0 && pawn.IsHeroOwner();
				HeroOK2 = HeroOK0 && !pawn.IsHeroOwner();

				//Log.Message("show options HeroOK0 " + HeroOK0);
				//Log.Message("show options HeroOK1 " + HeroOK1);
				//Log.Message("show options HeroOK2 " + HeroOK2);

			}
			else if (!RJWSettings.override_control)
				return false;

			//not hero, not override_control - quit
			if (!HeroOK0 && !RJWSettings.override_control)
				return false;

			//not owned hero - quit
			if (HeroOK0 && HeroOK2)
				return false;

			if (pawn.IsPrisoner || xxx.is_slave(pawn))
				return false;

			return true;
		}

		public static List<FloatMenuOption> GenerateRMBOptions(Pawn pawn, LocalTargetInfo target)
		{
			List<FloatMenuOption> opts = new List<FloatMenuOption>();
			FloatMenuOption option = null;

			if (ModsConfig.BiotechActive)
			{
				if (MechanitorUtility.IsMechanitor(pawn))
				{
					if(target.Pawn != null)
					{
						if (target.Pawn.IsPregnant(true))
						{
							Hediff_MechanoidPregnancy pregnancy = (Hediff_MechanoidPregnancy)target.Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("RJW_pregnancy_mech"));
							if (pregnancy != null)
							{
								if (pregnancy.is_checked && !pregnancy.is_hacked)
								{
									option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_HackMechPregnancy".Translate(), delegate ()
									{
										Job job22 = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("RJW_HackMechPregnancy"), target.Pawn);
										pawn.jobs.TryTakeOrderedJob(job22, JobTag.Misc);
									}, MenuOptionPriority.High), pawn, target);
									opts.AddDistinct(option);
								}
							}
						}
					}
				}
				else
				{
					if(target.Pawn != null)
						if (pawn.IsColonyMechPlayerControlled)
						{
							//mechimplant


							//			option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_Masturbate".Translate(), delegate ()
							//			{
							//				FloatMenuUtility.MakeMenu(GenerateSoloSexPoseOptions(pawn, target).Where(x => x.action != null), (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);

							//			}, MenuOptionPriority.High), pawn, target);
							//			opts.AddDistinct(option);
						}
				}
			}

			// Already doing sex.
			// Pose switch
			//if (pawn.jobs.curDriver is JobDriver_Sex)
			//{
			//	// Masturbating
			//	if (target.Pawn == pawn)
			//	{
			//		if (pawn.jobs.curDriver is JobDriver_Masturbate)
			//		{
			//			option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_Masturbate".Translate(), delegate ()
			//			{
			//				FloatMenuUtility.MakeMenu(GenerateSoloSexPoseOptions(pawn, target).Where(x => x.action != null), (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);

			//			}, MenuOptionPriority.High), pawn, target);
			//			opts.AddDistinct(option);
			//		}
			//	}
			//	// TODO: Add pose switch
			//	return opts;
			//}

			return opts;
		}


		public static List<FloatMenuOption> GenerateNonSoloSexRoleOptions(Pawn pawn, LocalTargetInfo target, JobDef job, bool rape, bool reverse = false)
		{
			List<FloatMenuOption> opts = new List<FloatMenuOption>();
			FloatMenuOption option = null;

			var partner = target.Pawn;

			if (target.Thing is Corpse)
				partner = (target.Thing as Corpse).InnerPawn;

			var arraytype = 0;      //sex
			if (xxx.is_animal(partner))
			{
				arraytype = 2;      //bestiality/breeding
			}
			else if (rape)
			{
				arraytype = 1;      //rape
			}
			List<InteractionDef> validintdefs = new List<InteractionDef>();
			foreach (InteractionDef d in SexUtility.SexInterractions)
			{
				var interaction = Modules.Interactions.Helpers.InteractionHelper.GetWithExtension(d);
				if (interaction.Extension.rjwSextype == xxx.rjwSextype.None.ToStringSafe())
					continue;
				if (interaction.Extension.rjwSextype == xxx.rjwSextype.Masturbation.ToStringSafe())
					continue;

				if (
					(interaction.HasInteractionTag(InteractionTag.Consensual) && arraytype == 0) ||
					(interaction.HasInteractionTag(InteractionTag.Rape) && arraytype == 1) ||
					(interaction.HasInteractionTag(InteractionTag.Bestiality) && arraytype == 2)
					)
					if (reverse && interaction.HasInteractionTag(InteractionTag.Reverse) || (!reverse && !interaction.HasInteractionTag(InteractionTag.Reverse)))
					{
						ILewdInteractionValidatorService service = LewdInteractionValidatorService.Instance;
						if (service.IsValid(d, pawn, partner))
							validintdefs.Add(d);
					}
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
					HaveSex(pawn, job, target, dictionaryKey);
				}, MenuOptionPriority.High), pawn, target);
				opts.AddDistinct(option);
			}

			if (opts.NullOrEmpty())
				opts.AddDistinct(new FloatMenuOption("none", null));

			return opts;
		}

		//multiplayer synch actions

		[SyncMethod]
		public static void HaveSex(Pawn pawn, JobDef jobDef, LocalTargetInfo target, InteractionDef dictionaryKey)
		{
			bool rape;
			Pawn partner = null;

			if (target.Thing is Corpse)
				partner = (target.Thing as Corpse).InnerPawn;
			else if (target.Pawn == null || pawn == target.Pawn) // masturbation
			{
			}
			else
				partner = target.Pawn;

			InteractionWithExtension interaction = Modules.Interactions.Helpers.InteractionHelper.GetWithExtension(dictionaryKey);

			rape = interaction.HasInteractionTag(InteractionTag.Rape);

			Job job;
			if (jobDef == xxx.casual_sex)
				job = new Job(jobDef, target, partner.CurrentBed());
			else if (jobDef == xxx.bestialityForFemale)
				job = new Job(jobDef, target, pawn.ownership.OwnedBed);
			else if (jobDef == xxx.Masturbate)
			{
				job = new Job(jobDef, pawn, null, target.Cell);
				partner = pawn;
			}
			else
				job = new Job(jobDef, target);

			var SP = new SexProps();
			SP.pawn = pawn;
			SP.partner = partner;
			SP.sexType = SexUtility.rjwSextypeGet(dictionaryKey); ;
			SP.isRape = rape;
			SP.isRapist = rape;
			SP.canBeGuilty = false; //pawn.IsHeroOwner();//TODO: fix for MP someday
			SP.dictionaryKey = dictionaryKey;
			SP.rulePack = SexUtility.SexRulePackGet(dictionaryKey);

			pawn.GetRJWPawnData().SexProps = SP;
			pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
			pawn.jobs.TryTakeOrderedJob(job);
		}
	}
}