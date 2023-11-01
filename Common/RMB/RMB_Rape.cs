using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using System;

namespace rjw
{
	[StaticConstructorOnStartup]
	static class RMB_Rape
	{

		static RMB_Rape()
		{
			Harmony harmony = new Harmony("rjw");
			//start sex options
			harmony.Patch(AccessTools.Method(typeof(RMB_Menu), "GenerateRMBOptions"), prefix: null,
				postfix: new HarmonyMethod(typeof(RMB_Rape), nameof(ChoicesAtFor)));
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
			bool rape = true;

			if (target.Pawn != pawn)
				if (target.Thing != pawn as Thing)// && !MP.IsInMultiplayer)
				{
					if (target.Pawn != null || target.Thing is Corpse)
						if (xxx.can_rape(pawn, true))
						{
							//Log.Message("targets can_rape " + target.Label);
							string text = null;
							Action action = null;

							if (target.Thing is Corpse && RJWSettings.necrophilia_enabled)
							{
								if (reverse)
								{
									text = "RJW_RMB_RapeCorpse_Reverse".Translate() + ((Corpse)target.Thing).InnerPawn.NameShortColored;
								}
								else
									text = "RJW_RMB_RapeCorpse".Translate() + ((Corpse)target.Thing).InnerPawn.NameShortColored;
								action = delegate ()
								{
									JobDef job = xxx.RapeCorpse;
									var validinteractions = RMB_Menu.GenerateNonSoloSexRoleOptions(pawn, target, job, rape, reverse).Where(x => x.action != null);
									if (validinteractions.Any())
										FloatMenuUtility.MakeMenu(validinteractions, (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
									else
										Messages.Message("No valid interactions found for " + text, MessageTypeDefOf.RejectInput, false);

								};
							}
							else if (target.Pawn != null && xxx.can_be_fucked(target.Pawn) &&
							(xxx.is_human(target.Pawn) || (xxx.is_animal(target.Pawn) && RJWSettings.bestiality_enabled)))
									//(xxx.is_human(target.Pawn)))
							{
								//Log.Message("targets can_rape 1 " + target.Label);
								if (target.Pawn.HostileTo(pawn))
								{
									//Log.Message("targets can_rape HostileTo " + target.Label);
									if (target.Pawn.Downed)
									{
										if (reverse)
										{
											text = "RJW_RMB_Rape_Reverse".Translate() + target.Pawn.NameShortColored;
										}
										else
											text = "RJW_RMB_Rape".Translate() + target.Pawn.NameShortColored;
										action = delegate ()
										{
											JobDef job = xxx.RapeEnemy;
											var validinteractions = RMB_Menu.GenerateNonSoloSexRoleOptions(pawn, target, job, rape, reverse).Where(x => x.action != null);
											if (validinteractions.Any())
												FloatMenuUtility.MakeMenu(validinteractions, (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
											else
												Messages.Message("No valid interactions found for " + text, MessageTypeDefOf.RejectInput, false);
										};
									}
								}
								else if (xxx.is_animal(target.Pawn) && xxx.can_fuck(pawn))
								{
									if (reverse)
									{
										text = "RJW_RMB_RapeAnimal_Reverse".Translate() + target.Pawn.NameShortColored;
									}
									else
										text = "RJW_RMB_RapeAnimal".Translate() + target.Pawn.NameShortColored;
									action = delegate ()
									{
										JobDef job = xxx.bestiality;
										var validinteractions = RMB_Menu.GenerateNonSoloSexRoleOptions(pawn, target, job, rape, reverse).Where(x => x.action != null);
										if (validinteractions.Any())
											FloatMenuUtility.MakeMenu(validinteractions, (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
										else
											Messages.Message("No valid interactions found for " + text, MessageTypeDefOf.RejectInput, false);
									};
								}
								else if (target.Pawn.IsDesignatedComfort())
								{
									//Log.Message("targets can_rape IsDesignatedComfort " + target.Label);
									if (reverse)
									{
										text = "RJW_RMB_Rape_Reverse".Translate() + target.Pawn.NameShortColored;
									}
									else
										text = "RJW_RMB_Rape".Translate() + target.Pawn.NameShortColored;
									action = delegate ()
									{
										JobDef job = xxx.RapeCP;
										var validinteractions = RMB_Menu.GenerateNonSoloSexRoleOptions(pawn, target, job, rape, reverse).Where(x => x.action != null);
										if (validinteractions.Any())
											FloatMenuUtility.MakeMenu(validinteractions, (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
										else
											Messages.Message("No valid interactions found for " + text, MessageTypeDefOf.RejectInput, false);
									};
								}
								else if (xxx.can_get_raped(target.Pawn) && (xxx.get_vulnerability(target.Pawn) >= xxx.get_vulnerability(pawn)))
								{
									//Log.Message("targets can_rape else " + target.Label);
									if (reverse)
									{
										text = "RJW_RMB_Rape_Reverse".Translate() + target.Pawn.NameShortColored;
									}
									else
										text = "RJW_RMB_Rape".Translate() + target.Pawn.NameShortColored;
									action = delegate ()
									{
										JobDef job = xxx.RapeRandom;
										var validinteractions = RMB_Menu.GenerateNonSoloSexRoleOptions(pawn, target, job, rape, reverse).Where(x => x.action != null);
										if (validinteractions.Any())
											FloatMenuUtility.MakeMenu(validinteractions, (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
										else
											Messages.Message("No valid interactions found for " + text, MessageTypeDefOf.RejectInput, false);
									};
								}
							}

							option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text, action, MenuOptionPriority.High), pawn, target);
							opts.AddDistinct(option);
						}
				}
			return opts;
		}
	}
}