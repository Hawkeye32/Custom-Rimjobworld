using System;

namespace rjw
{
	public class RJWAssociatedHediffAttribute : Attribute
	{
		public string defName { get; private set; }

		public RJWAssociatedHediffAttribute(string defName)
		{
			this.defName = defName;
		}
	}
}