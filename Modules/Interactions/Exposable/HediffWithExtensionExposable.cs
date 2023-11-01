using rjw.Modules.Interactions.DefModExtensions;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared.Logs;
using System.Text;
using Verse;

namespace rjw.Modules.Interactions.Exposable
{
	public class HediffWithExtensionExposable : IExposable
	{
		private static ILog _log = LogManager.GetLogger<HediffWithExtensionExposable>();

		public Hediff hediff;

		public GenitalPartExtension GenitalPart 
		{ 
			get 
			{
				return hediff.def.GetModExtension<GenitalPartExtension>();
			} 
		}
		public PartProps PartProps
		{
			get
			{
				return hediff.def.GetModExtension<PartProps>();
			}
		}

		public void ExposeData()
		{
			Scribe_References.Look(ref hediff, nameof(hediff));
		}

		public static HediffWithExtension Convert(HediffWithExtensionExposable self)
		{
			_log.Debug(self.ToString());

			return new HediffWithExtension()
			{
				Hediff = self.hediff,
				GenitalPart = self.GenitalPart,
				PartProps = self.PartProps
			};
		}
		public static HediffWithExtensionExposable Convert(HediffWithExtension self)
		{
			return new HediffWithExtensionExposable()
			{
				hediff = self.Hediff
			};
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"{nameof(hediff)} = {hediff?.def.defName}");

			return stringBuilder.ToString();
		}
	}
}
