using Verse;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;

namespace rjw
{
	/// <summary>
	/// Collection of pawn designators lists
	/// </summary>
	public class DesignatorsData : WorldComponent
	{
		public DesignatorsData(World world) : base(world)
		{
		}

		public static List<Pawn> rjwHero = new List<Pawn>();
		public static List<Pawn> rjwComfort = new List<Pawn>();
		public static List<Pawn> rjwService = new List<Pawn>();
		public static List<Pawn> rjwMilking = new List<Pawn>();
		public static List<Pawn> rjwBreeding = new List<Pawn>();
		public static List<Pawn> rjwBreedingAnimal = new List<Pawn>();

		//public static Dictionary<string, List<Pawn>> Designators = new Dictionary<string, List<Pawn>>();

		/// <summary>
		/// update designators on game load
		/// </summary>
		public void Update()
		{
			//if (RJWSettings.DevMode) 
				//ModLog.Message("DesignatorsData::Update()");
			rjwHero = PawnsFinder.All_AliveOrDead.Where(p => p.IsDesignatedHero()).ToList();
			rjwComfort = PawnsFinder.All_AliveOrDead.Where(p => p.IsDesignatedComfort()).ToList();
			rjwService = PawnsFinder.All_AliveOrDead.Where(p => p.IsDesignatedService()).ToList();
			rjwMilking = PawnsFinder.All_AliveOrDead.Where(p => p.IsDesignatedMilking()).ToList();
			rjwBreeding = PawnsFinder.All_AliveOrDead.Where(p => p.IsDesignatedBreeding()).ToList();
			rjwBreedingAnimal = PawnsFinder.All_AliveOrDead.Where(p => p.IsDesignatedBreedingAnimal()).ToList();
			//ModLog.Message("rjwHero "+rjwHero.Count());
			//ModLog.Message("rjwComfort "+rjwComfort.Count());
			//ModLog.Message("rjwService "+rjwService.Count());
			//ModLog.Message("rjwMilking "+rjwMilking.Count());
			//ModLog.Message("rjwBreeding "+rjwBreeding.Count());
			//ModLog.Message("rjwBreedingAnimal "+rjwBreedingAnimal.Count());

			//Designators = new Dictionary<string, List<Pawn>>();
			//Designators.Add("rjwHero", rjwHero);
			//Designators.Add("rjwComfort", rjwComfort);
			//Designators.Add("rjwService", rjwService);
			//Designators.Add("rjwMilking", rjwMilking);
			//Designators.Add("rjwBreeding", rjwBreeding);
			//Designators.Add("rjwBreedingAnimal", rjwBreedingAnimal);
		}
	}
}
