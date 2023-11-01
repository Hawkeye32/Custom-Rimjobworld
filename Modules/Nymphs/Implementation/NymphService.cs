using RimWorld;
using rjw.Modules.Shared.Extensions;
using rjw.Modules.Shared.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Nymphs.Implementation
{
	public class NymphService : INymphService
	{
		private static ILog _log = LogManager.GetLogger<NymphService, NymphLogProvider>();

		public static readonly INymphService Instance;

		static NymphService()
		{
			Instance = new NymphService();
		}

		public Pawn GenerateNymph(Map map, PawnKindDef nymphKind = null, Faction faction = null)
		{
			if (nymphKind == null)
			{
				nymphKind = RandomNymphKind();
			}

			PawnGenerationRequest request = new PawnGenerationRequest(
				kind: nymphKind,
				faction: faction,
				tile: map.Tile,
				forceGenerateNewPawn: true,
				canGeneratePawnRelations: true,
				colonistRelationChanceFactor: 0.0f,
				inhabitant: true,
				relationWithExtraPawnChanceFactor: 0
				);

			Pawn nymph = PawnGenerator.GeneratePawn(request);

			_log.Debug($"Generated Nymph {nymph.GetName()}");

			return nymph;
		}

		public IEnumerable<Pawn> GenerateNymphs(Map map, int count)
		{
			if (count <= 0)
			{
				yield break;
			}

			PawnKindDef nymphKind = RandomNymphKind();

			for (int i = 0; i < count; i++)
			{
				yield return GenerateNymph(map, nymphKind);
			}
		}

		public PawnKindDef RandomNymphKind()
		{
			return ListNymphKindDefs()
				.RandomElement();
		}
		public IEnumerable<PawnKindDef> ListNymphKindDefs()
		{
			return DefDatabase<PawnKindDef>.AllDefs
				.Where(x => x.defName.Contains("Nymph"));
		}

		public void SetManhunter(Pawn nymph)
		{
			if (RJWSettings.NymphPermanentManhunter)
			{
				nymph.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
			}
			else
			{
				nymph.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter);
			}
		}
	}
}
