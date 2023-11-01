using Verse;

namespace rjw
{
	public class CompProperties_RJW : CompProperties
	{
		public CompProperties_RJW()
		{
			compClass = typeof(CompRJW);
		}
	}

	public class CompProperties_HediffBodyPart : HediffCompProperties
	{
		public CompProperties_HediffBodyPart()
		{
			compClass = typeof(CompHediffBodyPart);
		}
	}

	public class CompProperties_ThingBodyPart : CompProperties
	{
		public CompProperties_ThingBodyPart()
		{
			compClass = typeof(CompThingBodyPart);
		}
	}
}