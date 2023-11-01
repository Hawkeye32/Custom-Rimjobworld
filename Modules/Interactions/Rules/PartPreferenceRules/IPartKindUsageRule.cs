using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared;
using System.Collections.Generic;

namespace rjw.Modules.Interactions.Rules.PartKindUsageRules
{
	public interface IPartPreferenceRule
	{
		IEnumerable<Weighted<LewdablePartKind>> ModifiersForDominant(InteractionContext context);
		IEnumerable<Weighted<LewdablePartKind>> ModifiersForSubmissive(InteractionContext context);
	}
}
