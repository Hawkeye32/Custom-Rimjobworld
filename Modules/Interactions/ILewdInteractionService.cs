using rjw.Modules.Interactions.Contexts;

namespace rjw.Modules.Interactions
{
	public interface ILewdInteractionService
	{
		InteractionOutputs GenerateInteraction(InteractionInputs inputs);
	}
}
