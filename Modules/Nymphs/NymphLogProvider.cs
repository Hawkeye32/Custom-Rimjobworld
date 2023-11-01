using rjw.Modules.Shared.Logs;

namespace rjw.Modules.Nymphs
{
	public class NymphLogProvider : ILogProvider
	{
		public bool IsActive => RJWSettings.DebugNymph;
	}
}
