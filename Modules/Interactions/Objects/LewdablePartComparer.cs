using rjw.Modules.Interactions.Objects.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Objects
{
	public class LewdablePartComparer : IEqualityComparer<ILewdablePart>
	{
		public bool Equals(ILewdablePart x, ILewdablePart y)
		{
			RJWLewdablePart rjwPartX = x as RJWLewdablePart;
			RJWLewdablePart rjwPartY = y as RJWLewdablePart;

			VanillaLewdablePart vanillaPartX = x as VanillaLewdablePart;
			VanillaLewdablePart vanillaPartY = y as VanillaLewdablePart;

			//One of them is rjw
			if (rjwPartX != null || rjwPartY != null)
			{
				//Compare the hediffs
				if (rjwPartX?.Hediff != rjwPartY?.Hediff)
				{
					return false;
				}
			}

			//One of them is vanilla
			if (vanillaPartX != null || vanillaPartY != null)
			{
				//Compare the BPR
				if (vanillaPartX?.Part != vanillaPartY?.Part)
				{
					return false;
				}
			}

			return true;
		}

		public int GetHashCode(ILewdablePart obj)
		{
			RJWLewdablePart rjwPart = obj as RJWLewdablePart;
			VanillaLewdablePart vanillaPart = obj as VanillaLewdablePart;

			if (rjwPart != null)
			{
				rjwPart.Hediff.GetHashCode();
			}

			if (vanillaPart != null)
			{
				vanillaPart.Part.GetHashCode();
			}

			return 0;
		}
	}
}
