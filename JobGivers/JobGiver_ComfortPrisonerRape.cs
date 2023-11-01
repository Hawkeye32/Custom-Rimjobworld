using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Multiplayer.API;

namespace rjw
{
	public class JobGiver_ComfortPrisonerRape : ThinkNode_JobGiver
	{
		[SyncMethod]
		public static Pawn find_targetCP(Pawn pawn, Map m)
		{
			if (!DesignatorsData.rjwComfort.Any()) return null;

			float min_fuckability = 0.10f;							// Don't rape prisoners with <10% fuckability
			float avg_fuckability = 0f;								// Average targets fuckability, choose target higher than that
			var valid_targets = new Dictionary<Pawn, float>();		// Valid pawns and their fuckability
			Pawn chosentarget = null;               // Final target pawn

			string pawnName = xxx.get_pawnname(pawn);
			if (RJWSettings.DebugRape) ModLog.Message($"FindComfortPrisoner({pawnName})");

			IEnumerable<Pawn> targets = DesignatorsData.rjwComfort.Where(x
				=> x != pawn
				&& xxx.can_get_raped(x)
				&& pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.Some, xxx.max_rapists_per_prisoner, 0)
				&& !x.IsForbidden(pawn)
				&& SexAppraiser.would_rape(pawn, x)
				);

			if (RJWSettings.DebugRape) ModLog.Message($"FindComfortPrisoner({pawnName}): found {targets.Count()}");

			if (xxx.is_animal(pawn))
			{
				// Animals only consider targets they can see, instead of seeking them out.
				targets = targets.Where(x => pawn.CanSee(x)).ToList();
			}

			foreach (Pawn target in targets)
			{
				if (!Pather_Utility.cells_to_target_rape(pawn, target.Position))
					continue;// too far

				float fuc = 0f;
				if (xxx.is_animal(target))
					fuc = SexAppraiser.would_fuck_animal(pawn, target, true);
				else if (xxx.is_human(target))
					fuc = SexAppraiser.would_fuck(pawn, target, true);

				if (RJWSettings.DebugRape) ModLog.Message($"FindComfortPrisoner({pawnName}): {fuc} has to be over {min_fuckability}");

				if (fuc > min_fuckability)
					if (Pather_Utility.can_path_to_target(pawn, target.Position))
						valid_targets.Add(target, fuc);
			}

			if (valid_targets.Any())
			{
				// avg_fuckability = valid_targets.Average(x => x.Value); // disabled for CP

				// choose pawns to fuck with above average fuckability
				var valid_targetsFiltered = valid_targets.Where(x => x.Value >= avg_fuckability);

				if (valid_targetsFiltered.Any())
					chosentarget = valid_targetsFiltered.RandomElement().Key;
			}

			return chosentarget;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			if (RJWSettings.DebugRape) ModLog.Message($"JobGiver_ComfortPrisonerRape::TryGiveJob({xxx.get_pawnname(pawn)}) called");

			if (!RJWSettings.WildMode)
			{
				// don't allow pawns marked as comfort prisoners to rape others
				if (RJWSettings.DebugRape) ModLog.Message($"JobGiver_ComfortPrisonerRape::TryGiveJob({xxx.get_pawnname(pawn)}): is healthy = {xxx.is_healthy(pawn)}, is cp = {pawn.IsDesignatedComfort()}, is ready = {SexUtility.ReadyForLovin(pawn)}, is frustrated = {xxx.is_frustrated(pawn)}");
				if (!xxx.is_healthy(pawn) || pawn.IsDesignatedComfort() || (!SexUtility.ReadyForLovin(pawn) && !xxx.is_frustrated(pawn))) return null;
			}


			if (RJWSettings.DebugRape) ModLog.Message($"FindComfortPrisoner({xxx.get_pawnname(pawn)}): can rape = {xxx.can_rape(pawn)}, is drafted = {pawn.Drafted}");
			if (pawn.Drafted || !xxx.can_rape(pawn)) return null;

			// It's unnecessary to include other job checks. Pawns seem to only look for new jobs when between jobs or laying down idle.
			if (!(pawn.jobs.curJob == null || pawn.jobs.curJob.def == JobDefOf.LayDown))
			{
				if (RJWSettings.DebugRape) ModLog.Message($"FindComfortPrisoner({xxx.get_pawnname(pawn)}): I already have a job ({pawn.CurJobDef})");
				return null;
			}

			// Faction check.
			if (!(pawn.Faction?.IsPlayer ?? false) && !pawn.IsPrisonerOfColony)
			{
				if (RJWSettings.DebugRape) ModLog.Message($"FindComfortPrisoner({xxx.get_pawnname(pawn)}): player faction: {pawn.Faction?.IsPlayer}, prisoner: {pawn.IsPrisonerOfColony}");
				return null;
			}

			Pawn target = find_targetCP(pawn, pawn.Map);
			if (target == null) return null;
			if (RJWSettings.DebugRape) ModLog.Message($"JobGiver_ComfortPrisonerRape::TryGiveJob({xxx.get_pawnname(pawn)}) with target {xxx.get_pawnname(target)}");

			if (xxx.is_animal(target))
				return JobMaker.MakeJob(xxx.bestiality, target);
			else
				return JobMaker.MakeJob(xxx.RapeCP, target);
		}
	}
}
