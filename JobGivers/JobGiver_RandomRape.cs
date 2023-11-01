using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using Multiplayer.API;

namespace rjw
{
	public class JobGiver_RandomRape : ThinkNode_JobGiver
	{
		[SyncMethod]
		public Pawn find_victim(Pawn pawn, Map m)
		{
			float min_fuckability = 0.10f;							// Don't rape pawns with <10% fuckability
			float avg_fuckability = 0f;                             // Average targets fuckability, choose target higher than that
			var valid_targets = new Dictionary<Pawn, float>();      // Valid pawns and their fuckability
			Pawn chosentarget = null;                               // Final target pawn

			// could be prisoner, colonist, or non-hostile outsider
			IEnumerable<Pawn> targets = m.mapPawns.AllPawnsSpawned.Where(x 
				=> x != pawn
				&& xxx.is_not_dying(x)
				&& xxx.can_get_raped(x)
				&& !x.Suspended
				&& !x.Drafted
				&& !x.IsForbidden(pawn)
				&& pawn.CanReserveAndReach(x, PathEndMode.Touch, Danger.Some, xxx.max_rapists_per_prisoner, 0)
				&& !x.HostileTo(pawn)
				);


			//Zoo rape Animal
			if (xxx.is_zoophile(pawn) && RJWSettings.bestiality_enabled)
			{
				foreach (Pawn target in targets.Where(x => xxx.is_animal(x)))
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
					var valid_targetsFilteredAnimals = valid_targets.Where(x => x.Value >= avg_fuckability);

					if (valid_targetsFilteredAnimals.Any())
						chosentarget = valid_targetsFilteredAnimals.RandomElement().Key;

					return chosentarget;
				}
			}

			valid_targets = new Dictionary<Pawn, float>();
			// rape Humanlike
			foreach (Pawn target in targets.Where(x => !xxx.is_animal(x)))
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
				var valid_targetsFilteredAnimals = valid_targets.Where(x => x.Value >= avg_fuckability);

				if (valid_targetsFilteredAnimals.Any())
					chosentarget = valid_targetsFilteredAnimals.RandomElement().Key;
			}

			return chosentarget;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			//ModLog.Message(" JobGiver_RandomRape::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) called");
			if (!xxx.can_rape(pawn)) return null;

			if (pawn.health.hediffSet.HasHediff(HediffDef.Named("Hediff_RapeEnemyCD"))) return null;
			pawn.health.AddHediff(HediffDef.Named("Hediff_RapeEnemyCD"), null, null, null);

			Pawn victim = find_victim(pawn, pawn.Map);
			if (victim == null) return null;

			//ModLog.Message(" JobGiver_RandomRape::TryGiveJob( " + xxx.get_pawnname(pawn) + " ) - found victim " + xxx.get_pawnname(victim));
			return JobMaker.MakeJob(xxx.RapeRandom, victim);
		}
	}
}