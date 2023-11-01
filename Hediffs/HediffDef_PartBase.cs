using System.Linq;
using Verse;
using RimWorld;
using System.Text;
using Multiplayer.API;
using UnityEngine;
using System.Collections.Generic;

namespace rjw
{
//TODO figure out how this thing works and move eggs to comps

	[StaticConstructorOnStartup]
	public class HediffDef_PartBase : HediffDef
	{
		public bool discovered = false;
		public string Eggs = "";					//for ovi eggs, maybe
		public string FluidType = "";				//cummies/milk - insectjelly/honey etc
		public string DefaultBodyPart = "";			//Bodypart to move this part to, after fucking up with pc or other mod
		public List<string> DefaultBodyPartList;	//Bodypart list to move this part to, after fucking up with pc or other mod
		public float FluidAmmount = 0;				//amount of Milk/Ejaculation/Wetness
		public bool produceEggs;					//set in xml
		public int minEggTick = 12000;
		public int maxEggTick = 120000;
		public int minEggsProduced = 1;             // min amount of eggs produces by a bodypart
		public int maxEggsProduced = 1;             // max amount of eggs produces by a bodypart
	}
}