using RimWorld;
using UnityEngine;
using Verse;

namespace rjw.MainTab.Checkbox
{
	[StaticConstructorOnStartup]
	public class PawnColumnWorker_Hero : PawnColumnCheckbox
	{
		//public static readonly Texture2D CheckboxOnTex = ContentFinder<Texture2D>.Get("UI/Commands/Hero_on");
		//public static readonly Texture2D CheckboxOffTex = ContentFinder<Texture2D>.Get("UI/Commands/Hero_off");
		//public static readonly Texture2D CheckboxDisabledTex = ContentFinder<Texture2D>.Get("UI/Commands/ComfortPrisoner_Refuse");

		//public static readonly Texture2D iconAccept = ContentFinder<Texture2D>.Get("UI/Commands/Hero_off");
		//static readonly Texture2D iconCancel = ContentFinder<Texture2D>.Get("UI/Commands/Hero_on");
		//protected override Texture2D GetIconFor(Pawn pawn)
		//{
		//	return pawn.CanDesignateHero() ? pawn.IsDesignatedHero() ? iconAccept : iconCancel : null;
		//}

		//protected override string GetIconTip(Pawn pawn)
		//{
		//	return "PawnColumnWorker_IsHero".Translate();
		//	;
		//}

		//public static Texture2D CheckboxOnTex = ContentFinder<Texture2D>.Get("UI/Commands/Hero_on");
		//public static Texture2D CheckboxOffTex = ContentFinder<Texture2D>.Get("UI/Commands/Hero_off");
		protected override bool HasCheckbox(Pawn pawn)
		{
			return pawn.CanDesignateHero() || pawn.IsDesignatedHero();
		}

		protected override bool GetValue(Pawn pawn)
		{
			return pawn.IsDesignatedHero();
		}

		protected override void SetValue(Pawn pawn, bool value)
		{
			if (pawn.IsDesignatedHero()) return;
			pawn.ToggleHero();

			//reload/update tab
			var rjwtab = DefDatabase<MainButtonDef>.GetNamed("RJW_MainButton");
			Find.MainTabsRoot.ToggleTab(rjwtab, false);//off
			Find.MainTabsRoot.ToggleTab(rjwtab, false);//on
		}
	}
}