using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace rjw
{
	//TODO: this needs rewrite to account reverse and group sex
	public class CondomUtility
	{
		public static readonly RecordDef CountOfCondomsUsed = DefDatabase<RecordDef>.GetNamed("CountOfCondomsUsed");
		public static readonly ThoughtDef SexWithCondom = DefDatabase<ThoughtDef>.GetNamed("SexWithCondom");
		public static readonly ThingDef Condom = DefDatabase<ThingDef>.GetNamedSilentFail("Condom");
		public static readonly ThingDef UsedCondom = DefDatabase<ThingDef>.GetNamedSilentFail("UsedCondom");

		/// <summary>
		/// Returns whether it was able to remove one condom from the pawn
		/// </summary>
		/// <param name="pawn"></param>
		/// <returns></returns>
		public static bool TryUseCondom(Pawn pawn)
		{
			if (Condom == null) return false;
			if (!xxx.is_human(pawn)) return false;
			if (xxx.has_quirk(pawn, "ImpregnationFetish")) return false;
			List<Thing> pawn_condoms = pawn.inventory.innerContainer.ToList().FindAll(obj => obj.def == Condom);
			if (pawn_condoms.Any())
			{
				var stack = pawn_condoms.Pop();
				stack.stackCount--;
				if (stack.stackCount <= 0)
				{
					stack.Destroy();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Applies the effects of having used a condom.
		/// </summary>
		/// <param name="pawn"></param>
		public static void useCondom(Pawn pawn)
		{
			if (Condom == null) return;
			if (!xxx.is_human(pawn)) return;
			pawn.records.Increment(CountOfCondomsUsed);
			pawn.needs.mood.thoughts.memories.TryGainMemory(SexWithCondom);
		}

		/// <summary>
		/// Try to get condom near sex location
		/// </summary>
		/// <param name="pawn"></param>
		public static void GetCondomFromRoom(Pawn pawn)
		{
			if (Condom == null) return;
			if (!xxx.is_human(pawn)) return;
			if (xxx.has_quirk(pawn, "ImpregnationFetish")) return;
			List<Thing> condoms_in_room = pawn.GetRoom().ContainedAndAdjacentThings.FindAll(obj => obj.def == Condom && pawn.Position.DistanceTo(obj.Position) < 10);
			//List<Thing> condoms_in_room = pawn.ownership.OwnedRoom?.ContainedAndAdjacentThings.FindAll(obj => obj.def == Condom);
			if (condoms_in_room.Any())
			{
				pawn.inventory.innerContainer.TryAdd(condoms_in_room.Pop().SplitOff(1));
			}
		}
	}
}
