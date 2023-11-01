using RimWorld;
using Verse;

namespace rjw
{
	public class CompGetBondageGear : CompUseEffect
	{
		public override float OrderPriority
		{
			get
			{
				return -79;
			}
		}

		public override void DoEffect(Pawn p)
		{
			base.DoEffect(p);
			var app = parent as Apparel;
			if ((p.apparel != null) && (app != null))
				p.apparel.Wear(app);
		}
	}
}