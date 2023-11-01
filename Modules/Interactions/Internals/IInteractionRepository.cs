using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Internals
{
	public interface IInteractionRepository
	{
		IEnumerable<InteractionWithExtension> List();

		IEnumerable<InteractionWithExtension> ListForMasturbation();
		IEnumerable<InteractionWithExtension> ListForAnimal();
		IEnumerable<InteractionWithExtension> ListForBestiality();
		IEnumerable<InteractionWithExtension> ListForRape();
		IEnumerable<InteractionWithExtension> ListForConsensual();
		IEnumerable<InteractionWithExtension> ListForMechanoid();
		IEnumerable<InteractionWithExtension> ListForWhoring();
		IEnumerable<InteractionWithExtension> ListForNecrophilia();
	}
}
