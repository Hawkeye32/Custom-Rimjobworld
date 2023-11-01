using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Rules.PartKindUsageRules.Implementation
{
	public class WhoringPartKindUsageRule : IPartPreferenceRule
	{
		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForDominant(InteractionContext context)
		{
			if (context.Internals.InteractionType == InteractionType.Whoring)
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.Vagina);
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.Penis);
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.FemaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.MaleOvipositor);

				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Mouth);
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Tongue);
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Beak);
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Breasts);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Anus);

				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Hand);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Foot);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Tail);
			}
		}

		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForSubmissive(InteractionContext context)
		{
			if (context.Internals.InteractionType == InteractionType.Whoring)
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.Vagina);
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.Penis);
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.FemaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.MaleOvipositor);

				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Mouth);
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Tongue);
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Beak);
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Breasts);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Anus);

				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Hand);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Foot);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Tail);
			}
		}
	}
}
