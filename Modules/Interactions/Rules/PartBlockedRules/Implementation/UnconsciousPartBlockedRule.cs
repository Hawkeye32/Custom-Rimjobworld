using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared;
using rjw.Modules.Shared.Implementation;
using System.Collections.Generic;

namespace rjw.Modules.Interactions.Rules.PartBlockedRules.Implementation
{
	public class UnconsciousPartBlockedRule : IPartBlockedRule
	{
		public static IPartBlockedRule Instance { get; private set; }

		static UnconsciousPartBlockedRule()
		{
			Instance = new UnconsciousPartBlockedRule();
			_pawnStateService = PawnStateService.Instance;
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private UnconsciousPartBlockedRule() { }

		private static readonly IPawnStateService _pawnStateService;

		public IEnumerable<LewdablePartKind> BlockedParts(InteractionPawn pawn)
		{
			yield break;

			//if (_pawnStateService.Detect(pawn.Pawn) == Shared.Enums.PawnState.Unconscious)
			//{
			//}
		}
	}
}
