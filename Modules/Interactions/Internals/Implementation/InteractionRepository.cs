using rjw.Modules.Interactions.Defs;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class InteractionRepository : IInteractionRepository
	{
		public static IInteractionRepository Instance { get; private set; }

		static InteractionRepository()
		{
			Instance = new InteractionRepository();
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private InteractionRepository() { }

		public IEnumerable<InteractionWithExtension> List()
		{
			return InteractionDefOf.Interactions;
		}
		
		public IEnumerable<InteractionWithExtension> ListForMasturbation()
		{
			return List()
				.Where(interaction => interaction.HasInteractionTag(InteractionTag.Masturbation));
		}

		public IEnumerable<InteractionWithExtension> ListForAnimal()
		{
			return List()
				.Where(interaction => interaction.HasInteractionTag(InteractionTag.Animal));
		}

		public IEnumerable<InteractionWithExtension> ListForBestiality()
		{
			return List()
				.Where(interaction => interaction.HasInteractionTag(InteractionTag.Bestiality));
		}

		public IEnumerable<InteractionWithExtension> ListForConsensual()
		{
			return List()
				.Where(interaction => interaction.HasInteractionTag(InteractionTag.Consensual));
		}

		public IEnumerable<InteractionWithExtension> ListForRape()
		{
			return List()
				.Where(interaction => interaction.HasInteractionTag(InteractionTag.Rape));
		}

		public IEnumerable<InteractionWithExtension> ListForWhoring()
		{
			return List()
				.Where(interaction => interaction.HasInteractionTag(InteractionTag.Whoring));
		}

		public IEnumerable<InteractionWithExtension> ListForNecrophilia()
		{
			return List()
				.Where(interaction => interaction.HasInteractionTag(InteractionTag.Necrophilia));
		}

		public IEnumerable<InteractionWithExtension> ListForMechanoid()
		{
			return List()
				.Where(interaction => interaction.HasInteractionTag(InteractionTag.MechImplant));
		}

		public IEnumerable<InteractionWithExtension> List(InteractionType interactionType)
		{
			switch (interactionType)
			{
				case InteractionType.Whoring:
					return ListForWhoring();
				case InteractionType.Rape:
					return ListForRape();
				case InteractionType.Bestiality:
					return ListForBestiality();
				case InteractionType.Animal:
					return ListForAnimal();
				case InteractionType.Necrophilia:
					return ListForNecrophilia();
				case InteractionType.Mechanoid:
					return ListForMechanoid();
				case InteractionType.Consensual:
				default:
					return ListForConsensual();
			}
		}
	}
}
