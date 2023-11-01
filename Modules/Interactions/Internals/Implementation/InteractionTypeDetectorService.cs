using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared.Extensions;
using rjw.Modules.Shared.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class InteractionTypeDetectorService : IInteractionTypeDetectorService
	{
		private static ILog _log = LogManager.GetLogger<InteractionTypeDetectorService, InteractionLogProvider>();

		public static IInteractionTypeDetectorService Instance { get; private set; }

		static InteractionTypeDetectorService()
		{
			Instance = new InteractionTypeDetectorService();
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private InteractionTypeDetectorService() { }

		public InteractionType DetectInteractionType(InteractionContext context)
		{
			InteractionType result = DetectInteractionType(
				context.Inputs.Initiator,
				context.Inputs.Partner,
				context.Inputs.IsRape,
				context.Inputs.IsWhoring
				);

			if (result == InteractionType.Masturbation)
			{
				context.Inputs.Partner = context.Inputs.Initiator;
			}

			return result;
		}

		private InteractionType DetectInteractionType(Pawn initiator, Pawn partner, bool isRape, bool isWhoring)
		{
			_log.Debug(initiator.GetName());
			_log.Debug(partner.GetName());

			if (initiator == partner || partner == null)
			{
				partner = initiator;

				return InteractionType.Masturbation;
			}

			if (partner.health.Dead == true)
			{
				return InteractionType.Necrophilia;
			}

			if (xxx.is_mechanoid(initiator))
			{
				return InteractionType.Mechanoid;
			}

			//Either one or the other but not both
			if (xxx.is_animal(initiator) ^ xxx.is_animal(partner))
			{
				return InteractionType.Bestiality;
			}

			if (xxx.is_animal(initiator) && xxx.is_animal(partner))
			{
				return InteractionType.Animal;
			}

			if (isWhoring)
			{
				return InteractionType.Whoring;
			}

			if (isRape)
			{
				return InteractionType.Rape;
			}

			return InteractionType.Consensual;
		}
	}
}
