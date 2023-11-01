using System.Text;
using Verse;
using RimWorld;
using Multiplayer.API;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HugsLib.Utils;
using System.Linq;
using HugsLib;

namespace rjw
{
	/// <summary>
	/// Comp for rjw hediff parts
	/// </summary>
	public class CompHediffBodyPart : HediffComp
	{
		public string Size => parent.CurStage?.label ?? "";
		public string RaceOwner = "Unknows species";		//base race when part created
		public string PreviousOwner = "Unknows";			//base race when part created
		public float SizeBase;								//base size/severity when part created, someday alter by operation
		public float SizeOwner = 0;							//modifier of 1st owner race body size
		public float EffSize;								//SizeBase x SizeOwner = current size | hediff.severity
		public string FluidType = "";						//cummies/milk - insectjelly/honey etc
		public float FluidAmmount;							//amount of Milk/Ejaculation/Wetness
		public float FluidModifier = 1f;					//
		public string Eggs;                                 //for ovi eggs, maybe
		public bool Transplanted = false;					//transplaned from someone else

		/// <summary>
		/// part size in labels
		/// </summary>

		//public override string CompLabelInBracketsExtra
		//{
		//	get
		//	{
		//		if (Size != "")
		//			return Size;

		//		return null;
		//	}
		//}

		/// <summary>
		/// save data
		/// </summary>
		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref SizeBase, "SizeBase");
			Scribe_Values.Look(ref SizeOwner, "SizeOwner");
			Scribe_Values.Look(ref RaceOwner, "RaceOwner");
			Scribe_Values.Look(ref PreviousOwner, "PreviousOwner");
			Scribe_Values.Look(ref FluidType, "FluidType");
			Scribe_Values.Look(ref FluidAmmount, "FluidAmmount");
			Scribe_Values.Look(ref FluidModifier, "FluidModifier");
			Scribe_Values.Look(ref Eggs, "Eggs");
			Scribe_Values.Look(ref Transplanted, "Transplanted");
		}

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			base.CompPostPostAdd(dinfo);
			try//error at world gen
			{
				Pawn.GetRJWPawnData().genitals = new List<Hediff>();
				Pawn.GetRJWPawnData().breasts = new List<Hediff>();
				Pawn.GetRJWPawnData().udders = new List<Hediff>();
				Pawn.GetRJWPawnData().anus = new List<Hediff>();
			}
			catch
			{
			}
		}
		public override void CompPostPostRemoved()
		{
			base.CompPostPostRemoved();
			try
			{
				Pawn.GetRJWPawnData().genitals = new List<Hediff>();
				Pawn.GetRJWPawnData().breasts = new List<Hediff>();
				Pawn.GetRJWPawnData().udders = new List<Hediff>();
				Pawn.GetRJWPawnData().anus = new List<Hediff>();
			}
			catch
			{
			}
		}


		/// <summary>
		/// show part info in healthab
		/// </summary>
		public override string CompTipStringExtra
		{
			get
			{
				if (SizeOwner == 0)
				{
					ModLog.Warning(" " + xxx.get_pawnname(Pawn) + " part " + parent.def.defName + " is broken, resetting");
					initComp();
					updatesize();
					updatepartposition();
				}
				// In theory the tip should be a condensed form and the longer form should be elsewhere, but where?
				return GetTipString(RJWSettings.ShowRjwParts == RJWSettings.ShowParts.Extended).Join("\n");
			}
		}

		IEnumerable<string> GetTipString(bool extended)
		{
			//ModLog.Message(" CompTipStringExtra " + xxx.get_pawnname(Pawn) + " " + parent.def.defName);
			// TODO: Part type should be a property in the extension.
			//StringBuilder defstringBuilder = new StringBuilder();
			if (parent.Part != null)
				if (parent.def.defName.ToLower().Contains("breasts"))
				{
					return GetBreastTip(extended);
				}
				else if (parent.def.defName.ToLower().Contains("penis"))
				{
					return GetPenisTip(extended);
				}
				else if (parent.def.defName.ToLower().Contains("anus") || parent.def.defName.ToLower().Contains("vagina"))
				{
					return GetOrificeTip(extended);
				}
				else if (parent.def.defName.ToLower().Contains("ovi"))
				{
					return GetEggTip(extended);
				}

			return Enumerable.Empty<string>();
		}

		IEnumerable<string> GetEggTip(bool extended)
		{
			if (Eggs != "")
			{
				yield return "RJW_hediff_eggs".Translate(Eggs);
			}
		}

		IEnumerable<string> GetPenisTip(bool extended)
		{
			if (PartSizeExtension.TryGetLength(parent, out float length))
			{
				yield return "RJW_hediff_length".Translate(length.ToString("F1"));
			}

			if (!extended)
			{
				yield break;
			}

			if (PartSizeExtension.TryGetGirth(parent, out float girth))
			{
				yield return "RJW_hediff_girth".Translate(girth.ToString("F1"));
			}

			if (PartSizeExtension.TryGetPenisWeight(parent, out float weight))
			{
				yield return "RJW_hediff_weight".Translate(weight.ToString("F1"));
			}

			foreach (var line in GetFluidTip())
			{
				yield return line;
			}

			if (PartProps.TryGetProps(parent, out List<string> propslist))
			{
				yield return "Properties: " + propslist.ToCommaList();
			}
		}

		IEnumerable<string> GetOrificeTip(bool extended)
		{
			if (!extended)
			{
				yield break;
			}
				//defstringBuilder.Append(this.Def.description);
				//defstringBuilder.AppendLine("Cum: " + FluidType);
				//defstringBuilder.AppendLine("Ejaculation: " + (FluidAmmount * FluidModifier).ToString("F2") + "ml");

			// TODO: Should be scaled bigger than penis size either in xml or here.
			if (PartSizeExtension.TryGetLength(parent, out float length))
			{
				yield return "RJW_hediff_maxLength".Translate(length.ToString("F1"));
			}

			if (PartSizeExtension.TryGetGirth(parent, out float girth))
			{
				yield return "RJW_hediff_maxGirth".Translate(girth.ToString("F1"));
			}

			foreach (var line in GetFluidTip())
			{
				yield return line;
			}

			if (PartProps.TryGetProps(parent, out List<string> propslist))
			{
				yield return "Properties: " + propslist.ToCommaList();
			}
		}

		IEnumerable<string> GetBreastTip(bool extended)
		{
			if (parent.Part.def == xxx.breastsDef)
			{
				foreach (var line in GetBraCupTip())
				{
					yield return line;
				}

				if (!extended)
				{
					yield break;
				}

				if (PartSizeExtension.TryGetCupSize(parent, out float size))
				{
					var cupSize = (int)size;

					if (cupSize > 0)
					{
						var bandSize = PartSizeExtension.GetBandSize(parent);
						yield return "RJW_hediff_braSize".Translate(
							bandSize,
							cupSize);

						if (PartSizeExtension.TryGetOverbustSize(parent, out float overbust))
						{
							var underbust = PartSizeExtension.GetUnderbustSize(parent);
							yield return "RJW_hediff_underbust".Translate(underbust.ToString("F1"));
							yield return "RJW_hediff_overbust".Translate(overbust.ToString("F1"));
						}

						if (PartSizeExtension.TryGetBreastWeight(parent, out float weight))
						{
							yield return "RJW_hediff_weight".Translate(weight.ToString("F1"));
						}
					}
					else
					{
						yield return "RJW_hediff_braSizeNone".Translate();
					}
				}
			}

			foreach (var line in GetFluidTip())
			{
				yield return line;
			}

			if (PartProps.TryGetProps(parent, out List<string> propslist))
			{
				yield return "Properties: " + propslist.ToCommaList();
			}
		}

		IEnumerable<string> GetBraCupTip()
		{
			if (PartSizeExtension.TryGetCupSize(parent, out float size))
			{
				var cupSize = (int)size;

				if (cupSize > 0)
				{
					yield return "RJW_hediff_braCup".Translate(
						PartStagesDef.GetCupSizeLabel(cupSize));
				}
			}
		}

		IEnumerable<string> GetFluidTip()
		{
			if (FluidAmmount != 0 && FluidType != "")
			{
				yield return "RJW_hediff_fluidTypeFluidAmount".Translate(
					FluidType,
					(FluidAmmount * FluidModifier).ToString("F1"));
			}
		}

		//TODO: someday part(SizeOwner) change operations
		public void updatesize(float value = 0)
		{
			if (value == 0) // refresh size
			{
				//Log.Message("CompHediffBodyPart::updatesize original owner part size ( " + SizeOwner + " )");
				//Log.Message("CompHediffBodyPart::updatesize current owner part size ( " + parent.pawn.BodySize + " )");
				value = (parent.pawn.RaceProps.baseBodySize - SizeOwner) / SizeOwner;
				value = SizeBase / (1 + value);

				if (!Transplanted) // update non transplanted part size (kids growth?)
					value /= parent.pawn.RaceProps.baseBodySize / parent.pawn.BodySize;
				//else
				//	value *= parent.pawn.RaceProps.baseBodySize / parent.pawn.BodySize;

				parent.Severity = value;
			}
			else // change size by %
			{
				if (value > 0)
					incsize(value);
				else
					decsize(value);
			}
		}

		//scale parts to bodysize
		public void incsize(float value = 0)
		{
			if (value < 0)
				return;
			else if(value == 0)
				return;
			else if(value != 0)
			{
				//Log.Message(xxx.get_pawnname(pawn) + " updatesize " + parent.def.defName);
				//Log.Message("CompHediffBodyPart::updatesize - increase");
				//Log.Message("CompHediffBodyPart::updatesize( " + SizeOwner + " )");
				//Log.Message("CompHediffBodyPart::updatesize( " + parent.pawn.BodySize + " )");
				//Log.Message("CompHediffBodyPart::updatesize - value");
				//Log.Message("CompHediffBodyPart::updatesize( " + value + " )");
				value = SizeBase * (1 + value);
			}
			parent.Severity = value;
		}

		public void decsize(float value = 0)
		{
			if (value > 0)
				return;
			else if(value == 0)
				return;
			else if (value != 0)
			{
				//Log.Message(xxx.get_pawnname(pawn) + " updatesize " + parent.def.defName);
				//Log.Message("CompHediffBodyPart::updatesize - decrease");
				//Log.Message("CompHediffBodyPart::updatesize( " + SizeOwner + " )");
				//Log.Message("CompHediffBodyPart::updatesize( " + parent.pawn.BodySize + " )");
				//Log.Message("CompHediffBodyPart::updatesize - value");
				//Log.Message("CompHediffBodyPart::updatesize( " + value + " )");
				value = SizeBase * (1 + value);
			}
			if (value <= 0) // aka dont remove 0 severity hediff
			{
				value = 0.01f;
			}
			parent.Severity = value;
		}

		[SyncMethod]
		public void updatepartposition()
		{
			var partBase = parent.def as HediffDef_PartBase;
			if (partBase != null)
			{
				//Log.Message("0 " + (parent.Part == null));
				//Log.Message("1 " +parent.Part);
				//Log.Message("2 " +parent.Part.def);
				//Log.Message("3 " +parent.Part.def.defName);
				//Log.Message("4 " +partBase);
				//Log.Message("5 " +partBase.DefaultBodyPart);
				//ModLog.Message("pawn " + xxx.get_pawnname(Pawn) + ", part: " + parent.def.defName);
				if (parent.Part == null)
				{
					var bp = parent.pawn.RaceProps.body.AllParts.Find(x => x.def.defName.Contains(partBase.DefaultBodyPart));
					if (bp != null)
					{
						ModLog.Warning(" " + xxx.get_pawnname(Pawn) + " part is null/wholebody? is in wrong BodyPart position, resetting to default: " + partBase.DefaultBodyPart);
						parent.Part = bp;
						Pawn.GetRJWPawnData().genitals = new List<Hediff>();
						Pawn.GetRJWPawnData().breasts = new List<Hediff>();
						Pawn.GetRJWPawnData().udders = new List<Hediff>();
						Pawn.GetRJWPawnData().anus = new List<Hediff>();
					}
					else
					{
						ModLog.Message(" " + xxx.get_pawnname(Pawn) + " default bodypart not found: " + partBase.DefaultBodyPart + " -skip.");
					}
				}
				else if (parent.Part != null && partBase.DefaultBodyPart != "" && parent.Part.def.defName != partBase.DefaultBodyPart)
				{
					//Log.Message("f");
					//Log.Message(partBase.DefaultBodyPartList.ToLineList());
					if (partBase.DefaultBodyPartList.Any() && partBase.DefaultBodyPartList.Contains(parent.Part.def.defName))
					{
						return;
					}
					var bp = parent.pawn.RaceProps.body.AllParts.Find(x => x.def.defName.Contains(partBase.DefaultBodyPart));
					if (bp != null)
					{
						ModLog.Warning(" " + xxx.get_pawnname(Pawn) + " part " + parent.def.defName + " is in wrong BodyPart position, resetting to default: " + partBase.DefaultBodyPart);
						parent.Part = bp;
						Pawn.GetRJWPawnData().genitals = new List<Hediff>();
						Pawn.GetRJWPawnData().breasts = new List<Hediff>();
						Pawn.GetRJWPawnData().udders = new List<Hediff>();
						Pawn.GetRJWPawnData().anus = new List<Hediff>();
					}
					else
					{
						ModLog.Message(" " + xxx.get_pawnname(Pawn) + " default bodypart not found: " + partBase.DefaultBodyPart + " -skip.");
					}
					//if (pawn.IsColonist)
					//{
					//	Log.Message(xxx.get_pawnname(pawn) + " has broken hediffs, removing " + this.ToString());
					//	Log.Message(Part.ToString());
					//	Log.Message(bp.def.defName);
					//	Log.Message(partBase.DefaultBodyPart.ToString());
					//}
				}
			}
		}

		/// <summary>
		/// fill comp data
		/// </summary>
		[SyncMethod]
		public void initComp(Pawn pawn = null, bool reroll = false)
		{
			if (pawn == null)
				pawn = parent.pawn;

			double u1 = 1.0 - Rand.Range(0.01f, 1); //uniform(0.01f, 1] random doubles
			double u2 = 1.0 - Rand.Range(0.01f, 1);
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
				Math.Sin(2.0 * Math.PI * u2);
			double value =
				(3 /*mostly prevents negatives*/ + .75 /*Normalizing factor*/ * randStdNormal) / 6; /*changes to percent */
			if (value <= 0)
			{
				value = 0.01f;
			} 
			else if (value > 1)
			{
				value = 1;
			}
			bool trap = false;
			if (reroll == true)
				value = SizeBase;
			{
				if (parent.def.defName.ToLower().Contains("breast"))
				{
					//FluidType = "Milk";
					FluidAmmount = 0;

					if (pawn != null)
					{
						if (pawn.gender == Gender.Male && RJWSettings.MaleTrap)
							if (pawn.Faction != null && !xxx.is_animal(pawn)) //null faction throws error
							{
								//natives/spacer futa
								float chance = (int)pawn.Faction.def.techLevel < 5 ? RJWSettings.futa_natives_chance : RJWSettings.futa_spacers_chance;
								//nymph futa gender
								chance = xxx.is_nympho(pawn) ? RJWSettings.futa_nymph_chance : chance;

								if (Rand.Chance(chance))
								{
									//make trap
									trap = true;
								}
							}

						if (pawn.gender == Gender.Male && !trap && reroll == false)
							value = 0.01f;

					}
				}
				else if (parent.def.defName.ToLower().Contains("penis") || parent.def.defName.ToLower().Contains("Ovipositorf") || parent.def.defName.ToLower().Contains("Ovipositorm"))
				{
					FluidAmmount = (float)value * pawn.RaceProps.baseBodySize * pawn.RaceProps.baseBodySize * 10 * 2 * Rand.Range(0.75f, 1.25f);
				}
				else if (parent.def.defName.ToLower().Contains("vagina"))
				{
					FluidAmmount = (float)value * pawn.RaceProps.baseBodySize * pawn.RaceProps.baseBodySize * 10 * Rand.Range(0.75f, 1.25f);
				}
				else if (parent.def.defName.ToLower().Contains("anus"))
				{
					FluidAmmount = 0;
				}
				else if (parent.def.defName.ToLower().Contains("tentacle"))
				{
					value *= 2;
					FluidAmmount = (float)value * pawn.RaceProps.baseBodySize * pawn.RaceProps.baseBodySize * 10 * 2 * Rand.Range(0.75f, 1.25f);
				}
				if (parent.def.defName.ToLower().Contains("ovi"))
				{
					Eggs = pawn?.kindDef?.label ?? "";
				}

				FluidType = (parent.def as HediffDef_PartBase).FluidType;
				SizeBase = (float)value;
				SizeOwner = pawn?.RaceProps?.baseBodySize ?? 1.0f;
				RaceOwner = pawn?.kindDef?.race.LabelCap ?? "RJW_hediff_unknownSpecies".Translate();
				PreviousOwner = xxx.get_pawnname(pawn);
				EffSize = SizeOwner*SizeBase;
				FluidModifier = 1f;
			}
		}
	}
}
