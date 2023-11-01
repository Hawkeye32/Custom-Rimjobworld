using rjw.Modules.Interactions.Enums;
using rjw.Modules.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using rjw.Modules.Interactions.Extensions;

namespace rjw.Modules.Interactions.Objects
{
	public class InteractionPawn
	{
		public Pawn Pawn { get; set; }
		public SexablePawnParts Parts { get; set; }

		public Genitals.Enums.Gender Gender { get; set; }

		public IEnumerable<Parts.ILewdablePart> ListLewdableParts()
		{
			foreach (var part in Parts.Penises)
			{
				yield return new Parts.RJWLewdablePart(part, LewdablePartKind.Penis);
			}

			foreach (var part in Parts.Vaginas)
			{
				yield return new Parts.RJWLewdablePart(part, LewdablePartKind.Vagina);
			}

			foreach (var part in Parts.Anuses)
			{
				yield return new Parts.RJWLewdablePart(part, LewdablePartKind.Anus);
			}

			foreach (var part in Parts.FemaleOvipositors)
			{
				yield return new Parts.RJWLewdablePart(part, LewdablePartKind.FemaleOvipositor);
			}

			foreach (var part in Parts.MaleOvipositors)
			{
				yield return new Parts.RJWLewdablePart(part, LewdablePartKind.MaleOvipositor);
			}

			foreach (var part in Parts.Breasts)
			{
				yield return new Parts.RJWLewdablePart(part, LewdablePartKind.Breasts);
			}

			foreach (var part in Parts.Udders)
			{
				yield return new Parts.RJWLewdablePart(part, LewdablePartKind.Udders);
			}

			foreach (var part in Parts.Mouths)
			{
				yield return new Parts.VanillaLewdablePart(Pawn, part, LewdablePartKind.Mouth);
			}

			foreach (var part in Parts.Beaks)
			{
				yield return new Parts.VanillaLewdablePart(Pawn, part, LewdablePartKind.Beak);
			}

			foreach (var part in Parts.Tongues)
			{
				yield return new Parts.VanillaLewdablePart(Pawn, part, LewdablePartKind.Tongue);
			}

			foreach (var part in Parts.Hands)
			{
				yield return new Parts.VanillaLewdablePart(Pawn, part, LewdablePartKind.Hand);
			}

			foreach (var part in Parts.Feet)
			{
				yield return new Parts.VanillaLewdablePart(Pawn, part, LewdablePartKind.Foot);
			}

			foreach (var part in Parts.Tails)
			{
				yield return new Parts.VanillaLewdablePart(Pawn, part, LewdablePartKind.Tail);
			}
		}

		public IList<LewdablePartKind> BlockedParts { get; set; }
		public IDictionary<LewdablePartKind, float> PartPreferences { get; set; }
		
		public bool HasBigBreasts()
		{
			return Parts.Breasts
				.BigBreasts()
				.Any();
		}
	}
}
