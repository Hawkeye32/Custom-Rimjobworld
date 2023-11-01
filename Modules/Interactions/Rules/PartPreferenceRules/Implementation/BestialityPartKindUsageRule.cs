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
	public class BestialityPartKindUsageRule : IPartPreferenceRule
	{
		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForDominant(InteractionContext context)
		{
			//Only bestiality here
			if (context.Internals.InteractionType != InteractionType.Bestiality)
			{
				return Enumerable.Empty<Weighted<LewdablePartKind>>();
			}

			InteractionPawn pawn = context.Internals.Dominant;

			if (xxx.is_zoophile(pawn.Pawn) == false && xxx.is_animal(pawn.Pawn) == false && context.Internals.InteractionType == InteractionType.Bestiality)
			{
				return ForPawn(pawn, context.Internals.Submissive, context.Inputs.Initiator == pawn.Pawn);
			}

			return Enumerable.Empty<Weighted<LewdablePartKind>>();
		}

		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForSubmissive(InteractionContext context)
		{
			//Only bestiality here
			if (context.Internals.InteractionType != InteractionType.Bestiality)
			{
				return Enumerable.Empty<Weighted<LewdablePartKind>>();
			}

			InteractionPawn pawn = context.Internals.Submissive;

			if (xxx.is_zoophile(pawn.Pawn) == false && xxx.is_animal(pawn.Pawn) == false)
			{
				return ForPawn(pawn, context.Internals.Dominant, context.Inputs.Initiator == pawn.Pawn);
			}

			return Enumerable.Empty<Weighted<LewdablePartKind>>();
		}

		private IEnumerable<Weighted<LewdablePartKind>> ForPawn(InteractionPawn pawn, InteractionPawn animal, bool isInitiator)
		{
			//Well ... the pawn probably would not want anything penetrative ...
			if (isInitiator == false)
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Vagina);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Penis);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.FemaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.MaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Anus);

				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Breasts);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Tail);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Hand);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Foot);

				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Mouth);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Tongue);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Beak);
			}

			//bonded
			if (pawn.Pawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, animal.Pawn))
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Average, LewdablePartKind.Vagina);
				yield return new Weighted<LewdablePartKind>(Multipliers.Average, LewdablePartKind.Penis);
				yield return new Weighted<LewdablePartKind>(Multipliers.Average, LewdablePartKind.FemaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.Average, LewdablePartKind.MaleOvipositor);

				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Anus);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Breasts);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Tail);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Hand);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Foot);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Mouth);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Tongue);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Beak);
			}
			else
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Vagina);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Penis);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.FemaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.MaleOvipositor);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Anus);

				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Breasts);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Tail);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Hand);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Foot);

				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Mouth);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Tongue);
				yield return new Weighted<LewdablePartKind>(Multipliers.AlmostNever, LewdablePartKind.Beak);
			}
		}
	}
}
