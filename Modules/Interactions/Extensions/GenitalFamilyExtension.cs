using rjw.Modules.Interactions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Extensions
{
	public static class GenitalFamilyExtension
	{
		public static LewdablePartKind ToPartKind(this GenitalFamily self)
		{
			switch (self)
			{
				case GenitalFamily.Vagina:
					return LewdablePartKind.Vagina;
				case GenitalFamily.Penis:
					return LewdablePartKind.Penis;
				case GenitalFamily.Breasts:
					return LewdablePartKind.Breasts;
				case GenitalFamily.Udders:
					return LewdablePartKind.Udders;
				case GenitalFamily.Anus:
					return LewdablePartKind.Anus;
				case GenitalFamily.FemaleOvipositor:
					return LewdablePartKind.FemaleOvipositor;
				case GenitalFamily.MaleOvipositor:
					return LewdablePartKind.MaleOvipositor;
				default:
					return LewdablePartKind.Unsupported;
			}
		}
	}
}
