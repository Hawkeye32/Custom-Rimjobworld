using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using System.Collections.Generic;

namespace rjw.Modules.Interactions.Internals
{
	public interface IBlockedPartDetectorService
	{
		IList<LewdablePartKind> BlockedPartsForPawn(InteractionPawn pawn);

		/// <summary>
		/// Detect the blocked parts for both pawn and fill
		/// <see cref="InteractionPawn.BlockedParts"/>
		/// </summary>
		/// <param name="context"></param>
		void DetectBlockedParts(InteractionInternals context);
	}
}
