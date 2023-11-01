using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace rjw
{
	//public static class bondage_gear_tradeability
	//{
	//	public static void init()
	//	{
	//		// Allows bondage gear to be selled by traders
	//		if (xxx.config.bondage_gear_enabled)
	//		{
	//			foreach (var def in DefDatabase<bondage_gear_def>.AllDefs)
	//				def.tradeability = Tradeability.Sellable;
	//		}
	//		// Forbids bondage gear to be selled by traders
	//		else
	//		{
	//			foreach (var def in DefDatabase<bondage_gear_def>.AllDefs)
	//				def.tradeability = Tradeability.None;
	//		}
	//	}
	//}

	public static class bondage_gear_extensions
	{
		public static bool has_lock(this Apparel app)
		{
			return (app.TryGetComp<CompHoloCryptoStamped>() != null);
		}

		public static bool is_wearing_locked_apparel(this Pawn p)
		{
			if (p.apparel != null)
				foreach (var app in p.apparel.WornApparel)
					if (app.has_lock())
						return true;
			return false;
		}

		// Tries to get p started on the job of using an item on either another pawn or on themself (if "other" is null).
		// Of course in order for this method to work, the item's useJob has to be able to handle use on another pawn. This
		// is true for the holokey and bondage gear in RJW but not the items in the core game
		public static void start_job(this CompUsable usa, Pawn p, LocalTargetInfo tar)
		{
			if (p.CanReserveAndReach(usa.parent, PathEndMode.Touch, Danger.Some) &&
				((tar == null) || p.CanReserveAndReach(tar, PathEndMode.Touch, Danger.Some)))
			{
				var comfor = usa.parent.GetComp<CompForbiddable>();
				if (comfor != null)
					comfor.Forbidden = false;
				var job = JobMaker.MakeJob(((CompProperties_Usable)usa.props).useJob, usa.parent, tar);
				p.jobs.TryTakeOrderedJob(job);
			}
		}

		// Creates a menu option to use an item. "tar" is expected to be a pawn, corpse or null if it doesn't apply (in which
		// case the pawn will presumably use the item on themself). "required_work" can also be null.
		public static FloatMenuOption make_option(this CompUsable usa, string label, Pawn p, LocalTargetInfo tar, WorkTypeDef required_work)
		{
			if ((tar != null) && (!p.CanReserve(tar)))
			{
				string key = "Reserved";
				string text = TranslatorFormattedStringExtensions.Translate(key);
				return new FloatMenuOption(label + " (" + text + ")", null, MenuOptionPriority.DisabledOption);
			}
			else if ((tar != null) && (!p.CanReach(tar, PathEndMode.Touch, Danger.Some)))
			{
				string key = "NoPath";
				string text = TranslatorFormattedStringExtensions.Translate(key);
				return new FloatMenuOption(label + " (" + text + ")", null, MenuOptionPriority.DisabledOption);
			}
			else if ((required_work != null) && p.WorkTagIsDisabled(required_work.workTags))
			{
				string key = "CannotPrioritizeWorkTypeDisabled";
				string text = TranslatorFormattedStringExtensions.Translate(key, required_work.gerundLabel);
				return new FloatMenuOption(label + " (" + text + ")", null, MenuOptionPriority.DisabledOption);
			}
			else
				return new FloatMenuOption(
					label,
					delegate
					{
						usa.start_job(p, tar);
					},
					MenuOptionPriority.Default);
		}
	}

	public class bondage_gear_def : ThingDef
	{
		public Type soul_type;
		public HediffDef equipped_hediff = null;
		public bool gives_bound_moodlet = false;
		public bool gives_gagged_moodlet = false;
		public bool blocks_hands = false;
		public bool blocks_oral = false;
		public bool blocks_penis = false;
		public bool blocks_vagina = false;
		public bool blocks_anus = false;
		public bool blocks_breasts = false;
		public bool blocks_udder = false;
		private bondage_gear_soul soul_ins = null;

		public List<BodyPartDef> HediffTargetBodyPartDefs;      //field for optional list of targeted parts for hediff applying
		public List<BodyPartGroupDef> BoundBodyPartGroupDefs;   //field for optional list of groups restrained of verbcasting

		public bondage_gear_soul soul
		{
			get
			{
				if (soul_ins == null)
					soul_ins = (bondage_gear_soul)Activator.CreateInstance(soul_type);
				return soul_ins;
			}
		}
	}

	public class bondage_gear_soul
	{
		// Adds the bondage gear's associated HediffDef and spawns a matching holokey
		public virtual void on_wear(Pawn wearer, Apparel gear)
		{
			var def = (bondage_gear_def)gear.def;

			if (def.equipped_hediff != null && def.HediffTargetBodyPartDefs != null)
			{
				foreach (BodyPartDef partDef in def.HediffTargetBodyPartDefs)                           //getting BodyPartDef, for example "Arm"
				{
					foreach (BodyPartRecord partRec in wearer.RaceProps.body.GetPartsWithDef(partDef))  //applying hediff to every single arm found on pawn
					{
						wearer.health.AddHediff(def.equipped_hediff, partRec);
					}
				}
			}
			else if (def.equipped_hediff != null && def.HediffTargetBodyPartDefs == null)               //backward compatibility/simplified gear define without HediffTargetBodyPartDefs
			{                                                                                           //Hediff applyed to whole body
				wearer.health.AddHediff(def.equipped_hediff);
			}

			var gear_stamp = gear.TryGetComp<CompHoloCryptoStamped>();
			if (gear_stamp != null)
			{
				var key = ThingMaker.MakeThing(ThingDef.Named("Holokey"));
				var key_stamp = key.TryGetComp<CompHoloCryptoStamped>();
				key_stamp.copy_stamp_from(gear_stamp);
				if (wearer.Map != null)
					GenSpawn.Spawn(key, wearer.Position, wearer.Map);
				else
					wearer.inventory.TryAddItemNotForSale(key);
			}
		}

		// Removes the gear's HediffDef
		public virtual void on_remove(Apparel gear, Pawn former_wearer)
		{
			var def = (bondage_gear_def)gear.def;
			if (def.equipped_hediff != null && def.HediffTargetBodyPartDefs != null)
			{                                                                                   //getting all Hediffs according with equipped_hediff def
				List<Hediff> hediffs = former_wearer.health.hediffSet.hediffs.Where(x => x.def == def.equipped_hediff).ToList();
				foreach (Hediff hedToRemove in hediffs)
				{
					if (def.HediffTargetBodyPartDefs.Contains(hedToRemove.Part.def))            //removing if applyed by this bondage_gear
						former_wearer.health.RemoveHediff(hedToRemove);                         //assuming there can be several different bondages
				}                                                                               //with the same equipped_hediff def
			}
			else if (def.equipped_hediff != null && def.HediffTargetBodyPartDefs == null)       //backward compatibility/simplified gear define without HediffTargetBodyPartDefs
			{
				var hed = former_wearer.health.hediffSet.GetFirstHediffOfDef(def.equipped_hediff);
				if (hed != null)
					former_wearer.health.RemoveHediff(hed);
			}
		}
	}

	// Give bondage gear an extremely low score when it's not being worn so pawns never equip it on themselves and give
	// it an extremely high score when it is being worn so pawns never try to take it off to equip something "better".
	public class bondage_gear : Apparel
	{
		// made this method universal for any bondage_gear, won't affect anything if gear's BoundBodyPartGroupDefs is empty or null
		public override bool AllowVerbCast(Verb verb)
		{
			if ((this.def as bondage_gear_def).BoundBodyPartGroupDefs != null &&
				verb.tool != null &&
				(this.def as bondage_gear_def).BoundBodyPartGroupDefs.Contains(verb.tool.linkedBodyPartsGroup))
			{
				return false;
			}
			return true;
		}

		//needed for save compatibility only
		public override void ExposeData()
		{
			base.ExposeData();
			//if (Scribe.mode == LoadSaveMode.PostLoadInit)
			//	CheckHediffs();
		}

		//save compatibility insurance, will prevent Armbinder hediff with 0part efficiency on whole body
		private void CheckHediffs()
		{
			var def = (bondage_gear_def)this.def;
			if (this.Wearer == null || def.equipped_hediff == null) return;

			bool changedHediff = false;
			void ApplyHediffDirect(HediffDef hedDef, BodyPartRecord partRec)
			{
				Hediff hediff = (Hediff)HediffMaker.MakeHediff(hedDef, Wearer);
				if (partRec != null)
					hediff.Part = partRec;
				Wearer.health.hediffSet.AddDirect(hediff);
				changedHediff = true;
			}

			void RemoveHediffDirect(Hediff hed)
			{
				Wearer.health.hediffSet.hediffs.Remove(hed);
				changedHediff = true;
			}

			List<bondage_gear> wornBondageGear = Wearer.apparel.WornApparel.Where(x => x is bondage_gear).Cast<bondage_gear>().ToList();
			List<Hediff> hediffs = new List<Hediff>();
			foreach (Hediff h in this.Wearer.health.hediffSet.hediffs) hediffs.Add(h);

			//checking current hediffs defined by bondage_gear for being on defined place, cleaning up the misplaced
			bool equippedHediff;
			bool onPlace;
			foreach (Hediff hed in hediffs)
			{
				equippedHediff = false;
				onPlace = false;
				foreach (bondage_gear gear in wornBondageGear)
				{
					if (hed.def == (gear.def as bondage_gear_def).equipped_hediff)
					{
						//if hediff from bondage_gear and on it's defined place then don't touch it else remove
						//assuming there can be several different bondages with the same equipped_hediff def and different hediff target parts, don't know why
						equippedHediff = true;

						if ((hed.Part != null && (gear.def as bondage_gear_def).HediffTargetBodyPartDefs == null))
						{
							//pass
						}
						else if ((hed.Part == null && (gear.def as bondage_gear_def).HediffTargetBodyPartDefs == null))
						{
							onPlace = true;
							break;
						}
						else if (hed.Part != null && (gear.def as bondage_gear_def).HediffTargetBodyPartDefs.Contains(hed.Part.def))
						{
							onPlace = true;
							break;
						}
					}
				}
				if (equippedHediff && !onPlace)
				{
					ModLog.Message("Removing Hediff " + hed.Label + " from " + Wearer + (hed.Part == null ? "'s body" : "'s " + hed.Part));
					RemoveHediffDirect(hed);
				}
			}

			// now iterating every gear for having all hediffs in place, adding missing
			foreach (bondage_gear gear in wornBondageGear)
			{
				if ((gear.def as bondage_gear_def).equipped_hediff == null) continue;
				if ((gear.def as bondage_gear_def).HediffTargetBodyPartDefs == null)                        //handling gear without HediffTargetBodyPartDefs
				{

					Hediff hed = Wearer.health.hediffSet.hediffs.Find(x => (x.def == (gear.def as bondage_gear_def).equipped_hediff && x.Part == null));//checking hediff defined by gear on whole body
					if (hed == null)                                                                        //if no legit hediff, adding
					{
						ModLog.Message("Adding missing Hediff " + (gear.def as bondage_gear_def).equipped_hediff.label + " to " + Wearer + "'s body");
						ApplyHediffDirect((gear.def as bondage_gear_def).equipped_hediff, null);
					}
				}
				else                                                                                            //handling gear with defined HediffTargetBodyPartDefs
				{
					foreach (BodyPartDef partDef in (gear.def as bondage_gear_def).HediffTargetBodyPartDefs)    //getting every partDef
					{
						foreach (BodyPartRecord partRec in Wearer.RaceProps.body.GetPartsWithDef(partDef))      //checking all parts of def for applyed hediff
						{
							Hediff hed = Wearer.health.hediffSet.hediffs.Find(x => (x.def == (gear.def as bondage_gear_def).equipped_hediff && x.Part == partRec));//checking hediff defined by gear on defined place
							if (hed == null)                                                                    //if hediff missing, adding
							{
								ModLog.Message("Adding missing Hediff " + (gear.def as bondage_gear_def).equipped_hediff.label + " to " + Wearer + "'s " + partRec.Label);
								ApplyHediffDirect((gear.def as bondage_gear_def).equipped_hediff, partRec);
							}
						}
					}
				}
				//possibility of several different items with THE SAME equipped_hediff def ON THE SAME PART is not considered, that's sick
			}
			if (changedHediff)
			{
				//Possible error in current toil on Notify_HediffChanged() if capacity.Manipulation is involved so making it in the end. 
				//Probably harmless, shouldn't happen again after corrected hediffs will be saved
				Wearer.health.Notify_HediffChanged(null);
			}
		}
	}

	public class armbinder : bondage_gear
	{
		// Prevents pawns in armbinders from melee attacking
		//public override bool AllowVerbCast (IntVec3 root, TargetInfo targ)
		//{
		//	return false;
		//}
	}

	public class yoke : bondage_gear
	{
		// Prevents pawns in armbinders from melee attacking
		//public override bool AllowVerbCast (IntVec3 root, TargetInfo targ)
		//{
		//	return false;
		//}
	}

	public class Restraints : bondage_gear
	{
		// Prevents pawns in armbinders from melee attacking
		//public override bool AllowVerbCast (IntVec3 root, TargetInfo targ)
		//{
		//	return false;
		//}
	}

	public class force_off_gear_def : RecipeDef
	{
		public ThingDef removes_apparel;
		public BodyPartDef failure_affects;
		public List<BodyPartDef> destroys_one_of = null;
		public List<BodyPartDef> major_burns_on = null;
		public List<BodyPartDef> minor_burns_on = null;
	}
}