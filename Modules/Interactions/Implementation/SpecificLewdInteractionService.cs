using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Extensions;
using rjw.Modules.Interactions.Helpers;
using rjw.Modules.Interactions.Internals;
using rjw.Modules.Interactions.Internals.Implementation;
using rjw.Modules.Shared.Extensions;
using rjw.Modules.Shared.Logs;
using System;
using System.Linq;

namespace rjw.Modules.Interactions.Implementation
{
	public class SpecificLewdInteractionService : ISpecificLewdInteractionService
	{
		private static ILog _log = LogManager.GetLogger<SpecificLewdInteractionService, InteractionLogProvider>();

		public static ISpecificLewdInteractionService Instance { get; private set; }

		static SpecificLewdInteractionService()
		{
			Instance = new SpecificLewdInteractionService();

			_interactionTypeDetectorService = InteractionTypeDetectorService.Instance;
			_interactionBuilderService = InteractionBuilderService.Instance;

			_blockedPartDetectorService = BlockedPartDetectorService.Instance;
			_partPreferenceDetectorService = PartPreferenceDetectorService.Instance;
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private SpecificLewdInteractionService() { }

		private readonly static IInteractionTypeDetectorService _interactionTypeDetectorService;
		private readonly static IInteractionBuilderService _interactionBuilderService;

		private readonly static IBlockedPartDetectorService _blockedPartDetectorService;
		private readonly static IPartPreferenceDetectorService _partPreferenceDetectorService;

		public InteractionOutputs GenerateSpecificInteraction(SpecificInteractionInputs inputs)
		{
			///TODO : remove the logs once it works
			InteractionContext context = new InteractionContext();

			_log.Debug($"Generating Specific Interaction {inputs.Interaction.defName} for {context.Inputs.Initiator?.GetName()} and {context.Inputs.Partner?.GetName()}");

			Initialize(context, inputs);

			//Detect parts that are unusable / missing
			_blockedPartDetectorService.DetectBlockedParts(context.Internals);

			_log.Debug($"Blocked parts for dominant {context.Internals.Dominant.BlockedParts.Select(e => $"[{e}]").Aggregate(String.Empty, (e, f) => $"{e}-{f}")}");
			_log.Debug($"Blocked parts for submissive {context.Internals.Submissive.BlockedParts.Select(e => $"[{e}]").Aggregate(String.Empty, (e, f) => $"{e}-{f}")}");

			//Calculate parts usage preferences for Dominant and Submissive
			_partPreferenceDetectorService.DetectPartPreferences(context);

			_log.Debug($"Part preferences for dominant {context.Internals.Dominant.PartPreferences.Select(e => $"[{e.Key}-{e.Value}]").Aggregate(String.Empty, (e, f) => $"{e}-{f}")}");
			_log.Debug($"Part preferences for submissive {context.Internals.Submissive.PartPreferences.Select(e => $"[{e.Key}-{e.Value}]").Aggregate(String.Empty, (e, f) => $"{e}-{f}")}");

			_interactionBuilderService.Build(context);

			_log.Debug($"SelectedParts for dominant {context.Outputs.Generated.SelectedDominantParts.Select(e => $"[{e.PartKind}]").Aggregate(String.Empty, (e, f) => $"{e}-{f}")}");
			_log.Debug($"SelectedParts for submissive {context.Outputs.Generated.SelectedSubmissiveParts.Select(e => $"[{e.PartKind}]").Aggregate(String.Empty, (e, f) => $"{e}-{f}")}");

			return context.Outputs;
		}

		private void Initialize(InteractionContext context, SpecificInteractionInputs inputs)
		{
			context.Internals.Selected = InteractionHelper.GetWithExtension(inputs.Interaction);
			context.Outputs.Generated = new Objects.Interaction()
			{
				InteractionDef = context.Internals.Selected
			};

			context.Inputs = new InteractionInputs()
			{
				Initiator = inputs.Initiator,
				Partner = inputs.Partner,
				IsRape = context.Outputs.Generated.InteractionDef.HasInteractionTag(Enums.InteractionTag.Rape),
				IsWhoring = context.Outputs.Generated.InteractionDef.HasInteractionTag(Enums.InteractionTag.Whoring)
			};

			context.Internals.InteractionType =
			context.Outputs.Generated.InteractionType =
				_interactionTypeDetectorService.DetectInteractionType(
					context
					);

			context.Internals.Dominant = new Objects.InteractionPawn
			{
				Pawn = context.Inputs.Initiator,
				Parts = context.Inputs.Initiator.GetSexablePawnParts()
			};
			context.Internals.Submissive = new Objects.InteractionPawn
			{
				Pawn = context.Inputs.Partner,
				Parts = context.Inputs.Partner.GetSexablePawnParts()
			};

			context.Internals.Dominant.Gender = Genitals.Helpers.GenderHelper.GetGender(context.Internals.Dominant);
			context.Internals.Submissive.Gender = Genitals.Helpers.GenderHelper.GetGender(context.Internals.Submissive);
		}
	}
}
