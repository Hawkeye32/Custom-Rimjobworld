using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;

namespace rjw.Modules.Interactions.Contexts
{
	/// <summary>
	/// All the stuff stored in there shouldn't be used by other modules ... hence the "internal"
	/// </summary>
	public class InteractionInternals
	{
		public InteractionPawn Dominant { get; set; }
		public InteractionPawn Submissive { get; set; }

		public InteractionType InteractionType { get; set; }

		public bool IsReverse { get; set; }

		public InteractionWithExtension Selected { get; set; }

		public InteractionInternals()
		{
		}
	}
}
