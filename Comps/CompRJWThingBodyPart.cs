using System.Text;
using Verse;
using RimWorld;
using Multiplayer.API;
using System.Collections.Generic;

namespace rjw
{
	/// <summary>
	/// Comp for things
	/// </summary>
	public class CompThingBodyPart : ThingComp
	{
		/// <summary>
		/// Comp for rjw Thing parts.
		/// </summary>

		public string Size;										//eventually replace with below, maybe
		public float SizeBase;									//base size when part created, someday alter by operation
		public float SizeOwner = 0;								//modifier of 1st owner race body size
		public string RaceOwner = "Unknows species";			//race of 1st owner race
		public string PreviousOwner = "Unknown";				//erm
		public float EffSize;									//SizeBase x SizeOwner
		public string FluidType;								//cummies/milk - insectjelly/honey etc
		public float FluidAmmount;								//amount of Milk/Ejaculation/Wetness
		public float FluidModifier;								//
		public string Eggs;										//for ovi eggs, maybe

		/// <summary>
		/// Thing/part size in labels, init comp on select
		/// </summary>

		//public override string CompInspectStringExtra()
		//{
		//	if (Size != "")
		//		return Translator.Translate("Size") + ": " + Size;
		//	else
		//		return null;

		//	return base.CompInspectStringExtra();
		//}

		/// <summary>
		/// Thing/part size in label
		/// </summary>
		public override string TransformLabel(string label)
		{
			if (SizeOwner == 0)
				InitComp();

			if (Size != "")
				return label + " (" + Size + ")" + " (" + SizeOwner + ")";
			//this.parent.GetStatValue(StatDefOf.Nutrition);

			return label;
		}

		/// <summary>
		/// Thing/part description + comp description
		/// </summary>
		public override string GetDescriptionPart()
		{
			//ModLog.Message(" CompTipStringExtra " + xxx.get_pawnname(Pawn) + " " + parent.def.defName);
			StringBuilder stringBuilder = new StringBuilder();
			if (SizeOwner == 0)
				InitComp();
			stringBuilder.AppendLine("Previous owner: " + PreviousOwner);
			stringBuilder.AppendLine("Original owner race: " + RaceOwner);
			stringBuilder.AppendLine("Original owner race size: " + SizeOwner);
			if (parent.def.defName.ToLower().Contains("breasts"))
			{
				if (FluidType != "")
					stringBuilder.AppendLine("Producing: " + FluidType);
				if (FluidAmmount != 0)
				{
					stringBuilder.AppendLine("Amount: " + FluidAmmount.ToString("F2"));
				}
				if (Size != "")
					stringBuilder.AppendLine("Size: " + Size);
			}
			else if (parent.def.defName.ToLower().Contains("penis") || parent.def.defName.ToLower().Contains("vagina"))
			{
				if (FluidAmmount != 0)
				{
					if (FluidType != "")
						stringBuilder.AppendLine("Cum: " + FluidType);
					if (parent.def.defName.ToLower().Contains("penis"))
						stringBuilder.AppendLine("Ejaculation: " + FluidAmmount.ToString("F2") + "ml");
					if (parent.def.defName.ToLower().Contains("vagina"))
						stringBuilder.AppendLine("Wetness: " + FluidAmmount.ToString("F2"));
				}
				if (Size != "")
					stringBuilder.AppendLine("Size: " + Size);
			}
			else if (parent.def.defName.ToLower().Contains("anus"))
			{
				if (FluidType != "" && FluidAmmount != 0)
				{
					stringBuilder.AppendLine("Lube: " + FluidType);
					if (parent.def.defName.ToLower().Contains("anus"))
						stringBuilder.AppendLine("Wetness: " + FluidAmmount.ToString("F2"));
				}
				if (Size != "")
					stringBuilder.AppendLine("Size: " + Size);
			}
			else if (parent.def.defName.ToLower().Contains("ovi"))
			{
				if (Eggs != "")
					stringBuilder.AppendLine("Eggs: " + Eggs);
			}
			//TODO: someday add properties
			//if (PartProps.TryGetProps(parent, out List<string> propslist))
			//{
			//	yield return "Properties: " + propslist.ToCommaList();
			//	if (Eggs != "")
			//		stringBuilder.AppendLine("Properties: " + propslist.ToCommaList());
			//}

			return stringBuilder.ToString();
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref Size, "Size");
			Scribe_Values.Look(ref SizeBase, "SizeBase");
			Scribe_Values.Look(ref SizeOwner, "SizeOwner");
			Scribe_Values.Look(ref RaceOwner, "RaceOwner");
			Scribe_Values.Look(ref PreviousOwner, "PreviousOwner");
			Scribe_Values.Look(ref FluidType, "FluidType");
			Scribe_Values.Look(ref FluidAmmount, "FluidAmmount");
			Scribe_Values.Look(ref FluidModifier, "FluidModifier");
			Scribe_Values.Look(ref Eggs, "Eggs");
		}

		/// <summary>
		/// fill comp data
		/// </summary>
		[SyncMethod]
		public void InitComp(Pawn pawn = null)
		{
			var def = DefDatabase<HediffDef>.GetNamed(parent.def.defName);
			//pick random pawn to create temp hediff
			pawn = PawnsFinder.AllMaps_FreeColonistsAndPrisonersSpawned?.RandomElement();
			if (pawn == null)// SOS2 or other start with no colonists
				pawn = PawnsFinder.All_AliveOrDead?.RandomElement();

			Hediff hd = HediffMaker.MakeHediff(def, pawn);

			CompHediffBodyPart CompHediff = hd.TryGetComp<CompHediffBodyPart>();
			if (CompHediff != null)
			{
				CompHediff.initComp(pawn);
				CompHediff.updatesize();

				FluidType = CompHediff.FluidType;
				FluidAmmount = CompHediff.FluidAmmount;
				FluidModifier = CompHediff.FluidModifier;
				Size = CompHediff.Size;
				SizeBase = CompHediff.SizeBase;
				SizeOwner = CompHediff.SizeOwner;
				RaceOwner = "Unknown";
				PreviousOwner = "Unknown";
				Eggs = CompHediff.Eggs;
			}
		}
	}
}