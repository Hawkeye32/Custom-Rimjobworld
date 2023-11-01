using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using Verse;

namespace rjw.Modules.Interactions.Internals
{
	public interface IInteractionTypeDetectorService
	{
		InteractionType DetectInteractionType(InteractionContext context);
	}
}
