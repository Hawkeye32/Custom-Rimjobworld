using rjw.Modules.Interactions.Objects;

namespace rjw.Modules.Interactions.Contexts
{
	public class InteractionOutputs
	{
		public Interaction Generated { get; set; }

		public InteractionOutputs()
		{
			Generated = new Interaction();
		}
	}
}
