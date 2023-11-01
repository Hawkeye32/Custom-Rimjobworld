using System.Collections.Generic;
using System.Text;

using RimWorld;
using Verse;

namespace rjw
{
	internal class InteractionWorker_AnalSexAttempt : InteractionWorker
	{
		//initiator - rapist
		//recipient - victim
		public static bool AttemptAnalSex(Pawn initiator, Pawn recipient)
		{
			//--Log.Message(xxx.get_pawnname(initiator) + " is attempting to anally rape " + xxx.get_pawnname(recipient));
			return true;
		}

		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			// this interaction is triggered by the jobdriver
			if (initiator == null || recipient == null)
				return 0.0f;

			return 0.0f; // base.RandomSelectionWeight(initiator, recipient);
		}

		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
			//add something fancy here later?
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = recipient;
			//Find.LetterStack.ReceiveLetter("Rape attempt", "A wandering nymph has decided to join your colony.", LetterDefOf.NegativeEvent, recipient);

			if (initiator == null || recipient == null)
				return;
			//--ModLog.Message(" InteractionWorker_AnalRapeAttempt::Interacted( " + xxx.get_pawnname(initiator) + ", " + xxx.get_pawnname(recipient) + " ) called");
			AttemptAnalSex(initiator, recipient);
		}
	}

	internal class InteractionWorker_VaginalSexAttempt : InteractionWorker
	{
		//initiator - rapist
		//recipient - victim
		public static bool AttemptAnalSex(Pawn initiator, Pawn recipient)
		{
			//--Log.Message(xxx.get_pawnname(initiator) + " is attempting to anally rape " + xxx.get_pawnname(recipient));
			return false;
		}

		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			// this interaction is triggered by the jobdriver
			if (initiator == null || recipient == null)
				return 0.0f;

			return 0.0f; // base.RandomSelectionWeight(initiator, recipient);
		}

		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
			//add something fancy here later?
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = recipient;
			//Find.LetterStack.ReceiveLetter("Rape attempt", "A wandering nymph has decided to join your colony.", LetterDefOf.NegativeEvent, recipient);

			if (initiator == null || recipient == null)
				return;
			//--ModLog.Message(" InteractionWorker_AnalRapeAttempt::Interacted( " + xxx.get_pawnname(initiator) + ", " + xxx.get_pawnname(recipient) + " ) called");
			AttemptAnalSex(initiator, recipient);
		}
	}

	internal class InteractionWorker_OtherSexAttempt : InteractionWorker
	{
		//initiator - rapist
		//recipient - victim
		public static bool AttemptAnalSex(Pawn initiator, Pawn recipient)
		{
			//--Log.Message(xxx.get_pawnname(initiator) + " is attempting to anally rape " + xxx.get_pawnname(recipient));
			return false;
		}

		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			// this interaction is triggered by the jobdriver
			if (initiator == null || recipient == null)
				return 0.0f;

			return 0.0f; // base.RandomSelectionWeight(initiator, recipient);
		}

		public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
		{
			//add something fancy here later?
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = recipient;
			//Find.LetterStack.ReceiveLetter("Rape attempt", "A wandering nymph has decided to join your colony.", LetterDefOf.NegativeEvent, recipient);

			if (initiator == null || recipient == null)
				return;
			//--ModLog.Message(" InteractionWorker_AnalRapeAttempt::Interacted( " + xxx.get_pawnname(initiator) + ", " + xxx.get_pawnname(recipient) + " ) called");
			AttemptAnalSex(initiator, recipient);
		}
	}
}