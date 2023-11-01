using rjw.Modules.Interactions.Contexts;

namespace rjw.Modules.Interactions
{
	public interface ISpecificLewdInteractionService
	{
		InteractionOutputs GenerateSpecificInteraction(SpecificInteractionInputs inputs);
	}
}
