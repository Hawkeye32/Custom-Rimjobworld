using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using rjw.MainTab.DefModExtensions;

namespace rjw.MainTab
{
	[StaticConstructorOnStartup]
	public class RJW_PawnTableList
	{
		public List<PawnTableDef> getdefs()
		{
			var defs = new List<PawnTableDef>();
			defs.AddRange(DefDatabase<PawnTableDef>.AllDefs.Where(x => x.HasModExtension<RJW_PawnTable>()));
			return defs;
		}
	}
	public class MainTabWindow : MainTabWindow_PawnTable
	{
		protected override float ExtraBottomSpace
		{
			get
			{
				return 53f; //default 53
			}
		}

		protected override float ExtraTopSpace
		{
			get
			{
				return 40f; //default 0
			}
		}

		protected override PawnTableDef PawnTableDef => pawnTableDef;

		protected override IEnumerable<Pawn> Pawns => pawns;


		public IEnumerable<Pawn> pawns = Find.CurrentMap.mapPawns.AllPawns.Where(p => p.RaceProps.Humanlike && p.IsColonist && !xxx.is_slave(p));
		public PawnTableDef pawnTableDef = DefDatabase<PawnTableDef>.GetNamed("RJW_PawnTable_Colonists");

		/// <summary>
		/// draw table
		/// </summary>
		/// <param name="rect"></param>
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			if (Widgets.ButtonText(new Rect(rect.x + 5f, rect.y + 5f, Mathf.Min(rect.width, 260f), 32f), "MainTabWindow_Designators".Translate(), true, true, true))
			{
				MakeMenu();
			}
		}

		public override void PostOpen()
		{
			base.PostOpen();
			Find.World.renderer.wantedMode = WorldRenderMode.None;
		}

		/// <summary>
		/// reload/update tab
		/// </summary>
		public static void Reloadtab()
		{
			var rjwtab = DefDatabase<MainButtonDef>.GetNamed("RJW_MainButton");
			Find.MainTabsRoot.ToggleTab(rjwtab, false);//off
			Find.MainTabsRoot.ToggleTab(rjwtab, false);//on
		}

		public void MakeMenu()
		{
			Find.WindowStack.Add(new FloatMenu(MakeOptions()));
		}

		/// <summary>
		/// switch pawnTable's
		/// patch this
		/// </summary>
		public List<FloatMenuOption> MakeOptions()
		{
			List<FloatMenuOption> opts = new List<FloatMenuOption>();
			PawnTableDef tabC = DefDatabase<PawnTableDef>.GetNamed("RJW_PawnTable_Colonists");
			PawnTableDef tabA = DefDatabase<PawnTableDef>.GetNamed("RJW_PawnTable_Animals");
			PawnTableDef tabP = DefDatabase<PawnTableDef>.GetNamed("RJW_PawnTable_Property");

			opts.Add(new FloatMenuOption(tabC.GetModExtension<RJW_PawnTable>().label, () =>
			{
				pawnTableDef = tabC;
				pawns = Find.CurrentMap.mapPawns.AllPawns.Where(p => p.RaceProps.Humanlike && p.IsColonist && !xxx.is_slave(p));
				Notify_ResolutionChanged();
				Reloadtab();
			}, MenuOptionPriority.Default));

			opts.Add(new FloatMenuOption(tabA.GetModExtension<RJW_PawnTable>().label, () =>
			{
				pawnTableDef = tabA;
				pawns = Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer).Where(p => xxx.is_animal(p));
				Notify_ResolutionChanged();
				Reloadtab();
			}, MenuOptionPriority.Default));

			opts.Add(new FloatMenuOption(tabP.GetModExtension<RJW_PawnTable>().label, () =>
			{
				pawnTableDef = tabP;
				pawns = Find.CurrentMap.mapPawns.AllPawns.Where(p => p.RaceProps.Humanlike && (p.IsColonist && xxx.is_slave(p) || p.IsPrisonerOfColony));
				Notify_ResolutionChanged();
				Reloadtab();
			}, MenuOptionPriority.Default));

			return opts;
		}
	}
}
