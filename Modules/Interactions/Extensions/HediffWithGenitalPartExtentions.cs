using rjw.Modules.Interactions.Defs;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Extensions
{
	public static class HediffWithGenitalPartExtentions
	{
		//Vanilla
		public static IEnumerable<HediffWithExtension> Penises(this IEnumerable<HediffWithExtension> self) =>
			self.Where(e => e.GenitalPart.family == GenitalFamily.Penis);
		public static IEnumerable<HediffWithExtension> Vaginas(this IEnumerable<HediffWithExtension> self) =>
			self.Where(e => e.GenitalPart.family == GenitalFamily.Vagina);
		public static IEnumerable<HediffWithExtension> Breasts(this IEnumerable<HediffWithExtension> self) =>
			self.Where(e => e.GenitalPart.family == GenitalFamily.Breasts);
		public static IEnumerable<HediffWithExtension> Udders(this IEnumerable<HediffWithExtension> self) =>
			self.Where(e => e.GenitalPart.family == GenitalFamily.Udders);
		public static IEnumerable<HediffWithExtension> Anuses(this IEnumerable<HediffWithExtension> self) =>
			self.Where(e => e.GenitalPart.family == GenitalFamily.Anus);

		//Ovi
		public static IEnumerable<HediffWithExtension> FemaleOvipositors(this IEnumerable<HediffWithExtension> self) =>
			self.Where(e => e.GenitalPart.family == GenitalFamily.FemaleOvipositor);
		public static IEnumerable<HediffWithExtension> MaleOvipositors(this IEnumerable<HediffWithExtension> self) =>
			self.Where(e => e.GenitalPart.family == GenitalFamily.MaleOvipositor);
	}
}
