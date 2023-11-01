#nullable enable

using System.Linq;
using Verse;
using rjw.Modules.Shared.Logs;
using rjw.Modules.Interactions.DefModExtensions;
using static rjw.Genital_Helper;

using Sex = rjw.GenderHelper.Sex;
using Seeded = rjw.Modules.Rand.Seeded;

namespace rjw.Modules.Testing
{
	static class TestHelper
	{
		static readonly ILog Log = LogManager.GetLogger("TestHelper");

		/// <summary>
		/// Adds some default options to this request that are desireable for testing.
		/// </summary>
		/// <param name="request">The request to modify.</param>
		/// <returns>The modified request.</returns>
		public static PawnGenerationRequest RequestDefaults(this PawnGenerationRequest request) =>
			request with
			{
				CanGeneratePawnRelations = false,
				ForceNoIdeo = true,
				ForceGenerateNewPawn = true
			};

		/// <summary>
		/// <para>Generates a natural pawn using the given request and a fixed seed.</para>
		/// <para>If there was an issue generating the pawn, it will return null.</para>
		/// </summary>
		/// <param name="request">The pawn request.</param>
		/// <returns>The generated pawn or null.</returns>
		public static Pawn? GenerateSeededPawn(PawnGenerationRequest request)
		{
			using (Seeded.With(42))
			{
				var tries = 5;
				while (tries >= 0)
				{
					var pawn = PawnGenerator.GeneratePawn(request);

					// Keep generating until we get a "natural" pawn.  We want to
					// control when futas and traps happen.
					switch (GenderHelper.GetSex(pawn))
					{
						case Sex.male when pawn.gender == Gender.Male:
						case Sex.female when pawn.gender == Gender.Female:
						case Sex.none when pawn.gender == Gender.None:
							return pawn;
					}
					tries -= 1;
				}
				Log.Error($"Could not generate test pawn for: {request.KindDef.defName}");
				return null;
			}
		}

		/// <summary>
		/// Tries to replace a sex-part of the given pawn using a recipe.
		/// </summary>
		/// <param name="pawn">The pawn to change.</param>
		/// <param name="recipe">The recipe to apply.</param>
		/// <returns>Whether the part was applied successfully.</returns>
		public static bool ApplyPartToPawn(Pawn pawn, RecipeDef recipe)
		{
			var worker = recipe.Worker;
			var hediffDef = recipe.addsHediff;
			if (!GenitalPartExtension.TryGet(hediffDef, out var ext)) return false;

			// They must have the correct body part for this sex part.
			var bpr = worker.GetPartsToApplyOn(pawn, recipe).FirstOrDefault();
			if (bpr is null) return false;

			var curParts = get_AllPartsHediffList(pawn)
				.Where((hed) => GenitalPartExtension.TryGet(hed, out var e) && e.family == ext.family)
				.ToArray();

			// We're replacing natural with artifical.
			if (curParts.Length == 0) return false;
			foreach (var part in curParts) pawn.health.RemoveHediff(part);

			var hediff = SexPartAdder.MakePart(hediffDef, pawn, bpr);
			pawn.health.AddHediff(hediff, bpr);
			return true;
		}

		/// <summary>
		/// Replaces the genitals of a pawn with artificial ones.
		/// </summary>
		/// <param name="pawn">The pawn to change.</param>
		/// <returns>Whether the part was swapped successfully.</returns>
		public static bool GiveArtificialGenitals(Pawn pawn)
		{
			var changed = false;
			if (get_genitalsBPR(pawn) is not { } bpr) return changed;

			if (has_penis_fertile(pawn))
			{
				foreach (var part in pawn.GetGenitalsList())
					if (is_fertile_penis(part))
						pawn.health.RemoveHediff(part);
				var hediff = SexPartAdder.MakePart(hydraulic_penis, pawn, bpr);
				pawn.health.AddHediff(hediff, bpr);
				changed = true;
			}

			if (has_vagina(pawn))
			{
				foreach (var part in pawn.GetGenitalsList())
					if (is_vagina(part))
						pawn.health.RemoveHediff(part);
				var hediff = SexPartAdder.MakePart(hydraulic_vagina, pawn, bpr);
				pawn.health.AddHediff(hediff, bpr);
				changed = true;
			}

			return changed;
		}

		/// <summary>
		/// <para>Adds parts to a pawn to make them into a trap.</para>
		/// <para>The pawn must be a natural male to be changed.  If the pawn is
		/// already a trap, it will return `true`.  Otherwise, it will return `false`
		/// if it failed to change the pawn.</para>
		/// <para>Note since Feb-2023: this is currently bugged, since the
		/// `SexPartAdder.add_breasts` does not respect the request to use female
		/// breasts.</para>
		/// </summary>
		/// <param name="pawn">The pawn to change.</param>
		/// <returns>Whether the pawn was modified.</returns>
		public static bool MakeIntoTrap(Pawn pawn)
		{
			if (GenderHelper.GetSex(pawn) is var sex and not Sex.male)
				return sex is Sex.trap;
			
			var parts = pawn.GetBreastList();
			foreach (var part in parts)
				pawn.health.RemoveHediff(part);
			SexPartAdder.add_breasts(pawn, gender: Gender.Female);
			return GenderHelper.GetSex(pawn) is Sex.trap;
		}

		/// <summary>
		/// <para>Adds a part to a pawn to make them into a futa.</para>
		/// <para>The pawn must be a natural male or female and will return `false`
		/// if it failed to change the pawn into a futa.</para>
		/// </summary>
		/// <param name="pawn">The pawn to change.</param>
		/// <param name="infertile">Whether the part should be infertile.</param>
		/// <returns>Whether the pawn was modified.</returns>
		public static bool MakeIntoFuta(Pawn pawn, bool infertile = false)
		{
			Hediff hediff;
			switch ((GenderHelper.GetSex(pawn), infertile))
			{
				case (Sex.male or Sex.trap, false):
					SexPartAdder.add_genitals(pawn, gender: Gender.Female);
					return GenderHelper.GetSex(pawn) is Sex.futa;
				case (Sex.male or Sex.trap, true) when get_genitalsBPR(pawn) is { } bpr:
					hediff = SexPartAdder.MakePart(hydraulic_vagina, pawn, bpr);
					pawn.health.AddHediff(hediff, bpr);
					return GenderHelper.GetSex(pawn) is Sex.futa;
				case (Sex.female, false):
					SexPartAdder.add_genitals(pawn, gender: Gender.Male);
					return GenderHelper.GetSex(pawn) is Sex.futa;
				case (Sex.female, true) when get_genitalsBPR(pawn) is { } bpr:
					hediff = SexPartAdder.MakePart(hydraulic_penis, pawn, bpr);
					pawn.health.AddHediff(hediff, bpr);
					return GenderHelper.GetSex(pawn) is Sex.futa;
				default:
					return false;
			}
		}
	}
}