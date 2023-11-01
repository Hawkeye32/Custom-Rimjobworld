using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace rjw
{
	public class RaceTag
	{
		// I only created tags for RaceGroupDef properties that seemed like keywords (like slime) rather than behavior (like oviPregnancy).
		public readonly static RaceTag Chitin = new RaceTag("Chitin");
		public readonly static RaceTag Demon = new RaceTag("Demon");
		public readonly static RaceTag Feathers = new RaceTag("Feathers");
		public readonly static RaceTag Fur = new RaceTag("Fur");
		public readonly static RaceTag Plant = new RaceTag("Plant");
		public readonly static RaceTag Robot = new RaceTag("Robot");
		public readonly static RaceTag Scales = new RaceTag("Scales");
		public readonly static RaceTag Skin = new RaceTag("Skin");
		public readonly static RaceTag Slime = new RaceTag("Slime");

		public string Key { get; }

		RaceTag(string key)
		{
			Key = key;
		}

		/// <summary>
		/// For backwards compatability only. Shouldn't add more special cases here.
		/// </summary>
		public bool DefaultWhenNoRaceGroupDef(Pawn pawn)
		{
			if (this == Demon)
			{
				return xxx.is_demon(pawn);
			}
			else if (this == Slime)
			{
				return xxx.is_slime(pawn);
			}
			else if (this == Skin)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
