using HugsLib.Settings;
using UnityEngine;
using Verse;

namespace rjw.Properties
{

	// This class allows you to handle specific events on the settings class:
	//  The SettingChanging event is raised before a setting's value is changed.
	//  The PropertyChanged event is raised after a setting's value is changed.
	//  The SettingsLoaded event is raised after the setting values are loaded.
	//  The SettingsSaving event is raised before the setting values are saved.
	internal sealed partial class Settings
	{

		public Settings()
		{
			// // To add event handlers for saving and changing settings, uncomment the lines below:
			//
			// this.SettingChanging += this.SettingChangingEventHandler;
			//
			// this.SettingsSaving += this.SettingsSavingEventHandler;
			//
		}

		private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
		{
			// Add code to handle the SettingChangingEvent event here.
		}

		private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Add code to handle the SettingsSaving event here.
		}

		private static readonly Color SelectedOptionColor = new Color(0.5f, 1f, 0.5f, 1f);

		public static bool CustomDrawer_Tabs(Rect rect, SettingHandle<string> selected, string defaultValues)
		{
			int labelWidth = 140;
			//int offset = -287;
			int offset = 0;
			bool change = false;

			Rect buttonRect = new Rect(rect)
			{
				width = labelWidth
			};
			buttonRect.position = new Vector2(buttonRect.position.x + offset, buttonRect.position.y);
			Color activeColor = GUI.color;
			bool isSelected = defaultValues == selected.Value;
			if (isSelected)
				GUI.color = SelectedOptionColor;
			bool clicked = Widgets.ButtonText(buttonRect, defaultValues);
			if (isSelected)
				GUI.color = activeColor;

			if (clicked)
			{
				selected.Value = selected.Value != defaultValues ? defaultValues : "none";
				change = true;
			}

			offset += labelWidth;
			return change;
		}
	}
}
