using System.Collections.Generic;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace rjw
{
	/// <summary>
	/// Hugslib Utility Data object storing the RJW info
	/// also implements extensions to Pawn method 
	/// is used as a static field in PawnData
	/// </summary>
	public class DataStore : WorldComponent
	{
		public DataStore(World world) : base(world)
		{
		}

		public Dictionary<int, PawnData> PawnData = new Dictionary<int, PawnData> ();
		//public Dictionary<int, PartsData> PartsData = new Dictionary<int, PartsData> ();

		public override void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				PawnData.RemoveAll(item => item.Value == null || !item.Value.IsValid);
			}
				
			base.ExposeData();
			Scribe_Collections.Look(ref PawnData, "Data", LookMode.Value, LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				if (PawnData == null) PawnData = new Dictionary<int, PawnData>();
			}
		}

		public PawnData GetPawnData(Pawn pawn)
		{
			PawnData res;
			//--Log.Message("Getting data for " + pawn);
			//--Log.Message("Pawn " + pawn + " id " + pawn.thingIDNumber);
			//--Log.Message("PawnData isn't null " + !(PawnData == null));
			var filled = PawnData.TryGetValue(pawn.thingIDNumber, out res);
			//--Log.Message("Output is not null" + PawnData.TryGetValue(pawn.thingIDNumber, out res));
			//--Log.Message("Out is not null " + (res != null));
			//--Log.Message("Out is valid " + (res != null && res.IsValid));
			if ((res==null) || (!res.IsValid))
			{
				if (filled)
				{
					//--Log.Message("Clearing incorrect data for " + pawn);
					PawnData.Remove(pawn.thingIDNumber);
				}
				//--Log.Message("PawnData missing, creating for " + pawn);
				res = new PawnData(pawn);
				PawnData.Add(pawn.thingIDNumber, res);
			}
			//--Log.Message("Finishing");
			//--Log.Message("PawnData is " + res.Comfort + " " + res.Service + " " + res.Breeding);
			return res;
		}
		void SetPawnData(Pawn pawn, PawnData data)
		{
			PawnData.Add(pawn.thingIDNumber, data);
		}
	}
}