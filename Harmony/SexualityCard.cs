using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace rjw
{
	[HarmonyPatch(typeof(CharacterCardUtility), "DrawCharacterCard")]
	public static class SexcardPatch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo LifestageAndXenotypeOptions = AccessTools.Method(typeof(CharacterCardUtility), "LifestageAndXenotypeOptions");
			MethodInfo SexualityCardToggle = AccessTools.Method(typeof(SexcardPatch), nameof(SexualityCardToggle));
			bool traits = false;
			bool found = false;
			foreach (CodeInstruction i in instructions)
			{
				if (found)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0) { labels = i.labels.ListFullCopy() };//rect
					yield return new CodeInstruction(OpCodes.Ldarg_1);//pawn
					yield return new CodeInstruction(OpCodes.Ldarg_3);//creationRect
					yield return new CodeInstruction(OpCodes.Call, SexualityCardToggle);
					found = false;
					i.labels.Clear();
				}
				if (i.opcode == OpCodes.Call && i.operand as MethodInfo == LifestageAndXenotypeOptions)
				{
					found = true;
				}
				if (i.opcode == OpCodes.Ldstr && i.operand.Equals("Traits"))
				{
					traits = true;
				}
				if (traits && i.opcode == OpCodes.Ldc_R4 && i.operand.Equals(2f))//replaces rect y calculation
				{
					yield return new CodeInstruction(OpCodes.Ldc_R4, 0f);
					traits = false;
					continue;
				}
				yield return i;
			}
		}

		public static void SexualityCardToggle(Rect rect, Pawn pawn, Rect creationRect)
		{
			if (pawn == null) return;
			if (CompRJW.Comp(pawn) == null) return;

			//character bio
			Rect rectNew = new Rect(CharacterCardUtility.BasePawnCardSize.x - 50f, 2f, 24f, 24f);
			//character creator/game start
			if (Current.ProgramState != ProgramState.Playing)
			{
				//1.4
				if (ModsConfig.BiotechActive)
				{
					rectNew = new Rect(creationRect.width - 24f, 108f, 24f, 24f);
				}
				//1.3
				else if (ModsConfig.IdeologyActive) // Move icon lower to avoid overlap.
				{
					rectNew = new Rect(creationRect.width - 24f, 82f, 24f, 24f);
					if (xxx.IndividualityIsActive) // Move icon lower to avoid overlap.
						rectNew = new Rect(creationRect.width - 24f, 108f, 24f, 24f);
				}
				//1.1, 1.2
				else if (xxx.IndividualityIsActive) // Move icon lower to avoid overlap.
					rectNew = new Rect(creationRect.width - 24f, 56f, 24f, 24f);
				else
					rectNew = new Rect(creationRect.width - 24f, 30f, 24f, 24f);
			}
			Color old = GUI.color;

			GUI.color = rectNew.Contains(Event.current.mousePosition) ? new Color(0.25f, 0.59f, 0.75f) : new Color(1f, 1f, 1f);
			// TODO: Replace the placeholder icons with... something
			//if (CompRJW.Comp(pawn).quirks.ToString() != "None")
			//	GUI.DrawTexture(rectNew, ContentFinder<Texture2D>.Get("UI/Commands/Service_on"));
			//else
			//	GUI.DrawTexture(rectNew, ContentFinder<Texture2D>.Get("UI/Commands/Service_off"));
				GUI.DrawTexture(rectNew, ContentFinder<Texture2D>.Get("RJW-LOGO"));

			TooltipHandler.TipRegion(rectNew, "SexcardTooltip".Translate());
			if (Widgets.ButtonInvisible(rectNew))
			{
				SoundDefOf.InfoCard_Open.PlayOneShotOnCamera();
				Find.WindowStack.Add(new Dialog_Sexcard(pawn));
			}
			GUI.color = old;
		}
	}
}
