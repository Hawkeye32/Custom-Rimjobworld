using RimWorld;
using UnityEngine;
using Verse;

namespace rjw.MainTab.Checkbox
{
	[StaticConstructorOnStartup]
	public class PawnColumnWorker_Comfort : PawnColumnWorker_Checkbox
	{
		public static readonly Texture2D CheckboxOnTex = ContentFinder<Texture2D>.Get("UI/Commands/ComfortPrisoner_on");
		public static readonly Texture2D CheckboxOffTex = ContentFinder<Texture2D>.Get("UI/Commands/ComfortPrisoner_off");
		public static readonly Texture2D CheckboxDisabledTex = ContentFinder<Texture2D>.Get("UI/Commands/ComfortPrisoner_Refuse");
		protected override bool HasCheckbox(Pawn pawn)
		{
			return pawn.CanDesignateComfort();
		}
		protected bool GetDisabled(Pawn pawn)
		{
			return !pawn.CanDesignateComfort();
		}

		protected override bool GetValue(Pawn pawn)
		{
			return pawn.IsDesignatedComfort();
		}

		protected override void SetValue(Pawn pawn, bool value, PawnTable table)
		{
			pawn.ToggleComfort();
		}
	}
}
