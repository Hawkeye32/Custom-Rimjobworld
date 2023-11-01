using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Rules.PartKindUsageRules;
using rjw.Modules.Interactions.Rules.PartKindUsageRules.Implementation;
using rjw.Modules.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class PartPreferenceDetectorService : IPartPreferenceDetectorService
	{
		public static IPartPreferenceDetectorService Instance { get; private set; }

		static PartPreferenceDetectorService()
		{
			Instance = new PartPreferenceDetectorService();

			_partKindUsageRules = new List<IPartPreferenceRule>()
			{
				new AnimalPartKindUsageRule(),
				new BestialityForZoophilePartKindUsageRule(),
				new BestialityPartKindUsageRule(),
				new BigBreastsPartKindUsageRule(),
				new MainPartKindUsageRule(),
				new PawnAlreadySatisfiedPartKindUsageRule(),
				new QuirksPartKindUsageRule(),
				new RapePartKindUsageRule(),
				new SizeDifferencePartKindUsageRule(),
				new WhoringPartKindUsageRule(),
				new PregnancyApproachPartKindUsageRule(),
			};
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private PartPreferenceDetectorService() { }

		private readonly static IList<IPartPreferenceRule> _partKindUsageRules;

		public void DetectPartPreferences(InteractionContext context)
		{
			context.Internals.Dominant.PartPreferences = PartPreferencesForDominant(context);
			context.Internals.Submissive.PartPreferences = BlockedPartsForSubmissive(context);
		}

		private IDictionary<LewdablePartKind, float> PartPreferencesForDominant(InteractionContext context)
		{
			IList<Weighted<LewdablePartKind>> preferences = _partKindUsageRules
					.SelectMany(e => e.ModifiersForDominant(context))
					.ToList();

			return MergeWithDefaultDictionary(context.Internals.Dominant, preferences);
		}

		private IDictionary<LewdablePartKind, float> BlockedPartsForSubmissive(InteractionContext context)
		{
			IList<Weighted<LewdablePartKind>> preferences  = _partKindUsageRules
					.SelectMany(e => e.ModifiersForSubmissive(context))
					.ToList();

			return MergeWithDefaultDictionary(context.Internals.Submissive, preferences);
		}

		private IDictionary<LewdablePartKind, float> MergeWithDefaultDictionary(InteractionPawn pawn, IList<Weighted<LewdablePartKind>> preferences)
		{
			//A dictionary containing every part kind with a value of 1.0f (aka defaults)
			//it'll prevent exceptions to  be thrown if you did preferences[missingKey]
			IDictionary<LewdablePartKind, float> result = Enum.GetValues(typeof(LewdablePartKind))
				.OfType<LewdablePartKind>()
				//We don't want that ...
				.Where(e => e != LewdablePartKind.Unsupported)
				//We don't want these either
				.Where(e => pawn.BlockedParts.Contains(e) == false)
				.ToDictionary(e => e, e => 1.0f);

			//Put the values in the dictionary
			foreach (KeyValuePair<LewdablePartKind, float> element in result.ToList())
			{
				if (preferences.Where(e => e.Element == element.Key).Any())
				{
					result[element.Key] *= preferences
						.Where(e => e.Element == element.Key)
						.Select(e => e.Weight)
						.Aggregate((e, f) => e * f);
				}
			}

			return result;
		}
	}
}
