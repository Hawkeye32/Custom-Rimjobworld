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
	public class MechImplantInteractionRule : IInteractionRule
	{
		static MechImplantInteractionRule()
		{
			_random = new Random();
			_interactionRepository = InteractionRepository.Instance;
		}

		private readonly static Random _random;
		private readonly static IInteractionRepository _interactionRepository;

		public InteractionType InteractionType => InteractionType.Mechanoid;

		public IEnumerable<InteractionWithExtension> Interactions => _interactionRepository
			.ListForMechanoid();

		public float SubmissivePreferenceWeight
		{
			get
			{
				//+/- 20%
				float variant = -1 + (float)_random.NextDouble() * 2;

				return 1.0f + variant;
			}
		}

		public InteractionWithExtension Default => InteractionDefOf.DefaultMechImplantSex;
	}
}
