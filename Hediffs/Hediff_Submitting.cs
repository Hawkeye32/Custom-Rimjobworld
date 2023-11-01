using Verse;

//Hediff worker for pawns' "lay down and submit" button
namespace rjw
{
	public class Hediff_Submitting: HediffWithComps
	{
		public override bool ShouldRemove {
			get
			{
				Pawn daddy = pawn.CarriedBy;
				if (daddy != null && daddy.Faction == pawn.Faction)
				{
					return true;
				}
				else
					return base.ShouldRemove;
			}
		}
	}
}
