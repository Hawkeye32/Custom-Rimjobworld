using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Shared.Extensions
{
	public static class BodyPartRecordExtension
	{
		public static bool IsMissingForPawn(this BodyPartRecord self, Pawn pawn)
		{
			if (pawn == null)
			{
				throw new ArgumentNullException(nameof(pawn));
			}
			if (self == null)
			{
				throw new ArgumentNullException(nameof(self));
			}

			return pawn.health.hediffSet.hediffs
				.Where(hediff => hediff.Part == self)
				.Where(hediff => hediff is Hediff_MissingPart)
				.Any();
		}
	}
}
