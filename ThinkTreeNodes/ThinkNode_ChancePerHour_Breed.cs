using System;
using Verse;
using Verse.AI;

namespace rjw
{
	/// <summary>
	/// Cooldown for breeding
	/// something like 4.0*0.2 = 0.8 hours
	/// </summary>
	public class ThinkNode_ChancePerHour_Breed : ThinkNode_ChancePerHour
	{
		protected override float MtbHours(Pawn pawn)
		{
			return xxx.config.comfort_prisoner_rape_mtbh_mul * 0.20f ;
		}

		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			try
			{
				return base.TryIssueJobPackage(pawn, jobParams);
			}
			catch (NullReferenceException)
			{
				return ThinkResult.NoJob; ;
			}
		}
	}
}