using rjw.Modules.Interactions.Defs.DefFragment;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Extensions;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Objects.Parts;
using rjw.Modules.Shared;
using rjw.Modules.Shared.Enums;
using rjw.Modules.Shared.Extensions;
using rjw.Modules.Shared.Implementation;
using rjw.Modules.Shared.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class InteractionRequirementService : IInteractionRequirementService
	{
		private static ILog _log = LogManager.GetLogger<InteractionRequirementService, InteractionLogProvider>();

		public static IInteractionRequirementService Instance { get; private set; }

		public static IList<ICustomRequirementHandler> CustomRequirementHandlers { get; private set; }

		static InteractionRequirementService()
		{
			Instance = new InteractionRequirementService();

			_partFinderService = PartFinderService.Instance;
			_pawnStateService = PawnStateService.Instance;

			CustomRequirementHandlers = new List<ICustomRequirementHandler>();
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private InteractionRequirementService() { }

		private static readonly IPartFinderService _partFinderService;
		private static readonly IPawnStateService _pawnStateService;

		/// <summary>
		/// Check if the pawns don't have the lewdable parts required
		/// to partake in this interaction
		/// </summary>
		public bool FufillRequirements(InteractionWithExtension interaction, InteractionPawn dominant, InteractionPawn submissive)
		{
			_log.Debug($"{interaction.Interaction.defName} checks");

			if (String.IsNullOrWhiteSpace(interaction.SelectorExtension.customRequirementHandler) == false)
			{
				if (TryCustomHandler(interaction, dominant, submissive, out bool result) == false)
				{
					_log.Debug($"{interaction.Interaction.defName} TryCustomHandler fail");
					return false;
				}
			}

			if (CheckRequirement(dominant, interaction.SelectorExtension.dominantRequirement) == false)
			{
				return false;
			}

			_log.Debug($"{submissive.Pawn.GetName()} checks");
			if (CheckRequirement(submissive, interaction.SelectorExtension.submissiveRequirement) == false)
			{
				return false;
			}

			return true;
		}
		private bool CheckRequirement(InteractionPawn pawn, InteractionRequirement requirement)
		{
			int required = 0;
			List<ILewdablePart> availableParts = new List<ILewdablePart>();

			_log.Debug($"{pawn.Pawn.GetName()} checks");

			//pawn state should match
			if (IsPawnstateValid(pawn, requirement) == false)
			{
				return false;
			}

			//need hand
			if (requirement.hand == true)
			{
				AppendByKind(availableParts, _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Hand), LewdablePartKind.Hand);
				required++;
			}
			//need foot
			if (requirement.foot == true)
			{
				AppendByKind(availableParts, _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Foot), LewdablePartKind.Foot);
				required++;
			}

			//need mouth
			if (requirement.mouth == true || requirement.mouthORbeak == true)
			{
				AppendByKind(availableParts, _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Mouth), LewdablePartKind.Mouth);
				required++;
			}
			//need beak
			if (requirement.beak == true || requirement.mouthORbeak == true)
			{
				AppendByKind(availableParts, _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Beak), LewdablePartKind.Beak);
				required++;
			}
			//need tongue
			if (requirement.tongue == true)
			{
				AppendByKind(availableParts, _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Tongue), LewdablePartKind.Tongue);
				required++;
			}
			//any oral thingy
			if (requirement.oral == true)
			{
				AppendByKind(availableParts, _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Tongue), LewdablePartKind.Tongue);
				AppendByKind(availableParts, _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Beak), LewdablePartKind.Beak);
				AppendByKind(availableParts, _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Mouth), LewdablePartKind.Mouth);
				required++;
			}

			//need tail
			if (requirement.tail == true)
			{
				AppendByKind(availableParts, _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Tail), LewdablePartKind.Tail);
				required++;
			}

			//need family
			if (requirement.families != null && requirement.families.Any())
			{
				foreach (GenitalFamily family in requirement.families)
				{
					AppendByFamily(availableParts, _partFinderService.FindUnblockedForPawn(pawn, family, requirement.partProps), family);
				}
				required++;
			}
			//need tag
			if (requirement.tags != null && requirement.tags.Any())
			{
				foreach (GenitalTag tag in requirement.tags)
				{
					AppendByTag(availableParts, _partFinderService.FindUnblockedForPawn(pawn, tag, requirement.partProps), tag);
				}
				required++;
			}

			//_log.Debug($"Requirement for {pawn.Pawn.GetName()} Min {requirement.minimumCount} Got {matches}");

			//The interaction have NO requirements
			if (required == 0)
			{
				return true;
			}

			int matches = availableParts
				.FilterSeverity(requirement.minimumSeverity)
				.Count();

			//Now ... all that's left is to check we have enough !
			if (requirement.minimumCount.HasValue)
			{
				return matches >= requirement.minimumCount.Value;
			}

			return matches >= 1;
		}

		private static void AppendByFamily(List<ILewdablePart> parts, IEnumerable<ILewdablePart> toAppend, GenitalFamily familly)
		{
			var array = toAppend.ToArray();

			if (array.Length > 0)
			{
				parts.AddRange(array);
			}
			else
			{
				_log.Debug($"Missing requirement : {familly}");
			}
		}

		private static void AppendByKind(List<ILewdablePart> parts, IEnumerable<ILewdablePart> toAppend, LewdablePartKind kind)
		{
			var array = toAppend.ToArray();

			if (array.Length > 0)
			{
				parts.AddRange(array);
			}
			else
			{
				_log.Debug($"Missing requirement : {kind}");
			}
		}

		private static void AppendByTag(List<ILewdablePart> parts, IEnumerable<ILewdablePart> toAppend, GenitalTag tag)
		{
			var array = toAppend.ToArray();

			if (array.Length > 0)
			{
				parts.AddRange(array);
			}
			else
			{
				_log.Debug($"Missing requirement : {tag}");
			}
		}

		private bool IsPawnstateValid(InteractionPawn pawn, InteractionRequirement requirement)
		{
			PawnState state = _pawnStateService.Detect(pawn.Pawn);

			//By default, the pawn must be healthy
			if (requirement.pawnStates == null || requirement.pawnStates.Any() == false)
			{
				return state == PawnState.Healthy;
			}

			return requirement.pawnStates.Contains(state);
		}

		private bool TryCustomHandler(InteractionWithExtension interaction, InteractionPawn dominant, InteractionPawn submissive, out bool result)
		{
			ICustomRequirementHandler handler = CustomRequirementHandlers
				.Where(e => e.HandlerKey == interaction.SelectorExtension.customRequirementHandler)
				.FirstOrDefault();

			if (handler == null)
			{
				result = false;
				return false;
			}

			try
			{
				result = handler.FufillRequirements(interaction, dominant, submissive);
			}
			catch(Exception e)
			{
				_log.Error($"Exception occured during call to custom handler {handler.GetType().FullName}. Will use regular requirement as fallback.", e);
				result = false;
				return false;
			}

			return result;
		}
	}
}
