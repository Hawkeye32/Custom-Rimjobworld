using rjw.Modules.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Shared.Implementation
{
	public class PawnStateService : IPawnStateService
	{
		public static IPawnStateService Instance { get; private set; }

		static PawnStateService()
		{
			Instance = new PawnStateService();
		}

		public PawnState Detect(Pawn pawn)
		{
			if (pawn.Dead)
			{
				return PawnState.Dead;
			}

			if (pawn.Downed)
			{
				if (pawn.health.capacities.CanBeAwake == false)
				{
					return PawnState.Unconscious;
				}

				return PawnState.Downed;
			}

			return PawnState.Healthy;
		}
	}
}
