using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace rjw
{
	public class PartProps : DefModExtension
	{
		/// <summary>
		/// Just some text flags.
		/// </summary>
		public List<string> props;

		public static bool TryGetProps(Hediff hediff, out List<string> p) =>
			// Wha..?  Why this indirection!?
			TryGetPartProps(hediff, extension => extension.props, out p);

		public static bool TryGetPartProps(
			Hediff hediff,
			Func<PartProps, List<string>> getList,
			out List<string> p)
		{
			if (hediff?.def.GetModExtension<PartProps>() is { } ext)
			{
				p = getList(ext);
				return p is not null;
			}
			p = null;
			return false;
		}
	}
}
