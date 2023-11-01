using System;
using Verse;

namespace rjw
{
	public static class SexPartTypeExtensions
	{
		public static BodyPartDef GetBodyPartDef(this SexPartType sexPartType)
		{
			return sexPartType switch
			{
				SexPartType.Anus => xxx.anusDef,
				SexPartType.FemaleBreast => xxx.breastsDef,
				SexPartType.FemaleGenital => xxx.genitalsDef,
				SexPartType.MaleBreast => xxx.breastsDef,
				SexPartType.MaleGenital => xxx.genitalsDef,
				_ => throw new ArgumentException($"Unrecognized sexPartType: {sexPartType}")
			};
		}
	}
}
