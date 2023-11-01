using RimWorld;
using rjw.Modules.Interactions.DefModExtensions;
using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace rjw.Modules.Interactions.Defs
{
	public class InteractionDefOf
	{
		public static IList<InteractionWithExtension> Interactions { get; set; }

		public static InteractionWithExtension DefaultConsensualSex => Interactions
			.Where(e => e.Interaction.defName == "Sex_Vaginal")
			.FirstOrDefault();
		public static InteractionWithExtension DefaultWhoringSex => Interactions
			.Where(e => e.Interaction.defName == "Sex_Vaginal")
			.FirstOrDefault();

		public static InteractionWithExtension DefaultRapeSex => Interactions
			.Where(e => e.Interaction.defName == "Rape_Vaginal")
			.FirstOrDefault();

		public static InteractionWithExtension DefaultMechImplantSex => Interactions
			.Where(e => e.Interaction.defName == "Rape_MechImplant")
			.FirstOrDefault();

		public static InteractionWithExtension DefaultNecrophiliaSex => Interactions
			.Where(e => e.Interaction.defName == "Rape_Vaginal")
			.FirstOrDefault();

		public static InteractionWithExtension DefaultBestialitySex => Interactions
			.Where(e => e.Interaction.defName == "Bestiality_Vaginal")
			.FirstOrDefault();

		public static InteractionWithExtension DefaultAnimalSex => Interactions
			.Where(e => e.Interaction.defName == "Bestiality_Vaginal")
			.FirstOrDefault();

		static InteractionDefOf()
		{
			Interactions = DefDatabase<InteractionDef>.AllDefs
				.Where(def => def.HasModExtension<InteractionSelectorExtension>())
				.Where(def => def.HasModExtension<InteractionExtension>())
				.Select(def => new InteractionWithExtension
				{
					Interaction = def,
					Extension = def.GetModExtension<InteractionExtension>(),
					SelectorExtension = def.GetModExtension<InteractionSelectorExtension>()
				})
				.Where(defWithExtensions => ParseHelper.FromString<xxx.rjwSextype>(defWithExtensions.Extension.rjwSextype) != xxx.rjwSextype.None)
				.ToList();
		}
	}
}
