using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.Internals
{
	public interface IRulePackService
	{
		RulePackDef FindInteractionRulePack(InteractionWithExtension interaction);
	}
}
