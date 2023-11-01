using rjw.Modules.Interactions.Defs.DefFragment;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Objects.Parts;
using System.Collections.Generic;

namespace rjw.Modules.Interactions.Internals
{
	public interface IPartSelectorService
	{
		IList<ILewdablePart> SelectPartsForPawn(InteractionPawn pawn, InteractionRequirement requirement);
	}
}
