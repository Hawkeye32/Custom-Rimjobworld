using rjw.Modules.Interactions.Defs;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Internals;
using rjw.Modules.Interactions.Internals.Implementation;
using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Rules.InteractionRules.Implementation
{
	public class MasturbationInteractionRule : IInteractionRule
	{
		static MasturbationInteractionRule()
		{
			_interactionRepository = InteractionRepository.Instance;
		}

		private readonly static IInteractionRepository _interactionRepository;

		public InteractionType InteractionType => InteractionType.Masturbation;

		public IEnumerable<InteractionWithExtension> Interactions => _interactionRepository
			.ListForMasturbation();

		public float SubmissivePreferenceWeight
		{
			get
			{
				return 0.0f;
			}
		}

		public InteractionWithExtension Default => InteractionDefOf.DefaultAnimalSex;
	}
}
