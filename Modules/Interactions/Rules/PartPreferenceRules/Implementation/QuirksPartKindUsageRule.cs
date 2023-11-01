using RimWorld;
using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Quirks;
using rjw.Modules.Quirks.Implementation;
using rjw.Modules.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Rules.PartKindUsageRules.Implementation
{
	public class QuirksPartKindUsageRule : IPartPreferenceRule
	{
		private IQuirkService _quirkService => QuirkService.Instance;

		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForDominant(InteractionContext context)
		{
			return Enumerable.Concat(
				ModifierForPodophile(context.Internals.Dominant, context.Internals.Submissive, true),
				ModifierForImpregnationFetish(context.Internals.Dominant, context.Internals.Submissive, true)
				);

		}

		public IEnumerable<Weighted<LewdablePartKind>> ModifiersForSubmissive(InteractionContext context)
		{
			return Enumerable.Concat(
				ModifierForPodophile(context.Internals.Submissive, context.Internals.Dominant, false),
				ModifierForImpregnationFetish(context.Internals.Submissive, context.Internals.Dominant, false)
				);
		}

		private IEnumerable<Weighted<LewdablePartKind>> ModifierForPodophile(InteractionPawn pawn, InteractionPawn partner, bool isDominant)
		{
			if(_quirkService.HasQuirk(pawn.Pawn, Quirks.Quirks.Podophile))
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Doubled, LewdablePartKind.Foot);
			}
			
			//Partner is podophile and dominant (aka requesting pawjob !)
			if(_quirkService.HasQuirk(partner.Pawn, Quirks.Quirks.Podophile) && isDominant == false)
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Doubled, LewdablePartKind.Foot);
			}
		}

		private IEnumerable<Weighted<LewdablePartKind>> ModifierForImpregnationFetish(InteractionPawn pawn, InteractionPawn partner, bool isDominant)
		{
			if (_quirkService.HasQuirk(pawn.Pawn, Quirks.Quirks.Impregnation))
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Doubled, LewdablePartKind.Vagina);
			}

			//Partner likes impregnation and is requesting the deal !
			if (_quirkService.HasQuirk(partner.Pawn, Quirks.Quirks.Impregnation) && isDominant == false)
			{
				yield return new Weighted<LewdablePartKind>(Multipliers.Doubled, LewdablePartKind.Penis);
			}
		}
	}
}
