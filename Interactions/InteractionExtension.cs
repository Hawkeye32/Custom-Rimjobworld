using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace rjw
{
	public class InteractionExtension : DefModExtension
	{
		/// <summary>
		/// </summary>
		public string RMBLabel = ""; // rmb menu
		public string rjwSextype = ""; // xxx.rjwSextype

		public List<string> rulepack_defs = new List<string>(); //rulepack(s) for this interaction
	}
}
