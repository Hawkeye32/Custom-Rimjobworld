using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Multiplayer.API;

namespace rjw
{
	/// <summary>
	/// Helper for sex with (humanlikes)
	/// </summary>
	public class CasualSex_Helper
	{
		// List of jobs that can be interrupted by quickies. 
		public static readonly List<JobDef> quickieAllowedJobs = new List<JobDef> { null, JobDefOf.Wait_Wander, JobDefOf.GotoWander, JobDefOf.Clean, JobDefOf.ClearSnow,
			JobDefOf.CutPlant, JobDefOf.HaulToCell, JobDefOf.Deconstruct, JobDefOf.LayDown, JobDefOf.Research, JobDefOf.SmoothFloor, JobDefOf.SmoothWall,
			JobDefOf.SocialRelax, JobDefOf.StandAndBeSociallyActive, JobDefOf.RemoveApparel, JobDefOf.Strip, JobDefOf.Wait, JobDefOf.FillFermentingBarrel,
			JobDefOf.Sow, JobDefOf.Shear, JobDefOf.DeliverFood, JobDefOf.Hunt, JobDefOf.Mine, JobDefOf.RearmTurret, JobDefOf.RemoveFloor, JobDefOf.RemoveRoof,
			JobDefOf.Repair, JobDefOf.TakeBeerOutOfFermentingBarrel, JobDefOf.Uninstall, JobDefOf.Meditate, JobDefOf.OperateDeepDrill,
			JobDefOf.OperateScanner, JobDefOf.Reign, JobDefOf.Slaughter, xxx.Masturbate};


		public static bool CanHaveSex(Pawn target)
		{
			return xxx.can_fuck(target) || xxx.can_be_fucked(target);
		}

		[SyncMethod]
		public static bool roll_to_skip(Pawn pawn, Pawn target, out float fuckability)
		{
			fuckability = SexAppraiser.would_fuck(pawn, target); // 0.0 to 1.0
			if (fuckability < RJWHookupSettings.MinimumFuckabilityToHookup)
			{
				if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" roll_to_skip(I, {xxx.get_pawnname(pawn)} won't fuck {xxx.get_pawnname(target)}), ({fuckability})");
				return false;
			}

			float reciprocity = xxx.is_animal(target) ? 1.0f : SexAppraiser.would_fuck(target, pawn);
			if (reciprocity < RJWHookupSettings.MinimumFuckabilityToHookup)
			{
				if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" roll_to_skip({xxx.get_pawnname(target)} won't fuck me, {xxx.get_pawnname(pawn)}), ({reciprocity})");
				return false;
			}

			float chance_to_skip = 0.9f - 0.7f * fuckability;
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			return Rand.Value < chance_to_skip;
		}

		[SyncMethod]
		public static Pawn find_partner(Pawn pawn, Map map, bool bedsex = false)
		{
			string pawnName = xxx.get_pawnname(pawn);
			if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): starting.");

			bool pawnIsNympho = xxx.is_nympho(pawn) || xxx.is_animal(pawn);
			bool pawnCanPickAnyone = RJWSettings.WildMode;// || (pawnIsNympho && RJWHookupSettings.NymphosCanPickAnyone);
			bool pawnCanPickAnimals = (pawnCanPickAnyone || xxx.is_zoophile(pawn)) && RJWSettings.bestiality_enabled;

			if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): nympho:{pawnIsNympho}, ignores rules:{pawnCanPickAnyone}, zoo:{pawnCanPickAnimals}");

			if (!RJWHookupSettings.ColonistsCanHookup && pawn.IsFreeColonist && !pawnCanPickAnyone)
			{
				if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): is a colonist and colonist hookups are disabled in mod settings");
				return null;
			}

			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());

			// Check AllPawns, not just colonists, to include guests.
			List<Pawn> targets = map.mapPawns.AllPawns.Where(x
				=> x != pawn
				&& ((!bedsex && !x.InBed()) || (bedsex && (Bed_Utility.is_laying_down_alone(x) || Bed_Utility.in_same_bed(x, pawn))))
				&& !x.IsForbidden(pawn)
				&& xxx.IsTargetPawnOkay(x)
				&& CanHaveSex(x)
				&& x.Map == pawn.Map
				&& !x.HostileTo(pawn)
				//&& ((pawnCanPickAnimals && xxx.is_animal(x)) || !xxx.is_animal(x))
				&& !xxx.is_animal(x)
				).ToList();

			if (!targets.Any())
			{
				if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): no eligible targets");
				return null;
			}

			// HippieMode means: everyone is allowed to have sex with everyone else ("free love")
			//	- no prioritization of partners
			//	- not necessary to be horny
			//	- never cheating
			// -> just pick a partner

			if (!RJWSettings.HippieMode)
			{
				if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): considering {targets.Count} targets");

				// find lover/partner on same map
					List<Pawn> partners = targets.Where(x
					  => pawn.relations.DirectRelationExists(PawnRelationDefOf.Lover, x)
					  || pawn.relations.DirectRelationExists(PawnRelationDefOf.Fiance, x)
					  || pawn.relations.DirectRelationExists(PawnRelationDefOf.Spouse, x)
					  || pawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, x)
					  ||(pawn.IsAnimal() && pawn.playerSettings.RespectedMaster == x)
					  ).ToList();


					if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): considering {partners.Count} partners");

					if (partners.Any())
					{
						partners.Shuffle(); //Randomize order.
						foreach (Pawn target in partners)
						{
							if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): checking lover {xxx.get_pawnname(target)}");

							//interruptible jobs
							if (!xxx.is_animal(pawn))
							{
								if (!bedsex && target.jobs?.curJob != null &&
									(target.jobs.curJob.playerForced ||
									!CasualSex_Helper.quickieAllowedJobs.Contains(target.jobs.curJob.def)
									))
								{

									if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_pawn_to_fuck({pawnName}): lover has important job ({target.jobs.curJob.def}), skipping");
									continue;
								}
							}
							if (Pather_Utility.cells_to_target_casual(pawn, target.Position)
								&& pawn.CanReserveAndReach(target, PathEndMode.OnCell, Danger.Some, 1, 0)
								&& target.CanReserve(pawn, 1, 0)
								&& SexAppraiser.would_fuck(pawn, target) > 0.1f
								&& SexAppraiser.would_fuck(target, pawn) > 0.1f)
							{
								if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): banging lover {xxx.get_pawnname(target)}");
								return target;
							}
						}
					}
					// No lovers around... see if the pawn fancies a hookup.  Nymphos and frustrated pawns always do!
					if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): no partners available.  checking canHookup");
					bool canHookup = pawnIsNympho || pawnCanPickAnyone || xxx.is_frustrated(pawn) || (xxx.is_horny(pawn) && Rand.Value < RJWHookupSettings.HookupChanceForNonNymphos);
					if (!canHookup)
					{
						if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): no hookup today");
						return null;
					}

					// No cheating from casual hookups... would probably make colony relationship management too annoying
					if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): checking canHookupWithoutCheating");
					bool hookupWouldBeCheating = xxx.HasNonPolyPartner(pawn);
					if (hookupWouldBeCheating)
					{
						if (RJWHookupSettings.NymphosCanCheat && pawnIsNympho && xxx.is_frustrated(pawn))
						{
							if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): I'm a nympho and I'm so frustrated that I'm going to cheat");
							// No return here so they continue searching for hookup
						}
						else if (pawn.health.hediffSet.HasHediff(HediffDef.Named("AlcoholHigh")))
						{
							if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): I want to bang and im too drunk to care if its cheating");
						}
						else
						{
							if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({pawnName}): I want to bang but that's cheating");
							return null;
						}
					}	
			}
			Pawn best_fuckee = FindBestPartner(pawn, targets, pawnCanPickAnyone, pawnIsNympho);
			return best_fuckee;
		}
			
		/// <summary> Checks all of our potential partners to see if anyone's eligible, returning the most attractive and convenient one. </summary>
		public static Pawn FindBestPartner(Pawn pawn, List<Pawn> targets, bool pawnCanPickAnyone, bool pawnIsNympho)
		{
			string pawnName = xxx.get_pawnname(pawn);

			Pawn best_fuckee = null;
			float best_fuckability_score = 0;

			foreach (Pawn targetPawn in targets)
			{
				if (targetPawn.relations == null) // probably droids or who knows what modded abomination
					continue;

				if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): checking hookup {xxx.get_pawnname(targetPawn)}");

				// Check to see if the mod settings for hookups allow this pairing
				if (!pawnCanPickAnyone && !HookupAllowedViaSettings(pawn, targetPawn))
					continue;

				//interruptible jobs
				if (!xxx.is_animal(pawn))
				{
					if (targetPawn.jobs?.curJob != null &&
						(targetPawn.jobs.curJob.playerForced ||
						!CasualSex_Helper.quickieAllowedJobs.Contains(targetPawn.jobs.curJob.def)
						))
					{
						if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): targetPawn has important job ({targetPawn.jobs.curJob.def}, playerForced: {targetPawn.jobs.curJob.playerForced}), skipping");
						continue;
					}
				}

				// Check for homewrecking (banging a pawn who's in a relationship)
				if (!xxx.is_animal(targetPawn) &&
					!RJWSettings.HippieMode &&
					xxx.HasNonPolyPartner(targetPawn))
				{
					if (RJWHookupSettings.NymphosCanHomewreck && pawnIsNympho && xxx.is_frustrated(pawn))
					{
						// Hookup allowed... rip colony mood
					}
					else if (RJWHookupSettings.NymphosCanHomewreckReverse && xxx.is_nympho(targetPawn) && xxx.is_frustrated(targetPawn))
					{
						// Hookup allowed... rip colony mood
					}
					else if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("AlcoholHigh")))
					{
						if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): not hooking up with {xxx.get_pawnname(targetPawn)} to avoid homewrecking");
						continue;
					}
				}

				// If the pawn has had sex recently and isn't horny right now, skip them.
				if (!SexUtility.ReadyForLovin(targetPawn) && !xxx.is_hornyorfrustrated(targetPawn) && !xxx.is_animal(pawn))
				{
					if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): hookup {xxx.get_pawnname(targetPawn)} isn't ready for lovin'");
					continue;
				}

				if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): hookup {xxx.get_pawnname(targetPawn)} is sufficiently single");

				if (!xxx.is_animal(targetPawn))
				{
					float relations = pawn.relations.OpinionOf(targetPawn);
					if (relations < RJWHookupSettings.MinimumRelationshipToHookup)
					{
						if (!(relations > 0 && xxx.is_nympho(pawn)))
						{
							if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): hookup {xxx.get_pawnname(targetPawn)}, i don't like them:({relations})");
							continue;
						}
					}

					relations = targetPawn.relations.OpinionOf(pawn);
					if (relations < RJWHookupSettings.MinimumRelationshipToHookup)
					{
						if (!(relations > 0 && xxx.is_nympho(targetPawn)))
						{
							if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): hookup {xxx.get_pawnname(targetPawn)}, don't like me:({relations})");
							continue;
						}
					}

					float attraction = pawn.relations.SecondaryRomanceChanceFactor(targetPawn);
					if (attraction < RJWHookupSettings.MinimumAttractivenessToHookup)
					{
						if (!(attraction > 0 && xxx.is_nympho(pawn)))
						{
							if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): hookup {xxx.get_pawnname(targetPawn)}, i don't find them attractive:({attraction})");
							continue;
						}
					}
					attraction = targetPawn.relations.SecondaryRomanceChanceFactor(pawn);
					if (attraction < RJWHookupSettings.MinimumAttractivenessToHookup)
					{
						if (!(attraction > 0 && xxx.is_nympho(targetPawn)))
						{
							if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): hookup {xxx.get_pawnname(targetPawn)}, doesn't find me attractive:({attraction})");
							continue;
						}
					}
				}

				// Check to see if the two pawns are willing to bang, and if so remember how much attractive we find them
				float fuckability = 0f;
				if (pawn.CanReserveAndReach(targetPawn, PathEndMode.OnCell, Danger.Some, 1, 0) &&
					targetPawn.CanReserve(pawn, 1, 0) &&
					roll_to_skip(pawn, targetPawn, out fuckability)) // do NOT check pawnIgnoresRules here - these checks, particularly roll_to_skip, are critical
				{
					float dis = pawn.Position.DistanceTo(targetPawn.Position);

					if (dis <= 4)
					{
						// Right next to me (in my bed)?  You'll do.
						if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): hookup {xxx.get_pawnname(targetPawn)} is right next to me. we'll bang, ok?");
						best_fuckability_score = 1.0e6f;
						best_fuckee = targetPawn;
						break;
					}
					else if (dis > RJWSettings.maxDistanceCellsCasual)
					{
						// too far
						if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): hookup {xxx.get_pawnname(targetPawn)} is too far... distance:{dis} max:{RJWSettings.maxDistanceCellsCasual}");
						continue;
					}
					else
					{
						// scaling fuckability by distance may give us more varied results and give the less attractive folks a chance
						float fuckability_score = fuckability / GenMath.Sqrt(GenMath.Sqrt(dis));
						if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): hookup {xxx.get_pawnname(targetPawn)} is totally bangable.  attraction: {fuckability}, score:{fuckability_score}");

						if (fuckability_score > best_fuckability_score)
						{
							best_fuckee = targetPawn;
							best_fuckability_score = fuckability_score;
						}
					}
				}
			}

			if (best_fuckee == null)
			{
				if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): couldn't find anyone to bang");
			}
			else
			{
				if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" FindBestPartner({pawnName}): found rando {xxx.get_pawnname(best_fuckee)} with score {best_fuckability_score}");
			}

			return best_fuckee;
		}

		/// <summary> Checks to see if the mod settings allow the two pawns to hookup. </summary>
		public static bool HookupAllowedViaSettings(Pawn pawn, Pawn targetPawn)
		{
			// Can prisoners hook up?
			if (pawn.IsPrisonerOfColony || pawn.IsPrisoner || xxx.is_slave(pawn))
			{
				if (!RJWHookupSettings.PrisonersCanHookupWithNonPrisoner && !(targetPawn.IsPrisonerOfColony || targetPawn.IsPrisoner || xxx.is_slave(targetPawn)))
				{
					if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({xxx.get_pawnname(pawn)}): not hooking up with {xxx.get_pawnname(targetPawn)} due to mod setting PrisonersCanHookupWithNonPrisoner");
					return false;
				}

				if (!RJWHookupSettings.PrisonersCanHookupWithPrisoner && (targetPawn.IsPrisonerOfColony || targetPawn.IsPrisoner || xxx.is_slave(targetPawn)))
				{
					if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({xxx.get_pawnname(pawn)}): not hooking up with {xxx.get_pawnname(targetPawn)} due to mod setting PrisonersCanHookupWithPrisoner");
					return false;
				}
			}
			else
			{
				// Can non prisoners hook up with prisoners?
				if (!RJWHookupSettings.CanHookupWithPrisoner && (targetPawn.IsPrisonerOfColony || targetPawn.IsPrisoner || xxx.is_slave(targetPawn)))
				{
					if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({xxx.get_pawnname(pawn)}): not hooking up with {xxx.get_pawnname(targetPawn)} due to mod setting CanHookupWithPrisoner");
					return false;
				}
			}

			// Can slave hook up?
			//if (xxx.is_slave(pawn))
			//{
			//	if (!RJWHookupSettings.SlaveCanHookup)
			//	{
			//		if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({xxx.get_pawnname(pawn)}): not hooking up with {xxx.get_pawnname(targetPawn)} due to mod setting SlaveCanHookup");
			//		return false;
			//	}
			//}

			// Can colonist hook up with visitors?
			if (pawn.IsFreeColonist && !xxx.is_slave(pawn))
			{
				if (!RJWHookupSettings.ColonistsCanHookupWithVisitor && targetPawn.Faction != Faction.OfPlayer && !targetPawn.IsPrisonerOfColony)
				{
					if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({xxx.get_pawnname(pawn)}): not hooking up with {xxx.get_pawnname(targetPawn)} due to mod setting ColonistsCanHookupWithVisitor");
					return false;
				}
			}

			// Can visitors hook up?
			if (pawn.Faction != Faction.OfPlayer && !pawn.IsPrisonerOfColony)
			{
				// visitors vs colonist
				if (!RJWHookupSettings.VisitorsCanHookupWithColonists && targetPawn.IsColonist)
				{
					if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({xxx.get_pawnname(pawn)}): not hooking up with {xxx.get_pawnname(targetPawn)} due to mod setting VisitorsCanHookupWithColonists");
					return false;
				}

				// visitors vs visitors
				if (!RJWHookupSettings.VisitorsCanHookupWithVisitors && targetPawn.Faction != Faction.OfPlayer)
				{
					if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" find_partner({xxx.get_pawnname(pawn)}): not hooking up with {xxx.get_pawnname(targetPawn)} due to mod setting VisitorsCanHookupWithVisitors");
					return false;
				}
			}

			// TODO: Not sure if this handles all the pawn-on-animal cases.

			return true;
		}

		[SyncMethod]
		public static IntVec3 FindSexLocation(Pawn pawn, Pawn partner = null)
		{
			IntVec3 position = pawn.Position;
			int bestPosition = -100;
			IntVec3 cell = pawn.Position;
			int maxDistance = 40;

			FloatRange temperature = pawn.ComfortableTemperatureRange();
			List<Pawn> all_pawns = pawn.Map.mapPawns.AllPawnsSpawned.Where(x
				=> x.Position.DistanceTo(pawn.Position) < 100
				&& xxx.is_human(x)
				&& x != pawn
				&& x != partner
				).ToList();

			bool is_somnophile = xxx.has_quirk(pawn, "Somnophile");
			bool is_exhibitionist = xxx.has_quirk(pawn, "Exhibitionist");

			//ModLog.Message(" Pawn is " + xxx.get_pawnname(pawn) + ", current cell is " + cell);

			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			List<IntVec3> random_cells = new List<IntVec3>();
			Dictionary<int, int> random_cells_rooms_doors = new Dictionary<int, int>();
			for (int loop = 0; loop < 50; ++loop)
			{
				random_cells.Add(position + IntVec3.FromVector3(Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) * Rand.RangeInclusive(1, maxDistance)));
			}

			random_cells = random_cells.Where(x
				=> x.Standable(pawn.Map)
				&& x.InAllowedArea(pawn)
				&& x.GetDangerFor(pawn, pawn.Map) != Danger.Deadly
				&& !x.ContainsTrap(pawn.Map)
				&& !x.ContainsStaticFire(pawn.Map)
				).Distinct().ToList();

			//ModLog.Message(" Found " + random_cells.Count + " valid cells.");

			foreach (var rcell in random_cells)
			{
				var rooom = rcell.GetRoom(pawn.Map);
				if (!random_cells_rooms_doors.ContainsKey(rooom.ID) && !rooom.PsychologicallyOutdoors)
					random_cells_rooms_doors.Add(rooom.ID, rooom.ContainedAndAdjacentThings.Count(
						x => x.def.IsDoor
						&& x.def.holdsRoof && x.def.blockLight));   //dubs hyg toilets stall doors(false,false)?
			}

			foreach (IntVec3 random_cell in random_cells)
			{
				if (!Pather_Utility.cells_to_target_casual(pawn, random_cell))
					continue;// too far

				int score = 0;
				Room room = random_cell.GetRoom(pawn.Map);

				bool might_be_seen = MightBeSeen(all_pawns, random_cell, pawn, partner);

				if (is_exhibitionist)
				{
					if (might_be_seen)
						score += 5;
					else
						score -= 10;
				}
				else
				{
					if (might_be_seen)
						score -= 30;
				}
				if (partner == null && is_somnophile) // Fap while Watching someone sleep. Not creepy at all!
				{
					if (all_pawns.Any(x
						=> !x.Awake()
						&& x.Position.DistanceTo(random_cell) < 6
						&& GenSight.LineOfSight(random_cell, x.Position, pawn.Map)
						))
						score += 50;
				}

				if (random_cell.GetTemperature(pawn.Map) > temperature.min && random_cell.GetTemperature(pawn.Map) < temperature.max)
					score += 20;
				else
					score -= 20;
				if (random_cell.Roofed(pawn.Map))
					score += 5;
				if (random_cell.HasEatSurface(pawn.Map))
					score += 5; // Hide in vegetation.
				if (random_cell.GetDangerFor(pawn, pawn.Map) == Danger.Some)
					score -= 25;
				else if (random_cell.GetDangerFor(pawn, pawn.Map) == Danger.None)
					score += 5;
				if (random_cell.GetTerrain(pawn.Map) == TerrainDefOf.WaterShallow ||
					random_cell.GetTerrain(pawn.Map) == TerrainDefOf.WaterMovingShallow ||
					random_cell.GetTerrain(pawn.Map) == TerrainDefOf.WaterOceanShallow)
					score -= 20;

				if (random_cell.GetThingList(pawn.Map).Any(x => x.def.IsWithinCategory(ThingCategoryDefOf.Corpses)))
					if (xxx.is_necrophiliac(pawn))
						score += 20;
					else
						score -= 20;
				if (random_cell.GetThingList(pawn.Map).Any(x => x.def.IsWithinCategory(ThingCategoryDefOf.Foods)))
					score -= 10;

				if (room == pawn.Position.GetRoom(pawn.MapHeld))
					score -= 10;
				if (room.PsychologicallyOutdoors)
					score += 5;
				if (room.IsHuge)
					score -= 5;
				if (room.ContainedBeds.Any())
				{
					score += 5;
					if (room.ContainedBeds.Any(x => x.Position == random_cell))
						score += 5;
				}
				if (!room.Owners.Any())
					score += 10;
				else if (room.Owners.Contains(pawn))
					score += 20;
				if (room.IsDoorway && !is_exhibitionist)
					score -= 100;
				if (room.Role == RoomRoleDefOf.Bedroom)
					score += 10;
				else if (room.Role == RoomRoleDefOf.Barracks || room.Role == RoomRoleDefOf.PrisonBarracks || room.Role == RoomRoleDefOf.PrisonCell
					|| room.Role == RoomRoleDefOf.Laboratory || room.Role == RoomRoleDefOf.RecRoom
					|| room.Role == RoomRoleDefOf.DiningRoom || room.Role == RoomRoleDefOf.Hospital
					)
					if (is_exhibitionist)
						score += 10;
					else
						score -= 5;
				if (room.GetStat(RoomStatDefOf.Cleanliness) < 0.01f)
					score -= 5;
				if (room.GetStat(RoomStatDefOf.GraveVisitingJoyGainFactor) > 0.1f)
					if (xxx.is_necrophiliac(pawn))
					{
						score += 5;
					}
					else
					{
						score -= 5;
					}

				var doors = random_cells_rooms_doors.TryGetValue(room.ID);
				if (doors > 1)
					if (is_exhibitionist)
					{
						score += 2 * doors;
					}
					else
					{
						score -= 5 * doors;
					}

				if (score <= bestPosition) continue;

				bestPosition = score;
				cell = random_cell;
			}

			return cell;

			//ModLog.Message(" Best cell is " + cell);
		}

		public static bool MightBeSeen(List<Pawn> otherPawns, IntVec3 cell, Pawn pawn, Pawn partner = null)
		{
			return otherPawns.Any(x
					=> x != partner
					&& x.Awake()
					&& x.Position.DistanceTo(cell) < 50
					&& GenSight.LineOfSight(x.Position, cell, pawn.Map)
					);
		}
	}
}