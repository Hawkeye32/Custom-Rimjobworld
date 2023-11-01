using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using System.Collections.Generic;

namespace rjw.Modules.Interactions.Rules.PartBlockedRules.Implementation
{
	public class MainPartBlockedRule : IPartBlockedRule
	{
		public static IPartBlockedRule Instance { get; private set; }

		static MainPartBlockedRule()
		{
			Instance = new MainPartBlockedRule();
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private MainPartBlockedRule() { }

		public IEnumerable<LewdablePartKind> BlockedParts(InteractionPawn pawn)
		{
			if (Genital_Helper.anus_blocked(pawn.Pawn))
			{
				yield return LewdablePartKind.Anus;
			}
			if (Genital_Helper.penis_blocked(pawn.Pawn))
			{
				yield return LewdablePartKind.Penis;
			}
			if (Genital_Helper.breasts_blocked(pawn.Pawn))
			{
				yield return LewdablePartKind.Breasts;
			}
			if (Genital_Helper.vagina_blocked(pawn.Pawn))
			{
				yield return LewdablePartKind.Vagina;
			}
			if (Genital_Helper.hands_blocked(pawn.Pawn))
			{
				yield return LewdablePartKind.Hand;
			}
			if (Genital_Helper.oral_blocked(pawn.Pawn))
			{
				yield return LewdablePartKind.Mouth;
				yield return LewdablePartKind.Beak;
				yield return LewdablePartKind.Tongue;
			}
			if (Genital_Helper.genitals_blocked(pawn.Pawn))
			{
				yield return LewdablePartKind.Penis;
				yield return LewdablePartKind.Vagina;
				yield return LewdablePartKind.FemaleOvipositor;
				yield return LewdablePartKind.MaleOvipositor;
			}
		}
	}
}
