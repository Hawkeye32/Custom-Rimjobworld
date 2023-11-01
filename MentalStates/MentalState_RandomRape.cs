using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse.AI;
using Verse;

namespace rjw
{
	public class MentalState_RandomRape : SexualMentalState
	{

		public override void PostStart(string reason)
		{
			base.PostStart(reason);
			this.pawn.mindState.canLovinTick = -1;
		}
		public override bool ForceHostileTo(Thing t)
		{
			/*
			//planning random raper hostile to other colonists. but now, injured raper wont rape.
			if ((this.pawn.jobs != null) &&
				(this.pawn.jobs.curDriver != null) &&
				(this.pawn.jobs.curDriver as JobDriver_Rape != null))
			{
				return true;
			}*/
			return false;
		}

		public override bool ForceHostileTo(Faction f)
		{
			/*
			if ((this.pawn.jobs != null) &&
				(this.pawn.jobs.curDriver != null) &&
				(this.pawn.jobs.curDriver as JobDriver_Rape != null))
			{
				return true;
			}*/
			return false;
		}

		public override RandomSocialMode SocialModeMax()
		{
			return RandomSocialMode.Off;
		}
	}
}
