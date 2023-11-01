using rjw.Modules.Interactions.Defs;
using rjw.Modules.Interactions.Defs.DefFragment;
using rjw.Modules.Interactions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace rjw.Modules.Interactions.DefModExtensions
{
	public class InteractionSelectorExtension : DefModExtension
	{
		public List<RulePackDef> rulepacks = new List<RulePackDef>();
		public List<InteractionTag> tags = new List<InteractionTag>();
		public InteractionRequirement dominantRequirement;
		public InteractionRequirement submissiveRequirement;
		public string customRequirementHandler;
	}
}
