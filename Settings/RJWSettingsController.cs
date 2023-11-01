using UnityEngine;
using Verse;
using Multiplayer.API;

namespace rjw.Settings
{
	public class RJWSettingsController : Mod
	{
		public RJWSettingsController(ModContentPack content) : base(content)
		{
			GetSettings<RJWSettings>();
		}

		public override string SettingsCategory()
		{
			return "RJWSettingsOne".Translate();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			if (MP.IsInMultiplayer)
				return;

			RJWSettings.DoWindowContents(inRect);
		}
	}

	public class RJWDebugController : Mod
	{
		public RJWDebugController(ModContentPack content) : base(content)
		{
			GetSettings<RJWDebugSettings>();
		}

		public override string SettingsCategory()
		{
			return "RJWDebugSettings".Translate();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			if (MP.IsInMultiplayer)
				return;

			RJWDebugSettings.DoWindowContents(inRect);
		}
	}

	public class RJWPregnancySettingsController : Mod
	{
		public RJWPregnancySettingsController(ModContentPack content) : base(content)
		{
			GetSettings<RJWPregnancySettings>();
		}

		public override string SettingsCategory()
		{
			return "RJWSettingsTwo".Translate();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			if (MP.IsInMultiplayer)
				return;

			//GUI.BeginGroup(inRect);
			//Rect outRect = new Rect(0f, 0f, inRect.width, inRect.height - 30f);
			//Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + 10f);
			//Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

			RJWPregnancySettings.DoWindowContents(inRect);

			//Widgets.EndScrollView();
			//GUI.EndGroup();
		}
	}

	public class RJWPreferenceSettingsController : Mod
	{
		public RJWPreferenceSettingsController(ModContentPack content) : base(content)
		{
			GetSettings<RJWPreferenceSettings>();
		}

		public override string SettingsCategory()
		{
			return "RJWSettingsThree".Translate();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			if (MP.IsInMultiplayer)
				return;

			RJWPreferenceSettings.DoWindowContents(inRect);
		}
	}

	public class RJWHookupSettingsController : Mod
	{
		public RJWHookupSettingsController(ModContentPack content) : base(content)
		{
			GetSettings<RJWHookupSettings>();
		}

		public override string SettingsCategory()
		{
			return "RJWSettingsFour".Translate();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			if (MP.IsInMultiplayer)
				return;
			RJWHookupSettings.DoWindowContents(inRect);
		}
	}

}