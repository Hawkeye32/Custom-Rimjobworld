using rjw.Modules.Quirks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Quirks.Implementation
{
	public class QuirkService : IQuirkService
	{
		public static IQuirkService Instance { get; private set; }

		static QuirkService()
		{
			Instance = new QuirkService();
		}

		private QuirkService() { }

		public bool HasQuirk(Pawn pawn, string quirk)
		{
			//No paw ! hum ... I meant pawn !
			if (pawn == null)
			{
				return false;
			}

			//No quirk !
			if (string.IsNullOrWhiteSpace(quirk))
			{
				return false;
			}

			string loweredQuirk = quirk.ToLower();

			string pawnQuirks = CompRJW.Comp(pawn).quirks
				.ToString()
				.ToLower();

			return pawnQuirks.Contains(loweredQuirk);
		}
	}
}
