using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Shared.Comparers
{
	public class StringComparer_IgnoreCase : IEqualityComparer<string>
	{
		public bool Equals(string x, string y)
		{
			return String.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
		}

		public int GetHashCode(string obj)
		{
			if (obj == null)
			{
				return 0;
			}

			return obj.GetHashCode();
		}
	}
}
