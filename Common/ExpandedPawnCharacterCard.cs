///Better not leave it be, this mod is not intended to reimpelement base functionality and break other mods by that.
//using Harmony;
//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using UnityEngine;
//using Verse;
//using Verse.Sound;

//namespace rjw.Source.Common
//{
//	public static class ExpandedPawnCharacterCard
//	{
//		/// <summary>
//		/// Better to leave it be, since it doesn't affect anything much.
//		/// </summary>
//		public static float cardX = 570f;  // vanilla is 570

//		/// <summary>
//		/// Basically, you want to increase this value if you want character info card be taller (thus allowing to have more
//		/// traits and skills shown). Height is in pixels, increase by ~25 pixels for each additional row of skill/trait.
//		/// </summary>
//		public static float cardY = 470f;  // vanilla is 470

//		[HarmonyPatch(typeof(CharacterCardUtility), "DrawCharacterCard", new[] { typeof(Rect), typeof(Pawn), typeof(Action), typeof(Rect), })]
//		public static class CharacterCardUtilityPatch
//		{
//			private static IEnumerable<WorkTags> WorkTagsFrom(WorkTags tags)
//			{
//				foreach (WorkTags workTag in tags.GetAllSelectedItems<WorkTags>())
//				{
//					if (workTag != WorkTags.None)
//					{
//						yield return workTag;
//					}
//				}
//			}

//			[HarmonyPrefix]
//			public static bool DrawCharacterCardP(ref Rect rect, ref Pawn pawn, ref Action randomizeCallback, ref Rect creationRect)
//			{
//				rect.position = new Vector2(17f, 17f);
//				rect.size = new Vector2(cardX, cardY);
//				bool flag = randomizeCallback != null;
//				GUI.BeginGroup((!flag) ? rect : creationRect);
//				Rect rectLabel = new Rect(0f, 0f, 300f, 30f);
//				NameTriple nameTriple = pawn.Name as NameTriple;
//				if (flag && nameTriple != null)
//				{
//					Rect rect3 = new Rect(rectLabel);
//					rect3.width *= 0.333f;
//					Rect rect4 = new Rect(rectLabel);
//					rect4.width *= 0.333f;
//					rect4.x += rect4.width;
//					Rect rect5 = new Rect(rectLabel);
//					rect5.width *= 0.333f;
//					rect5.x += rect4.width * 2f;
//					string first = nameTriple.First;
//					string nick = nameTriple.Nick;
//					string last = nameTriple.Last;
//					CharacterCardUtility.DoNameInputRect(rect3, ref first, 12);
//					if (nameTriple.Nick == nameTriple.First || nameTriple.Nick == nameTriple.Last)
//					{
//						GUI.color = new Color(1f, 1f, 1f, 0.5f);
//					}
//					CharacterCardUtility.DoNameInputRect(rect4, ref nick, 9);
//					GUI.color = Color.white;
//					CharacterCardUtility.DoNameInputRect(rect5, ref last, 12);
//					if (nameTriple.First != first || nameTriple.Nick != nick || nameTriple.Last != last)
//					{
//						pawn.Name = new NameTriple(first, nick, last);
//					}
//					TooltipHandler.TipRegion(rect3, "FirstNameDesc".Translate());
//					TooltipHandler.TipRegion(rect4, "ShortIdentifierDesc".Translate());
//					TooltipHandler.TipRegion(rect5, "LastNameDesc".Translate());
//				}
//				else
//				{
//					rectLabel.width = 999f;
//					Text.Font = GameFont.Medium;
//					Widgets.Label(rectLabel, pawn.Name.ToStringFull);
//					Text.Font = GameFont.Small;
//				}
//				if (randomizeCallback != null)
//				{
//					Rect rectRandomise = new Rect(creationRect.width - 24f - 100f, 0f, 100f, rectLabel.height);
//					if (Widgets.ButtonText(rectRandomise, "Randomize".Translate(), true, false, true))
//					{
//						SoundDefOf.TickTiny.PlayOneShotOnCamera(null);
//						randomizeCallback();
//					}
//					UIHighlighter.HighlightOpportunity(rectRandomise, "RandomizePawn");
//				}
//				if (flag)
//				{
//					Widgets.InfoCardButton(creationRect.width - 24f, 0f, pawn);
//				}
//				else if (!pawn.health.Dead)
//				{
//					float num = CharacterCardUtility.PawnCardSize.x - 85f;
//					if ((pawn.Faction == Faction.OfPlayer || pawn.IsPrisonerOfColony) && pawn.Spawned)
//					{
//						Rect rectBanish = new Rect(num, 0f, 30f, 30f);
//						TooltipHandler.TipRegion(rectBanish, PawnBanishUtility.GetBanishButtonTip(pawn));
//						if (Widgets.ButtonImage(rectBanish, ContentFinder<Texture2D>.Get("UI/Buttons/Banish", true)))
//						{
//							if (pawn.Downed)
//							{
//								Messages.Message("MessageCantBanishDownedPawn".Translate(new object[]
//								{
//								pawn.LabelShort
//								}).AdjustedFor(pawn), pawn, MessageTypeDefOf.RejectInput);
//							}
//							else
//							{
//								PawnBanishUtility.ShowBanishPawnConfirmationDialog(pawn);
//							}
//						}
//						num -= 40f;
//					}
//					if (pawn.IsColonist)
//					{
//						Rect rect8 = new Rect(num, 0f, 30f, 30f);
//						TooltipHandler.TipRegion(rect8, "RenameColonist".Translate());
//						if (Widgets.ButtonImage(rect8, ContentFinder<Texture2D>.Get("UI/Buttons/Rename", true)))
//						{
//							Find.WindowStack.Add(new Dialog_ChangeNameTriple(pawn));
//						}
//						num -= 40f;
//					}
//				}
//				string label = pawn.MainDesc(true);
//				Rect rectMainDesc = new Rect(0f, 45f, rect.width, 60f);
//				Widgets.Label(rectMainDesc, label);
//				Pawn p = pawn;
//				TooltipHandler.TipRegion(rectMainDesc, () => p.ageTracker.AgeTooltipString, 6873641);
//				Rect positionLeftHalf = new Rect(0f, 100f, 250f, cardY);
//				Rect positionRightHalf = new Rect(positionLeftHalf.xMax, 100f, 258f, cardY);
//				GUI.BeginGroup(positionLeftHalf);
//				float currentY = 0f;
//				Text.Font = GameFont.Medium;
//				Widgets.Label(new Rect(0f, 0f, 200f, 30f), "Backstory".Translate());
//				currentY += 30f;
//				Text.Font = GameFont.Small;
//				foreach (BackstorySlot backstorySlot in Enum.GetValues(typeof(BackstorySlot)))
//				{
//					Backstory backstory = pawn.story.GetBackstory(backstorySlot);
//					if (backstory != null)
//					{
//						Rect rect10 = new Rect(0f, currentY, positionLeftHalf.width, 24f);
//						if (Mouse.IsOver(rect10))
//						{
//							Widgets.DrawHighlight(rect10);
//						}
//						TooltipHandler.TipRegion(rect10, backstory.FullDescriptionFor(pawn));
//						Text.Anchor = TextAnchor.MiddleLeft;
//						string str = (backstorySlot != BackstorySlot.Adulthood) ? "Childhood".Translate() : "Adulthood".Translate();
//						Widgets.Label(rect10, str + ":");
//						Text.Anchor = TextAnchor.UpperLeft;
//						Rect rect11 = new Rect(rect10);
//						rect11.x += 90f;
//						rect11.width -= 90f;
//						string title = backstory.Title;
//						Widgets.Label(rect11, title);
//						currentY += rect10.height + 2f;
//					}
//				}
//				currentY += 25f;
//				Text.Font = GameFont.Small;
//				Widgets.Label(new Rect(0f, currentY, 200f, 30f), "IncapableOf".Translate());
//				currentY += 30f;
//				Text.Font = GameFont.Small;
//				StringBuilder stringBuilder = new StringBuilder();
//				WorkTags combinedDisabledWorkTags = pawn.story.CombinedDisabledWorkTags;
//				if (combinedDisabledWorkTags == WorkTags.None)
//				{
//					stringBuilder.Append("(" + "NoneLower".Translate() + "), ");
//				}
//				else
//				{
//					List<WorkTags> list = WorkTagsFrom(combinedDisabledWorkTags).ToList<WorkTags>();
//					bool flag2 = true;
//					foreach (WorkTags current in list)
//					{
//						if (flag2)
//						{
//							stringBuilder.Append(current.LabelTranslated().CapitalizeFirst());
//						}
//						else
//						{
//							stringBuilder.Append(current.LabelTranslated());
//						}
//						stringBuilder.Append(", ");
//						flag2 = false;
//					}
//				}
//				string text = stringBuilder.ToString();
//				text = text.Substring(0, text.Length - 2);
//				Rect rect12 = new Rect(0f, currentY, positionLeftHalf.width, 999f);
//				Widgets.Label(rect12, text);
//				currentY += 100f;
//				Text.Font = GameFont.Medium;
//				Widgets.Label(new Rect(0f, currentY, 200f, 30f), "Traits".Translate());
//				currentY += 30f;
//				Text.Font = GameFont.Small;
//				for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
//				{
//					Trait trait = pawn.story.traits.allTraits[i];
//					Rect rectCurrentTrait = new Rect(0f, currentY, positionLeftHalf.width, 24f);
//					if (Mouse.IsOver(rectCurrentTrait))
//					{
//						Widgets.DrawHighlight(rectCurrentTrait);
//					}
//					Widgets.Label(rectCurrentTrait, trait.LabelCap);
//					currentY += rectCurrentTrait.height + 2f;
//					Trait trLocal = trait;
//					TipSignal tip = new TipSignal(() => trLocal.TipString(p), (int)currentY * 37);
//					TooltipHandler.TipRegion(rectCurrentTrait, tip);
//				}
//				GUI.EndGroup();
//				GUI.BeginGroup(positionRightHalf);
//				Text.Font = GameFont.Medium;
//				Widgets.Label(new Rect(0f, 0f, 200f, 30f), "Skills".Translate());
//				SkillUI.SkillDrawMode mode;
//				if (Current.ProgramState == ProgramState.Playing)
//				{
//					mode = SkillUI.SkillDrawMode.Gameplay;
//				}
//				else
//				{
//					mode = SkillUI.SkillDrawMode.Menu;
//				}
//				SkillUI.DrawSkillsOf(pawn, new Vector2(0f, 35f), mode);
//				GUI.EndGroup();
//				GUI.EndGroup();

//				return false;
//			}
//		}
//		[HarmonyPatch(typeof(ITab_Pawn_Character), "FillTab")]
//		public static class ITab_Pawn_CharacterPatch
//		{
//			[HarmonyPrefix]
//			public static bool FillTabP(ITab_Pawn_Character __instance)
//			{
				
//				FieldInfo fi = typeof(ITab_Pawn_Character).GetField("size", Unprivater.flags);
//				fi.SetValue(__instance, new Vector2(cardX+34f, cardY+34f));

//				var p = Unprivater.GetProtectedProperty<Pawn>("PawnToShowInfoAbout", __instance);

//				Rect rect = new Rect(17f, 17f, 570f, cardY);
//				CharacterCardUtility.DrawCharacterCard(rect, p, null, default(Rect));

//				return false;
//			}
//		}

//	}
//}
