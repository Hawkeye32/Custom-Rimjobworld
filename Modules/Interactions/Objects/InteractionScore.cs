using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace rjw.Modules.Interactions.Objects
{
	public class InteractionScore
	{
		/// <summary>
		/// The dominant score, calculated using part preference
		/// </summary>
		public float Dominant { get; set; }
		/// <summary>
		/// The submissive score, calculated using part preference
		/// </summary>
		public float Submissive { get; set; }
		/// <summary>
		/// The interaction score, value from the settings
		/// </summary>
		public float Setting { get; set; }

		/// <summary>
		/// When compiling the final score of an interaction, we take into account a weight that can be applied to
		/// the submissive score
		/// for exemple, the submissive's preferences should not be taken into account in a rape
		/// </summary>
		/// <returns>
		/// the final interaction score
		/// </returns>
		//public float GetScore(float submissiveWeight)
		//{
		//	//if it's ignored, pulls toward 1
		//	float finalSubmissiveScore;
			
		//	{
		//		float invertedWeight = Mathf.Max(0, 1 - submissiveWeight);

		//		//no, i'm not sure that it's right
		//		finalSubmissiveScore = (1 * invertedWeight) + (Submissive * submissiveWeight);
		//	}

		//	return Dominant * finalSubmissiveScore * Setting;
		//}
		public float GetScore(float submissiveWeight)
		{
			// Get weighted average of dom and subs prefs and mutiply by the system weight

			// Fix sub weights of less than zero
			float fixedSubmissiveWeight = Mathf.Max(0, submissiveWeight);
			float dominantWeight = 1f;

			return Setting * (
				(Dominant * dominantWeight + Submissive * fixedSubmissiveWeight) /
				(dominantWeight + fixedSubmissiveWeight)
			);
		}
	}
}
