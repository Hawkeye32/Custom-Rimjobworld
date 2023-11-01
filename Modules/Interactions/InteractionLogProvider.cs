using rjw.Modules.Shared.Logs;

namespace rjw.Modules.Interactions
{
	public class InteractionLogProvider : ILogProvider
	{
		public bool IsActive => RJWSettings.DebugInteraction;
	}
}
