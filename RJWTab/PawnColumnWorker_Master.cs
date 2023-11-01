using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x020013F0 RID: 5104
	public class PawnColumnWorker_Master : PawnColumnWorker
	{
		// Token: 0x17001618 RID: 5656
		// (get) Token: 0x06007D97 RID: 32151 RVA: 0x00012AE6 File Offset: 0x00010CE6
		protected override GameFont DefaultHeaderFont
		{
			get
			{
				return GameFont.Tiny;
			}
		}

		// Token: 0x06007D98 RID: 32152 RVA: 0x002CCC79 File Offset: 0x002CAE79
		public override int GetMinWidth(PawnTable table)
		{
			return Mathf.Max(base.GetMinWidth(table), 100);
		}

		// Token: 0x06007D99 RID: 32153 RVA: 0x002CCC89 File Offset: 0x002CAE89
		public override int GetOptimalWidth(PawnTable table)
		{
			return Mathf.Clamp(170, this.GetMinWidth(table), this.GetMaxWidth(table));
		}

		// Token: 0x06007D9A RID: 32154 RVA: 0x002CB126 File Offset: 0x002C9326
		public override void DoHeader(Rect rect, PawnTable table)
		{
			base.DoHeader(rect, table);
			MouseoverSounds.DoRegion(rect);
		}

		// Token: 0x06007D9B RID: 32155 RVA: 0x002CCCA3 File Offset: 0x002CAEA3
		public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
		{
			if (!this.CanAssignMaster(pawn))
			{
				return;
			}
			TrainableUtility.MasterSelectButton(rect.ContractedBy(2f), pawn, true);
		}

		// Token: 0x06007D9C RID: 32156 RVA: 0x002CCCC4 File Offset: 0x002CAEC4
		public override int Compare(Pawn a, Pawn b)
		{
			int valueToCompare = this.GetValueToCompare1(a);
			int valueToCompare2 = this.GetValueToCompare1(b);
			if (valueToCompare != valueToCompare2)
			{
				return valueToCompare.CompareTo(valueToCompare2);
			}
			return this.GetValueToCompare2(a).CompareTo(this.GetValueToCompare2(b));
		}

		// Token: 0x06007D9D RID: 32157 RVA: 0x002CCD01 File Offset: 0x002CAF01
		private bool CanAssignMaster(Pawn pawn)
		{
			return pawn.RaceProps.Animal && pawn.Faction == Faction.OfPlayer && pawn.training.HasLearned(TrainableDefOf.Obedience);
		}

		// Token: 0x06007D9E RID: 32158 RVA: 0x002CCD34 File Offset: 0x002CAF34
		private int GetValueToCompare1(Pawn pawn)
		{
			if (!this.CanAssignMaster(pawn))
			{
				return 0;
			}
			if (pawn.playerSettings.Master == null)
			{
				return 1;
			}
			return 2;
		}

		// Token: 0x06007D9F RID: 32159 RVA: 0x002CCD51 File Offset: 0x002CAF51
		private string GetValueToCompare2(Pawn pawn)
		{
			if (pawn.playerSettings != null && pawn.playerSettings.Master != null)
			{
				return pawn.playerSettings.Master.Label;
			}
			return "";
		}
	}
}
