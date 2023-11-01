using RimWorld;
using rjw.Modules.Interactions.DefModExtensions;
using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Helpers
{
	public static class InteractionHelper
	{
		public static InteractionWithExtension GetWithExtension(InteractionDef interactionDef)
		{
			if (interactionDef.HasModExtension<InteractionSelectorExtension>() == false)
			{
				return null;
			}

			if (interactionDef.HasModExtension<InteractionExtension>() == false)
			{
				return null;
			}

			return new InteractionWithExtension
			{
				Interaction = interactionDef,
				Extension = interactionDef.GetModExtension<InteractionExtension>(),
				SelectorExtension = interactionDef.GetModExtension<InteractionSelectorExtension>()
			};
		}
	}
}
