using rjw.Modules.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Shared
{
	public interface IPawnStateService
	{
		PawnState Detect(Pawn pawn);
	}
}
