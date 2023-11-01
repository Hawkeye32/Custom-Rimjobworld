using System.Collections.Generic;
using Verse;
using System;
using System.ComponentModel;
using System.Linq;
using Verse.Noise;

namespace rjw
{
	/// <summary>
	/// Data about part sizes that is not tied to an individual hediffdef.
	/// </summary>
	public class PartStagesDef : Def
	{
		public float bandSizeBase;
		public float bandSizeInterval;
		public float cupSizeInterval;
		public List<string> cupSizeLabels;

		static readonly Lazy<PartStagesDef> instance = new Lazy<PartStagesDef>(() => DefDatabase<PartStagesDef>.AllDefs.Single());

		public static PartStagesDef Instance
		{
			get
			{
				return instance.Value;
			}
		}

		public static string GetCupSizeLabel(float size)
		{
			var i = Math.Max(0, Math.Min(Instance.cupSizeLabels.Count - 1, (int)size));
			return Instance.cupSizeLabels[i];
		}
	}
}
