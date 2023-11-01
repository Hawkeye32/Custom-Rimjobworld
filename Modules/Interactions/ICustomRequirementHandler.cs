using rjw.Modules.Interactions.Objects;

namespace rjw.Modules.Interactions
{
	public interface ICustomRequirementHandler
	{
		/// <summary>
		/// Will be used to match the field <see cref="InteractionSelectorExtension.customRequirementHandler"/>
		/// </summary>
		string HandlerKey { get; }

		bool FufillRequirements(InteractionWithExtension interaction, InteractionPawn dominant, InteractionPawn submissive);
	}
}
