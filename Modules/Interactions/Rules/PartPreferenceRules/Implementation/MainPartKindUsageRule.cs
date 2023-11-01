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
	public class MainPartKindUsageRule : IPartPreferenceRule
	{
		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForDominant(InteractionContext context)
		{
			return Modifiers(context.Internals.Dominant);
		}

		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForSubmissive(InteractionContext context)
		{
			return Modifiers(context.Internals.Submissive);
		}

		private IEnumerable<Weighted<LewdablePartKind>> Modifiers(InteractionPawn pawn)
		{
			bool hasMainPart = false;

			if (pawn.Parts.Penises.Any())
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.Penis);
				hasMainPart = true;
			}
			if (pawn.Parts.Vaginas.Any())
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.Vagina);
				hasMainPart = true;
			}
			if (pawn.Parts.FemaleOvipositors.Any())
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.FemaleOvipositor);
				hasMainPart = true;
			}
			if (pawn.Parts.MaleOvipositors.Any())
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.MaleOvipositor);
				hasMainPart = true;
			}

			//Since the pawn has a "main" part, we lower the rest
			if (hasMainPart == true)
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Anus);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Hand);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Foot);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Tail);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Breasts);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Mouth);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Tongue);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Beak);
			}
		}
	}
}
