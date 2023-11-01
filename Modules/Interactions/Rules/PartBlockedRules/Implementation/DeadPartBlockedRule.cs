using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared;
using rjw.Modules.Shared.Implementation;
using System.Collections.Generic;

namespace rjw.Modules.Interactions.Rules.PartBlockedRules.Implementation
{
	public class DeadPartBlockedRule : IPartBlockedRule
	{
		public static IPartBlockedRule Instance { get; private set; }

		static DeadPartBlockedRule()
		{
			Instance = new DeadPartBlockedRule();

			_pawnStateService = PawnStateService.Instance;
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private DeadPartBlockedRule() { }

		private static readonly IPawnStateService _pawnStateService;

		public IEnumerable<LewdablePartKind> BlockedParts(InteractionPawn pawn)
		{
			yield break;

			//if (_pawnStateService.Detect(pawn.Pawn) == Shared.Enums.PawnState.Dead)
			//{
			//}
		}
	}
}
