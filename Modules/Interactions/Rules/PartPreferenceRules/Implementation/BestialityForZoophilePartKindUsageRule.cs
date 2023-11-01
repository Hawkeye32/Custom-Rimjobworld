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
	public class BestialityForZoophilePartKindUsageRule : IPartPreferenceRule
	{
		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForDominant(InteractionContext context)
		{
			//Only bestiality here
			if (context.Internals.InteractionType != InteractionType.Bestiality)
			{
				return Enumerable.Empty<Weighted<LewdablePartKind>>();
			}

			InteractionPawn pawn = context.Internals.Dominant;

			if (xxx.is_zoophile(pawn.Pawn) && xxx.is_animal(pawn.Pawn) == false)
			{
				return ForPawn(pawn, context.Internals.Submissive);
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

			if (xxx.is_zoophile(pawn.Pawn) && xxx.is_animal(pawn.Pawn) == false)
			{
				return ForPawn(pawn, context.Internals.Dominant);
			}

			return Enumerable.Empty<Weighted<LewdablePartKind>>();
		}

		private IEnumerable<Weighted<LewdablePartKind>> ForPawn(InteractionPawn pawn, InteractionPawn animal)
		{
			//bonded
			if (pawn.Pawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, animal.Pawn))
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryFrequent, LewdablePartKind.Vagina);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryFrequent, LewdablePartKind.Penis);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryFrequent, LewdablePartKind.FemaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryFrequent, LewdablePartKind.MaleOvipositor);

				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Anus);
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Breasts);
			}
			else
			//faction animal
			if (pawn.Pawn.Faction == animal.Pawn.Faction)
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.Vagina);
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.Penis);
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.FemaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.Frequent, LewdablePartKind.MaleOvipositor);

				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Breasts);
				yield return new Weighted<LewdablePartKind>(Multipliers.Uncommon, LewdablePartKind.Anus);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Tail);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Hand);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Foot);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Mouth);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Tongue);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Beak);
			}
			//wild or other faction
			else
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Vagina);
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.Penis);
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.FemaleOvipositor);
				yield return new Weighted<LewdablePartKind>(Multipliers.Common, LewdablePartKind.MaleOvipositor);

				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Breasts);
				yield return new Weighted<LewdablePartKind>(Multipliers.Rare, LewdablePartKind.Anus);

				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Tail);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Hand);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Foot);

				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Mouth);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Tongue);
				yield return new Weighted<LewdablePartKind>(Multipliers.VeryRare, LewdablePartKind.Beak);
			}
		}
	}
}
