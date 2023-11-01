using RimWorld;
using rjw.Modules.Interactions.DefModExtensions;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.Extensions
{
	public static class PawnExtensions
	{
		public static SexablePawnParts GetSexablePawnParts(this Pawn self)
		{
			if (self == null)
			{
				return null;
			}

			//We get the genital parts once so we don't parse ALL the hediff all the time
			IList<HediffWithExtension> hediffWithGenitalParts = self.health.hediffSet.hediffs
				.Where(hediff => hediff.def.HasModExtension<GenitalPartExtension>())
				.Select(ToHediffWithExtension)
				.ToList();

			return new SexablePawnParts
			{
				Mouths = self.Mouths(),
				Beaks = self.Beaks(),
				Tongues = self.Tongues(),
				Hands = self.Hands(),
				Feet = self.Feet(),
				Tails = self.Tails(),

				AllParts = hediffWithGenitalParts,

				Penises = hediffWithGenitalParts.Penises(),
				Vaginas = hediffWithGenitalParts.Vaginas(),
				Breasts = hediffWithGenitalParts.Breasts(),
				Udders = hediffWithGenitalParts.Udders(),
				Anuses = hediffWithGenitalParts.Anuses(),

				FemaleOvipositors = hediffWithGenitalParts.FemaleOvipositors(),
				MaleOvipositors = hediffWithGenitalParts.MaleOvipositors()
			};
		}

		private static HediffWithExtension ToHediffWithExtension(Hediff hediff)
		{
			HediffWithExtension result = new HediffWithExtension()
			{
				Hediff = hediff,
				GenitalPart = hediff.def.GetModExtension<GenitalPartExtension>()
			};

			if (hediff.def.HasModExtension<PartProps>())
			{
				result.PartProps = hediff.def.GetModExtension<PartProps>();
			}
			return result;
		}

		private static IList<BodyPartRecord> Mouths(this Pawn self)
		{
			return self.RaceProps.body.AllParts
				//EatingSource = mouth
				.Where(part => part.def.tags.Contains(RimWorld.BodyPartTagDefOf.EatingSource))
				.Where(part => part.def.defName?.ToLower().Contains("beak") == false)
				.Where(part => part.IsMissingForPawn(self) == false)
				.ToList();
		}
		private static IList<BodyPartRecord> Beaks(this Pawn self)
		{
			return self.RaceProps.body.AllParts
				//EatingSource = mouth
				.Where(part => part.def.tags.Contains(RimWorld.BodyPartTagDefOf.EatingSource))
				.Where(part => part.def.defName?.ToLower().Contains("beak") == true)
				.Where(part => part.IsMissingForPawn(self) == false)
				.ToList();
		}
		private static IList<BodyPartRecord> Tongues(this Pawn self)
		{
			return self.RaceProps.body.AllParts
				//EatingSource = mouth
				.Where(part => part.def.defName?.ToLower().Contains("tongue") == true)
				.Where(part => part.IsMissingForPawn(self) == false)
				.ToList();
		}
		private static IList<BodyPartRecord> Tails(this Pawn self)
		{
			return self.RaceProps.body.AllParts
				.Where(part => part.def.defName?.ToLower().Contains("tail") == true)
				.Where(part => part.IsMissingForPawn(self) == false)
				.ToList();
		}
		private static IList<BodyPartRecord> Hands(this Pawn self)
		{
			return self.RaceProps.body.AllParts
				.Where(part => part.IsInGroup(BodyPartGroupDefOf.LeftHand) || part.IsInGroup(BodyPartGroupDefOf.RightHand) || part.def.defName?.ToLower().Contains("hand") == true || part.def.defName?.ToLower().Contains("arm") == true)
				.Where(part => part.IsMissingForPawn(self) == false)
				.ToList();
		}
		private static IList<BodyPartRecord> Feet(this Pawn self)
		{
			return self.RaceProps.body.AllParts
				.Where(part => part.def.defName?.ToLower().Contains("leftfoot") == true || part.def.defName?.ToLower().Contains("rightfoot") == true|| part.def.defName?.ToLower().Contains("foot") == true || part.def.defName?.ToLower().Contains("paw") == true)
				.Where(part => part.IsMissingForPawn(self) == false)
				.ToList();
		}
	}
}
