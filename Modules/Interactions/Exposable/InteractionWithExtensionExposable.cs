using RimWorld;
using rjw.Modules.Interactions.DefModExtensions;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared.Logs;
using System.Text;
using Verse;

namespace rjw.Modules.Interactions.Exposable
{
	public class InteractionWithExtensionExposable : IExposable
	{
		private static ILog _log = LogManager.GetLogger<InteractionWithExtensionExposable>();

		public InteractionDef interactionDef;

		public InteractionSelectorExtension InteractionSelectorExtension
		{
			get
			{
				return interactionDef.GetModExtension<InteractionSelectorExtension>();
			}
		}
		public InteractionExtension Extension
		{
			get
			{
				return interactionDef.GetModExtension<InteractionExtension>();
			}
		}

		public void ExposeData()
		{
			Scribe_Defs.Look(ref interactionDef, nameof(interactionDef));
		}

		public static InteractionWithExtension Convert(InteractionWithExtensionExposable toCast)
		{
			return new InteractionWithExtension()
			{
				Interaction = toCast.interactionDef,
				Extension = toCast.Extension,
				SelectorExtension = toCast.InteractionSelectorExtension
			};
		}
		public static InteractionWithExtensionExposable Convert(InteractionWithExtension toCast)
		{
			return new InteractionWithExtensionExposable()
			{
				interactionDef = toCast.Interaction
			};
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"{nameof(interactionDef)} = {interactionDef?.defName}");

			return stringBuilder.ToString();
		}
	}
}
