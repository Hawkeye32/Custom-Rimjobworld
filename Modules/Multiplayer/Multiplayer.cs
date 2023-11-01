using Verse;
using RimWorld;
using Multiplayer.API;

namespace rjw
{
	[StaticConstructorOnStartup]
	public static class RJW_Multiplayer
	{
		static RJW_Multiplayer()
		{
			if (!MP.enabled) return;

			// This is where the magic happens and your attributes
			// auto register, similar to Harmony's PatchAll.
			MP.RegisterAll();

				/*
				Log.Message("RJW MP compat testing");
				var type = AccessTools.TypeByName("rjw.RJWdesignations");
				//Log.Message("rjw MP compat " + type.Name);
				Log.Message("is host " + MP.IsHosting);
				Log.Message("PlayerName " + MP.PlayerName);
				Log.Message("IsInMultiplayer " + MP.IsInMultiplayer);

				//MP.RegisterSyncMethod(type, "Comfort");
				/*
				MP.RegisterSyncMethod(type, "<GetGizmos>Service");
				MP.RegisterSyncMethod(type, "<GetGizmos>BreedingHuman");
				MP.RegisterSyncMethod(type, "<GetGizmos>BreedingAnimal");
				MP.RegisterSyncMethod(type, "<GetGizmos>Breeder");
				MP.RegisterSyncMethod(type, "<GetGizmos>Milking");
				MP.RegisterSyncMethod(type, "<GetGizmos>Hero");
				*/


			// You can choose to not auto register and do it manually
			// with the MP.Register* methods.

			// Use MP.IsInMultiplayer to act upon it in other places
			// user can have it enabled and not be in session
		}

		//generate PredictableSeed for Verse.Rand
		public static int PredictableSeed()
		{
			int seed = 0;
			try
			{
				Map map = Find.CurrentMap;
				//int seedHourOfDay = GenLocalDate.HourOfDay(map);
				//int seedDayOfYear = GenLocalDate.DayOfYear(map);
				//int seedYear = GenLocalDate.Year(map);
				seed = (GenLocalDate.HourOfDay(map) + GenLocalDate.DayOfYear(map)) * GenLocalDate.Year(map);
				//int seed = (seedHourOfDay + seedDayOfYear) * seedYear;
				//Log.Warning("seedHourOfDay: " + seedHourOfDay + "\nseedDayOfYear: " + seedDayOfYear + "\nseedYear: " + seedYear + "\n" + seed);
			}
			catch
			{
				seed = Rand.Int;
			}
			return seed;
		}

		//generate PredictableSeed for Verse.Rand
		[SyncMethod]
		public static float RJW_MP_RAND()
		{
			return Rand.Value;
		}
	}
}