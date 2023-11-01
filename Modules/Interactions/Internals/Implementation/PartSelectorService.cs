using rjw.Modules.Interactions.Defs.DefFragment;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Objects.Parts;
using System.Collections.Generic;
using System.Linq;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class PartSelectorService : IPartSelectorService
	{
		public static IPartSelectorService Instance { get; private set; }

		static PartSelectorService()
		{
			Instance = new PartSelectorService();

			_partFinderService = PartFinderService.Instance;
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private PartSelectorService() { }

		private readonly static IPartFinderService _partFinderService;

		public IList<ILewdablePart> SelectPartsForPawn(InteractionPawn pawn, InteractionRequirement requirement)
		{
			int required = 1;

			if (requirement.minimumCount.HasValue)
			{
				required = requirement.minimumCount.Value;
			}

			return EligebleParts(pawn, requirement)
				.Where(e => pawn.PartPreferences.ContainsKey(e.PartKind))
				.Select(e => new { Part = e, Preference = pawn.PartPreferences[e.PartKind] })
				.OrderByDescending(e => e.Preference)
				.Take(required)
				.Select(e => e.Part)
				.ToList();
		}

		private IEnumerable<ILewdablePart> EligebleParts(InteractionPawn pawn, InteractionRequirement requirement)
		{
			if (requirement.hand == true)
			{
				foreach (var part in _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Hand))
				{
					yield return part;
				}
			}
			if (requirement.foot == true)
			{
				foreach (var part in _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Foot))
				{
					yield return part;
				}
			}
			if (requirement.mouth == true || requirement.mouthORbeak == true)
			{
				foreach (var part in _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Mouth))
				{
					yield return part;
				}
			}
			if (requirement.beak == true || requirement.mouthORbeak == true)
			{
				foreach (var part in _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Beak))
				{
					yield return part;
				}
			}
			if (requirement.tongue == true)
			{
				foreach (var part in _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Tongue))
				{
					yield return part;
				}
			}
			if (requirement.tail == true)
			{
				foreach (var part in _partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Tail))
				{
					yield return part;
				}
			}
			
			if (requirement.families != null && requirement.families.Any())
			{
				foreach (GenitalFamily family in requirement.families)
				{
					foreach (var part in _partFinderService.FindUnblockedForPawn(pawn, family))
					{
						yield return part;
					}
				}
			}
			if (requirement.tags != null && requirement.tags.Any())
			{
				foreach (GenitalTag tag in requirement.tags)
				{
					foreach (var part in _partFinderService.FindUnblockedForPawn(pawn, tag))
					{
						yield return part;
					}
				}
			}
		}
	}
}
