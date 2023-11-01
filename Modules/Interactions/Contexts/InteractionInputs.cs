using Verse;

namespace rjw.Modules.Interactions.Contexts
{
	public class InteractionInputs
	{
		public Pawn Initiator { get; set; }
		public Pawn Partner { get; set; }

		public bool IsRape { get; set; }
		public bool IsWhoring { get; set; }
	}
}
