using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared.Extensions;
using rjw.Modules.Shared.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace rjw.Modules.Interactions.Exposable
{
	public class InteractionPawnExposable : IExposable
	{
		private static ILog _log = LogManager.GetLogger<InteractionPawnExposable>();

		public Pawn pawn;

		public List<BodyPartRecord> mouths;
		public List<BodyPartRecord> beaks;
		public List<BodyPartRecord> tongues;
		public List<BodyPartRecord> hands;
		public List<BodyPartRecord> tails;

		public List<HediffWithExtensionExposable> penises;
		public List<HediffWithExtensionExposable> vaginas;
		public List<HediffWithExtensionExposable> breasts;
		public List<HediffWithExtensionExposable> udders;
		public List<HediffWithExtensionExposable> anuses;

		public List<HediffWithExtensionExposable> femaleOvipositors;
		public List<HediffWithExtensionExposable> maleOvipositors;

		public List<LewdablePartKind> blockedParts;
		public Dictionary<LewdablePartKind, float> partPreferences;

		public void ExposeData()
		{
			Scribe_References.Look(ref pawn, nameof(pawn));

			Scribe_Collections.Look(ref beaks, nameof(beaks), LookMode.BodyPart);
			Scribe_Collections.Look(ref mouths, nameof(mouths), LookMode.BodyPart);
			Scribe_Collections.Look(ref tongues, nameof(tongues), LookMode.BodyPart);
			Scribe_Collections.Look(ref hands, nameof(hands), LookMode.BodyPart);
			Scribe_Collections.Look(ref tails, nameof(tails), LookMode.BodyPart);

			Scribe_Collections.Look(ref penises, nameof(penises), LookMode.Deep);
			Scribe_Collections.Look(ref vaginas, nameof(vaginas), LookMode.Deep);
			Scribe_Collections.Look(ref breasts, nameof(breasts), LookMode.Deep);
			Scribe_Collections.Look(ref udders, nameof(udders), LookMode.Deep);
			Scribe_Collections.Look(ref anuses, nameof(anuses), LookMode.Deep);

			Scribe_Collections.Look(ref femaleOvipositors, nameof(femaleOvipositors), LookMode.Deep);
			Scribe_Collections.Look(ref maleOvipositors, nameof(maleOvipositors), LookMode.Deep);

			Scribe_Collections.Look(ref blockedParts, nameof(blockedParts), LookMode.Value);
			Scribe_Collections.Look(ref partPreferences, nameof(partPreferences), LookMode.Value, LookMode.Value);
		}

		public static InteractionPawn Convert(InteractionPawnExposable toCast)
		{
			return new InteractionPawn()
			{
				Pawn = toCast.pawn,
				Parts = new SexablePawnParts()
				{
					Mouths = toCast.mouths,
					Beaks = toCast.beaks,
					Tongues = toCast.tongues,
					Hands = toCast.hands,
					Tails = toCast.tails,

					Penises = toCast.penises.Select(HediffWithExtensionExposable.Convert).ToList(),
					Vaginas = toCast.vaginas.Select(HediffWithExtensionExposable.Convert).ToList(),
					Breasts = toCast.breasts.Select(HediffWithExtensionExposable.Convert).ToList(),
					Udders = toCast.udders.Select(HediffWithExtensionExposable.Convert).ToList(),
					Anuses = toCast.anuses.Select(HediffWithExtensionExposable.Convert).ToList(),

					FemaleOvipositors = toCast.femaleOvipositors.Select(HediffWithExtensionExposable.Convert).ToList(),
					MaleOvipositors = toCast.maleOvipositors.Select(HediffWithExtensionExposable.Convert).ToList(),
				},
				BlockedParts = toCast.blockedParts,
				PartPreferences = toCast.partPreferences
			};
		}
		public static InteractionPawnExposable Convert(InteractionPawn toCast)
		{
			return new InteractionPawnExposable()
			{
				pawn = toCast.Pawn,

				mouths = toCast.Parts.Mouths.ToList(),
				beaks = toCast.Parts.Beaks.ToList(),
				tongues = toCast.Parts.Tongues.ToList(),
				hands = toCast.Parts.Hands.ToList(),
				tails = toCast.Parts.Tails.ToList(),

				penises = toCast.Parts.Penises.Select(HediffWithExtensionExposable.Convert).ToList(),
				vaginas = toCast.Parts.Vaginas.Select(HediffWithExtensionExposable.Convert).ToList(),
				breasts = toCast.Parts.Breasts.Select(HediffWithExtensionExposable.Convert).ToList(),
				udders = toCast.Parts.Udders.Select(HediffWithExtensionExposable.Convert).ToList(),
				anuses = toCast.Parts.Anuses.Select(HediffWithExtensionExposable.Convert).ToList(),

				femaleOvipositors = toCast.Parts.FemaleOvipositors.Select(HediffWithExtensionExposable.Convert).ToList(),
				maleOvipositors = toCast.Parts.MaleOvipositors.Select(HediffWithExtensionExposable.Convert).ToList(),

				blockedParts = toCast.BlockedParts.ToList(),
				partPreferences = toCast.PartPreferences.ToDictionary(e => e.Key, e => e.Value)
			};
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"{nameof(pawn)} = {pawn.GetName()}");

			stringBuilder.AppendLine($"{nameof(blockedParts)} = {String.Join("/", blockedParts)}");
			stringBuilder.AppendLine($"{nameof(partPreferences)} = {String.Join("/", partPreferences.Select(e => $"{e.Key}-{e.Value}"))}");

			return stringBuilder.ToString();
		}
	}
}
