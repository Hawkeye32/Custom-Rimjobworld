using RimWorld;
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
	public class AnimalPartKindUsageRule : IPartPreferenceRule
	{
		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForDominant(InteractionContext context)
		{
			if (xxx.is_animal(context.Internals.Dominant.Pawn))
			{
				return ForAnimal();
			}

			return Enumerable.Empty<Weighted<LewdablePartKind>>();
		}

		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForSubmissive(InteractionContext context)
		{
			if (xxx.is_animal(context.Internals.Submissive.Pawn))
			{
				return ForAnimal();
			}

			return Enumerable.Empty<Weighted<LewdablePartKind>>();
		}

		private IEnumerable<Weighted<LewdablePartKind>> ForAnimal()
		{
			yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Vagina);
			yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Penis);
			yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.FemaleOvipositor);
			yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.MaleOvipositor);

			yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Foot);
			yield return new Weighted<LewdablePartKind>(Multipliers.Never, LewdablePartKind.Tail);
			yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Hand);
			yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Hand);
			yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Breasts);

			yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Mouth);
			yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Tongue);
			yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Beak);
		}
	}
}
