using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;
using RimWorld;
using Verse.Sound;

namespace rjw.MainTab.Checkbox
{
	[StaticConstructorOnStartup]
	public abstract class PawnColumnCheckbox : PawnColumnWorker
	{
		public static readonly Texture2D CheckboxOnTex;
		public static readonly Texture2D CheckboxOffTex;
		public static readonly Texture2D CheckboxDisabledTex;

		public const int HorizontalPadding = 2;

		public override void DoCell(Rect rect, Pawn pawn, RimWorld.PawnTable table)
		{
			if (!this.HasCheckbox(pawn))
			{
				return;
			}
			if (Find.TickManager.TicksGame % 60 == 0)
			{
				pawn.UpdatePermissions();
				//Log.Message("GetDisabled UpdateCanDesignateService for " + xxx.get_pawnname(pawn));
				//Log.Message("UpdateCanDesignateService " + pawn.UpdateCanDesignateService());
				//Log.Message("CanDesignateService " + pawn.CanDesignateService());
				//Log.Message("GetDisabled " + GetDisabled(pawn));
			}
			int num = (int)((rect.width - 24f) / 2f);
			int num2 = Mathf.Max(3, 0);
			Vector2 vector = new Vector2(rect.x + (float)num, rect.y + (float)num2);
			Rect rect2 = new Rect(vector.x, vector.y, 24f, 24f);
			bool disabled = this.GetDisabled(pawn);
			bool value;
			if (disabled) 
			{
				value = false;
			}
			else 
			{
				value = this.GetValue(pawn);
			}
			bool flag = value;
			Vector2 topLeft = vector;
			//Widgets.Checkbox(topLeft, ref value, 24f, disabled, CheckboxOnTex, CheckboxOffTex, CheckboxDisabledTex);
			MakeCheckbox(topLeft, ref value, 24f, disabled, CheckboxOnTex, CheckboxOffTex, CheckboxDisabledTex);
			if (Mouse.IsOver(rect2))
			{
				string tip = this.GetTip(pawn);
				if (!tip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect2, tip);
				}
			}
			if (value != flag)
			{
				this.SetValue(pawn, value);
			}
		}

		protected void MakeCheckbox(Vector2 topLeft, ref bool value, float v = 24f, bool disabled = false, Texture2D checkboxOnTex = null, Texture2D checkboxOffTex = null, Texture2D checkboxDisabledTex = null)
		{
			Widgets.Checkbox(topLeft, ref value, v, disabled, checkboxOnTex, checkboxOffTex, checkboxDisabledTex);
		}

		protected virtual string GetTip(Pawn pawn)
		{
			return null;
		}

		protected virtual bool HasCheckbox(Pawn pawn)
		{
			return false;
		}

		protected abstract bool GetValue(Pawn pawn);

		protected abstract void SetValue(Pawn pawn, bool value);

		protected virtual bool GetDisabled(Pawn pawn)
		{
			return false;
		}
	}
}

