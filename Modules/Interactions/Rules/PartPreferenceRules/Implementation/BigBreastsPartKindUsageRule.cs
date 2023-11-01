using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Rules.PartKindUsageRules.Implementation
{
	public class BigBreastsPartKindUsageRule : IPartPreferenceRule
	{
		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForDominant(InteractionContext context)
		{
			return Modifiers(context.Internals.Dominant);
		}

		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForSubmissive(InteractionContext context)
		{
			return Modifiers(context.Internals.Submissive);
		}

		public IEnumerable<Weighted<LewdablePartKind>> Modifiers(InteractionPawn pawn)
		{
			if (pawn.HasBigBreasts())
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Breasts);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Hand);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Foot);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Mouth);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Tongue);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Beak);
			}
		}
	}
}
