#region license
/*The MIT License (MIT)
Contract Modifier Skins - A simple MonoBehaviour to initialize skins and textures

Copyright (c) 2015 DMagic

KSP Plugin Framework by TriggerAu, 2014: http://forum.kerbalspaceprogram.com/threads/66503-KSP-Plugin-Framework

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

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
		internal static GUIStyle configSliderLabel;

		internal static Texture2D footerBar;
		internal static Texture2D verticalBar;
		internal static Texture2D toolbarIcon;
		internal static Texture2D dropDownTex;
		internal static Texture2D windowTex;
		internal static Texture2D buttonHover;

		protected override void OnGUIOnceOnly()
		{
			windowTex = GameDatabase.Instance.GetTexture("ContractRewardModifier/Textures/WindowTex", false);
			dropDownTex = GameDatabase.Instance.GetTexture("ContractRewardModifier/Textures/DropDownTex", false);
			footerBar = GameDatabase.Instance.GetTexture("ContractRewardModifier/Textures/FooterBar", false);
			verticalBar = GameDatabase.Instance.GetTexture("ContractRewardModifier/Textures/VerticalBar", false);
			toolbarIcon = GameDatabase.Instance.GetTexture("ContractRewardModifier/Textures/ContractModifierAppIcon", false);
			buttonHover = GameDatabase.Instance.GetTexture("ContractRewardModifier/Textures/ButtonHover", false);

			cmUnitySkin = DMCM_SkinsLibrary.CopySkin(DMCM_SkinsLibrary.DefSkinType.Unity);
			DMCM_SkinsLibrary.AddSkin("CMUnitySkin", cmUnitySkin);

			newWindowStyle = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.window);
			newWindowStyle.name = "WindowStyle";
			newWindowStyle.fontSize = 14;
			newWindowStyle.fontStyle = FontStyle.Bold;
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
			resetBox.fontStyle = FontStyle.Bold;
			resetBox.normal.textColor = XKCDColors.VomitYellow;
			resetBox.wordWrap = true;
			resetBox.alignment = TextAnchor.UpperCenter;

			resetButton = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.button);
			resetButton.name = "ResetButton";
			resetButton.fontSize = 15;
			resetButton.fontStyle = FontStyle.Bold;
			resetButton.alignment = TextAnchor.MiddleCenter;

			smallLabel = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.label);
			smallLabel.name = "SmallLabel";
			smallLabel.fontSize = 10;
			smallLabel.normal.textColor = Color.white;

			configDropMenu = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.label);
			configDropMenu.name = "ConfigDropMenu";
			configDropMenu.fontSize = 13;
			configDropMenu.fontStyle = FontStyle.Bold;
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
			configDropDown.fontStyle = FontStyle.Bold;

			configHeader = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.label);
			configHeader.name = "ConfigHeader";
			configHeader.fontSize = 16;
			configHeader.normal.textColor = XKCDColors.DustyOrange;
			configHeader.alignment = TextAnchor.MiddleLeft;
			configHeader.fontStyle = FontStyle.Bold;

			configClose = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.button);
			configClose.name = "ConfigClose";
			configClose.normal.background = DMCM_SkinsLibrary.DefUnitySkin.label.normal.background;
			configClose.hover.background = buttonHover;
			configClose.padding = new RectOffset(1, 1, 2, 2);
			configClose.normal.textColor = XKCDColors.DustyOrange;

			configLabel = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.label);
			configLabel.name = "ConfigLabel";
			configLabel.alignment = TextAnchor.MiddleRight;
			configLabel.fontSize = 13;

			configButton = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.button);
			configButton.name = "ConfigButton";
			configButton.fontSize = 13;
			configButton.fontStyle = FontStyle.Bold;

			configToggle = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.toggle);
			configToggle.name = "ConfigToggle";
			configToggle.fontSize = 14;
			configToggle.fontStyle = FontStyle.Bold;

			configCenterLabel = new GUIStyle(configLabel);
			configCenterLabel.name = "ConfigCenterLabel";
			configCenterLabel.alignment = TextAnchor.MiddleCenter;

			configSliderLabel = new GUIStyle(DMCM_SkinsLibrary.DefUnitySkin.horizontalSlider);

			DMCM_SkinsLibrary.List["CMUnitySkin"].window = new GUIStyle(newWindowStyle);
			DMCM_SkinsLibrary.List["CMUnitySkin"].button = new GUIStyle(configButton);
			DMCM_SkinsLibrary.List["CMUnitySkin"].label = new GUIStyle(configLabel);
			DMCM_SkinsLibrary.List["CMUnitySkin"].toggle = new GUIStyle(configToggle);
			DMCM_SkinsLibrary.List["CMUnitySkin"].box = new GUIStyle(dropDown);

			DMCM_SkinsLibrary.AddStyle("CMUnitySkin", newWindowStyle);
			DMCM_SkinsLibrary.AddStyle("CMUnitySkin", configButton);
			DMCM_SkinsLibrary.AddStyle("CMUnitySkin", configLabel);
			DMCM_SkinsLibrary.AddStyle("CMUnitySkin", configToggle);
			DMCM_SkinsLibrary.AddStyle("CMUnitySkin", dropDown);
		}

	}
}
