using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Extensions;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Objects.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class PartFinderService : IPartFinderService
	{
		public static IPartFinderService Instance { get; private set; }

		static PartFinderService()
		{
			Instance = new PartFinderService();
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private PartFinderService() { }

		public IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, string partProp)
		{
			return pawn.ListLewdableParts()
				.Where(e => pawn.BlockedParts.Contains(e.PartKind) == false)
				.OfType<RJWLewdablePart>()
				.Where(e => e.Props.Contains(partProp));
		}

		public IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, LewdablePartKind partKind)
		{
			if (pawn.BlockedParts.Contains(partKind))
			{
				return Enumerable.Empty<ILewdablePart>();
			}

			return pawn.ListLewdableParts()
				.Where(e => e.PartKind == partKind);
		}

		public IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, GenitalFamily family)
		{
			return pawn.ListLewdableParts()
				.Where(e => pawn.BlockedParts.Contains(e.PartKind) == false)
				.OfType<RJWLewdablePart>()
				.Where(e => e.Hediff.GenitalPart.family == family);
		}

		public IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, GenitalFamily family, IList<string> partProp)
		{
			var eligibles = pawn.ListLewdableParts()
				.Where(e => pawn.BlockedParts.Contains(e.PartKind) == false)
				.OfType<RJWLewdablePart>()
				.Where(e => e.Hediff.GenitalPart.family == family);

			if (partProp == null || partProp.Any() == false)
			{
				return eligibles;
			}

			return eligibles
				.Where(e => e.Props.Any())
				.Where(e => partProp.All(prop => e.Props.Contains(prop)));
		}

		public IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, GenitalTag tag)
		{
			return pawn.ListLewdableParts()
				.Where(e => pawn.BlockedParts.Contains(e.PartKind) == false)
				.OfType<RJWLewdablePart>()
				.Where(e => e.Hediff.GenitalPart.tags.Contains(tag));
		}

		public IEnumerable<ILewdablePart> FindUnblockedForPawn(InteractionPawn pawn, GenitalTag tag, IList<string> partProp)
		{
			var eligibles = pawn.ListLewdableParts()
				.Where(e => pawn.BlockedParts.Contains(e.PartKind) == false)
				.OfType<RJWLewdablePart>()
				.Where(e => e.Hediff.GenitalPart.tags.Contains(tag));

			if (partProp == null || partProp.Any() == false)
			{
				return eligibles;
			}

			return eligibles
				.Where(e => e.Props.Any())
				.Where(e => partProp.All(prop => e.Props.Contains(prop)));
		}

	}
}
