#region license
/*The MIT License (MIT)
Contract Toolbar- Addon for toolbar interface

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

using System.IO;
using System;

using ContractModifier.Framework;
using UnityEngine;

namespace ContractModifier.Toolbar
{

	class cmToolbar : DMCM_MBE
	{
		private IButton contractButton;

		protected override void Start()
		{
			setupToolbar();
		}

		private void setupToolbar()
		{
			if (!ToolbarManager.ToolbarAvailable) return;

			contractButton = ToolbarManager.Instance.add("ContractModifier", "CMToolbarID");

			if (File.Exists(Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/ContractModifier/Textures/ContractsIcon.png").Replace("\\", "/")))
				contractButton.TexturePath = "ContractModifier/Textures/ContractsIcon";
			else
				contractButton.TexturePath = "000_Toolbar/resize-cursor";

			contractButton.ToolTip = "Contract Modifier";
			contractButton.OnClick += (e) =>
				{
					if (contractModifierScenario.Instance == null)
						DMCM_MBE.LogFormatted("Contract Scenario Not Loaded...");
					else if (contractModifierScenario.Instance.configWindow == null)
						DMCM_MBE.LogFormatted("Contract Modifier Window Not Loaded...");
					else
					{
						contractModifierScenario.Instance.configWindow.Visible = !contractModifierScenario.Instance.configWindow.Visible;
					}
				};
		}

		protected override void OnDestroy()
		{
			if (!ToolbarManager.ToolbarAvailable) return;
			if (contractButton != null)
				contractButton.Destroy();
		}
	}
}
