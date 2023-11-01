using System.Linq;
using Verse;

namespace rjw
{
	public class Hediff_PartsSizeChangerPC : HediffWithComps
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			foreach (Hediff hed in pawn.health.hediffSet.hediffs.Where(x => x.Part != null && x.Part == Part && (x is Hediff_PartBaseNatural || x is Hediff_PartBaseArtifical)).ToList())
			{
				CompHediffBodyPart CompHediff = hed.TryGetComp<rjw.CompHediffBodyPart>();
				if (CompHediff != null)
				{
					//Log.Message("  Hediff_PartsSizeChanger: " + hed.Label);
					//Log.Message("  Hediff_PartsSizeChanger: " + hed.Severity);
					//Log.Message("  Hediff_PartsSizeChanger: " + CompHediff.SizeBase);
					//Log.Message("  Hediff_PartsSizeChanger: " + "-----");
					//Log.Message("  Hediff_PartsSizeChanger: " + this.Label);
					//Log.Message("  Hediff_PartsSizeChanger: " + this.Severity);
					CompHediff.SizeBase = this.CurStage.minSeverity;
					CompHediff.initComp(reroll: true);
					CompHediff.updatesize();
					//Log.Message("  Hediff_PartsSizeChanger: " + "-----");
					//Log.Message("  Hediff_PartsSizeChanger: " + hed.Label);
					//Log.Message("  Hediff_PartsSizeChanger: " + hed.Severity);
					//Log.Message("  Hediff_PartsSizeChanger: " + CompHediff.SizeBase);
				}
			}
			pawn.health.RemoveHediff(this);
		}
	}
	public class Hediff_PartsSizeChangerCE : HediffWithComps
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			foreach (Hediff hed in pawn.health.hediffSet.hediffs.Where(x => x.Part != null && x.Part == Part && (x is Hediff_PartBaseNatural || x is Hediff_PartBaseArtifical)).ToList())
			{
				CompHediffBodyPart CompHediff = hed.TryGetComp<rjw.CompHediffBodyPart>();
				if (CompHediff != null)
				{
					CompHediff.SizeBase = this.def.initialSeverity;
					CompHediff.initComp(reroll: true);
					CompHediff.updatesize();
				}
			}
			pawn.health.RemoveHediff(this);
		}
	}
}