using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Multiplayer.API;

namespace rjw
{
	[StaticConstructorOnStartup]
	static class RMB_Socialize
	{

		static RMB_Socialize()
		{
			Harmony harmony = new Harmony("rjw");
			//start sex options
			harmony.Patch(AccessTools.Method(typeof(RMB_Menu), "GenerateRMBOptions"), prefix: null,
				postfix: new HarmonyMethod(typeof(RMB_Socialize), nameof(ChoicesAtFor)));
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

			if (target.Pawn != null && target.Pawn != pawn && !pawn.interactions.InteractedTooRecentlyToInteract() && pawn.interactions.CanInteractNowWith(target.Pawn))
			{
				//if (pawn.jobs.curDriver is JobDriver_Masturbate)
				{
					option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_Social".Translate(), delegate ()
					{
						FloatMenuUtility.MakeMenu(GenerateSocialOptions(pawn, target).Where(x => x.action != null), (FloatMenuOption opt) => opt.Label, (FloatMenuOption opt) => opt.action);
					}, MenuOptionPriority.High), pawn, target);
					opts.AddDistinct(option);
				}
			}
			return opts;
		}

		public static List<FloatMenuOption> GenerateSocialOptions(Pawn pawn, LocalTargetInfo target)
		{
			List<FloatMenuOption> opts = new List<FloatMenuOption>();
			FloatMenuOption option = null;

			option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_Insult".Translate(), delegate ()
			{
				Socialize(pawn, target, InteractionDefOf.Insult);
			}, MenuOptionPriority.High), pawn, target);
			opts.AddDistinct(option);

			if (!pawn.HostileTo(target.Pawn))
			{
				option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_SocialFight".Translate(), delegate ()
				{
					SocializeFight(pawn, target);
				}, MenuOptionPriority.High), pawn, target);
				opts.AddDistinct(option);

				option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_Chat".Translate(), delegate ()
				{
					Socialize(pawn, target, InteractionDefOf.Chitchat);
				}, MenuOptionPriority.High), pawn, target);
				opts.AddDistinct(option);

				// OP shit +12 relations, enable in cheat menu
				if (RJWSettings.Allow_RMB_DeepTalk)
				{
					option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_DeepTalk".Translate(), delegate ()
					{
						pawn.interactions.TryInteractWith(target.Pawn, InteractionDefOf.DeepTalk);
					}, MenuOptionPriority.High), pawn, target);
					opts.AddDistinct(option);
				}

				if (!LovePartnerRelationUtility.LovePartnerRelationExists(pawn, target.Pawn))
				{
					option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_RomanceAttempt".Translate(), delegate ()
					{
						Socialize(pawn, target, InteractionDefOf.RomanceAttempt);
					}, MenuOptionPriority.High), pawn, target);
					opts.AddDistinct(option);
				}

				if (pawn.relations.DirectRelationExists(PawnRelationDefOf.Lover, target.Pawn) || pawn.relations.DirectRelationExists(PawnRelationDefOf.Fiance, target.Pawn))
				{
					option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RJW_RMB_MarriageProposal".Translate(), delegate ()
					{
						Socialize(pawn, target, InteractionDefOf.MarriageProposal);
					}, MenuOptionPriority.High), pawn, target);
					opts.AddDistinct(option); 
				}
			}

			return opts;
		}

		//multiplayer synch actions
		[SyncMethod]
		static void SocializeFight(Pawn pawn, LocalTargetInfo target)
		{
			pawn.interactions.StartSocialFight(target.Pawn);
		}
		[SyncMethod]
		static void Socialize(Pawn pawn, LocalTargetInfo target, InteractionDef interaction)
		{
			pawn.interactions.TryInteractWith(target.Pawn, interaction);
		}
	}
}