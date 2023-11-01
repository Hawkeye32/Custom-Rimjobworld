using Verse;

namespace rjw
{
	public class config : Def
	{
		// TODO: Clean these.

		public float minor_pain_threshold;	        // 0.3
		public float significant_pain_threshold;	// 0.6
		public float extreme_pain_threshold;		// 0.95
		public float base_chance_to_hit_prisoner;	// 50
		public int min_ticks_between_hits;			// 500
		public int max_ticks_between_hits;			// 700

		public float max_nymph_fraction;
		public float comfort_prisoner_rape_mtbh_mul;
	}
}