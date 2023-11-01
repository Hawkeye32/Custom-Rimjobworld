//using System;
//using RimWorld;
//using Verse;

//namespace rjw
//{
//	public class BackstoryDef : Def
//	{
//		public BackstoryDef()
//		{
//			this.slot = BackstorySlot.Childhood;
//		}

//		public static BackstoryDef Named(string defName)
//		{
//			return DefDatabase<BackstoryDef>.GetNamed(defName, true);
//		}

//		public override void ResolveReferences()
//		{
//			base.ResolveReferences();
//			if (BackstoryDatabase.allBackstories.ContainsKey(this.defName))
//			{
//				ModLog.Error("BackstoryDatabase already contains: " + this.defName);
//				return;
//			}
//			{
//				//Log.Warning("BackstoryDatabase does not contains: " + this.defName);
//			}
//			if (!this.title.NullOrEmpty())
//			{
//				Backstory backstory = new Backstory();
//				backstory.SetTitle(this.title, this.titleFemale);
//				backstory.SetTitleShort(this.titleShort, this.titleFemaleShort);
//				backstory.baseDesc = this.baseDescription;
//				backstory.slot = this.slot;
//				backstory.spawnCategories.Add(this.categoryName);
//				backstory.ResolveReferences();
//				backstory.PostLoad();
//				backstory.identifier = this.defName;
//				BackstoryDatabase.allBackstories.Add(backstory.identifier, backstory);
//				//Log.Warning("BackstoryDatabase added: " + backstory.identifier);
//				//Log.Warning("BackstoryDatabase added: " + backstory.spawnCategories.ToCommaList());
//			}
//		}

//		public string baseDescription;

//		public string title;

//		public string titleShort;

//		public string titleFemale;

//		public string titleFemaleShort;

//		public string categoryName;

//		public BackstorySlot slot;
//	}
//}
