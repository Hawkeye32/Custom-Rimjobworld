using rjw.Modules.Interactions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.Objects.Parts
{
	public class VanillaLewdablePart : ILewdablePart
	{
		public Pawn Owner { get; private set; }
		public BodyPartRecord Part { get; private set; }

		public LewdablePartKind PartKind { get; private set; }

		public float Size
		{
			get
			{
				//For reference, averge size penis = 0.25f

				switch (PartKind)
				{
					case LewdablePartKind.Hand:
						return 0.3f * Owner.BodySize;
					case LewdablePartKind.Foot:
						return 0.4f * Owner.BodySize;
					case LewdablePartKind.Tail:
						return 0.15f * Owner.BodySize;
					default:
						return 0.25f;
				}
			}
		}

		//No props in vanilla parts ... sad day ...
		public IList<string> Props => new List<string>();

		public VanillaLewdablePart(Pawn owner, BodyPartRecord part, LewdablePartKind partKind)
		{
			Owner = owner;
			Part = part;
			PartKind = partKind;
		}
	}
}
