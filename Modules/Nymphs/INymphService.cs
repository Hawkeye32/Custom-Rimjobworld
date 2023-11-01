using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Nymphs
{
	public interface INymphService
	{
		Pawn GenerateNymph(Map map, PawnKindDef nymphKind = null, Faction faction = null);
		IEnumerable<Pawn> GenerateNymphs(Map map, int count);

		PawnKindDef RandomNymphKind();
		IEnumerable<PawnKindDef> ListNymphKindDefs();

		void SetManhunter(Pawn nymph);

	}
}
