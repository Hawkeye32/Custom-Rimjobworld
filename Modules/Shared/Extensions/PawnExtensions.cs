using System;
using Verse;

namespace rjw.Modules.Shared.Extensions
{
	public static class PawnExtensions
	{
		public static string GetName(this Pawn pawn)
		{
			if (pawn == null)
			{
				return "null";
			}

			if (String.IsNullOrWhiteSpace(pawn.Name?.ToStringFull) == false)
			{
				return pawn.Name.ToStringFull;
			}

			return pawn.def.defName;
		}
	}
}
