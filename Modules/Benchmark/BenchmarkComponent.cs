using System;
using System.Text;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace rjw.Modules.Benchmark
{
	// RimWorld only instantiates WorldComponents that exist, so if benchmarking
	// is not enabled, this class does not exist and never gets added to the game.
	// So, it wastes no resources at all.
#if BENCHMARK
	public class BenchmarkComponent : WorldComponent
	{
		const int loggingInterval = GenDate.TicksPerHour;
		const long maxNano = 1000L;
		const long maxMicro = maxNano * maxNano;
		const long maxMilli = maxNano * maxNano * maxNano;

		public BenchmarkComponent(World world) : base(world) { }

		public override void WorldComponentTick()
		{
			if (Find.TickManager.TicksGame % loggingInterval != 0) return;
			
			foreach (var record in BenchmarkCore.GetSamples().OrderBy((s) => s.methodName))
				LogResults(record);
		}

		private void LogResults(BenchyRecord record)
		{
			// Sort it so we can make some assumptions.
			record.samples.InsertionSort((l, r) => l.CompareTo(r));

			var methodName = record.methodName;
			var totalCalls = record.totalCalls;
			var length = record.samples.Length;
			var totalRuntime = record.samples.Sum();
			var averageNanoSecs = (float)record.samples.Average();
			var minNanoSecs = record.samples.First();
			var maxNanoSecs = record.samples.Last();
			(var loAvg, var hiAvg) = GetSplitPercentile(record.samples);

			StringBuilder sb = new();
			sb.Append("Benchy<").Append(methodName).Append(">(");
			sb.Append("avg: ").Append(ToUnitString(averageNanoSecs)).Append(", ");
			sb.Append("lo90%: ").Append(ToUnitString(loAvg)).Append(", ");
			sb.Append("hi10%: ").Append(ToUnitString(hiAvg)).Append(", ");
			sb.Append("min: ").Append(ToUnitString(minNanoSecs)).Append(", ");
			sb.Append("max: ").Append(ToUnitString(maxNanoSecs)).Append(")");
			sb.AppendLine().Append("  Total calls: ")
				.Append(totalCalls);
			sb.AppendLine().Append("  Total of last ").Append(length).Append(" samples: ")
				.Append(ToUnitString(totalRuntime));

			ModLog.Message(sb.ToString());
		}

		private (float, float) GetSplitPercentile(long[] samples)
		{
			var hiRange = Math.Max(Mathf.CeilToInt(0.1f * samples.Length), 1);
			var loRange = Math.Max(samples.Length - hiRange, 1);
			var hiAvg = (float)samples.TakeLast(hiRange).Average();
			var loAvg = (float)samples.Take(loRange).Average();
			return (loAvg, hiAvg);
		}

		private string ToUnitString(long nanoSecs) => nanoSecs switch
		{
			< maxNano => $"{nanoSecs}ns",
			_ => ToUnitString((float)nanoSecs)
		};

		private string ToUnitString(float nanoSecs) => nanoSecs switch
		{
			< maxNano => $"{nanoSecs:F3}ns",
			< maxMicro => $"{nanoSecs / maxNano:F3}µs",
			< maxMilli => $"{nanoSecs / maxMicro:F3}ms",
			_ => $"{nanoSecs / maxMilli:F3}s"
		};

	}
#endif
}