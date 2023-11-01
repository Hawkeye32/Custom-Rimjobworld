using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace rjw.MainTab
{
	public abstract class PawnColumnWorker_TextCenter : PawnColumnWorker_Text
	{
		public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
		{
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, Mathf.Min(rect.height, 30f));
			string textFor = GetTextFor(pawn);
			if (textFor != null)
			{
				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.MiddleCenter;
				Text.WordWrap = false;
				Widgets.Label(rect2, textFor);
				Text.WordWrap = true;
				Text.Anchor = TextAnchor.UpperLeft;
				string tip = GetTip(pawn);
				if (!tip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect2, tip);
				}
			}
		}
	}
}
