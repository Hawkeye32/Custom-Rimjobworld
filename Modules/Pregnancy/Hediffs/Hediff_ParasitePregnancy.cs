using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Multiplayer.API;

namespace rjw
{
	internal class Hediff_Parasite : Hediff_Pregnant
	{
		[SyncMethod]
		new public static void DoBirthSpawn(Pawn mother, Pawn father)
		{
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			int num = (mother.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(mother.RaceProps.litterSizeCurve));
			if (num < 1)
			{
				num = 1;
			}
			PawnGenerationRequest request = new PawnGenerationRequest(
					kind: father.kindDef,
					faction: father.Faction,
					forceGenerateNewPawn: true,
					developmentalStages: DevelopmentalStage.Newborn,
					allowDowned: true,
					canGeneratePawnRelations: false,
					mustBeCapableOfViolence: true,
					colonistRelationChanceFactor: 0,
					allowFood: false,
					allowAddictions: false,
					relationWithExtraPawnChanceFactor: 0
				);

			Pawn pawn = null;
			for (int i = 0; i < num; i++)
			{
				pawn = PawnGenerator.GeneratePawn(request);
				if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, mother))
				{
					if (pawn.playerSettings != null && mother.playerSettings != null)
					{
						pawn.playerSettings.AreaRestriction = father.playerSettings.AreaRestriction;
					}
					if (pawn.RaceProps.IsFlesh)
					{
						pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, mother);
						if (father != null)
						{
							pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, father);
						}
					}
				}
				else
				{
					Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
				}
			}
			if (mother.Spawned)
			{
				FilthMaker.TryMakeFilth(mother.Position, mother.Map, ThingDefOf.Filth_AmnioticFluid, mother.LabelIndefinite(), 5);
				if (mother.caller != null)
				{
					mother.caller.DoCall();
				}
				if (pawn.caller != null)
				{
					pawn.caller.DoCall();
				}
			}
		}
	}
}