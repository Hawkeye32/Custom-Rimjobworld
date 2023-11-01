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
	public class RapePartKindUsageRule : IPartPreferenceRule
	{
		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForDominant(InteractionContext context)
		{
			if (context.Internals.InteractionType == InteractionType.Rape)
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Anus);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Mouth);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Tongue);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Beak);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Breasts);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Vagina);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Penis);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.FemaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.MaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Tail);

				yield return new Weighted<LewdablePartKind>(Multipliers.Average, LewdablePartKind.Hand);
				yield return new Weighted<LewdablePartKind>(Multipliers.Average, LewdablePartKind.Foot);
			}
		}

		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForSubmissive(InteractionContext context)
		{
			if (context.Internals.InteractionType == InteractionType.Rape)
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Anus);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Vagina);

				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Mouth);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Tongue);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Beak);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Breasts);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Penis);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.FemaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.MaleOvipositor);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Tail);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Hand);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Foot);
			}
		}
	}
}
