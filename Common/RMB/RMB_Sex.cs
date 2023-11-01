using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace rjw
{
	[StaticConstructorOnStartup]
	static class RMB_Sex
	{

		static RMB_Sex()
		{
			Harmony harmony = new Harmony("rjw");
			//start sex options
			harmony.Patch(AccessTools.Method(typeof(RMB_Menu), "GenerateRMBOptions"), prefix: null,
				postfix: new HarmonyMethod(typeof(RMB_Sex), nameof(ChoicesAtFor)));
		}

		public static List<FloatMenuOption> ChoicesAtFor(List<FloatMenuOption> __instance, Pawn pawn, LocalTargetInfo target) 
		{
			FloatMenuOption(pawn, ref __instance, ref target);
			return __instance;
		}
		public static void FloatMenuOption(Pawn pawn, ref List<FloatMenuOption> opts, ref LocalTargetInfo target)
		{
			opts.AddRange(GenerateRMBOptions(pawn, target).Where(x => x.action != null));
			opts.AddRange(GenerateRMBOptions(pawn, target, true).Where(x => x.action != null));
		}

		public static List<FloatMenuOption> GenerateRMBOptions(Pawn pawn, LocalTargetInfo target, bool reverse = false)
		{
			List<FloatMenuOption> opts = new List<FloatMenuOption>();
			FloatMenuOption option = null;
			bool rape = false;

			if (target.Pawn != pawn)
				if (target.Thing != pawn as Thing)// && !MP.IsInMultiplayer)
				{
					if (target.Pawn != null)
						if (!target.Pawn.Downed && !target.Pawn.HostileTo(pawn) && (xxx.can_be_fucked(target.Pawn) || xxx.can_fuck(target.Pawn)))
						{
							string text = null;
							//Action action = null;
							if (xxx.is_human(target.Pawn) && SexAppraiser.would_fuck(target.Pawn, pawn) > 0.1f)
							{
								if (reverse)
								{
									text = "RJW_RMB_Sex_Reverse".Translate() + target.Pawn.NameShortColored;
								}
								else
									text = "RJW_RMB_Sex".Translate() + target.Pawn.NameShortColored;
								option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text, delegate ()
								{
									JobDef job = null;
									if (target.Pawn.InBed())
										job = xxx.casual_sex;
									else
										job = xxx.quick_sex;

									var validinteractions = RMB_Menu.GenerateNonSoloSexRoleOptions(pawn, target, job, rape, reverse).Where(x => x.action != null);
									if (validinteractions.Any())
										FloatMenuUtility.MakeMenu(validinteractions, (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
									else
										Messages.Message("No valid interactions found for " + text, MessageTypeDefOf.RejectInput, false);
								}, MenuOptionPriority.High), pawn, target);
								opts.AddDistinct(option);
							}
							else if (xxx.is_animal(target.Pawn) && RJWSettings.bestiality_enabled && target.Pawn.Faction == pawn.Faction)
							{
								if (pawn.ownership.OwnedBed != null && target.Pawn.CanReach(pawn.ownership.OwnedBed, PathEndMode.OnCell, Danger.Some) && !AnimalPenUtility.NeedsToBeManagedByRope(target.Pawn))
								{
									if (reverse)
									{
										text = "RJW_RMB_Bestiality_Reverse".Translate() + target.Pawn.NameShortColored;
									}
									else
										text = "RJW_RMB_Bestiality".Translate() + target.Pawn.NameShortColored;
									option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text, delegate ()
									{
										JobDef job = xxx.bestialityForFemale;
										var validinteractions = RMB_Menu.GenerateNonSoloSexRoleOptions(pawn, target, job, rape, reverse).Where(x => x.action != null);
										if (validinteractions.Any())
											FloatMenuUtility.MakeMenu(validinteractions, (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
										else
											Messages.Message("No valid interactions found for " + text, MessageTypeDefOf.RejectInput, false);
									}, MenuOptionPriority.High), pawn, target);
									opts.AddDistinct(option);
								}
							}
						}
				}
			return opts;
		}
	}
}