using rjw.Modules.Genitals.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared.Extensions;
using rjw.Modules.Shared.Logs;
using System.Linq;

namespace rjw.Modules.Genitals.Helpers
{
	public class GenderHelper
	{
		private static ILog _log = LogManager.GetLogger<GenderHelper>();

		public static Gender GetGender(InteractionPawn pawn)
		{
			bool hasVagina = pawn.Parts.Vaginas.Any();
			bool hasPenis = pawn.Parts.Penises.Any();
			bool hasOviFemale = pawn.Parts.FemaleOvipositors.Any();
			bool hasOviMale = pawn.Parts.MaleOvipositors.Any();
			bool hasBreasts = pawn.HasBigBreasts();

			Gender result = Gender.Unknown;


			if (hasVagina && !hasPenis)
			{
				result = Gender.Female;
			}
			else
			if (hasPenis && hasVagina)
			{
				result = Gender.Futa;
			}
			else
			if (hasPenis && hasBreasts)
			{
				result = Gender.Trap;
			}
			else
			if (hasPenis)
			{
				result = Gender.Male;
			}
			else
			if (hasOviMale)
			{
				result = Gender.MaleOvi;
			}
			else
			if (hasOviFemale)
			{
				result = Gender.FemaleOvi;
			}
			else
			if (pawn.Pawn.gender == Verse.Gender.Male)
			{
				result = Gender.Male;
			}
			else
			if (pawn.Pawn.gender == Verse.Gender.Female)
			{
				result = Gender.Female;
			}

			_log.Debug($"{pawn.Pawn.GetName()} detected as {result}");

			return result;
		}
	}
}
