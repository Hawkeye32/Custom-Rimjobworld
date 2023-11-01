using rjw.Modules.Interactions.Enums;
using rjw.Modules.Interactions.Objects;
using rjw.Modules.Interactions.Objects.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace rjw.Modules.Interactions.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<ILewdablePart> FilterSeverity(this IEnumerable<ILewdablePart> self, Nullable<float> severity)
		{
			IEnumerable<ILewdablePart> result = self;

			if (severity.HasValue == true)
			{
				result = Enumerable.Union(
					result.Where(e => e is VanillaLewdablePart),
					result.Where(e => e is RJWLewdablePart)
						.Where(e => (e as RJWLewdablePart).Hediff.Hediff.Severity >= severity.Value)
					);
			}

			foreach (ILewdablePart part in result)
			{
				yield return part;
			}
		}

		public static IEnumerable<ILewdablePart> WithPartKind(this IEnumerable<ILewdablePart> self, LewdablePartKind partKind)
		{
			return self
				.Where(e => e.PartKind == partKind);
		}
		public static IEnumerable<ILewdablePart> WithPartTag(this IEnumerable<ILewdablePart> self, GenitalTag tag)
		{
			return self
				.OfType<RJWLewdablePart>()
				.Where(e => e.Hediff.GenitalPart.tags.Contains(tag));
		}
		public static IEnumerable<ILewdablePart> WithPartKindAndTag(this IEnumerable<ILewdablePart> self, LewdablePartKind partKind, GenitalTag tag)
		{
			return self
				.WithPartKind(partKind)
				.WithPartTag(tag);
		}

		public static bool HasPartKind(this IEnumerable<ILewdablePart> self, LewdablePartKind partKind)
		{
			return self
				.WithPartKind(partKind)
				.Any();
		}
		public static bool HasPartTag(this IEnumerable<ILewdablePart> self, GenitalTag tag)
		{
			return self
				.WithPartTag(tag)
				.Any();
		}
		public static bool HasPartKindAndTag(this IEnumerable<ILewdablePart> self, LewdablePartKind partKind, GenitalTag tag)
		{
			return self
				.WithPartKindAndTag(partKind, tag)
				.Any();
		}
		
		public static IEnumerable<HediffWithExtension> BigBreasts(this IEnumerable<HediffWithExtension> self)
		{
			if (self == null)
			{
				return null;
			}

			return self
				.Where(e => e.GenitalPart.family == GenitalFamily.Breasts)
				.Where(e => e.Hediff.CurStageIndex >= 1);
		}
		
		public static bool HasBigBreasts(this IEnumerable<HediffWithExtension> self)
		{
			if (self == null)
			{
				return false;
			}

			return self
				.BigBreasts()
				.Any();
		}

		public static HediffWithExtension Largest(this IEnumerable<HediffWithExtension> self)
		{
			if (self == null)
			{
				return null;
			}

			return self
				.OrderByDescending(e => e.Hediff.Severity)
				.FirstOrDefault();
		}
		public static HediffWithExtension Smallest(this IEnumerable<HediffWithExtension> self)
		{
			if (self == null)
			{
				return null;
			}

			return self
				.OrderBy(e => e.Hediff.Severity)
				.FirstOrDefault();
		}

		public static HediffWithExtension GetBestSizeAppropriate(this IEnumerable<HediffWithExtension> self, HediffWithExtension toFit)
		{
			if (self == null)
			{
				return null;
			}

			return self
				.OrderBy(e => Mathf.Abs(e.Hediff.Severity - toFit.Hediff.Severity))
				.FirstOrDefault();
		}
	}
}
