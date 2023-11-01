using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Internals
{
	public interface IInteractionRequirementService
	{
		bool FufillRequirements(InteractionWithExtension interaction, InteractionPawn dominant, InteractionPawn submissive);
	}
}
