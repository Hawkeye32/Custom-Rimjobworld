using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using Multiplayer.API;

namespace rjw
{
	[StaticConstructorOnStartup]
	public class SexGizmo : Gizmo
	{
		public SexGizmo(Pawn __instance)
		{
			this.pawn = __instance;
			//this.order = -100f;
			this.LimitedTex = ContentFinder<Texture2D>.Get("UI/Icons/EntropyLimit/Limited", true);
			this.UnlimitedTex = ContentFinder<Texture2D>.Get("UI/Icons/EntropyLimit/Unlimited", true);
			this.AttackTex = ContentFinder<Texture2D>.Get("UI/Icons/HostilityResponse/Attack", true);
			//this.LimitedTex = ContentFinder<Texture2D>.Get("UI/Icons/HostilityResponse/Ignore", true);
			//this.UnlimitedTex = ContentFinder<Texture2D>.Get("UI/Icons/HostilityResponse/Attack", true);
			//this.LimitedTex = ContentFinder<Texture2D>.Get("UI/Commands/AttackMelee", true);
			//this.UnlimitedTex = ContentFinder<Texture2D>.Get("UI/Commands/AttackMelee", true);
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			if (pawn.jobs?.curDriver is JobDriver_Sex)
			{
				Rect rect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
				Rect rect2 = rect.ContractedBy(6f);
				Widgets.DrawWindowBackground(rect);

				//row1 label1 - Orgasm
				Text.Font = GameFont.Small;
				Rect rect3 = rect2;
				rect3.y += 6f;
				rect3.height = Text.LineHeight;
				Widgets.Label(rect3, "RJW_SexGizmo_Orgasm".Translate());

				//row2 label2 - Stamina
				Rect rect4 = rect2;
				rect4.y += 38f;
				rect4.height = Text.LineHeight;
				Widgets.Label(rect4, "RJW_SexGizmo_RestNeed".Translate());

				//row1 meter bar fill up
				Rect rect5 = rect2;
				rect5.x += 63f;
				rect5.y += 6f;
				rect5.width = 100f;
				rect5.height = 22f;
				float orgasm = (pawn.jobs?.curDriver as JobDriver_Sex).OrgasmProgress;
				//orgasm = sex_ticks;
				Widgets.FillableBar(rect5, Mathf.Max(orgasm, 0f), SexGizmo.EntropyBarTex, SexGizmo.EmptyBarTex, true);

				//row1 meter values
				string label = (orgasm * 100).ToString("F0") + " / " + 100;
				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect5, label);

				Text.Anchor = TextAnchor.UpperLeft;
				Text.Font = GameFont.Tiny;
				GUI.color = Color.white;

				//row1 meter description
				//Rect rect6 = rect2;
				//rect6.width = 175f;
				//rect6.height = 38f;
				//TooltipHandler.TipRegion(rect6, delegate()
				//{
				//	float f = this.tracker.EntropyValue / this.tracker.RecoveryRate;
				//	return string.Format("PawnTooltipPsychicEntropyStats".Translate(), new object[]
				//	{
				//		Mathf.Round(this.tracker.EntropyValue),
				//		Mathf.Round(this.tracker.MaxEntropy),
				//		this.tracker.RecoveryRate.ToString("0.#"),
				//		Mathf.Round(f)
				//	}) + "\n\n" + "PawnTooltipPsychicEntropyDesc".Translate();
				//}, Gen.HashCombineInt(this.tracker.GetHashCode(), 133858));

				//row2 meter bar fill
				Rect rect7 = rect2;
				rect7.x += 63f;
				rect7.y += 38f;
				rect7.width = 100f;
				rect7.height = 22f;
				if (pawn.needs?.TryGetNeed<Need_Rest>() != null)
					Widgets.FillableBar(rect7, Mathf.Min(pawn.needs.TryGetNeed<Need_Rest>().CurLevel, 1f), SexGizmo.PsyfocusBarTex, SexGizmo.EmptyBarTex, true);

				GUI.color = Color.white;

				//toggles
				if (!MP.IsInMultiplayer)   //TODO: someday write mp synchronizers?
					if (pawn.IsColonistPlayerControlled && pawn.CanChangeDesignationColonist())
					{
						//row1 toggle - SexOverdrive
						if (pawn.jobs?.curDriver is JobDriver_SexBaseInitiator)
						{
							float num = 32f;
							float num2 = 4f;
							float num3 = rect2.height / 2f - num + num2;
							float num4 = rect2.width - num;
							Rect rect9 = new Rect(rect2.x + num4, rect2.y + num3, num, num);
							if (Widgets.ButtonImage(rect9, !(pawn.jobs?.curDriver as JobDriver_Sex).neverendingsex ? this.LimitedTex : this.UnlimitedTex, true))
							{
								if (pawn.jobs?.curDriver is JobDriver_SexBaseInitiator)
								{
									(pawn.jobs?.curDriver as JobDriver_Sex).neverendingsex = !(pawn.jobs?.curDriver as JobDriver_Sex).neverendingsex;
									if ((pawn.jobs?.curDriver as JobDriver_Sex).neverendingsex)
									{
										SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
									}
									else
									{
										SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
									}
								}
							}
							TooltipHandler.TipRegionByKey(rect9, "RJW_SexGizmo_SexOverdriveTooltip");
						}

						//row2 toggle - HitPawn
						if (pawn.jobs?.curDriver is JobDriver_SexBaseInitiator)   //TODO: maybe allow for receiver control someday?
						{
							if ((pawn.jobs?.curDriver as JobDriver_Sex).Sexprops != null)
								if ((pawn.jobs?.curDriver as JobDriver_Sex).Sexprops.isRapist)   //TODO: beating interrupts loving, fix someday?
								{
									float num = 32f;
									float num2 = 34f;
									float num3 = rect2.height / 2f - num + num2;
									float num4 = rect2.width - num;
									Rect rect11 = new Rect(rect2.x + num4, rect2.y + num3, num, num);
									if (Widgets.ButtonImage(rect11, AttackTex, true))
									{
										(pawn.jobs?.curDriver as JobDriver_Sex).beatonce = true;
										//SexUtility.Sex_Beatings_Dohit(pawn, ((JobDriver_Sex)pawn.jobs?.curDriver).Partner, ((JobDriver_Sex)pawn.jobs?.curDriver).isRape);
										SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
									}
									TooltipHandler.TipRegionByKey(rect11, "RJW_SexGizmo_HitPawn");
								}
						}
					}
			}

			return new GizmoResult(GizmoState.Clear);
		}

		public override float GetWidth(float maxWidth)
		{
			return 212f;
		}

		private Pawn pawn;

		private Texture2D LimitedTex;

		private Texture2D UnlimitedTex;

		private Texture2D AttackTex;

		private const string LimitedIconPath = "UI/Icons/EntropyLimit/Limited";

		private const string UnlimitedIconPath = "UI/Icons/EntropyLimit/Unlimited";

		private const string AttackIconPath = "UI/Icons/HostilityResponse/Attack";

		private static readonly Color PainBoostColor = new Color(0.2f, 0.65f, 0.35f);

		private static readonly Texture2D EntropyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.46f, 0.34f, 0.35f));

		private static readonly Texture2D OverLimitBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.75f, 0.2f, 0.15f));

		private static readonly Texture2D PsyfocusBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.34f, 0.42f, 0.43f));

		private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.03f, 0.035f, 0.05f));
	}
}
