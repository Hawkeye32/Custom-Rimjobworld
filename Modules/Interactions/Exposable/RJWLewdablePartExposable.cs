using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects.Parts;
using rjw.Modules.Shared.Logs;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace rjw.Modules.Interactions.Exposable
{
	public class RJWLewdablePartExposable : IExposable
	{
		private static ILog _log = LogManager.GetLogger<RJWLewdablePartExposable>();

		public HediffWithExtensionExposable hediff;

		public LewdablePartKind partKind;

		public float Size => hediff.hediff.Severity;

		public IList<string> Props => hediff.PartProps?.props ?? new();

		public void ExposeData()
		{
			Scribe_Deep.Look(ref hediff, nameof(hediff));
			Scribe_Values.Look(ref partKind, nameof(partKind));
		}

		public static RJWLewdablePart Convert(RJWLewdablePartExposable toCast)
		{
			return new RJWLewdablePart(
				HediffWithExtensionExposable.Convert(toCast.hediff), 
				toCast.partKind
				);
		}
		public static RJWLewdablePartExposable Convert(RJWLewdablePart toCast)
		{
			return new RJWLewdablePartExposable()
			{
				hediff = HediffWithExtensionExposable.Convert(toCast.Hediff),
				partKind = toCast.PartKind
			};
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"{nameof(partKind)} = {partKind}");

			return stringBuilder.ToString();
		}
	}
}
