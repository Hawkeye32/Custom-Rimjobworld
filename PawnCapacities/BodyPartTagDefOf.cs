using Verse;

namespace rjw
{
	/// <summary>
	/// Looks up and returns a BodyPartTagDef defined in the XML
	/// </summary>
	public static class BodyPartTagDefOf {
		public static BodyPartTagDef RJW_Fertility
		{
			get
			{
				if (a == null) a = (BodyPartTagDef)GenDefDatabase.GetDef(typeof(BodyPartTagDef), "RJW_Fertility");
				return a;
			}
		}
		private static BodyPartTagDef a;
	}
	
}
