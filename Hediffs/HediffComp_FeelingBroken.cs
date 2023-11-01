using Verse;
using RimWorld;

namespace rjw
{
	/// <summary>
	/// FeelingBroken raise/lower severity
	/// </summary>
	public class HediffCompProperties_FeelingBrokenSeverityReduce : HediffCompProperties
	{
		public HediffCompProperties_FeelingBrokenSeverityReduce()
		{
			this.compClass = typeof(HediffComp_FeelingBrokenSeverityReduce);
		}

		public SimpleCurve severityPerDayReduce;
	}
	class HediffComp_FeelingBrokenSeverityReduce : HediffComp_SeverityModifierBase
	{
		private HediffCompProperties_FeelingBrokenSeverityReduce Props
		{
			get
			{
				return (HediffCompProperties_FeelingBrokenSeverityReduce)this.props;
			}
		}
		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (base.Pawn.IsHashIntervalTick(SeverityUpdateInterval))
			{
				float num = this.SeverityChangePerDay();
				num *= 0.00333333341f;
				if (xxx.has_traits(Pawn))
				{
					if (xxx.RoMIsActive)
						if (Pawn.story.traits.HasTrait(xxx.Succubus))
							num *= 4.0f;
					if (Pawn.story.traits.HasTrait(TraitDefOf.Tough))
					{
						num *= 2.0f;
					}
					if (Pawn.story.traits.HasTrait(TraitDefOf.Tough))
					{
						num *= 2.0f;
					}
					if (Pawn.story.traits.HasTrait(TraitDefOf.Wimp))
					{
						num *= 0.5f;
					}
					if (Pawn.story.traits.HasTrait(TraitDefOf.Nerves))
					{
						int td = Pawn.story.traits.DegreeOfTrait(TraitDefOf.Nerves);
						switch (td)
						{
							case -2:
								num *= 2.0f;
								break;
							case -1:
								num *= 1.5f;
								break;
							case 1:
								num *= 0.5f;
								break;
							case 2:
								num *= 0.25f;
								break;
						}
					}
				}
				severityAdjustment += num;
			}
		}
		public override float SeverityChangePerDay()
		{
			return this.Props.severityPerDayReduce.Evaluate(this.parent.ageTicks / GenDate.TicksPerDay);
		}
	}



	public class HediffCompProperties_FeelingBrokenSeverityIncrease : HediffCompProperties
	{
		public HediffCompProperties_FeelingBrokenSeverityIncrease()
		{
			this.compClass = typeof(HediffComp_FeelingBrokenSeverityIncrease);
		}

		public SimpleCurve severityPerDayIncrease;
	}
	class HediffComp_FeelingBrokenSeverityIncrease : AdvancedHediffComp
	{
		private HediffCompProperties_FeelingBrokenSeverityIncrease Props
		{
			get
			{
				return (HediffCompProperties_FeelingBrokenSeverityIncrease)this.props;
			}
		}
		public override void CompPostMerged(Hediff other)
		{
			float num = Props.severityPerDayIncrease.Evaluate(this.parent.ageTicks / GenDate.TicksPerDay);
			if (xxx.has_traits(Pawn))
			{
				if (xxx.RoMIsActive)
					if (Pawn.story.traits.HasTrait(xxx.Succubus))
						num *= 0.25f;

				if (Pawn.story.traits.HasTrait(TraitDefOf.Tough))
				{
					num *= 0.5f;
				}
				if (Pawn.story.traits.HasTrait(TraitDefOf.Wimp))
				{
					num *= 2.0f;
				}
				if (Pawn.story.traits.HasTrait(TraitDefOf.Nerves))
				{
					int td = Pawn.story.traits.DegreeOfTrait(TraitDefOf.Nerves);
					switch (td)
					{
						case -2:
							num *= 0.25f;
							break;
						case -1:
							num *= 0.5f;
							break;
						case 1:
							num *= 1.5f;
							break;
						case 2:
							num *= 2.0f;
							break;
					}
				}
			}
			other.Severity *= num;
		}
	}


	public class AdvancedHediffWithComps : HediffWithComps
	{
		public override bool TryMergeWith(Hediff other)
		{
			for (int i = 0; i < this.comps.Count; i++)
			{
				if(this.comps[i] is AdvancedHediffComp) ((AdvancedHediffComp)this.comps[i]).CompBeforeMerged(other);
			}
			return base.TryMergeWith(other);
		}
	}
	public class AdvancedHediffComp : HediffComp
	{
		public virtual void CompBeforeMerged(Hediff other)
		{
		}
	}
}
