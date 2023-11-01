using System.Collections.Generic;
using RimWorld;
using Verse;

using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Shared;
using rjw.Modules.Interactions.Objects;
using System.Linq;

namespace rjw.Modules.Interactions.Rules.PartKindUsageRules.Implementation
{
	public class PregnancyApproachPartKindUsageRule : IPartPreferenceRule
	{
		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForDominant(InteractionContext context)
		{
            return ModifiersForEither(context.Internals.Dominant, context.Internals.Submissive);
		}

		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForSubmissive(InteractionContext context)
		{
            return ModifiersForEither(context.Internals.Submissive, context.Internals.Dominant);
		}

        public IEnumerable<Weighted<LewdablePartKind>> ModifiersForEither(InteractionPawn OwO, InteractionPawn UwU)
        {
            if (OwO.Pawn.relations == null || UwU.Pawn.relations == null)
            {
                yield break;
            }

            float weight = OwO.Pawn.relations.GetPregnancyApproachForPartner(UwU.Pawn).GetPregnancyChanceFactor();
            if (OwO.Parts.Vaginas.Any() && UwU.Parts.Penises.Any())
            {
                yield return new Weighted<LewdablePartKind>(weight, LewdablePartKind.Penis);
            }
            if (OwO.Parts.Penises.Any() && UwU.Parts.Vaginas.Any())
            {
                yield return new Weighted<LewdablePartKind>(weight, LewdablePartKind.Vagina);
            }
        }
	}
}