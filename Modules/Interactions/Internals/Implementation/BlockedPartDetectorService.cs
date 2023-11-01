using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Rules.PartBlockedRules;
using rjw.Modules.Interactions.Rules.PartBlockedRules.Implementation;
using System.Collections.Generic;
using System.Linq;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class BlockedPartDetectorService : IBlockedPartDetectorService
	{
		public static IBlockedPartDetectorService Instance { get; private set; }

		static BlockedPartDetectorService()
		{
			Instance = new BlockedPartDetectorService();

			_partBlockedRules = new List<IPartBlockedRule>()
			{
				DeadPartBlockedRule.Instance,
				DownedPartBlockedRule.Instance,
				UnconsciousPartBlockedRule.Instance,

				MainPartBlockedRule.Instance,
				PartAvailibilityPartBlockedRule.Instance,
			};
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private BlockedPartDetectorService() { }

		private readonly static IList<IPartBlockedRule> _partBlockedRules;

		public void DetectBlockedParts(InteractionInternals context)
		{
			context.Dominant.BlockedParts = BlockedPartsForPawn(context.Dominant);
			context.Submissive.BlockedParts = BlockedPartsForPawn(context.Submissive);
		}

		public IList<LewdablePartKind> BlockedPartsForPawn(InteractionPawn pawn)
		{
			return _partBlockedRules
				.SelectMany(e => e.BlockedParts(pawn))
				//Eliminate the duplicates
				.Distinct()
				.ToList();
		}
	}
}
