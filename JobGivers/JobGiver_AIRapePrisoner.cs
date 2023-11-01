using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using Multiplayer.API;

namespace rjw
{
	//Rape to Prisoner of QuestPrisonerWillingToJoin
	class JobGiver_AIRapePrisoner : ThinkNode_JobGiver
	{
		[SyncMethod]
		public static Pawn find_victim(Pawn pawn, Map m)
		{
			float min_fuckability = 0.10f;                          // Don't rape pawns with <10% fuckability
			float avg_fuckability = 0f;                             // Average targets fuckability, choose target higher than that
			var valid_targets = new Dictionary<Pawn, float>();      // Valid pawns and their fuckability
			Pawn chosentarget = null;                               // Final target pawn

			IEnumerable<Pawn> targets = m.mapPawns.AllPawns.Where(x
				=> x != pawn
				&& IsPrisonerOf(x, pawn.Faction)
				&& xxx.can_get_raped(x)
				&& pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.Some, xxx.max_rapists_per_prisoner, 0)
				&& !x.Position.IsForbidden(pawn)
				);

			foreach (Pawn target in targets)
			{
				if (!Pather_Utility.cells_to_target_rape(pawn, target.Position))
					continue;// too far

				float fuc = SexAppraiser.would_fuck(pawn, target, true, true);

				if (fuc > min_fuckability)
					if (Pather_Utility.can_path_to_target(pawn, target.Position))
						valid_targets.Add(target, fuc);
			}

			if (valid_targets.Any())
			{
				avg_fuckability = valid_targets.Average(x => x.Value);

				// choose pawns to fuck with above average fuckability
				var valid_targetsFiltered = valid_targets.Where(x => x.Value >= avg_fuckability);

				if (valid_targetsFiltered.Any())
					chosentarget = valid_targetsFiltered.RandomElement().Key;
			}

			return chosentarget;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			if (RJWSettings.DebugRape) ModLog.Message(" JobGiver_AIRapePrisoner::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) called ");

			if (!xxx.can_rape(pawn)) return null;

			if (SexUtility.ReadyForLovin(pawn) || xxx.is_hornyorfrustrated(pawn))
			{
				// don't allow pawns marked as comfort prisoners to rape others
				if (xxx.is_healthy(pawn))
				{
					Pawn prisoner = find_victim(pawn, pawn.Map);

					if (prisoner != null)
					{
						if (RJWSettings.DebugRape) ModLog.Message(" JobGiver_RandomRape::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) - found victim " + xxx.get_pawnname(prisoner));
						return JobMaker.MakeJob(xxx.RapeRandom, prisoner);
					}
				}
			}

			return null;
		}

		protected static bool IsPrisonerOf(Pawn pawn,Faction faction)
		{
			if (pawn?.guest == null) return false;
			return pawn.guest.HostFaction == faction && pawn.guest.IsPrisoner;
		}
	}
}
