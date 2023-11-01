using rjw.Modules.Interactions.Enums;
using System.Collections.Generic;

namespace rjw.Modules.Interactions.Objects.Parts
{
	public class RJWLewdablePart : ILewdablePart
	{
		public HediffWithExtension Hediff { get; private set; }

		public LewdablePartKind PartKind { get; private set; }

		public float Size => Hediff.Hediff.Severity;

		private readonly IList<string> _props;
		public IList<string> Props => _props;

		public RJWLewdablePart(HediffWithExtension hediff, LewdablePartKind partKind)
		{
			Hediff = hediff;
			PartKind = partKind;
			_props = hediff.PartProps?.props ?? new();
		}
	}
}
