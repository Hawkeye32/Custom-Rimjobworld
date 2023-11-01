using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.DefModExtensions;
using rjw.Modules.Interactions.Extensions;
using rjw.Modules.Interactions.Internals;
using rjw.Modules.Interactions.Internals.Implementation;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared.Extensions;
using rjw.Modules.Shared.Logs;
using System;
using System.Linq;
using RimWorld;
using Verse;

namespace rjw.Modules.Interactions.Implementation
{
	public class LewdInteractionValidatorService : ILewdInteractionValidatorService
	{
		private static ILog _log = LogManager.GetLogger<LewdInteractionService, InteractionLogProvider>();

		public static ILewdInteractionValidatorService Instance { get; private set; }

		static LewdInteractionValidatorService()
		{
			Instance = new LewdInteractionValidatorService();

			_interactionRequirementService = InteractionRequirementService.Instance;
			_blockedPartDetectorService = BlockedPartDetectorService.Instance;
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private LewdInteractionValidatorService() { }

		private readonly static IInteractionRequirementService _interactionRequirementService;

		private readonly static IBlockedPartDetectorService _blockedPartDetectorService;

		public bool IsValid(InteractionDef interaction, Pawn dominant, Pawn submissive)
		{
			InteractionPawn iDominant, iSubmissive;
			InteractionWithExtension iInteraction;

			try
			{
				Assert(interaction);
			}
			catch(NotImplementedException)
			{
				return false;
			}

			iDominant = ConvertPawn(dominant);
			iSubmissive = ConvertPawn(submissive);
			iInteraction = ConvertInteraction(interaction);

			//Detect parts that are unusable / missing
			iDominant.BlockedParts = _blockedPartDetectorService.BlockedPartsForPawn(iDominant);
			iSubmissive.BlockedParts = _blockedPartDetectorService.BlockedPartsForPawn(iSubmissive);

			return _interactionRequirementService.FufillRequirements(iInteraction, iDominant, iSubmissive);
		}

		private void Assert(InteractionDef interaction)
		{
			if (interaction.HasModExtension<InteractionSelectorExtension>() == false)
			{
				string message = $"The interaction {interaction.defName} doesn't have the required {nameof(InteractionSelectorExtension)} extention";

				_log.Error(message);
				throw new NotImplementedException(message);
			}

			if (interaction.HasModExtension<InteractionExtension>() == false)
			{
				string message = $"The interaction {interaction.defName} doesn't have the required {nameof(InteractionExtension)} extention";

				_log.Error(message);
				throw new NotImplementedException(message);
			}
		}

		private InteractionWithExtension ConvertInteraction(InteractionDef interaction)
		{
			return new InteractionWithExtension
			{
				Interaction = interaction,
				Extension = interaction.GetModExtension<InteractionExtension>(),
				SelectorExtension = interaction.GetModExtension<InteractionSelectorExtension>()
			};
		}

		private InteractionPawn ConvertPawn(Pawn pawn)
		{
			return new InteractionPawn
			{
				Pawn = pawn,
				Parts = pawn.GetSexablePawnParts()
			};
		}
	}
}
