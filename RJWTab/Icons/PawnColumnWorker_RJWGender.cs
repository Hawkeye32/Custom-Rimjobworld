using RimWorld;
using UnityEngine;
using Verse;

using static rjw.GenderHelper;

namespace rjw.MainTab.Icon
{
	[StaticConstructorOnStartup]
	public class PawnColumnWorker_RJWGender : PawnColumnWorker_Gender
	{
		public static readonly Texture2D hermIcon = ContentFinder<Texture2D>.Get("UI/Icons/Gender/Genders", true);

		protected override Texture2D GetIconFor(Pawn pawn) => GetSex(pawn) switch
		{
			Sex.futa => hermIcon,
			_ => pawn.gender.GetIcon()
		};
		protected override string GetIconTip(Pawn pawn) => GetSex(pawn) switch
		{
			Sex.futa => "PawnColumnWorker_RJWGender_IsHerm".Translate(),
			_ => pawn.GetGenderLabel().CapitalizeFirst()
		};
	}
}