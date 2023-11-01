using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Multiplayer.API;

namespace rjw
{
	internal class JobDef_RapeEnemy : JobDef
	{
		public List<JobDef> interruptJobs;
		public List<string> TargetDefNames = new List<string>();
		public int priority = 0;

		protected JobDriver_RapeEnemy instance
		{
			get
			{
				if (_tmpInstance == null)
				{
					_tmpInstance = (JobDriver_RapeEnemy)Activator.CreateInstance(driverClass);
				}
				return _tmpInstance;
			}
		}

		private JobDriver_RapeEnemy _tmpInstance;

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			interruptJobs = new List<JobDef> { null, JobDefOf.LayDown, JobDefOf.Wait_Wander, JobDefOf.GotoWander, JobDefOf.AttackMelee };
		}

		public virtual bool CanUseThisJobForPawn(Pawn rapist)
		{
			bool busy = !interruptJobs.Contains(rapist.CurJob?.def);
			if (RJWSettings.DebugRape) ModLog.Message(" JobDef_RapeEnemy::CanUseThisJobForPawn( " + xxx.get_pawnname(rapist) + " ) - busy:" + busy + " with current job: " + rapist.CurJob?.def?.ToString());
			if (busy) return false;

			return instance.CanUseThisJobForPawn(rapist);// || TargetDefNames.Contains(rapist.def.defName);
		}

		public virtual Pawn FindVictim(Pawn rapist, Map m)
		{
			return instance.FindVictim(rapist, m);
		}
	}



	public class JobDriver_RapeEnemy : JobDriver_Rape
	{
		//override can_rape mechanics
		protected bool requireCanRape = true;

		public virtual bool CanUseThisJobForPawn(Pawn rapist)
		{
			return xxx.is_human(rapist);
		}

		// this is probably useseless, maybe there be something in future
		public virtual bool considerStillAliveEnemies => true;

		[SyncMethod]
		public virtual Pawn FindVictim(Pawn rapist, Map m)
		{
			if (RJWSettings.DebugRape) ModLog.Message($"{this.GetType().ToString()}::TryGiveJob({xxx.get_pawnname(rapist)}) map {m?.ToString()}");
			if (rapist == null || m == null) return null;

			if (RJWSettings.DebugRape) ModLog.Message($" can rape = {xxx.can_rape(rapist)}");
			if (requireCanRape && !xxx.can_rape(rapist)) return null;

			List<Pawn> validTargets = new List<Pawn>();
			float min_fuckability = 0.10f;                          // Don't rape pawns with <10% fuckability
			float avg_fuckability = 0f;                             // Average targets fuckability, choose target higher than that
			var valid_targets = new Dictionary<Pawn, float>();      // Valid pawns and their fuckability
			Pawn chosentarget = null;                               // Final target pawn

			IEnumerable<Pawn> targets = m.mapPawns.AllPawnsSpawned.Where(x
				=> !x.IsForbidden(rapist) && x != rapist && x.HostileTo(rapist)
				&& IsValidTarget(rapist, x))
				.ToList();

			if (RJWSettings.DebugRape) ModLog.Message($" targets {targets.Count()}");

			if (targets.Any(x => IsBlocking(rapist, x)))	//If any of the targets is not downed and visible - don't proceed with rape (you have more pressing things to do).
			{												//This is a bit whacky bearing in mind target selection. For example vulnerable pawns will block, but non-vulnearable will not
				return null;
			}

			foreach (var target in targets)
			{
				if (!Pather_Utility.cells_to_target_rape(rapist, target.Position))
				{
					//if (RJWSettings.DebugRape) ModLog.Message($" {xxx.get_pawnname(target)} too far (cells) = {rapist.Position.DistanceToSquared(target.Position)}, skipping");
					if (RJWSettings.DebugRape) ModLog.Message($" {xxx.get_pawnname(target)} too far (cells) = {rapist.Position.DistanceTo(target.Position)}, skipping");
					continue;// too far
				}

				float fuc = GetFuckability(rapist, target);

				if (fuc > min_fuckability)
				{
					if (Pather_Utility.can_path_to_target(rapist, target.Position))
						valid_targets.Add(target, fuc);
					else
						if (RJWSettings.DebugRape) ModLog.Message($" {xxx.get_pawnname(target)} too far (path), skipping");
				}
				else
					if (RJWSettings.DebugRape) ModLog.Message($" {xxx.get_pawnname(target)} fuckability too low = {fuc}, skipping");

			}
			if (RJWSettings.DebugRape) ModLog.Message($" fuckable targets {valid_targets.Count()}");

			if (valid_targets.Any())
			{
				avg_fuckability = valid_targets.Average(x => x.Value);
				if (RJWSettings.DebugRape) ModLog.Message($" avg_fuckability {avg_fuckability}");

				// choose pawns to fuck with above average fuckability
				var valid_targetsFiltered = valid_targets.Where(x => x.Value >= avg_fuckability);
				if (RJWSettings.DebugRape) ModLog.Message($" targets above avg_fuckability {valid_targetsFiltered.Count()}");

				if (valid_targetsFiltered.Any())
					chosentarget = valid_targetsFiltered.RandomElement().Key;
			}

			return chosentarget;
		}

		bool IsBlocking(Pawn rapist, Pawn target)
		{
			return considerStillAliveEnemies && !target.Downed && rapist.CanSee(target);
		}

		bool IsValidTarget(Pawn rapist, Pawn target)
		{

			if (!RJWSettings.bestiality_enabled)
			{
				if (xxx.is_animal(target) && xxx.is_human(rapist))
				{
					//bestiality disabled, skip.
					return false;
				}
				if (xxx.is_animal(rapist) && xxx.is_human(target))
				{
					//bestiality disabled, skip.
					return false;
				}
			}

			if (!RJWSettings.animal_on_animal_enabled)
				if ((xxx.is_animal(target) && xxx.is_animal(rapist)))
				{
					//animal_on_animal disabled, skip.
					return false;
				}

			if ((xxx.is_mechanoid(rapist) && xxx.is_animal(target)) || (xxx.is_animal(rapist) && xxx.is_mechanoid(target)))
				return false; //no Mech on Animal action, ref JobDriver_RapeEnemyByMech::GetFuckability()

			if (target.CurJob?.def == xxx.gettin_raped || target.CurJob?.def == xxx.gettin_loved)
			{
				//already having sex with someone, skip, give chance to other victims.
				return false;
			}

			return Can_rape_Easily(target) &&
				(xxx.is_human(target) || xxx.is_animal(target)) &&
				rapist.CanReserveAndReach(target, PathEndMode.OnCell, Danger.Some, xxx.max_rapists_per_prisoner, 0);
		}

		public virtual float GetFuckability(Pawn rapist, Pawn target)
		{
			float fuckability = 0;
			if (target.health.hediffSet.HasHediff(xxx.submitting)) // it's not about attractiveness anymore, it's about showing who's whos bitch
			{
				fuckability = 2 * SexAppraiser.would_fuck(rapist, target, invert_opinion: true, ignore_bleeding: true, ignore_gender: true);
			}
			else if (SexAppraiser.would_rape(rapist, target))
			{
				fuckability = SexAppraiser.would_fuck(rapist, target, invert_opinion: true, ignore_bleeding: true, ignore_gender: true);
			}

			if (RJWSettings.DebugRape) ModLog.Message($"JobDriver_RapeEnemy::GetFuckability({xxx.get_pawnname(rapist)}, {xxx.get_pawnname(target)})");

			return fuckability;
		}

		protected bool Can_rape_Easily(Pawn pawn)
		{
			return xxx.can_get_raped(pawn) && !pawn.IsBurning();
		}
	}
}
