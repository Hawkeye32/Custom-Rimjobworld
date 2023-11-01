using RimWorld;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.Objects
{
	public class Interaction
	{
		public InteractionPawn Dominant { get; set; }
		public InteractionPawn Submissive { get; set; }

		public InteractionPawn Initiator { get; set; }
		public InteractionPawn Receiver { get; set; }

		public InteractionWithExtension InteractionDef { get; set; }

		public InteractionType InteractionType { get; internal set; }
		public xxx.rjwSextype RjwSexType { get; set; }

		public RulePackDef RulePack { get; set; }

		public IList<ILewdablePart> SelectedDominantParts { get; set; }
		public IList<ILewdablePart> SelectedSubmissiveParts { get; set; }
	}
}
