using rjw.Modules.Interactions.Defs;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Extensions
{
	public static class InteractionWithExtensions
	{
		public static IEnumerable<InteractionWithExtension> Reverse(this IEnumerable<InteractionWithExtension> self)
		{
			return self
				.Where(interaction => interaction.HasInteractionTag(InteractionTag.Reverse));
		}
		public static IEnumerable<InteractionWithExtension> NonReverse(this IEnumerable<InteractionWithExtension> self)
		{
			return self
				.Where(interaction => interaction.HasInteractionTag(InteractionTag.Reverse) == false);
		}
		public static IEnumerable<InteractionWithExtension> FilterReverse(this IEnumerable<InteractionWithExtension> self, bool isReverse)
		{
			return self
				.Where(interaction => interaction.HasInteractionTag(InteractionTag.Reverse) == isReverse);
		}
	}
}
