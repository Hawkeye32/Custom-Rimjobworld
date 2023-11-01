using rjw.Modules.Interactions.Defs;
using rjw.Modules.Interactions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.DefModExtensions
{
	public class GenitalPartExtension : DefModExtension
	{
		public GenitalFamily family = default;
		public List<GenitalTag> tags = new();

		public static bool TryGet(Hediff hed, out GenitalPartExtension ext) =>
			TryGet(hed?.def, out ext);

		public static bool TryGet(HediffDef def, out GenitalPartExtension ext)
		{
			ext = def?.GetModExtension<GenitalPartExtension>();
			return ext is not null;
		}
	}
}
