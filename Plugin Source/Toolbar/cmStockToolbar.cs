﻿#region license
/*The MIT License (MIT)
Contract Stock Toolbar- Addon for stock app launcher interface

Copyright (c) 2014 DMagic

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
using System.Collections;

using ContractModifier.Framework;
using UnityEngine;
using KSP.UI.Screens;

namespace ContractModifier.Toolbar
{
	public class cmStockToolbar : MonoBehaviour
	{
		private ApplicationLauncherButton stockToolbarButton = null;

		private void Start()
		{
			setupToolbar();
		}

		private void setupToolbar()
		{
			StartCoroutine(addButton());
		}

		private void OnDestroy()
		{
			GameEvents.onGUIApplicationLauncherUnreadifying.Remove(removeButton);

			removeButton(HighLogic.LoadedScene);
		}

		IEnumerator addButton()
		{
			while (!ApplicationLauncher.Ready)
				yield return null;

			stockToolbarButton = ApplicationLauncher.Instance.AddModApplication(toggle, toggle, null, null, null, null, (ApplicationLauncher.AppScenes)63, cmSkins.toolbarIcon);

			GameEvents.onGUIApplicationLauncherUnreadifying.Add(removeButton);
		}

		private void removeButton(GameScenes scene)
		{
			if (stockToolbarButton != null)
			{
				ApplicationLauncher.Instance.RemoveModApplication(stockToolbarButton);
				stockToolbarButton = null;
			}
		}

		private void toggle()
		{

			//int sceneInt = ContractModifierScenario.currentScene(HighLogic.LoadedScene);
			if (contractModifierScenario.Instance == null)
				DMCM_MBE.LogFormatted("Contract Scenario Not Loaded...");
			else if (contractModifierScenario.Instance.configWindow == null)
				DMCM_MBE.LogFormatted("Contract Window Not Loaded...");
			else
			{
				contractModifierScenario.Instance.configWindow.Visible = !contractModifierScenario.Instance.configWindow.Visible;
			}
		}

	}
}
