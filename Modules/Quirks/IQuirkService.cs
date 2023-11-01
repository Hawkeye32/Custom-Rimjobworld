using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Quirks
{
	public interface IQuirkService
	{
		bool HasQuirk(Pawn pawn, string quirk);
	}
}
