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
	public class WhoringInteractionRule : IInteractionRule
	{
		static WhoringInteractionRule()
		{
			_random = new Random();
			_interactionRepository = InteractionRepository.Instance;
		}

		private readonly static Random _random;
		private readonly static IInteractionRepository _interactionRepository;

		public InteractionType InteractionType => InteractionType.Whoring;

		public IEnumerable<InteractionWithExtension> Interactions => _interactionRepository
			.ListForWhoring();

		public float SubmissivePreferenceWeight
		{
			get
			{
				return (float)_random.NextDouble(); // Full random !
			}
		}
		public InteractionWithExtension Default => InteractionDefOf.DefaultWhoringSex;
	}
}
