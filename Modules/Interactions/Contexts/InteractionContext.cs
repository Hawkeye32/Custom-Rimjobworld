namespace rjw.Modules.Interactions.Contexts
{
	public class InteractionContext
	{
		public InteractionInputs Inputs { get; set; }
		public InteractionInternals Internals { get; set; }
		public InteractionOutputs Outputs { get; set; }

		public InteractionContext() { }
		public InteractionContext(InteractionInputs inputs)
		{
			Inputs = inputs;
			Internals = new InteractionInternals();
			Outputs = new InteractionOutputs();
		}
	}
}
