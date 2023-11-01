using System;
using System.Linq;
using Verse;

namespace rjw
{
	public class AgeConfigDef : Def
	{
		public SimpleCurve attractivenessByAgeFemale;
		public SimpleCurve attractivenessByAgeMale;
		public SimpleCurve lovinIntervalHoursByAge;
		public SimpleCurve rigidityByAge;
		public SimpleCurve whoringPriceByAge;

		static readonly Lazy<AgeConfigDef> instance = new Lazy<AgeConfigDef>(() => DefDatabase<AgeConfigDef>.AllDefs.Single());

		public static AgeConfigDef Instance
		{
			get
			{
				return instance.Value;
			}
		}
	}
}
