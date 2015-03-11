using System;
using System.Collections.Generic;
using System.Linq;

using ContractModifier.Framework;
using UnityEngine;

namespace ContractModifier
{

	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	class cmSkins : DMCM_MBE
	{
		internal static GUISkin cmUnitySkin;

		internal static GUIStyle newWindowStyle;
		internal static GUIStyle resetBox;
		internal static GUIStyle dropDown;
		internal static GUIStyle resetButton;
		internal static GUIStyle smallLabel;
		internal static GUIStyle configDropDown;
		internal static GUIStyle configHeader;
		internal static GUIStyle configClose;
		internal static GUIStyle configDropMenu;
		internal static GUIStyle configLabel;
		internal static GUIStyle configCenterLabel;
		internal static GUIStyle configButton;
		internal static GUIStyle configToggle;

		internal static Texture2D footerBar;
		internal static Texture2D verticalBar;
		internal static Texture2D toolbarIcon;
		internal static Texture2D dropDownTex;
		internal static Texture2D windowTex;
		internal static Texture2D buttonHover;

		protected override void OnGUIOnceOnly()
		{
			windowTex = GameDatabase.Instance.GetTexture("ContractModifier/Textures/WindowTex", false);
			dropDownTex = GameDatabase.Instance.GetTexture("ContractModifier/Textures/DropDownTex", false);
			footerBar = GameDatabase.Instance.GetTexture("ContractModifier/Textures/FooterBar", false);
			verticalBar = GameDatabase.Instance.GetTexture("ContractModifier/Textures/VerticalBar", false);
			toolbarIcon = GameDatabase.Instance.GetTexture("ContractModifier/Textures/ContractsIconApp", false);
			buttonHover = GameDatabase.Instance.GetTexture("Contracts Window/Textures/ButtonHover", false);

			cmUnitySkin = DMCM_SkinsLibrary.CopySkin(DMCM_SkinsLibrary.DefSkinType.Unity);
			DMCM_SkinsLibrary.AddSkin("CMUnitySkin", cmUnitySkin);

			newWindowStyle = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.window);
			newWindowStyle.name = "WindowStyle";
			newWindowStyle.fontSize = 14;
			newWindowStyle.padding = new RectOffset(0, 1, 20, 12);
			newWindowStyle.normal.background = windowTex;
			newWindowStyle.focused.background = newWindowStyle.normal.background;
			newWindowStyle.onNormal.background = newWindowStyle.normal.background;

			dropDown = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.box);
			dropDown.name = "DropDown";
			dropDown.normal.background = dropDownTex;

			resetBox = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.label);
			resetBox.name = "ResetBox";
			resetBox.fontSize = 16;
			resetBox.normal.textColor = XKCDColors.VomitYellow;
			resetBox.wordWrap = true;
			resetBox.alignment = TextAnchor.UpperCenter;

			resetButton = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.button);
			resetButton.name = "ResetButton";
			resetButton.fontSize = 15;
			resetButton.alignment = TextAnchor.MiddleCenter;

			smallLabel = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.label);
			smallLabel.name = "SmallLabel";
			smallLabel.fontSize = 10;
			smallLabel.normal.textColor = Color.white;

			configDropMenu = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.label);
			configDropMenu.name = "ConfigDropMenu";
			configDropMenu.fontSize = 12;
			configDropMenu.padding = new RectOffset(2, 2, 2, 2);
			configDropMenu.normal.textColor = XKCDColors.White;
			configDropMenu.hover.textColor = XKCDColors.AlmostBlack;
			Texture2D menuBackground = new Texture2D(1, 1);
			menuBackground.SetPixel(1, 1, XKCDColors.OffWhite);
			menuBackground.Apply();
			configDropMenu.hover.background = menuBackground;
			configDropMenu.alignment = TextAnchor.MiddleLeft;
			configDropMenu.wordWrap = false;

			configDropDown = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.button);
			configDropDown.name = "ConfigDropDown";
			configDropDown.fontSize = 16;
			configDropDown.normal.textColor = XKCDColors.DustyOrange;
			configDropDown.alignment = TextAnchor.MiddleCenter;

			configHeader = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.label);
			configHeader.name = "ConfigHeader";
			configHeader.fontSize = 16;
			configHeader.normal.textColor = XKCDColors.DustyOrange;
			configHeader.alignment = TextAnchor.MiddleLeft;

			configClose = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.button);
			configClose.name = "ConfigClose";
			configClose.normal.background = DMCM_SkinsLibrary.DefUnitySkin.label.normal.background;
			configClose.hover.background = buttonHover;
			configClose.padding = new RectOffset(1, 1, 2, 2);
			configClose.normal.textColor = XKCDColors.DustyOrange;

			configLabel = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.label);
			configLabel.name = "ConfigLabel";
			configLabel.alignment = TextAnchor.MiddleRight;
			configLabel.fontSize = 12;

			configButton = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.button);
			configButton.name = "ConfigButton";
			configButton.fontSize = 12;

			configToggle = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.toggle);
			configToggle.name = "ConfigToggle";
			configToggle.fontSize = 12;

			configCenterLabel = new GUIStyle(configLabel);
			configCenterLabel.name = "ConfigCenterLabel";
			configCenterLabel.alignment = TextAnchor.MiddleCenter;

			DMCM_SkinsLibrary.List["CMUnitySkin"].window = new GUIStyle(newWindowStyle);

			DMCM_SkinsLibrary.AddStyle("CMUnitySkin", newWindowStyle);
		}

	}
}
