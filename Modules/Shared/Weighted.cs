using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Shared
{
	public class Weighted<TType>
	{
		public TType Element { get; set; }
		public float Weight { get; set; }

		public Weighted(float weight, TType element)
		{
			Weight = weight;
			Element = element;
		}
	}
}
