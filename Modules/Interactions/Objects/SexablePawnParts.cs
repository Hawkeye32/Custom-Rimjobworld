using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace rjw.Modules.Interactions.Objects
{
	public class SexablePawnParts
	{
		public IEnumerable<BodyPartRecord> Mouths { get; set; }
		public IEnumerable<BodyPartRecord> Beaks { get; set; }
		public IEnumerable<BodyPartRecord> Tongues { get; set; }
		public IEnumerable<BodyPartRecord> Feet { get; set; }
		public IEnumerable<BodyPartRecord> Hands { get; set; }
		public IEnumerable<BodyPartRecord> Tails { get; set; }

		public bool HasMouth => Mouths == null ? false : Mouths.Any();
		public bool HasHand => Hands == null ? false : Hands.Any();
		public bool HasTail => Tails == null ? false : Tails.Any();

		public IEnumerable<HediffWithExtension> AllParts { get; set; }

		public IEnumerable<HediffWithExtension> Penises { get; set; }
		public IEnumerable<HediffWithExtension> Vaginas { get; set; }
		public IEnumerable<HediffWithExtension> Breasts { get; set; }
		public IEnumerable<HediffWithExtension> Udders { get; set; }
		public IEnumerable<HediffWithExtension> Anuses { get; set; }

		public IEnumerable<HediffWithExtension> FemaleOvipositors { get; set; }
		public IEnumerable<HediffWithExtension> MaleOvipositors { get; set; }
	}
}
