using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Need_Sex : Need_Seeker
	{
		public bool isInvisible => pawn.Map == null;

		private static float decay_per_day = 0.3f;

		// TODO: make these threshold values constants or at least static
		// readonly properties, if they're meant for patching.

		public float thresh_frustrated() => 0.05f;

		public float thresh_horny() => 0.25f;

		public float thresh_neutral() => 0.50f;

		public float thresh_satisfied() => 0.75f;

		public float thresh_ahegao() => 0.95f;

		public Need_Sex(Pawn pawn) : base(pawn)
		{
			//if (xxx.is_mechanoid(pawn)) return; //Added by nizhuan-jjr:Misc.Robots are not allowed to have sex, so they don't need sex actually.
			threshPercents = new List<float>
			{
				thresh_frustrated(),
				thresh_horny(),
				thresh_neutral(),
				thresh_satisfied(),
				thresh_ahegao()
			};
		}

		// These are overridden just for other mods to patch and alter their behavior.
		// Without it, they would need to patch `Need` itself, adding a type check to
		// every single need IN THE GAME before executing the new behavior.
		public override float CurInstantLevel
		{
			get { return base.CurInstantLevel; }
		}

		public override float CurLevel
		{
			get { return base.CurLevel; }
			set { base.CurLevel = value; }
		}

		//public override bool ShowOnNeedList
		//{
		//	get
		//	{
		//		if (Genital_Helper.has_genitals(pawn))
		//			return true;

		//		ModLog.Message("curLevelInt " + curLevelInt);
		//		return false;
		//	}
		//}

		//public override string GetTipString()
		//{
		//	return string.Concat(new string[]
		//	{
		//		this.LabelCap,
		//		": ",
		//		this.CurLevelPercentage.ToStringPercent(),
		//		"\n",
		//		this.def.description,
		//		"\n",
		//	});
		//}

		public static float brokenbodyfactor(Pawn pawn)
		{
			//This adds in the broken body system
			float broken_body_factor = 1f;
			if (pawn.health.hediffSet.HasHediff(xxx.feelingBroken))
			{
				switch (pawn.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken).CurStageIndex)
				{
					case 0:
						return 0.75f;
					case 1:
						return 1.4f;
					case 2:
						return 2f;
				}
			}
			return broken_body_factor;
		}

		public static float druggedfactor(Pawn pawn)
		{
			if (pawn.health.hediffSet.HasHediff(RJWHediffDefOf.HumpShroomEffect))
			{
				//ModLog.Message("Need_Sex::druggedfactor 3 pawn is " + xxx.get_pawnname(pawn));
				return 3f;
			}

			// No humpshroom effect but addicted?
			if (pawn.health.hediffSet.HasHediff(RJWHediffDefOf.HumpShroomAddiction))
			{
				//ModLog.Message("Need_Sex::druggedfactor 0.5 pawn is " + xxx.get_pawnname(pawn));
				return 0.5f;
			}

			//ModLog.Message("Need_Sex::druggedfactor 1 pawn is " + xxx.get_pawnname(pawn));
			return 1f;
		}

		static float diseasefactor(Pawn pawn)
		{
			return 1f;
		}

		static float futafactor(Pawn pawn)
		{
			// Checks for doubly-fertile futa.
			// Presumably, they got twice the hormones coursing through their brain.
			return Genital_Helper.is_futa(pawn) ? 2.0f : 1.0f;
		}

		static float agefactor(Pawn pawn)
		{
			// Age check was moved to start of `NeedInterval` and this factor
			// is no longer useful in base RJW.  It is left here in case any
			// mod patches this factor.
			return 1f;
		}

		/// <summary>
		/// Gets the cumulative factors affecting decay for a given pawn.
		/// </summary>
		public static float GetFallFactorFor(Pawn pawn)
		{
			return brokenbodyfactor(pawn) *
				druggedfactor(pawn) *
				diseasefactor(pawn) *
				agefactor(pawn) *
				futafactor(pawn);
		}

		static float fall_per_tick(Pawn pawn)
		{
			var fall_per_tick = decay_per_day / GenDate.TicksPerDay * GetFallFactorFor(pawn);
			//--ModLog.Message("Need_Sex::NeedInterval is called - pawn is " + xxx.get_pawnname(pawn) + " is has both genders " + (Genital_Helper.has_penis(pawn) && Genital_Helper.has_vagina(pawn)));
			//ModLog.Message(" " + xxx.get_pawnname(pawn) + "'s sex need stats:: fall_per_tick: " + fall_per_tick + ", sex_need_factor_from_lifestage: " + sex_need_factor_from_lifestage(pawn) );
			return fall_per_tick;
		}

		public override void NeedInterval()
		{
			if (isInvisible) return; // no caravans

			// Asexual or too young.
			if (xxx.is_asexual(pawn) || !xxx.can_do_loving(pawn))
			{
				CurLevel = 0.5f;
				return;
			}

			//--ModLog.Message("Need_Sex::NeedInterval is called0 - pawn is "+xxx.get_pawnname(pawn));

			if (!def.freezeWhileSleeping || pawn.Awake())
			{
				// `NeedInterval` is called by `Pawn_NeedsTracker` once every 150 ticks, which is around .06 game hours.
				var fallPerCall = 150 * fall_per_tick(pawn);
				var sexDriveModifier = xxx.get_sex_drive(pawn);
				var decayRateModifier = RJWSettings.sexneed_decay_rate;
				var decayThisCall = fallPerCall * sexDriveModifier * decayRateModifier;
				CurLevel -= decayThisCall;

				// ModLog.Message($" {xxx.get_pawnname(pawn)}'s sex need stats:: Decay/call: {decayThisCall}, Dec. rate: {decayRateModifier}, Cur.lvl: {CurLevel}, Sex drive: {sexDriveModifier}");
			}

			//--ModLog.Message("Need_Sex::NeedInterval is called1");
		}
	}
}