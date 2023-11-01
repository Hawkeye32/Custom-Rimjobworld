using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;
using System.Linq;
using UnityEngine;


namespace rjw
{
	///<summary>
	///This hediff class simulates pregnancy with mechanoids, mother may be human. It is not intended to be reasonable.
	///Differences from bestial pregnancy are that ... it is lethal
	///TODO: extend with something "friendlier"? than Mech_Scyther.... two Mech_Scyther's? muhahaha
	///</summary>	
	[RJWAssociatedHediff("RJW_pregnancy_mech")]
	public class Hediff_MechanoidPregnancy : Hediff_BasePregnancy
	{
		public override bool canBeAborted
		{
			get
			{
				return false;
			}
		}

		public override bool canMiscarry
		{
			get
			{
				return false;
			}
		}

		public override void PregnancyMessage()
		{
			string message_title = "RJW_PregnantTitle".Translate(pawn.LabelIndefinite()).CapitalizeFirst();
			string message_text1 = "RJW_PregnantText".Translate(pawn.LabelIndefinite()).CapitalizeFirst();
			string message_text2 = "RJW_PregnantMechStrange".Translate();
			Find.LetterStack.ReceiveLetter(message_title, message_text1 + "\n" + message_text2, LetterDefOf.ThreatBig, pawn);
		}

		public void Hack()
		{
			is_hacked = true;
		}

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			GiveBirth();
		}

		//Handles the spawning of pawns
		public override void GiveBirth()
		{
			Pawn mother = pawn;
			if (mother == null)
				return;

			if (!babies.NullOrEmpty())
			{
				foreach (Pawn baby in babies)
				{
					baby.Destroy();
					baby.Discard(true);
				}
				babies.Clear();
			}

			Faction spawn_faction = null;

			if (!is_hacked)
				spawn_faction = Faction.OfMechanoids;
			else if (ModsConfig.BiotechActive)
				spawn_faction = Faction.OfPlayer;

			PawnKindDef children = null;

			if (RJWSettings.DevMode) ModLog.Message(xxx.get_pawnname(pawn) + " birth:" + this.ToString());

			foreach (HediffDef_MechImplants implant in DefDatabase<HediffDef_MechImplants>.AllDefs.Where(x => x.parentDefs.Contains(father.kindDef.ToString())))         //try to find predefined
			{
				string childrendef;																			//try to find predefined
				List<string> childlist = new List<string>();
				if (!implant.childrenDefs.NullOrEmpty())
				{
					foreach (var child in (implant.childrenDefs))
					{
						if (DefDatabase<PawnKindDef>.GetNamedSilentFail(child) != null)
							childlist.AddDistinct(child);
					}
					childrendef = childlist.RandomElement();												//try to find predefined
					children = DefDatabase<PawnKindDef>.GetNamedSilentFail(childrendef);
					if (children != null)
						continue;
				}
			}

			if (children == null)                                                                           //fallback, use fatherDef
				children = father.kindDef;

			PawnGenerationRequest request = new PawnGenerationRequest(
				kind: children,
				faction: spawn_faction,
				forceGenerateNewPawn: true,
				developmentalStages: DevelopmentalStage.Newborn
				);

			Pawn mech = PawnGenerator.GeneratePawn(request);
			PawnUtility.TrySpawnHatchedOrBornPawn(mech, mother);
			if (!is_hacked)
			{
				LordJob_MechanoidsDefend lordJob = new LordJob_MechanoidsDefend();
				Lord lord = LordMaker.MakeNewLord(mech.Faction, lordJob, mech.Map);
				lord.AddPawn(mech);
			}
			FilthMaker.TryMakeFilth(mech.PositionHeld, mech.MapHeld, mother.RaceProps.BloodDef, mother.LabelIndefinite());

			IEnumerable<BodyPartRecord> source = from x in mother.health.hediffSet.GetNotMissingParts() where 
												x.IsInGroup(BodyPartGroupDefOf.Torso)
												&& !x.IsCorePart
												//&& x.groups.Contains(BodyPartGroupDefOf.Torso)
												//&& x.depth == BodyPartDepth.Inside
												//&& x.height == BodyPartHeight.Bottom
												//someday include depth filter
												//so it doesn't cut out external organs (breasts)?
												//vag  is genital part and genital is external
												//anal is internal
												//make sep part of vag?
												//&& x.depth == BodyPartDepth.Inside
												select x;

			if (source.Any())
			{
				foreach (BodyPartRecord part in source)
				{
					mother.health.DropBloodFilth();
				}
				if (RJWPregnancySettings.safer_mechanoid_pregnancy && is_checked)
				{
					foreach (BodyPartRecord part in source)
					{
						DamageDef surgicalCut = DamageDefOf.SurgicalCut;
						float amount = 5f;
						float armorPenetration = 999f;
						mother.TakeDamage(new DamageInfo(surgicalCut, amount, armorPenetration, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
					}
				}
				else
				{
					foreach (BodyPartRecord part in source)
					{
						Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, mother, part);
						hediff_MissingPart.lastInjury = HediffDefOf.Cut;
						hediff_MissingPart.IsFresh = true;
						mother.health.AddHediff(hediff_MissingPart);
					}
				}
			}
			mother.health.RemoveHediff(this);
		}
	}
}
