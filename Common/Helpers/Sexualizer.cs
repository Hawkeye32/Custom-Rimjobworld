using Multiplayer.API;
using Verse;

namespace rjw
{
	public static class Sexualizer
	{
		static void SexualizeSingleGenderPawn(Pawn pawn)
		{
			// Single gender is futa without the female gender change.
			SexPartAdder.add_genitals(pawn, null, Gender.Male);
			SexPartAdder.add_genitals(pawn, null, Gender.Female);
			SexPartAdder.add_breasts(pawn, null, Gender.Female);
			SexPartAdder.add_anus(pawn, null);
		}

		static void SexualizeGenderlessPawn(Pawn pawn)
		{
			if (RJWSettings.GenderlessAsFuta && !xxx.is_mechanoid(pawn) && (pawn.RaceProps.Animal || pawn.RaceProps.Humanlike))
			{
				if (RJWSettings.DevMode)
					ModLog.Message(" SexualizeGenderlessPawn() - genderless pawn, treating Genderless pawn As Futa" + xxx.get_pawnname(pawn));
				//set gender to female for futas
				pawn.gender = Gender.Female;
				SexPartAdder.add_genitals(pawn, null, Gender.Male);
				SexPartAdder.add_genitals(pawn, null, Gender.Female);
				SexPartAdder.add_breasts(pawn, null, Gender.Female);
				SexPartAdder.add_anus(pawn, null);
			}
			else
			{
				if (RJWSettings.DevMode)
					ModLog.Message(" SexualizeGenderlessPawn() - unable to sexualize genderless pawn " + xxx.get_pawnname(pawn) + " gender: " + pawn.gender);
			}
		}

		[SyncMethod]
		static void SexualizeGenderedPawn(Pawn pawn)
		{
			//apply normal gender
			SexPartAdder.add_genitals(pawn, null, pawn.gender);

			//apply futa gender
			//if (pawn.gender == Gender.Female) // changing male to futa will break pawn generation due to relations

			if (pawn.Faction != null && !xxx.is_animal(pawn)) //null faction throws error
			{
				//ModLog.Message(" SexualizeGenderedPawn( " + xxx.get_pawnname(pawn) + " ) techLevel: " + (int)pawn.Faction.def.techLevel);
				//ModLog.Message(" SexualizeGenderedPawn( " + xxx.get_pawnname(pawn) + " ) techLevel: " + pawn.Faction.Name);

				//natives/spacer futa
				float chance = (int)pawn.Faction.def.techLevel < 5 ? RJWSettings.futa_natives_chance : RJWSettings.futa_spacers_chance;
				//nymph futa gender
				chance = xxx.is_nympho(pawn) ? RJWSettings.futa_nymph_chance : chance;

				// ModLog.Message($"SexualizeGenderedPawn {chance} from {RJWSettings.futa_nymph_chance} {RJWSettings.futa_natives_chance} {RJWSettings.futa_spacers_chance}");
				if (Rand.Chance(chance))
				{
					//make futa
					if (pawn.gender == Gender.Female && RJWSettings.FemaleFuta)
						SexPartAdder.add_genitals(pawn, null, Gender.Male);
					//make trap
					else if (pawn.gender == Gender.Male && RJWSettings.MaleTrap)
						SexPartAdder.add_breasts(pawn, null, Gender.Female);
				}
			}
			SexPartAdder.add_breasts(pawn, null, pawn.gender);
			SexPartAdder.add_anus(pawn, null);
		}

		[SyncMethod]
		public static void sexualize_pawn(Pawn pawn)
		{
			//ModLog.Message(" sexualize_pawn( " + xxx.get_pawnname(pawn) + " ) called");
			if (pawn == null)
			{
				return;
			}

			//override pawn
			if (RacePartDef_Helper.TryRacePartDef_partAdders(pawn))
				return;

			if (RaceGroupDef_Helper.TryGetRaceGroupDef(pawn, out var raceGroupDef) &&
				raceGroupDef.hasSingleGender)
			{
				if (RJWSettings.DevMode)
					ModLog.Message($"sexualize_pawn() - sexualizing single gender pawn {xxx.get_pawnname(pawn)}  race: {raceGroupDef.defName}");
				SexualizeSingleGenderPawn(pawn);
			}
			else if (pawn.RaceProps.hasGenders)
			{
				SexualizeGenderedPawn(pawn);
			}
			else
			{
				if (Current.ProgramState == ProgramState.Playing) // DO NOT run at world generation, throws error in generating relationship stuff
				{
					SexualizeGenderlessPawn(pawn);
					return;
				}
			}

			if (!pawn.Dead)
			{
				//Add ticks at generation, so pawns don't instantly start lovin' when generated (esp. on scenario start).
				//if (xxx.RoMIsActive && pawn.health.hediffSet.HasHediff(HediffDef.Named("TM_ShapeshiftHD"))) // Rimworld of Magic's polymorph/shapeshift
				//	pawn.mindState.canLovinTick = Find.TickManager.TicksGame + (int)(SexUtility.GenerateMinTicksToNextLovin(pawn) * Rand.Range(0.01f, 0.05f));
				if (!xxx.is_insect(pawn))
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + (int)(SexUtility.GenerateMinTicksToNextLovin(pawn) * Rand.Range(0.1f, 1.0f));
				else
					pawn.mindState.canLovinTick = Find.TickManager.TicksGame + (int)(SexUtility.GenerateMinTicksToNextLovin(pawn) * Rand.Range(0.01f, 0.2f));

				//ModLog.Message(" sexualize_pawn( " + xxx.get_pawnname(pawn) + " ) add sexneed");
				if (pawn.RaceProps.Humanlike)
				{
					var sex_need = pawn.needs.TryGetNeed<Need_Sex>();
					if (pawn.Faction != null && !(pawn.Faction?.IsPlayer ?? false) && sex_need != null)
					{
						sex_need.CurLevel = Rand.Range(0.01f, 0.75f);
					}
				}
			}
		}
	}
}
