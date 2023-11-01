using RimWorld;
using rjw.Modules.Interactions.DefModExtensions;
using rjw.Modules.Interactions.Defs.DefFragment;
using rjw.Modules.Interactions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.Objects
{
	public class InteractionWithExtension
	{
		public InteractionDef Interaction { get; set; }
		public InteractionSelectorExtension SelectorExtension { get; set; }
		public InteractionExtension Extension { get; set; }

		#region InteractionTag

		public bool HasInteractionTag(InteractionTag tag)
		{
			if (SelectorExtension == null || SelectorExtension.tags == null || SelectorExtension.tags.Any() == false)
			{
				return false;
			}

			return SelectorExtension.tags.Contains(tag);
		}

		#endregion

		#region PartProps

		public bool DominantHasPartProp(string partProp)
		{
			return HasPartProp(SelectorExtension.dominantRequirement, partProp);
		}

		public bool SubmissiveHasPartProp(string partProp)
		{
			return HasPartProp(SelectorExtension.submissiveRequirement, partProp);
		}

		private bool HasPartProp(InteractionRequirement requirement, string partProp)
		{
			if (requirement == null || requirement.partProps == null || requirement.partProps.Any() == false)
			{
				return false;
			}

			return requirement.partProps.Contains(partProp);
		}

		#endregion

		#region Familly

		public bool DominantHasFamily(GenitalFamily family)
		{
			return HasFamily(SelectorExtension.dominantRequirement, family);
		}

		public bool SubmissiveHasFamily(GenitalFamily family)
		{
			return HasFamily(SelectorExtension.submissiveRequirement, family);
		}

		private bool HasFamily(InteractionRequirement requirement, GenitalFamily family)
		{
			if (requirement == null || requirement.families == null || requirement.families.Any() == false)
			{
				return false;
			}

			return requirement.families.Contains(family);
		}

		#endregion

		#region Tag

		public bool DominantHasTag(GenitalTag tag)
		{
			return HasTag(SelectorExtension.dominantRequirement, tag);
		}

		public bool SubmissiveHasTag(GenitalTag tag)
		{
			return HasTag(SelectorExtension.submissiveRequirement, tag);
		}

		private bool HasTag(InteractionRequirement requirement, GenitalTag tag)
		{
			if (requirement == null || requirement.tags == null || requirement.tags.Any() == false)
			{
				return false;
			}

			return requirement.tags.Contains(tag);
		}

		#endregion
	}
}
