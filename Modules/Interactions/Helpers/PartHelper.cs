using RimWorld;
using rjw.Modules.Interactions.DefModExtensions;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rjw.Modules.Interactions.Extensions;
using rjw.Modules.Interactions.Objects.Parts;
using Verse;

namespace rjw.Modules.Interactions.Helpers
{
	public static class PartHelper
	{
		private readonly static Internals.IPartFinderService _partFinderService;
		private readonly static Internals.IBlockedPartDetectorService _blockedPartDetectorService;

		static PartHelper()
		{
			_partFinderService = Internals.Implementation.PartFinderService.Instance;
			_blockedPartDetectorService = Internals.Implementation.BlockedPartDetectorService.Instance;
		}

		public static IList<ILewdablePart> FindParts(Pawn pawn, GenitalFamily family)
		{
			InteractionPawn interactionPawn = ToInteractionPawn(pawn);

			return _partFinderService.FindUnblockedForPawn(interactionPawn, family)
				.ToList();
		}
		public static IList<ILewdablePart> FindParts(Pawn pawn, GenitalFamily family, IList<string> props)
		{
			InteractionPawn interactionPawn = ToInteractionPawn(pawn);

			return _partFinderService.FindUnblockedForPawn(interactionPawn, family, props)
				.ToList();
		}
		public static IList<ILewdablePart> FindParts(Pawn pawn, GenitalTag tag)
		{
			InteractionPawn interactionPawn = ToInteractionPawn(pawn);

			return _partFinderService.FindUnblockedForPawn(interactionPawn, tag)
				.ToList();
		}
		public static IList<ILewdablePart> FindParts(Pawn pawn, GenitalTag tag, IList<string> props)
		{
			InteractionPawn interactionPawn = ToInteractionPawn(pawn);

			return _partFinderService.FindUnblockedForPawn(interactionPawn, tag, props)
				.ToList();
		}

		private static InteractionPawn ToInteractionPawn(Pawn pawn)
		{
			 InteractionPawn interactionPawn = new InteractionPawn
			{
				Pawn = pawn,
				Parts = pawn.GetSexablePawnParts()
			};

			interactionPawn.BlockedParts = _blockedPartDetectorService.BlockedPartsForPawn(interactionPawn);

			return interactionPawn;
		}
	}
}
