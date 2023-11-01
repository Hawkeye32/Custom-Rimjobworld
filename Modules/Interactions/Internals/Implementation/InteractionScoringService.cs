using rjw.Modules.Interactions.Defs.DefFragment;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Objects.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class InteractionScoringService : IInteractionScoringService
	{
		public static IInteractionScoringService Instance { get; private set; }

		static InteractionScoringService()
		{
			Instance = new InteractionScoringService();

			_partFinderService = PartFinderService.Instance;
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private InteractionScoringService() { }

		private static readonly IPartFinderService _partFinderService;

		public InteractionScore Score(InteractionWithExtension interaction, InteractionPawn dominant, InteractionPawn submissive)
		{
			return new InteractionScore() 
			{
				Dominant = Score(interaction.SelectorExtension.dominantRequirement, dominant),
				Submissive = Score(interaction.SelectorExtension.submissiveRequirement, submissive),
				Setting = SettingScore(interaction)
			};
		}

		private float Score(InteractionRequirement requirement, InteractionPawn pawn)
		{
			int allowed = 1;

			IList<ILewdablePart> availableParts;
			IEnumerable<(ILewdablePart Part, float Score)> scoredParts;

			//Find the parts !
			availableParts = GetAvailablePartsForInteraction(requirement, pawn);

			//Score the parts !
			scoredParts = availableParts
				.Select(e => (e, pawn.PartPreferences[e.PartKind]));

			//Order the parts !
			scoredParts = scoredParts
				.OrderByDescending(e => e.Score);

			if (requirement.minimumCount.HasValue == true && requirement.minimumCount.Value > 0)
			{
				allowed = requirement.minimumCount.Value;
			}

			return scoredParts
				//Take the allowed part count
				.Take(allowed)
				.Select(e => e.Score)
				//Multiply the parts scores
				.Aggregate(1f, (e, f) => e * f);
		}

		private IList<ILewdablePart> GetAvailablePartsForInteraction(InteractionRequirement requirement, InteractionPawn pawn)
		{
			List<ILewdablePart> result = new List<ILewdablePart>();

			//need hand
			if (requirement.hand == true)
			{
				result.AddRange(_partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Hand));
			}
			//need foot
			if (requirement.foot == true)
			{
				result.AddRange(_partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Foot));
			}
			//need mouth
			if (requirement.mouth == true || requirement.mouthORbeak == true)
			{
				result.AddRange(_partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Mouth));
			}
			//need beak
			if (requirement.beak == true || requirement.mouthORbeak == true)
			{
				result.AddRange(_partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Beak));
			}
			//need tongue
			if (requirement.tongue == true)
			{
				result.AddRange(_partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Tongue));
			}
			//need tail
			if (requirement.tail == true)
			{
				result.AddRange(_partFinderService.FindUnblockedForPawn(pawn, LewdablePartKind.Tail));
			}

			//need family
			if (requirement.families != null && requirement.families.Any())
			{
				foreach (GenitalFamily family in requirement.families)
				{
					result.AddRange(_partFinderService.FindUnblockedForPawn(pawn, family));
				}
			}
			//need tag
			if (requirement.tags != null && requirement.tags.Any())
			{
				foreach (GenitalTag tag in requirement.tags)
				{
					result.AddRange(_partFinderService.FindUnblockedForPawn(pawn, tag));
				}
			}

			return result;
		}

		private float SettingScore(InteractionWithExtension interaction)
		{
			xxx.rjwSextype type = ParseHelper.FromString<xxx.rjwSextype>(interaction.Extension.rjwSextype);

			switch (type)
			{
				case xxx.rjwSextype.Vaginal:
					return RJWPreferenceSettings.vaginal;
				case xxx.rjwSextype.Anal:
					return RJWPreferenceSettings.anal;
				case xxx.rjwSextype.DoublePenetration:
					return RJWPreferenceSettings.double_penetration;
				case xxx.rjwSextype.Boobjob:
					return RJWPreferenceSettings.breastjob;
				case xxx.rjwSextype.Handjob:
					return RJWPreferenceSettings.handjob;
				case xxx.rjwSextype.Footjob:
					return RJWPreferenceSettings.footjob;
				case xxx.rjwSextype.Fingering:
					return RJWPreferenceSettings.fingering;
				case xxx.rjwSextype.Scissoring:
					return RJWPreferenceSettings.scissoring;
				case xxx.rjwSextype.MutualMasturbation:
					return RJWPreferenceSettings.mutual_masturbation;
				case xxx.rjwSextype.Fisting:
					return RJWPreferenceSettings.fisting;
				case xxx.rjwSextype.Rimming:
					return RJWPreferenceSettings.rimming;
				case xxx.rjwSextype.Fellatio:
					return RJWPreferenceSettings.fellatio;
				case xxx.rjwSextype.Cunnilingus:
					return RJWPreferenceSettings.cunnilingus;
				case xxx.rjwSextype.Sixtynine:
					return RJWPreferenceSettings.sixtynine;
				default:
					return 1;
			}
		}
	}
}
