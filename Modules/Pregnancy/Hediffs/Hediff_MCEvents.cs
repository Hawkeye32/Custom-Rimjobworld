using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace rjw
{
	public class Hediff_Orgasm : HediffWithComps
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			string key = "FeltOrgasm";
			string text = TranslatorFormattedStringExtensions.Translate(key, pawn.LabelIndefinite()).CapitalizeFirst();
			Messages.Message(text, pawn, MessageTypeDefOf.NeutralEvent);
		}
	}

	public class Hediff_TransportCums : HediffWithComps
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			if (pawn.gender == Gender.Female)
			{
				string key = "CumsTransported";
				string text = TranslatorFormattedStringExtensions.Translate(key, pawn.LabelIndefinite()).CapitalizeFirst();
				Messages.Message(text, pawn, MessageTypeDefOf.NeutralEvent);
				PawnGenerationRequest req = new PawnGenerationRequest(PawnKindDefOf.Drifter, fixedGender:Gender.Male );
				Pawn cumSender = PawnGenerator.GeneratePawn(req);
				Find.WorldPawns.PassToWorld(cumSender);
				//Pawn cumSender = (from p in Find.WorldPawns.AllPawnsAlive where p.gender == Gender.Male select p).RandomElement<Pawn>();
				//--ModLog.Message("" + this.GetType().ToString() + "PostAdd() - Sending " + xxx.get_pawnname(cumSender) + "'s cum into " + xxx.get_pawnname(pawn) + "'s vagina");
				//PregnancyHelper.impregnate(pawn, cumSender, xxx.rjwSextype.Vaginal);
			}
			pawn.health.RemoveHediff(this);
		}
	}

	public class Hediff_TransportEggs : HediffWithComps
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			if (pawn.gender == Gender.Female)
			{
				string key = "EggsTransported";
				string text = TranslatorFormattedStringExtensions.Translate(key, pawn.LabelIndefinite()).CapitalizeFirst();
				Messages.Message(text, pawn, MessageTypeDefOf.NeutralEvent);

				PawnKindDef spawn = PawnKindDefOf.Megascarab;
				PawnGenerationRequest req1 = new PawnGenerationRequest(spawn, fixedGender:Gender.Female );
				PawnGenerationRequest req2 = new PawnGenerationRequest(spawn, fixedGender:Gender.Male );
				Pawn implanter = PawnGenerator.GeneratePawn(req1);
				Pawn fertilizer = PawnGenerator.GeneratePawn(req2);
				Find.WorldPawns.PassToWorld(implanter);
				Find.WorldPawns.PassToWorld(fertilizer);
				Sexualizer.sexualize_pawn(implanter);
				Sexualizer.sexualize_pawn(fertilizer);
				//Pawn cumSender = (from p in Find.WorldPawns.AllPawnsAlive where p.gender == Gender.Male select p).RandomElement<Pawn>();
				//--ModLog.Message("" + this.GetType().ToString() + "PostAdd() - Sending " + xxx.get_pawnname(cumSender) + "'s cum into " + xxx.get_pawnname(pawn) + "'s vagina");
				//PregnancyHelper.impregnate(pawn, implanter, xxx.rjwSextype.Vaginal);
				//PregnancyHelper.impregnate(pawn, fertilizer, xxx.rjwSextype.Vaginal);
			}
			pawn.health.RemoveHediff(this);
		}
	}
}