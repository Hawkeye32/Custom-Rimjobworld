using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;
using Multiplayer.API;
using System.Linq;

namespace rjw
{
	public abstract class JobDriver_Sex : JobDriver
	{

		public readonly TargetIndex iTarget = TargetIndex.A;	//pawn or corpse
		public readonly TargetIndex iBed = TargetIndex.B;		//bed(maybe some furniture in future?)
		public readonly TargetIndex iCell = TargetIndex.C;		//cell/location to have sex at(fapping)

		public float satisfaction = 1.0f;

		public bool shouldreserve = true;

		public int stackCount = 0;

		public int ticks_between_hearts = 60;
		public int ticks_between_hits = 60;
		public int ticks_between_thrusts = 60;
		public int ticks_left = 1000;	//toil ticks
		public int sex_ticks = 1000;	//orgasm ticks
		public int orgasms = 0;

		// Sex ticks will decrease until it hits the orgasm tick, then orgasm happens and sex_ticks may be reset
		// Sex continues for around 3 seconds
		public int orgasmstick = 180; // ~3 sec
		public int orgasmStartTick = 5000;
		public int duration = 5000;

		public bool face2face = false;
		public bool isEndytophile = false;
		public bool isAnimalOnAnimal = false;
		public bool shouldGainFocus = false;
		public bool shouldGainFocusP = false;
		public bool isSuccubus = false;
		public bool isSuccubusP = false;

		//toggles
		public bool beatings = false;// toggle on off, todo maybe?
		public bool beatonce = false;
		public bool neverendingsex = false;

		public Thing Target			// for reservation
		{
			get
			{
				if (job == null)
				{
					return null;
				}

				if (job.GetTarget(TargetIndex.A).Pawn != null)
					return job.GetTarget(TargetIndex.A).Pawn;

				return job.GetTarget(TargetIndex.A).Thing;
			}
		}

		public Pawn Partner
		{
			get
			{
				if (PartnerPawn != null)
					return PartnerPawn;
				else if (Target is Pawn)
					return job.GetTarget(TargetIndex.A).Pawn;
				else if (Target is Corpse)
					return (job.GetTarget(TargetIndex.A).Thing as Corpse).InnerPawn;
				else
					return null;
			}
		}

		public Building_Bed Bed
		{
			get
			{
				if (pBed != null)
					return pBed;
				else if (job.GetTarget(TargetIndex.B).Thing is Building_Bed)
					return job.GetTarget(TargetIndex.B).Thing as Building_Bed;
				else
					return null;
			}
		}
		//not bed; chair, maybe something else in future
		public Building Building
		{
			get
			{
				if (job.GetTarget(TargetIndex.B).Thing is Building && !(job.GetTarget(TargetIndex.B).Thing is Building_Bed))
					return job.GetTarget(TargetIndex.B).Thing as Building;
				else
					return null;
			}
		}

		public Pawn PartnerPawn = null;
		public Building_Bed pBed = null;
		public SexProps Sexprops = null;

		[SyncMethod]
		public void setup_ticks()
		{
			SetupDurationTicks();
			SetupOrgasmTicks();

			ticks_between_hearts = Rand.RangeInclusive(70, 130);
			ticks_between_hits = Rand.Range(xxx.config.min_ticks_between_hits, xxx.config.max_ticks_between_hits);
			if (xxx.is_bloodlust(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.75);
			if (xxx.is_brawler(pawn))
				ticks_between_hits = (int)(ticks_between_hits * 0.90);
			ticks_between_thrusts = 120;
		}

		public void Set_bed(Building_Bed newBed)
		{
			pBed = newBed;
		}

		public float OrgasmProgress {
			get {
				var orgasmDuration = orgasmStartTick - orgasmstick;
				var ticksUntilOrgasm = sex_ticks - orgasmstick;
				
				return 1f - (ticksUntilOrgasm / (float) orgasmDuration);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticks_left, "ticks_left", 0, false);
			Scribe_Values.Look(ref ticks_between_hearts, "ticks_between_hearts", 0, false);
			Scribe_Values.Look(ref ticks_between_hits, "ticks_between_hits", 0, false);
			Scribe_Values.Look(ref ticks_between_thrusts, "ticks_between_thrusts", 0, false);
			Scribe_Values.Look(ref duration, "duration", 0, false);
			Scribe_Values.Look(ref sex_ticks, "sex_ticks", 0, false);
			Scribe_Values.Look(ref orgasms, "orgasms", 0, false);
			Scribe_Values.Look(ref orgasmstick, "orgasmstick", 0, false);
			Scribe_Values.Look(ref orgasmStartTick, "orgasmStartTick", 0, false);

			Scribe_References.Look(ref pBed, "pBed");
			Scribe_References.Look(ref PartnerPawn, "PartnerPawn");
			Scribe_Deep.Look(ref Sexprops, "Sexprops", new SexProps[0]);
			Scribe_Values.Look(ref beatings, "beatings");
			Scribe_Values.Look(ref beatonce, "beatonce");
			Scribe_Values.Look(ref neverendingsex, "neverendingsex");
			Scribe_Values.Look(ref face2face, "face2face");
			Scribe_Values.Look(ref isEndytophile, "isEndytophile");
			Scribe_Values.Look(ref shouldGainFocus, "shouldGainFocus");
			Scribe_Values.Look(ref shouldGainFocusP, "shouldGainFocusP");
			Scribe_Values.Look(ref isSuccubus, "isSuccubus");
			Scribe_Values.Look(ref isSuccubusP, "isSuccubusP");
		}

		public void SexTick(Pawn pawn, Thing target, bool pawnnude = true, bool partnernude = true)
		{
			ticks_left--;
			sex_ticks--;
			Orgasm();

			var partner = target as Pawn;
			if (partner != null)
			{
				if (partner.jobs?.curDriver is JobDriver_SexBaseReciever)//tick partner
				{
					((JobDriver_SexBaseReciever)partner.jobs.curDriver as JobDriver_SexBaseReciever).ticks_left--;
					((JobDriver_SexBaseReciever)partner.jobs.curDriver as JobDriver_SexBaseReciever).sex_ticks--;
					((JobDriver_SexBaseReciever)partner.jobs.curDriver as JobDriver_SexBaseReciever).Orgasm();
				}

				if (pawn.jobs?.curDriver is JobDriver_SexBaseInitiator)
				{
					var hit = false;
					if (beatonce)
					{
						beatonce = false;
						SexUtility.Sex_Beatings_Dohit(pawn, Partner, Sexprops.isRapist);
					}
					else if (pawn.IsHashIntervalTick(ticks_between_hits))
					{
						Roll_to_hit();
					}
					if (hit)
						if (!isEndytophile)
						{
							SexUtility.DrawNude(pawn);

							if (partner != null)
								SexUtility.DrawNude(partner);
						}
				}
			}

			if (pawn.IsHashIntervalTick(ticks_between_thrusts))
			{
				ChangePsyfocus(pawn, partner);
				Animate(pawn, partner);
				PlaySexSound();
				if (!Sexprops.isRape)
				{
					pawn.GainComfortFromCellIfPossible();
					if (partner != null)
						partner.GainComfortFromCellIfPossible();
				}
			}
		}

		/// <summary>
		/// simple rjw thrust animation
		/// </summary>
		public void Animate(Pawn pawn, Thing target)
		{
			RotatePawns(pawn, Partner);
			//attack/ride 1x2 cell cocksleeve/dildo?
			//if (Building != null)
			//	target = Building;
			if (target != null)
			{
				pawn.Drawer.Notify_MeleeAttackOn(target);

				var partner = target as Pawn;
				if (partner != null && !Sexprops.isRapist)
					partner.Drawer.Notify_MeleeAttackOn(pawn);


				//refresh DrawNude after beating and Notify_MeleeAttackOn
				// Endytophiles prefer clothed sex, everyone else gets nude.
				if (!isEndytophile)
				{
					SexUtility.DrawNude(pawn);

					if (partner != null)
						SexUtility.DrawNude(partner);
				}
			}
			else
			{
				//refresh DrawNude after beating and Notify_MeleeAttackOn
				// Endytophiles prefer clothed sex, everyone else gets nude.
				if (!isEndytophile)
				{
					SexUtility.DrawNude(pawn);
				}
			}
		}

		/// <summary>
		/// increase Psyfocus by having sex
		/// </summary>
		public void ChangePsyfocus(Pawn pawn, Thing target)
		{
			if (ModsConfig.RoyaltyActive)
			{
				if (pawn.jobs?.curDriver is JobDriver_ViolateCorpse)
					if (xxx.is_necrophiliac(pawn) && MeditationFocusTypeAvailabilityCache.PawnCanUse(pawn, DefDatabase<MeditationFocusDef>.GetNamedSilentFail("Morbid")))
					{
						SexUtility.OffsetPsyfocus(pawn, 0.01f);
					}

				if (target != null)
				{
					var partner = target as Pawn;
					if (partner != null)
					{
						if (shouldGainFocus)
							SexUtility.OffsetPsyfocus(pawn, 0.01f);

						if (shouldGainFocusP)
							SexUtility.OffsetPsyfocus(partner, 0.01f);

						if (isSuccubus)
							SexUtility.OffsetPsyfocus(pawn, 0.01f);

						if (isSuccubusP)
							SexUtility.OffsetPsyfocus(partner, 0.01f);
					}
				}
			}
		}

		/// <summary>
		/// rotate pawns
		/// </summary>
		public void RotatePawns(Pawn pawn, Thing target)
		{
			if (Building != null)
			{
				if (face2face)
					pawn.Rotation = Building.Rotation.Opposite;
				else
					pawn.Rotation = Building.Rotation;

				return;
			}
			if (target == null) // solo
			{
				//pawn.Rotation = Rot4.South;
				return;
			}

			if (target is not Pawn partner || partner.Dead) // necro
			{
				pawn.rotationTracker.Face(target.DrawPos);
				return;
			}

			if (partner.jobs?.curDriver is JobDriver_SexBaseReciever receiverDriver)
				if (receiverDriver.parteners.Count > 1)
					return;

			//maybe could do a hand check for monster girls but w/e
			//bool partnerHasHands = Receiver.health.hediffSet.GetNotMissingParts().Any(part => part.IsInGroup(BodyPartGroupDefOf.RightHand) || part.IsInGroup(BodyPartGroupDefOf.LeftHand));

			// most of animal sex is likely doggystyle.
			if (isAnimalOnAnimal)
			{
				if (Sexprops.sexType == xxx.rjwSextype.Anal || Sexprops.sexType == xxx.rjwSextype.Vaginal || Sexprops.sexType == xxx.rjwSextype.DoublePenetration)
				{
					//>>
					//Log.Message("animal doggy");
					pawn.rotationTracker.Face(partner.DrawPos);
					partner.Rotation = pawn.Rotation;
				}
				else
				{
					//><
					//Log.Message("animal non doggy");
					pawn.rotationTracker.Face(target.DrawPos);
					partner.rotationTracker.Face(pawn.DrawPos);
				}
			}
			else
			{
				if (this is JobDriver_BestialityForFemale)
				{
					if (Sexprops.sexType == xxx.rjwSextype.Anal || Sexprops.sexType == xxx.rjwSextype.Vaginal || Sexprops.sexType == xxx.rjwSextype.DoublePenetration)
					{
						//<<
						//Log.Message("bestialityFF doggy");
						partner.rotationTracker.Face(pawn.DrawPos);
						pawn.Rotation = partner.Rotation;
					}
					else
					{
						//><
						//Log.Message("bestialityFF non doggy");
						pawn.rotationTracker.Face(target.DrawPos);
						partner.rotationTracker.Face(pawn.DrawPos);
					}
				}
				else if (partner.GetPosture() == PawnPosture.LayingInBed)
				{
					//x^
					//Log.Message("loving/casualsex in bed");
					// this could use better handling for cowgirl/reverse cowgirl and who pen who, if such would be implemented
					//until then...

					if (!face2face && Sexprops.sexType == xxx.rjwSextype.Anal ||
										Sexprops.sexType == xxx.rjwSextype.Vaginal ||
										Sexprops.sexType == xxx.rjwSextype.DoublePenetration ||
										Sexprops.sexType == xxx.rjwSextype.Fisting)
						//if (xxx.is_female(pawn) && xxx.is_female(partner))
					{
						// in bed loving face down
						pawn.Rotation = partner.CurrentBed().Rotation.Opposite;
					}
					//else if (!(xxx.is_male(pawn) && xxx.is_male(partner)))
					//{
					//	// in bed loving face down
					//	pawn.Rotation = partner.CurrentBed().Rotation.Opposite;
					//}
					else
					{
						// in bed loving, face up
						pawn.Rotation = partner.CurrentBed().Rotation;
					}
				}
				// 30% chance of face-to-face regardless, for variety.
				else if (!face2face && (Sexprops.sexType == xxx.rjwSextype.Anal ||
										Sexprops.sexType == xxx.rjwSextype.Vaginal ||
										Sexprops.sexType == xxx.rjwSextype.DoublePenetration ||
										Sexprops.sexType == xxx.rjwSextype.Fisting))
				{
					//>>
					//Log.Message("doggy");
					pawn.rotationTracker.Face(target.DrawPos);
					partner.Rotation = pawn.Rotation;
				}
				// non doggystyle, or face-to-face regardless
				else
				{
					//><
					//Log.Message("non doggy");
					pawn.rotationTracker.Face(target.DrawPos);
					partner.rotationTracker.Face(pawn.DrawPos);
				}
			}
		}

		[SyncMethod]
		public void Rollface2face(float chance = 0.3f)
		{
			Setface2face(Rand.Chance(chance));
		}
		public void Setface2face(bool chance)
		{
			face2face = chance;
		}

		public void Roll_to_hit()
		{
			if (beatings || (Sexprops.isRapist && RJWSettings.rape_beating))
				SexUtility.Sex_Beatings(Sexprops);
		}
		public void ThrowMetaIcon(IntVec3 pos, Map map, ThingDef icon)
		{
			MoteMaker.MakeStaticMote(pos, map, icon);
		}
		public void ThrowMetaIconF(IntVec3 pos, Map map, FleckDef icon)
		{
			FleckMaker.ThrowMetaIcon(pos, map, icon);
		}
		public void PlaySexSound()
		{
			if (RJWSettings.sounds_enabled)
			{
				SoundInfo sound = new TargetInfo(pawn.Position, pawn.Map);
				sound.volumeFactor = RJWSettings.sounds_sex_volume;
				if(isAnimalOnAnimal)
					sound.volumeFactor *= RJWSettings.sounds_animal_on_animal_volume;
				SoundDef.Named("Sex").PlayOneShot(sound);
			}
		}
		public void PlayCumSound()
		{
			if (RJWSettings.sounds_enabled)
			{
				SoundInfo sound = new TargetInfo(pawn.Position, pawn.Map);
				sound.volumeFactor = RJWSettings.sounds_cum_volume;
				if (isAnimalOnAnimal)
					sound.volumeFactor *= RJWSettings.sounds_animal_on_animal_volume;
				SoundDef.Named("Cum").PlayOneShot(sound);
			}
		}
		public void PlaySexVoice()
		{
			//if (RJWSettings.sounds_enabled)
			//{
			//	SoundInfo sound = new TargetInfo(pawn.Position, pawn.Map);
			//	sound.volumeFactor = RJWSettings.sounds_voice_volume;
			//if (isAnimalOnAnimal)
			//	sound.volumeFactor *= RJWSettings.sounds_animal_on_animal_volume;
			//	SoundDef.Named("Sex").PlayOneShot(sound);
			//}
		}
		public void PlayOrgasmVoice()
		{
			//if (RJWSettings.sounds_enabled)
			//{
			//	SoundInfo sound = new TargetInfo(pawn.Position, pawn.Map);
			//	sound.volumeFactor = RJWSettings.sounds_orgasm_volume;
			//if (isAnimalOnAnimal)
			//	sound.volumeFactor *= RJWSettings.sounds_animal_on_animal_volume;
			//	SoundDef.Named("Orgasm").PlayOneShot(sound);
			//}
		}

		public void Orgasm()
		{
			if (sex_ticks > orgasmstick) //~3s at speed 1
			{
				return;
			}

			//if (RJWSettings.DevMode) Log.Message(xxx.get_pawnname(pawn) + " Orgasmed")
			orgasms++;
			// Sexprops are often passed to other mods for later evaluation of thoughts, memes, etc. 
			// the original orgasm counter is kept for stability
			Sexprops.orgasms ++;

			PlayCumSound();
			PlayOrgasmVoice();

			//Log.Message(xxx.get_pawnname(Sexprops.Giver));
			//Log.Message(xxx.get_pawnname(Sexprops.Reciever));
			//Log.Message(Sexprops.SexType.ToStringSafe());
			//Log.Message(Sexprops.DictionaryKey.ToStringSafe());
			//Log.Message(Sexprops.RulePack.ToStringSafe());
			
			if (!Sexprops.usedCondom)
			{
				//apply cum to floor:
				SexUtility.CumFilthGenerator(pawn);
				if (Partner != null)
					if (!pawn.Dead && !Partner.Dead)
					{
						//TODO: someday unfuck this
						//if (!(this is JobDriver_SexBaseReciever))
							if (!Sexprops.usedCondom)
							{
								PregnancyHelper.impregnate(Sexprops);
								SexUtility.TransferNutrition(Sexprops);
							}
					}
			}
			CalculateSatisfactionPerTick();

			SexUtility.SatisfyPersonal(Sexprops, satisfaction);
			//else
			//	SexUtility.SatisfyPersonal(pawn, ((JobDriver_SexBaseReciever)pawn.jobs?.curDriver).parteners.FirstOrFallback(), Sexprops, Sexprops.isRapist, satisfaction);

			SetupOrgasmTicks();

			if (neverendingsex)
				ticks_left = duration;
		}

		[SyncMethod]
		protected virtual void SetupDurationTicks()
		{
			ticks_left = (int)(2000.0f * Rand.Range(0.50f, 0.90f));
			duration = ticks_left;
		}

		[SyncMethod]
		protected virtual void SetupOrgasmTicks()
		{
			float need;
			if (xxx.is_human(pawn)) {
				need = 1.0f + xxx.need_some_sex(pawn); //1-4
			} else {
				need = 1.0f;
			}

			orgasmstick = 180;
			sex_ticks = (int)((duration / need) * Rand.Range(0.75f, 0.90f));
			orgasmStartTick = sex_ticks;
		}

		public void CalculateSatisfactionPerTick()
		{
			satisfaction = 0.4f;
		}

		public static bool IsInOrByBed(Building_Bed b, Pawn p)
		{
			for (int i = 0; i < b.SleepingSlotsCount; i++)
			{
				if (b.GetSleepingSlotPos(i).InHorDistOf(p.Position, 1f))
				{
					return true;
				}
			}
			return false;
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true; // No reservations needed.
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			return null;
		}
	}
}
