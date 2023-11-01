using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Objects.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Internals
{
	public interface IPartFinderService
	{
		IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, string partProp);
		IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, LewdablePartKind partKind);
		IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, GenitalFamily family);
		IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, GenitalFamily family, IList<string> partProp);
		IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, GenitalTag tag);
		IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, GenitalTag tag, IList<string> partProp);
	}
}
