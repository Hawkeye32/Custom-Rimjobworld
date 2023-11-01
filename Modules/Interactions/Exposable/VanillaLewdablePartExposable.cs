using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects.Parts;
using rjw.Modules.Shared.Logs;
using System.Text;
using Verse;

namespace rjw.Modules.Interactions.Exposable
{
	public class VanillaLewdablePartExposable : IExposable
	{
		private static ILog _log = LogManager.GetLogger<VanillaLewdablePartExposable>();

		public Pawn pawn;

		public BodyPartRecord part;

		public LewdablePartKind partKind;

		public void ExposeData()
		{
			Scribe_References.Look(ref pawn, nameof(pawn));
			Scribe_BodyParts.Look(ref part, nameof(part));
			Scribe_Values.Look(ref partKind, nameof(partKind));
		}

		public static VanillaLewdablePart Convert(VanillaLewdablePartExposable toCast)
		{
			return new VanillaLewdablePart(
					toCast.pawn,
					toCast.part,
					toCast.partKind
				);
		}
		public static VanillaLewdablePartExposable Convert(VanillaLewdablePart toCast)
		{
			return new VanillaLewdablePartExposable()
			{
				pawn = toCast.Owner,
				part = toCast.Part,
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
