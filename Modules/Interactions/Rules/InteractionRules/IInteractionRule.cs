using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Rules.InteractionRules
{
	public interface IInteractionRule
	{
		InteractionType InteractionType { get; }

		IEnumerable<InteractionWithExtension> Interactions { get; }

		float SubmissivePreferenceWeight { get; }

		InteractionWithExtension Default { get; }
	}
}
