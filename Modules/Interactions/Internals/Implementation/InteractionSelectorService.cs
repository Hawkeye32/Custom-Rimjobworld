using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Defs;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Extensions;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Rules.InteractionRules;
using rjw.Modules.Interactions.Rules.InteractionRules.Implementation;
using rjw.Modules.Shared;
using rjw.Modules.Shared.Extensions;
using rjw.Modules.Shared.Helpers;
using rjw.Modules.Shared.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class InteractionSelectorService : IInteractionSelectorService
	{
		private static ILog _log = LogManager.GetLogger<InteractionSelectorService, InteractionLogProvider>();

		public static IInteractionSelectorService Instance { get; private set; }

		static InteractionSelectorService()
		{
			Instance = new InteractionSelectorService();

			_interactionCompatibilityService = InteractionRequirementService.Instance;
			_interactionScoringService = InteractionScoringService.Instance;

			_interactionRules = new List<IInteractionRule>()
			{
				new MasturbationInteractionRule(),
				new AnimalInteractionRule(),
				new BestialityInteractionRule(),
				new ConsensualInteractionRule(),
				new RapeInteractionRule(),
				new WhoringInteractionRule(),
				new NecrophiliaInteractionRule(),
				new MechImplantInteractionRule()
			};
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private InteractionSelectorService() { }

		private static readonly IInteractionRequirementService _interactionCompatibilityService;
		private static readonly IInteractionScoringService _interactionScoringService;
		private static readonly IList<IInteractionRule> _interactionRules;

		public InteractionWithExtension Select(InteractionContext context)
		{
			IInteractionRule rule = FindRule(context.Internals.InteractionType);

			_log.Debug($"[available] {rule.Interactions.Select(e => $"[{e.Interaction.defName}]").Aggregate(String.Empty, (e, f) => $"{e}-{f}")}");

			IEnumerable<InteractionWithExtension> interactions = rule.Interactions
				.FilterReverse(context.Internals.IsReverse)
				// Filter the interaction by removing those where the requirements are not met
				.Where(e => _interactionCompatibilityService.FufillRequirements(e, context.Internals.Dominant, context.Internals.Submissive));

			_log.Debug($"[available] {rule.Interactions.Select(e => $"[{e.Interaction.defName}]").Aggregate(String.Empty, (e, f) => $"{e}-{f}")}");

			//Now we score each remaining interactions
			IList<Weighted<InteractionWithExtension>> scored = interactions
				.Select(e => new Weighted<InteractionWithExtension>(Score(context, e, rule), e))
				.ToList();

			_log.Debug($"[Scores] {scored.Select(e => $"[{e.Element.Interaction.defName}-{e.Weight}]").Aggregate(String.Empty, (e, f) => $"{e}-{f}")}");

			InteractionWithExtension result = RandomHelper.WeightedRandom(scored);

			if (result == null)
			{
				result = rule.Default;
				_log.Warning($"No eligible interaction found for Type {context.Internals.InteractionType}, IsReverse {context.Internals.IsReverse}, Initiator {context.Inputs.Initiator.GetName()}, Partner {context.Inputs.Partner.GetName()}. Using default {result?.Interaction.defName}.");
			}

			return result;
		}

		private float Score(InteractionContext context, InteractionWithExtension interaction, IInteractionRule rule)
		{
			return _interactionScoringService.Score(interaction, context.Internals.Dominant, context.Internals.Submissive)
				.GetScore(rule.SubmissivePreferenceWeight);
		}

		private IInteractionRule FindRule(InteractionType interactionType)
		{
			return _interactionRules
				.Where(e => e.InteractionType == interactionType)
				.FirstOrDefault();
		}
	}
}
