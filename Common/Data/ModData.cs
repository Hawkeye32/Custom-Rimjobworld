using HugsLib;
using RimWorld;
using Verse;

namespace rjw
{
	/// <summary>
	/// Rjw settings store
	/// </summary>
	public class SaveStorage : ModBase
	{
		public override string ModIdentifier => ModId;
		public static string ModId => "RJW";

		public static DataStore DataStore;//reference to savegame data, hopefully
		public static DesignatorsData DesignatorsData;//reference to savegame data, hopefully

		public override void SettingsChanged()
		{
			ToggleTabIfNeeded();
		}

		public override void WorldLoaded()
		{
			DataStore = Find.World.GetComponent<DataStore>();
			DesignatorsData = Find.World.GetComponent<DesignatorsData>();
			DesignatorsData.Update();
			ToggleTabIfNeeded();
			FixRjwHediffsOnlLoad();
		}
		protected override bool HarmonyAutoPatch { get => false; }//first.cs creates harmony and does some convoulted stuff with it

		private void ToggleTabIfNeeded()
		{
			//DefDatabase<MainButtonDef>.GetNamed("RJW_Brothel").buttonVisible = RJWSettings.whoringtab_enabled;
		}
		private void FixRjwHediffsOnlLoad()
		{
			foreach (var pawn in PawnsFinder.All_AliveOrDead)
			{
				foreach (var hd in pawn.health.hediffSet.hediffs)
				{
					if (hd is Hediff_PartBaseNatural || hd is Hediff_PartBaseArtifical)
					{
						hd.TryGetComp<CompHediffBodyPart>().updatepartposition();
					}
				}
			}
		}
	}
}
