using rjw.Modules.Interactions.DefModExtensions;
using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class RulePackService : IRulePackService
	{
		public static IRulePackService Instance { get; private set; }

		static RulePackService()
		{
			Instance = new RulePackService();
		}

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private RulePackService() { }

		public RulePackDef FindInteractionRulePack(InteractionWithExtension interaction)
		{
			RulePackDef result;

			if (TryFindRulePack(interaction.SelectorExtension, out result))
			{
				return result;
			}

			if (TryFindRulePack(interaction.Extension, out result))
			{
				return result;
			}

			return null;
		}

		private bool TryFindRulePack(InteractionSelectorExtension extension, out RulePackDef def)
		{
			def = null;

			if (extension.rulepacks == null || extension.rulepacks.Any() == false)
			{
				return false;
			}

			def = extension.rulepacks.RandomElement();

			return def != null;
		}

		private bool TryFindRulePack(InteractionExtension extension, out RulePackDef def)
		{
			def = null;

			//no defs
			if (extension.rulepack_defs == null || extension.rulepack_defs.Any() == false)
			{
				return false;
			}

			string defname = extension.rulepack_defs.RandomElement();

			//null name ? should not happen
			if (String.IsNullOrWhiteSpace(defname) == true)
			{
				return false;
			}

			def = RulePackDef.Named(defname);

			return def != null;
		}
	}
}
