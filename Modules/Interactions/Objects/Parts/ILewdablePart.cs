using rjw.Modules.Interactions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Objects.Parts
{
	public interface ILewdablePart
	{
		LewdablePartKind PartKind { get; }

		/// <summary>
		/// The severity of the hediff
		/// </summary>
		float Size { get; }

		IList<string> Props { get; }
	}
}
