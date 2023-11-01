using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared;
using rjw.Modules.Shared.Implementation;
using System.Collections.Generic;

namespace rjw.Modules.Interactions.Rules.PartBlockedRules.Implementation
{
	public class DownedPartBlockedRule : IPartBlockedRule
	{
		public static IPartBlockedRule Instance { get; private set; }

		static DownedPartBlockedRule()
		{
			Instance = new DownedPartBlockedRule();

			_pawnStateService = PawnStateService.Instance;
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private DownedPartBlockedRule() { }

		private static readonly IPawnStateService _pawnStateService;

		public IEnumerable<LewdablePartKind> BlockedParts(InteractionPawn pawn)
		{
			yield break;

			//if (_pawnStateService.Detect(pawn.Pawn) == Shared.Enums.PawnState.Downed)
			//{
			//}
		}
	}
}
