using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Shared.Helpers
{
	public static class RandomHelper
	{
		private static Random _random;

		static RandomHelper()
		{
			_random = new Random();
		}

		/// <remarks>this is not foolproof</remarks>
		public static TType WeightedRandom<TType>(IList<Weighted<TType>> weights)
		{
			if (weights == null || weights.Any() == false || weights.Where(e => e.Weight < 0).Any())
			{
				return default(TType);
			}

			Weighted<TType> result;

			if (weights.TryRandomElementByWeight(e => e.Weight, out result) == true)
			{
				return result.Element;
			}

			return weights.RandomElement().Element;
		}
	}
}
