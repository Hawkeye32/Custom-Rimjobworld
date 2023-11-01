using rjw.Modules.Interactions.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class InteractionBuilderService : IInteractionBuilderService
	{
		public static IInteractionBuilderService Instance { get; private set; }

		static InteractionBuilderService()
		{
			Instance = new InteractionBuilderService();

			_rulePackService = RulePackService.Instance;
			_partSelectorService = PartSelectorService.Instance;
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private InteractionBuilderService() { }

		private readonly static IRulePackService _rulePackService;
		private readonly static IPartSelectorService _partSelectorService;

		public void Build(InteractionContext context)
		{
			context.Outputs.Generated.Dominant = context.Internals.Dominant;
			context.Outputs.Generated.Submissive = context.Internals.Submissive;

			//Participants
			if (context.Internals.IsReverse)
			{
				context.Outputs.Generated.Initiator = context.Internals.Submissive;
				context.Outputs.Generated.Receiver = context.Internals.Dominant;

			}
			else
			{
				context.Outputs.Generated.Initiator = context.Internals.Dominant;
				context.Outputs.Generated.Receiver = context.Internals.Submissive;
			}

			context.Outputs.Generated.InteractionDef = context.Internals.Selected;
			context.Outputs.Generated.RjwSexType = ParseHelper.FromString<xxx.rjwSextype>(context.Internals.Selected.Extension.rjwSextype);

			context.Outputs.Generated.RulePack = _rulePackService.FindInteractionRulePack(context.Internals.Selected);

			context.Outputs.Generated.SelectedDominantParts = _partSelectorService.SelectPartsForPawn(
				context.Internals.Dominant, 
				context.Internals.Selected.SelectorExtension.dominantRequirement
				);
			context.Outputs.Generated.SelectedSubmissiveParts = _partSelectorService.SelectPartsForPawn(
				context.Internals.Submissive,
				context.Internals.Selected.SelectorExtension.submissiveRequirement
				);
		}
	}
}
