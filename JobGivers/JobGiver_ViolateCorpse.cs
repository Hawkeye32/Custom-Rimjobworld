using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Linq;

namespace rjw
{
	public class JobGiver_ViolateCorpse : ThinkNode_JobGiver
	{
		public static Corpse find_corpse(Pawn pawn, Map m)
		{
			float min_fuckability = 0.10f;                            // Don't rape pawns with <10% fuckability
			float avg_fuckability = 0f;                               // Average targets fuckability, choose target higher than that
			var valid_targets = new Dictionary<Corpse, float>();      // Valid pawns and their fuckability
			Corpse chosentarget = null;                               // Final target pawn

			IEnumerable<Thing> targets = m.spawnedThings.Where(x 
				=> x is Corpse 
				&& pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.Some)
				&& !x.IsForbidden(pawn)
				);

			foreach (Corpse target in targets)
			{
				if (!Pather_Utility.cells_to_target_rape(pawn, target.Position))
					continue;// too far

				// Filter out rotters if not necrophile.
				if (!xxx.is_necrophiliac(pawn) && target.CurRotDrawMode != RotDrawMode.Fresh)
					continue;

				float fuc = SexAppraiser.would_fuck(pawn, target, false, false);

				if (fuc > min_fuckability)
					if (Pather_Utility.can_path_to_target(pawn, target.Position))
						valid_targets.Add(target, fuc);
			}

			if (valid_targets.Any())
			{
				avg_fuckability = valid_targets.Average(x => x.Value);

				// choose pawns to fuck with above average fuckability
				var valid_targetsFilteredAnimals = valid_targets.Where(x => x.Value >= avg_fuckability);

				if (valid_targetsFilteredAnimals.Any())
					chosentarget = valid_targetsFilteredAnimals.RandomElement().Key;
			}

			return chosentarget;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			// Most checks are done in ThinkNode_ConditionalNecro.

			// filter out necro for nymphs
			if (!RJWSettings.necrophilia_enabled) return null;

			if (pawn.Drafted) return null;

			//--ModLog.Message(" JobGiver_ViolateCorpse::TryGiveJob for ( " + xxx.get_pawnname(pawn) + " )");
			if (SexUtility.ReadyForLovin(pawn) || xxx.is_hornyorfrustrated(pawn))
			{
				//--ModLog.Message(" JobGiver_ViolateCorpse::TryGiveJob, can love ");
				if (!xxx.can_rape(pawn)) return null;

				var target = find_corpse(pawn, pawn.Map);
				//--ModLog.Message(" JobGiver_ViolateCorpse::TryGiveJob - target is " + (target == null ? "NULL" : "Found"));
				if (target != null)
				{
					return JobMaker.MakeJob(xxx.RapeCorpse, target);
				}
				// Ticks should only be increased after successful sex.
			}

			return null;
		}
	}
}