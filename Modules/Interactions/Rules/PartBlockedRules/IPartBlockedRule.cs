using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Objects.Parts;
using rjw.Modules.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.Rules.PartBlockedRules
{
	public interface IPartBlockedRule
	{
		IEnumerable<LewdablePartKind> BlockedParts(InteractionPawn pawn);
	}
}
