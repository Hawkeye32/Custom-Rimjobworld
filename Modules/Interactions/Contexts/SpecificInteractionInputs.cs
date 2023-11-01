using RimWorld;
using Verse;

namespace rjw.Modules.Interactions.Contexts
{
	public class SpecificInteractionInputs
	{
		public Pawn Initiator { get; set; }
		public Pawn Partner { get; set; }

		public InteractionDef Interaction { get; set; }
	}
}
