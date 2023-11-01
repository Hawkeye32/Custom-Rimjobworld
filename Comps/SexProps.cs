using Verse;
using RimWorld;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// data for sex related stuff/outcome
	/// </summary>
	public class SexProps : IExposable
	{
		public Pawn pawn;
		public Pawn partner;
		public bool hasPartner() => partner != null && partner != pawn;

		public xxx.rjwSextype sexType = xxx.rjwSextype.None;
		public InteractionDef dictionaryKey = null;
		public string rulePack = null;

		public bool usedCondom = false;
		public bool isRape = false;
		public bool isReceiver = false;// as JobDriver_SexBaseReciever
		public bool isRevese = false;// interaction.HasInteractionTag(InteractionTag.Reverse)
		public bool isRapist = false;
		public bool isCoreLovin = false;//should really clean this up someday
		public bool isWhoring = false;
		public bool canBeGuilty = true;// can initiator pawn be counted guilty for percepts, player initiated/rmb actrions = false

		public int orgasms = 0; // The orgasms had by the paw
		public SexProps()
		{
		}
		
		public bool IsInitiator() => !isReceiver;

		public bool IsSubmissive() => (isReceiver && !isRevese) || (!isReceiver && isRevese);

		public SexProps GetForPartner()
		{
			return new SexProps
			{
				pawn = partner,
				partner = pawn,
				sexType = sexType,
				dictionaryKey = dictionaryKey,
				rulePack = rulePack,
				usedCondom = usedCondom,
				isRape = isRape,
				isReceiver = !isReceiver,
				isRevese = isRevese,
				isRapist = isRapist,
				isCoreLovin = isCoreLovin,
				isWhoring = isWhoring,
				canBeGuilty = canBeGuilty,
				orgasms = orgasms
			};
		}

		public void ExposeData()
		{
			Scribe_References.Look(ref pawn, "pawn");
			Scribe_References.Look(ref partner, "partner");

			Scribe_Values.Look(ref sexType, "sexType");
			Scribe_Defs.Look(ref dictionaryKey, "dictionaryKey");
			Scribe_Values.Look(ref rulePack, "rulePack");

			Scribe_Values.Look(ref usedCondom, "usedCondom");
			Scribe_Values.Look(ref isRape, "isRape");
			Scribe_Values.Look(ref isReceiver, "isReceiver");
			Scribe_Values.Look(ref isRapist, "isRapist");
			Scribe_Values.Look(ref isCoreLovin, "isCoreLovin");
			Scribe_Values.Look(ref isWhoring, "isWhoring");
			Scribe_Values.Look(ref canBeGuilty, "canBeGuilty");
			Scribe_Values.Look(ref orgasms, "orgasms");
		}
	}
}
