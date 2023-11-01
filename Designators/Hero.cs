using Verse;
using RimWorld;
using Multiplayer.API;

namespace rjw
{
	public static class PawnDesignations_Hero
	{
		public static bool UpdateCanDesignateHero(this Pawn pawn)
		{
			if ((RJWSettings.RPG_hero_control)
				&& xxx.is_human(pawn)
				&& pawn.IsColonist
				&& !xxx.is_slave(pawn)
				&& !pawn.IsPrisoner)
			{
				if (!pawn.IsDesignatedHero())
				{
					foreach (Pawn item in DesignatorsData.rjwHero)
					{
						if (item.IsHeroOwner())
						{
							if (RJWSettings.RPG_hero_control_Ironman && !SaveStorage.DataStore.GetPawnData(item).Ironman)
								SetHeroIronman(item);
							if (item.Dead && !SaveStorage.DataStore.GetPawnData(item).Ironman)
							{
								UnDesignateHero(item);
								//Log.Warning("CanDesignateHero:: "  + MP.PlayerName + " hero is dead remove hero tag from " + item.Name);
							}
							else
							{
								//Log.Warning("CanDesignateHero:: "  + MP.PlayerName + " already has hero - " + item.Name);
								return pawn.GetRJWPawnData().CanDesignateHero = false;
							}

						}
						else
							continue;
					}
					return pawn.GetRJWPawnData().CanDesignateHero = true;
				}
			}
			return pawn.GetRJWPawnData().CanDesignateHero = false;
		}
		public static bool CanDesignateHero(this Pawn pawn)
		{
			return pawn.GetRJWPawnData().CanDesignateHero;
		}
		public static void ToggleHero(this Pawn pawn)
		{
			pawn.UpdateCanDesignateHero();
			if (pawn.CanDesignateHero() && Find.Selector.NumSelected <= 1)
			{
				if (!pawn.IsDesignatedHero())
					DesignateHero(pawn);
			}
		}
		public static bool IsDesignatedHero(this Pawn pawn)
		{
			return pawn.GetRJWPawnData().Hero;
		}
		public static void DesignateHero(this Pawn pawn)
		{
			SyncHero(pawn, MP.PlayerName);
		}
		[SyncMethod]
		public static void UnDesignateHero(this Pawn pawn)
		{
			DesignatorsData.rjwHero.Remove(pawn);
			pawn.GetRJWPawnData().Hero = false;
		}
		public static bool IsHeroOwner(this Pawn pawn)
		{
			if (!MP.enabled)
				return pawn.GetRJWPawnData().HeroOwner == "Player" || pawn.GetRJWPawnData().HeroOwner == null || pawn.GetRJWPawnData().HeroOwner == "";
			else
				return pawn.GetRJWPawnData().HeroOwner == MP.PlayerName;
		}
		[SyncMethod]
		static void SyncHero(Pawn pawn, string theName)
		{
			if (!MP.enabled)
				theName = "Player";
			pawn.GetRJWPawnData().Hero = true;
			pawn.GetRJWPawnData().HeroOwner = theName;
			pawn.GetRJWPawnData().Ironman = RJWSettings.RPG_hero_control_Ironman;
			DesignatorsData.rjwHero.AddDistinct(pawn);
			string text = pawn.Name + " is now hero of " + theName;
			Messages.Message(text, pawn, MessageTypeDefOf.NeutralEvent);
			//Log.Message(MP.PlayerName + "  set " + pawn.Name + " to hero:" + pawn.GetPawnData().Hero);

			pawn.UpdatePermissions();
		}
		[SyncMethod]
		public static void SetHeroIronman(this Pawn pawn)
		{
			pawn.GetRJWPawnData().Ironman = true;
		}
	}
}
