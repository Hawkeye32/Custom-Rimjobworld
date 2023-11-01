using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Objects.Parts;
using rjw.Modules.Shared.Extensions;
using rjw.Modules.Shared.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace rjw.Modules.Interactions.Exposable
{
	public class InteractionExposable : IExposable
	{
		private static ILog _log = LogManager.GetLogger<InteractionExposable>();

		private InteractionType interactionType;

		private bool isReverse;

		private InteractionPawnExposable initiator;
		private InteractionPawnExposable receiver;

		private InteractionWithExtensionExposable interactionWithExtension;

		private RulePackDef rulePack;

		private xxx.rjwSextype RjwSexType
		{
			get
			{
				return ParseHelper.FromString<xxx.rjwSextype>(interactionWithExtension.Extension.rjwSextype);
			}
		}

		private List<RJWLewdablePartExposable> selectedDominantParts_RJW;
		private List<VanillaLewdablePartExposable> selectedDominantParts_Vanilla;

		private List<RJWLewdablePartExposable> selectedSubmissiveParts_RJW;
		private List<VanillaLewdablePartExposable> selectedSubmissiveParts_Vanilla;

		public void ExposeData()
		{
			Scribe_Values.Look(ref interactionType, nameof(interactionType));

			Scribe_Values.Look(ref isReverse, nameof(isReverse));

			Scribe_Deep.Look(ref initiator, nameof(initiator));
			Scribe_Deep.Look(ref receiver, nameof(receiver));

			Scribe_Deep.Look(ref interactionWithExtension, nameof(interactionWithExtension));

			Scribe_Defs.Look(ref rulePack, nameof(rulePack));

			Scribe_Collections.Look(ref selectedDominantParts_RJW, nameof(selectedDominantParts_RJW));
			Scribe_Collections.Look(ref selectedDominantParts_Vanilla, nameof(selectedDominantParts_Vanilla));

			Scribe_Collections.Look(ref selectedSubmissiveParts_RJW, nameof(selectedSubmissiveParts_RJW));
			Scribe_Collections.Look(ref selectedSubmissiveParts_Vanilla, nameof(selectedSubmissiveParts_Vanilla));
		}

		public static Interaction Convert(InteractionExposable toCast)
		{
			return new Interaction()
			{
				InteractionType = toCast.interactionType,

				Initiator = InteractionPawnExposable.Convert(toCast.initiator),
				Receiver = InteractionPawnExposable.Convert(toCast.receiver),

				Dominant = InteractionPawnExposable.Convert(toCast.initiator),
				Submissive = InteractionPawnExposable.Convert(toCast.receiver),

				InteractionDef = InteractionWithExtensionExposable.Convert(toCast.interactionWithExtension),

				RjwSexType = toCast.RjwSexType,
				RulePack = toCast.rulePack,
				
				SelectedDominantParts = Enumerable.Concat(
					toCast.selectedDominantParts_RJW.Select(RJWLewdablePartExposable.Convert).OfType<ILewdablePart>(),
					toCast.selectedDominantParts_Vanilla.Select(VanillaLewdablePartExposable.Convert).OfType<ILewdablePart>()
					).ToList(),

				SelectedSubmissiveParts = Enumerable.Concat(
					toCast.selectedSubmissiveParts_RJW.Select(RJWLewdablePartExposable.Convert).OfType<ILewdablePart>(),
					toCast.selectedSubmissiveParts_Vanilla.Select(VanillaLewdablePartExposable.Convert).OfType<ILewdablePart>()
					).ToList()
			};
		}
		public static InteractionExposable Convert(Interaction toCast)
		{
			return new InteractionExposable()
			{
				interactionType = toCast.InteractionType,

				initiator = InteractionPawnExposable.Convert(toCast.Initiator),
				receiver = InteractionPawnExposable.Convert(toCast.Receiver),

				interactionWithExtension = InteractionWithExtensionExposable.Convert(toCast.InteractionDef),

				rulePack = toCast.RulePack,

				selectedDominantParts_RJW = toCast.SelectedDominantParts
					.OfType<RJWLewdablePart>()
					.Select(RJWLewdablePartExposable.Convert)
					.ToList(),
				selectedDominantParts_Vanilla = toCast.SelectedDominantParts
					.OfType<VanillaLewdablePart>()
					.Select(VanillaLewdablePartExposable.Convert)
					.ToList(),

				selectedSubmissiveParts_RJW = toCast.SelectedDominantParts
					.OfType<RJWLewdablePart>()
					.Select(RJWLewdablePartExposable.Convert)
					.ToList(),
				selectedSubmissiveParts_Vanilla = toCast.SelectedDominantParts
					.OfType<VanillaLewdablePart>()
					.Select(VanillaLewdablePartExposable.Convert)
					.ToList()
			};
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"{nameof(interactionType)} = {interactionType}");
			stringBuilder.AppendLine($"{nameof(initiator)} = {initiator?.pawn.GetName()}");
			stringBuilder.AppendLine($"{nameof(receiver)} = {receiver?.pawn.GetName()}");

			stringBuilder.AppendLine($"{nameof(interactionWithExtension)} = {interactionWithExtension?.interactionDef.defName}");

			stringBuilder.AppendLine($"{nameof(rulePack)} = {rulePack?.defName}");

			return stringBuilder.ToString();
		}
	}
}
