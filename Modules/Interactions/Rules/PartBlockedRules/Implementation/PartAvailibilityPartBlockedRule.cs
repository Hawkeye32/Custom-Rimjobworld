using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Rules.PartBlockedRules.Implementation
{
	public class PartAvailibilityPartBlockedRule : IPartBlockedRule
	{
		public static IPartBlockedRule Instance { get; private set; }

		static PartAvailibilityPartBlockedRule()
		{
			Instance = new PartAvailibilityPartBlockedRule();
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private PartAvailibilityPartBlockedRule() { }

		public IEnumerable<LewdablePartKind> BlockedParts(InteractionPawn pawn)
		{
			if (pawn.Parts.Hands.Any() == false)
			{
				yield return LewdablePartKind.Hand;
			}
			if (pawn.Parts.Mouths.Any() == false)
			{
				yield return LewdablePartKind.Mouth;
			}
			if (pawn.Parts.Tails.Any() == false)
			{
				yield return LewdablePartKind.Tail;
			}
			//No feet detection, pawn always have thoose ... guess you can still do a good job with a peg leg !
			//if (pawn.Parts.Feet.Any() == false)
			//{
			//	yield return LewdablePartKind.Foot;
			//}

			if (pawn.Parts.Penises.Any() == false)
			{
				yield return LewdablePartKind.Penis;
			}
			if (pawn.Parts.Vaginas.Any() == false)
			{
				yield return LewdablePartKind.Vagina;
			}
			if (pawn.Parts.FemaleOvipositors.Any() == false)
			{
				yield return LewdablePartKind.FemaleOvipositor;
			}
			if (pawn.Parts.MaleOvipositors.Any() == false)
			{
				yield return LewdablePartKind.MaleOvipositor;
			}
			if (pawn.Parts.Anuses.Any() == false)
			{
				yield return LewdablePartKind.Anus;
			}
			if (pawn.Parts.Breasts.Where(e => e.Hediff.CurStageIndex > 1).Any() == false)
			{
				yield return LewdablePartKind.Breasts;
			}
		}
	}
}
