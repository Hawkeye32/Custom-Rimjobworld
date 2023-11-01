using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace rjw
{
	public class PartSizeExtension : DefModExtension
	{
		/// <summary>
		/// Human standard would be 1.0. Null for no weight display.
		/// </summary>
		public bool? bodysizescale = false; // rescales parts sizes based on bodysize of initial owner race
		public float? density = null;

		public List<float> lengths;
		public List<float> girths;
		public List<float> cupSizes;

		public static bool TryGetLength(Hediff hediff, out float size)
		{
			return TryGetSizeFromCurve(hediff, extension => extension.lengths, true, out size);
		}

		public static bool TryGetGirth(Hediff hediff, out float size)
		{
			return TryGetSizeFromCurve(hediff, extension => extension.girths, true, out size);
		}

		public static bool TryGetCupSize(Hediff hediff, out float size)
		{
			// Cup size is already "scaled" because the same breast volume has a smaller cup size on a larger band size.
			return TryGetSizeFromCurve(hediff, extension => extension.cupSizes, false, out size);
		}

		public static float GetBandSize(Hediff hediff)
		{
			var size = GetUnderbustSize(hediff);
			size /= PartStagesDef.Instance.bandSizeInterval;
			size = (float)Math.Round(size, MidpointRounding.AwayFromZero);
			size *= PartStagesDef.Instance.bandSizeInterval;
			return size;
		}

		public static float GetUnderbustSize(Hediff hediff)
		{
			return PartStagesDef.Instance.bandSizeBase * GetLinearScale(hediff);
		}

		static float GetLinearScale(Hediff hediff)
		{
			var t = hediff.TryGetComp<CompHediffBodyPart>();
			if (t != null)
			{
				return t.SizeOwner;
			}
			return hediff.pawn.BodySize;
		}

		public static bool TryGetOverbustSize(Hediff hediff, out float size)
		{
			if (!TryGetCupSize(hediff, out var cupSize))
			{
				size = 0f;
				return false;
			}

			// Cup size is rounded up, so to do the math backwards subtract .9
			size = GetUnderbustSize(hediff) + ((cupSize - .9f) * PartStagesDef.Instance.cupSizeInterval);
			return true;
		}

		static bool TryGetSizeFromCurve(
			Hediff hediff,
			Func<PartSizeExtension, List<float>> getList,
			bool shouldScale,
			out float size)
		{
			if (!hediff.def.HasModExtension<PartSizeExtension>())
			{
				size = 0f;
				return false;
			}

			var extension = hediff.def.GetModExtension<PartSizeExtension>();
			var list = getList(extension);

			if (list == null)
			{
				size = 0f;
				return false;
			}

			var curve = new SimpleCurve(hediff.def.stages.Zip(list, (stage, size) => new CurvePoint(stage.minSeverity, size)));
			var scaleFactor = shouldScale ? GetLinearScale(hediff) : 1.0f;
			size = curve.Evaluate(hediff.Severity) * scaleFactor;
			return true;
		}

		public static bool TryGetPenisWeight(Hediff hediff, out float weight)
		{
			if (!TryGetLength(hediff, out float length) ||
				!TryGetGirth(hediff, out float girth))
			{
				weight = 0f;
				return false;
			}

			var density = hediff.def.GetModExtension<PartSizeExtension>().density;
			if (density == null)
			{
				weight = 0f;
				return false;
			}

			var r = girth / (2.0 * Math.PI);
			var volume = r * r * Math.PI * length;

			weight = (float)(volume * density.Value / 1000f);
			return true;
		}

		public static bool TryGetBreastWeight(Hediff hediff, out float weight)
		{
			if (!TryGetCupSize(hediff, out float rawSize))
			{
				weight = 0f;
				return false;
			}

			var density = hediff.def.GetModExtension<PartSizeExtension>().density;
			if (density == null)
			{
				weight = 0f;
				return false;
			}

			// Up a band size and down a cup size is about the same volume.
			var extraBandSize =  PartStagesDef.Instance.bandSizeBase * (1.0f - GetLinearScale(hediff));
			var extraCupSizes = extraBandSize / PartStagesDef.Instance.bandSizeInterval;
			var size = rawSize + extraCupSizes;

			var pounds = 0.765f
				+ 0.415f * size
				+ -0.0168f * size * size
				+ 2.47E-03f * size * size * size;
			var kg = Math.Max(0, pounds * 0.45359237f);
			weight = kg * density.Value;
			return true;
		}
	}
}
