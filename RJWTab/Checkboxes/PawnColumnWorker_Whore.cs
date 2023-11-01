using RimWorld;
using UnityEngine;
using Verse;

namespace rjw.MainTab.Checkbox
{
	[StaticConstructorOnStartup]
	public class PawnColumnWorker_Whore : PawnColumnWorker_Checkbox
	{
		public static readonly Texture2D CheckboxOnTex = ContentFinder<Texture2D>.Get("UI/Commands/Service_on");
		public static readonly Texture2D CheckboxOffTex = ContentFinder<Texture2D>.Get("UI/Commands/Service_off");
		public static readonly Texture2D CheckboxDisabledTex = ContentFinder<Texture2D>.Get("UI/Commands/Service_Refuse");
		protected override bool HasCheckbox(Pawn pawn)
		{
			return pawn.CanDesignateService();
		}
		protected bool GetDisabled(Pawn pawn)
		{
			return !pawn.CanDesignateService();
		}

		protected override bool GetValue(Pawn pawn)
		{
			return pawn.IsDesignatedService();
		}

		protected override void SetValue(Pawn pawn, bool value, PawnTable table)
		{
			pawn.ToggleService();
		}
	}
}
