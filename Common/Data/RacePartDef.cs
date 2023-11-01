using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw
{
	/// <summary>
	/// Defines how to create a single sex part on a newly created pawn.
	/// </summary>
	public class RacePartDef : Def
	{
		public bool IsNone => string.IsNullOrEmpty(hediffName);
		public static readonly RacePartDef None = new RacePartDef();

		/// <summary>
		/// The name of the hediff to create.
		/// Doesn't use the hediff directly because that causes issues when another mod wants to
		/// create and reference custom parts inheriting from rjw base defs.
		/// </summary>
		public string hediffName;
		public string fluidType;
		public float? fluidModifier;
		public SimpleCurve severityCurve;

		public bool TryGetHediffDef(out HediffDef hediffDef)
		{
			hediffDef = DefDatabase<HediffDef>.GetNamedSilentFail(hediffName);
			if (hediffDef == null)
			{
				ModLog.Error($"Could not find a HediffDef named {hediffName} referenced by RacePartDef named {defName}.");
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
