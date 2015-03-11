#region license
/*The MIT License (MIT)
Contract Config - Addon to control contract config options

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

using Contracts;
using Contracts.Parameters;
using ContractModifier.Framework;
using ContractModifier.Toolbar;
using UnityEngine;

namespace ContractModifier
{
	class contractConfig : DMCM_MBW
	{
		private const string lockID = "ContractConfigLockID";
		private bool dropDown, cDropDown, pDropDown, rCPopup, rPPopup, wPopup, zPopup, toolbarPopup, activePopup;
		private bool spacecenterLocked, trackingLocked, editorLocked;
		private bool stockToolbar = true;
		private bool alterActive, allowZero, showToolbar;
		private Rect ddRect;
		private Vector2 cScroll, pScroll;
		private List<contractTypeContainer> cList;
		private List<paramTypeContainer> pList;
		private paramTypeContainer paramType;
		private contractTypeContainer contractType;
		private float cFRew, cFAdv, cFPen, cRRew, cRPen, cSRew, cOffer, cActive, cDur, pFRew, pFPen, pRRew, pRPen, pSRew;
		private float[] oldCValues;
		private float[] oldPValues;

		protected override void Awake()
		{
			WindowCaption = "Contract Configuration";
			WindowRect = new Rect(40, 80, 780, 380);
			WindowStyle = cmSkins.newWindowStyle;
			Visible = false;
			DragEnabled = true;
			TooltipMouseOffset = new Vector2d(-10, -25);

			//Make sure our click-through control locks are disabled
			InputLockManager.RemoveControlLock(lockID);

			DMCM_SkinsLibrary.SetCurrent("CMUnitySkin");
		}

		protected override void Start()
		{
			cList = contractModifierScenario.Instance.setContractTypes(cList);
			pList = contractModifierScenario.Instance.setParamTypes(pList);
			if (cList.Count > 0 && pList.Count > 0)
			{
				setContractType(cList[0], contractModifierScenario.Instance.allowZero);
				setParameterType(pList[0], contractModifierScenario.Instance.allowZero);
			}
			stockToolbar = contractModifierScenario.Instance.stockToolbar;
			alterActive = contractModifierScenario.Instance.alterActive;
			allowZero = contractModifierScenario.Instance.allowZero;
			showToolbar = contractModifierScenario.Instance.CMNode.ShowToolbar;
		}

		protected override void OnDestroy()
		{
			if (InputLockManager.lockStack.ContainsKey(lockID))
				EditorLogic.fetch.Unlock(lockID);
		}

		protected override void DrawWindowPre(int id)
		{
			//Prevent click through from activating part options
			if (HighLogic.LoadedSceneIsFlight)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && GUIUtility.hotControl == 0 && Input.GetMouseButton(0))
				{
					foreach (var window in GameObject.FindObjectsOfType(typeof(UIPartActionWindow)).OfType<UIPartActionWindow>().Where(p => p.Display == UIPartActionWindow.DisplayType.Selected))
					{
						window.enabled = false;
						window.displayDirty = true;
					}
				}
			}

			//Lock space center click through
			if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !spacecenterLocked)
				{
					InputLockManager.SetControlLock(ControlTypes.CAMERACONTROLS | ControlTypes.KSC_FACILITIES | ControlTypes.KSC_UI, lockID);
					spacecenterLocked = true;
				}
				else if (!WindowRect.Contains(mousePos) && spacecenterLocked)
				{
					InputLockManager.RemoveControlLock(lockID);
					spacecenterLocked = false;
				}
			}

			//Lock tracking scene click through
			if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !trackingLocked)
				{
					InputLockManager.SetControlLock(ControlTypes.CAMERACONTROLS | ControlTypes.TRACKINGSTATION_ALL, lockID);
					trackingLocked = true;
				}
				else if (!WindowRect.Contains(mousePos) && trackingLocked)
				{
					InputLockManager.RemoveControlLock(lockID);
					trackingLocked = false;
				}
			}

			//Lock editor click through
			if (HighLogic.LoadedSceneIsEditor)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !editorLocked)
				{
					EditorLogic.fetch.Lock(true, true, true, lockID);
					editorLocked = true;
				}
				else if (!WindowRect.Contains(mousePos) && editorLocked)
				{
					EditorLogic.fetch.Unlock(lockID);
					editorLocked = false;
				}
			}

			if (!dropDown)
			{
				cDropDown = false;
				pDropDown = false;
				rCPopup = false;
				rPPopup = false;
				zPopup = false;
				wPopup = false;
				activePopup = false;
			}
		}

		protected override void DrawWindow(int id)
		{
			closeButton(id);						/* Draw the close button */

			GUILayout.BeginVertical();
				GUILayout.Space(10);
				GUILayout.BeginHorizontal();
					GUILayout.Space(8);
					GUILayout.BeginVertical();
						contractSelectionMenu(id);	/* Drop down menu and label for the current contract type */
						contractOptions(id);		/* Contract reward/penalty sliders */
					GUILayout.EndVertical();
					GUILayout.Space(50);
					GUILayout.BeginVertical();
						parameterSelectionMenu(id);	/* Drop down menu and label for the current parameter */
						parameterOptions(id);		/* Parameter reward/penalty sliders */
						windowConfig(id);			/* Options and settings */
					GUILayout.EndVertical();
					GUILayout.Space(8);
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			windowFrame(id);						/* Draw simple textures to divide the window space */
			dropDownMenu(id);						/* Draw the drop down menus when open */
		}

		protected override void DrawWindowPost(int id)
		{
			if (stockToolbar != contractModifierScenario.Instance.stockToolbar)
			{
				stockToolbar = contractModifierScenario.Instance.stockToolbar;
				if (stockToolbar)
				{
					contractModifierScenario.Instance.appLauncherButton = gameObject.AddComponent<cmStockToolbar>();
					if (contractModifierScenario.Instance.blizzyToolbarButton != null)
					{
						Destroy(contractModifierScenario.Instance.blizzyToolbarButton);
					}
				}
				else
				{
					contractModifierScenario.Instance.blizzyToolbarButton = gameObject.AddComponent<cmToolbar>();
					if (contractModifierScenario.Instance.appLauncherButton != null)
					{
						Destroy(contractModifierScenario.Instance.appLauncherButton);
					}
				}
			}

			if (!contractModifierScenario.Instance.CMNode.WarnedZero)
			{
				if (allowZero != contractModifierScenario.Instance.allowZero)
				{
					if (!allowZero)
					{
						contractModifierScenario.Instance.allowZero = false;
						dropDown = true;
						zPopup = true;
					}
				}
			}

			if (!contractModifierScenario.Instance.CMNode.WarnedAlterActive)
			{
				if (alterActive != contractModifierScenario.Instance.alterActive)
				{
					if (!alterActive)
					{
						contractModifierScenario.Instance.alterActive = false;
						dropDown = true;
						activePopup = true;
					}
				}
			}

			if (!contractModifierScenario.Instance.CMNode.WarnedToolbar)
			{
				if (showToolbar != contractModifierScenario.Instance.CMNode.ShowToolbar)
				{
					if (showToolbar)
					{
						contractModifierScenario.Instance.CMNode.ShowToolbar = true;
						dropDown = true;
						toolbarPopup = true;
					}
				}
			}

			if (dropDown && Event.current.type == EventType.mouseDown && !ddRect.Contains(Event.current.mousePosition))
				dropDown = false;
		}

		//Draw the close button in the upper right corner
		private void closeButton(int id)
		{
			Rect r = new Rect(WindowRect.width - 20, 0, 18, 18);
			if (GUI.Button(r, "✖", cmSkins.configClose))
			{
				InputLockManager.RemoveControlLock(lockID);
				spacecenterLocked = false;
				trackingLocked = false;
				editorLocked = false;
				Visible = false;
			}
		}

		//Contract type selector
		private void contractSelectionMenu(int id)
		{
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if(GUILayout.Button("Contract Type:", cmSkins.configDropDown, GUILayout.MaxWidth(130)))
				{
					dropDown = !dropDown;
					cDropDown = !cDropDown;
				}

				if (contractType != null)
					GUILayout.Label(contractType.Name, cmSkins.configHeader, GUILayout.MaxWidth(160));
				else
					GUILayout.Label("Unknown", cmSkins.configHeader, GUILayout.MaxWidth(160));
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			if (contractType.Generic)
			{
				GUILayout.Space(-4);
				if (GUILayout.Button("Apply To All Contracts", cmSkins.configButton))
				{
					foreach (contractTypeContainer c in cList)
					{
						c.RewardFund = contractType.RewardFund;
						c.AdvanceFund = contractType.AdvanceFund;
						c.PenaltyFund = contractType.PenaltyFund;
						c.RewardRep = contractType.RewardRep;
						c.PenaltyRep = contractType.PenaltyRep;
						c.RewardScience = contractType.RewardScience;
						c.DurationTime = contractType.DurationTime;
						c.MaxOffer = contractType.MaxOffer;
						c.MaxActive = contractType.MaxActive;
					}
				}
			}
		}

		//Contract options
		private void contractOptions(int id)
		{
			GUILayout.Space(12);
			GUILayout.BeginHorizontal();
				GUILayout.Label("Funds Reward: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				string percent = "";
				if (!contractModifierScenario.Instance.allowZero && contractType.RewardFund <= 0.009)
					percent = "0.1%";
				else
					percent = contractType.RewardFund.ToString("P0");
				GUILayout.Label(percent, cmSkins.configCenterLabel, GUILayout.Width(48));

				Rect r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldCValues = (float[])contractType.ContractValues.Clone();

				contractType.RewardFund = logSlider(ref cFRew, -1, 1, r, 2);

				eventCheck(contractType.RewardFund, 0, oldCValues, 0);

				drawSliderLabel(r, "0%", "1000%", "100%");
			GUILayout.EndHorizontal();

			GUILayout.Space(14);

			GUILayout.BeginHorizontal();
				GUILayout.Label("Funds Advance: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				if (!contractModifierScenario.Instance.allowZero && contractType.AdvanceFund <= 0.009)
					percent = "0.1%";
				else
					percent = contractType.AdvanceFund.ToString("P0");
				GUILayout.Label(percent, cmSkins.configCenterLabel, GUILayout.Width(48));

				r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldCValues = (float[])contractType.ContractValues.Clone();

				contractType.AdvanceFund = logSlider(ref cFAdv, -1, 1, r, 2);

				eventCheck(contractType.AdvanceFund, 1, oldCValues, 0);

				drawSliderLabel(r, "0%", "1000%", "100%");
			GUILayout.EndHorizontal();

			GUILayout.Space(14);

			GUILayout.BeginHorizontal();
				GUILayout.Label("Funds Penalty: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				if (!contractModifierScenario.Instance.allowZero && contractType.PenaltyFund <= 0.009)
					percent = "0.1%";
				else
					percent = contractType.PenaltyFund.ToString("P0");
				GUILayout.Label(percent, cmSkins.configCenterLabel, GUILayout.Width(48));

				r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldCValues = (float[])contractType.ContractValues.Clone();

				contractType.PenaltyFund = logSlider(ref cFPen, -1, 1, r, 2);

				eventCheck(contractType.PenaltyFund, 2, oldCValues, 0);

				drawSliderLabel(r, "0%", "1000%", "100%");
			GUILayout.EndHorizontal();

			GUILayout.Space(14);

			GUILayout.BeginHorizontal();
				GUILayout.Label("Rep Reward: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				if (!contractModifierScenario.Instance.allowZero && contractType.RewardRep <= 0.009)
					percent = "0.1%";
				else
					percent = contractType.RewardRep.ToString("P0");
				GUILayout.Label(percent, cmSkins.configCenterLabel, GUILayout.Width(48));

				r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldCValues = (float[])contractType.ContractValues.Clone();

				contractType.RewardRep = logSlider(ref cRRew, -1, 1, r, 2);

				eventCheck(contractType.RewardRep, 3, oldCValues, 0);

				drawSliderLabel(r, "0%", "1000%", "100%");
			GUILayout.EndHorizontal();

			GUILayout.Space(14);

			GUILayout.BeginHorizontal();
				GUILayout.Label("Rep Penalty: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				if (!contractModifierScenario.Instance.allowZero && contractType.PenaltyRep <= 0.009)
					percent = "0.1%";
				else
					percent = contractType.PenaltyRep.ToString("P0");
				GUILayout.Label(percent, cmSkins.configCenterLabel, GUILayout.Width(48));

				r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldCValues = (float[])contractType.ContractValues.Clone();

				contractType.PenaltyRep = logSlider(ref cRPen, -1, 1, r, 2);

				eventCheck(contractType.PenaltyRep, 4, oldCValues, 0);

				drawSliderLabel(r, "0%", "1000%", "100%");
			GUILayout.EndHorizontal();

			GUILayout.Space(14);

			GUILayout.BeginHorizontal();
				GUILayout.Label("Science Reward: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				if (!contractModifierScenario.Instance.allowZero && contractType.RewardScience <= 0.009)
					percent = "0.1%";
				else
					percent = contractType.RewardScience.ToString("P0");
				GUILayout.Label(percent, cmSkins.configCenterLabel, GUILayout.Width(48));

				r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldCValues = (float[])contractType.ContractValues.Clone();

				contractType.RewardScience = logSlider(ref cSRew, -1, 1, r, 2);

				eventCheck(contractType.RewardScience, 5, oldCValues, 0);

				drawSliderLabel(r, "0%", "1000%", "100%");
			GUILayout.EndHorizontal();

			GUILayout.Space(14);

			GUILayout.BeginHorizontal();
				GUILayout.Label("Duration: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				GUILayout.Label(contractType.DurationTime.ToString("P0"), cmSkins.configCenterLabel, GUILayout.Width(48));

				r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldCValues = (float[])contractType.ContractValues.Clone();

				contractType.DurationTime = logSlider(ref cDur, -0.9f, 1, r, 2);

				eventCheck(contractType.DurationTime, 6, oldCValues, 0);

				drawSliderLabel(r, "10%", "1000%", "100%");
			GUILayout.EndHorizontal();

			GUILayout.Space(14);

			GUILayout.BeginHorizontal();
				string offers = "";
				if (contractType.MaxOffer < 10)
					offers = (contractType.MaxOffer * 10).ToString("N0");
				else
					offers = ("∞");
				GUILayout.Label("Max Offered: ", cmSkins.configCenterLabel, GUILayout.Width(48));
				GUILayout.Space(-15);
				GUILayout.Label(offers, cmSkins.configCenterLabel, GUILayout.Width(30));

				r = GUILayoutUtility.GetLastRect();
				r.x += 35;
				r.width = 110;

				oldCValues = (float[])contractType.ContractValues.Clone();

				contractType.MaxOffer = logSlider(ref cOffer, -1, 1, r, 2);

				eventCheck(contractType.MaxOffer, 7, oldCValues, 0);

				drawSliderLabel(r, "0", "   ∞", "10");

				GUILayout.Space(115);

				string actives = "";
				if (contractType.MaxActive < 10)
					actives = (contractType.MaxActive * 10).ToString("N0");
				else
					actives = "∞";
				GUILayout.Label("Max Active: ", cmSkins.configCenterLabel, GUILayout.Width(47));
				GUILayout.Space(-15);
				GUILayout.Label(actives, cmSkins.configCenterLabel, GUILayout.Width(30));

				r = GUILayoutUtility.GetLastRect();
				r.x += 35;
				r.width = 110;

				oldCValues = (float[])contractType.ContractValues.Clone();

				contractType.MaxActive = logSlider(ref cActive, -1, 1, r, 2);

				eventCheck(contractType.MaxActive, 8, oldCValues, 0);

				drawSliderLabel(r, "0", "   ∞", "10");
			GUILayout.EndHorizontal();
		}

		//Parameter type selector
		private void parameterSelectionMenu(int id)
		{
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Parameter Type:", cmSkins.configDropDown, GUILayout.MaxWidth(140)))
				{
					dropDown = !dropDown;
					pDropDown = !pDropDown;
				}

				if (paramType != null)
					GUILayout.Label(paramType.Name, cmSkins.configHeader, GUILayout.MaxWidth(190));
				else
					GUILayout.Label("Unknown", cmSkins.configHeader, GUILayout.MaxWidth(190));
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			if (paramType.Generic)
			{
				GUILayout.Space(-4);
				if (GUILayout.Button("Apply To All Parameters", cmSkins.configButton))
				{
					foreach (paramTypeContainer p in pList)
					{
						p.RewardFund = paramType.RewardFund;
						p.PenaltyFund = paramType.PenaltyFund;
						p.RewardRep = paramType.RewardRep;
						p.PenaltyRep = paramType.PenaltyRep;
						p.RewardScience = paramType.RewardScience;
					}
				}
			}
		}

		//Parameter options
		private void parameterOptions(int id)
		{
			GUILayout.Space(12);
			GUILayout.BeginHorizontal();
				GUILayout.Label("Funds Reward: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				string percent = "";
				if (!contractModifierScenario.Instance.allowZero && paramType.RewardFund <= 0.009)
					percent = "0.1%";
				else
					percent = paramType.RewardFund.ToString("P0");
				GUILayout.Label(percent, cmSkins.configCenterLabel, GUILayout.Width(48));

				Rect r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldPValues = (float[])paramType.ParamValues.Clone();

				paramType.RewardFund = logSlider(ref pFRew, -1, 1, r, 2);

				eventCheck(paramType.RewardFund, 0, oldPValues, 1);

				drawSliderLabel(r, "0%", "1000%", "100%");
			GUILayout.EndHorizontal();

			GUILayout.Space(14);

			GUILayout.BeginHorizontal();
				GUILayout.Label("Funds Penalty: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				if (!contractModifierScenario.Instance.allowZero && paramType.PenaltyFund <= 0.009)
					percent = "0.1%";
				else
					percent = paramType.PenaltyFund.ToString("P0");
				GUILayout.Label(percent, cmSkins.configCenterLabel, GUILayout.Width(48));

				r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldPValues = (float[])paramType.ParamValues.Clone();

				paramType.PenaltyFund = logSlider(ref pFPen, -1, 1, r, 2);

				eventCheck(paramType.PenaltyFund, 1, oldPValues, 1);

				drawSliderLabel(r, "0%", "1000%", "100%");
			GUILayout.EndHorizontal();

			GUILayout.Space(14);

			GUILayout.BeginHorizontal();
				GUILayout.Label("Rep Reward: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				if (!contractModifierScenario.Instance.allowZero && paramType.RewardRep <= 0.009)
					percent = "0.1%";
				else
					percent = paramType.RewardRep.ToString("P0");
				GUILayout.Label(percent, cmSkins.configCenterLabel, GUILayout.Width(48));

				r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldPValues = (float[])paramType.ParamValues.Clone();

				paramType.RewardRep = logSlider(ref pRRew, -1, 1, r, 2);

				eventCheck(paramType.RewardRep, 2, oldPValues, 1);

				drawSliderLabel(r, "0%", "1000%", "100%");
			GUILayout.EndHorizontal();

			GUILayout.Space(14);

			GUILayout.BeginHorizontal();
				GUILayout.Label("Rep Penalty: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				if (!contractModifierScenario.Instance.allowZero && paramType.PenaltyRep <= 0.009)
					percent = "0.1%";
				else
					percent = paramType.PenaltyRep.ToString("P0");
				GUILayout.Label(percent, cmSkins.configCenterLabel, GUILayout.Width(48));

				r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldPValues = (float[])paramType.ParamValues.Clone();

				paramType.PenaltyRep = logSlider(ref pRPen, -1, 1, r, 2);

				eventCheck(paramType.PenaltyRep, 3, oldPValues, 1);

				drawSliderLabel(r, "0%", "1000%", "100%");
			GUILayout.EndHorizontal();

			GUILayout.Space(14);

			GUILayout.BeginHorizontal();
				GUILayout.Label("Science Reward: ", cmSkins.configLabel, GUILayout.Width(100));
				GUILayout.Space(-4);
				if (!contractModifierScenario.Instance.allowZero && paramType.RewardScience <= 0.009)
					percent = "0.1%";
				else
					percent = paramType.RewardScience.ToString("P0");
				GUILayout.Label(percent, cmSkins.configCenterLabel, GUILayout.Width(48));

				r = GUILayoutUtility.GetLastRect();
				r.x += 50;
				r.width = 200;

				oldPValues = (float[])paramType.ParamValues.Clone();

				paramType.RewardScience = logSlider(ref pSRew, -1, 1, r, 2);

				eventCheck(paramType.RewardScience, 4, oldPValues, 1);

				drawSliderLabel(r, "0%", "1000%", "100%");
			GUILayout.EndHorizontal();
		}

		//Draw all of the config option toggles and buttons
		private void windowConfig(int id)
		{
			GUILayout.Space(30);
			GUILayout.BeginHorizontal();
				GUILayout.Space(20);
				GUILayout.BeginVertical();
					contractModifierScenario.Instance.allowZero = GUILayout.Toggle(contractModifierScenario.Instance.allowZero, "Allow 0% Values", cmSkins.configToggle);

					contractModifierScenario.Instance.alterActive = GUILayout.Toggle(contractModifierScenario.Instance.alterActive, "Alter Active Contracts", cmSkins.configToggle);

					if (ToolbarManager.ToolbarAvailable)
						contractModifierScenario.Instance.stockToolbar = GUILayout.Toggle(contractModifierScenario.Instance.stockToolbar, "Use Stock Toolbar", cmSkins.configToggle);

					contractModifierScenario.Instance.CMNode.ShowToolbar = GUILayout.Toggle(!contractModifierScenario.Instance.CMNode.ShowToolbar, "Disable All Toolbars", cmSkins.configToggle);
				GUILayout.EndVertical();

				GUILayout.Space(20);

				GUILayout.BeginVertical();
					if (GUILayout.Button("Reset Contract Values", cmSkins.configButton))
					{
						dropDown = !dropDown;
						rCPopup = !rCPopup;
					}

					if (GUILayout.Button("Reset Parameter Values", cmSkins.configButton))
					{
						dropDown = !dropDown;
						rPPopup = !rPPopup;
					}

					if (GUILayout.Button("Save To Config", cmSkins.configButton))
					{
						dropDown = !dropDown;
						wPopup = !wPopup;
					}
				GUILayout.EndVertical();
				GUILayout.Space(20);
			GUILayout.EndHorizontal();
		}

		//Draw some line textures to break up the window into sections
		private void windowFrame(int id)
		{
			Rect r = new Rect(WindowRect.width - (WindowRect.width / 2) - 4, 265, (WindowRect.width / 2) + 2 , 4);
			GUI.DrawTexture(r, cmSkins.footerBar);

			r.x -= 2;
			r.y = WindowRect.height - (WindowRect.height - 64);
			r.width = 4;
			r.height = WindowRect.height - 60;
			GUI.DrawTexture(r, cmSkins.verticalBar);
		}

		//Handle all of the drop down menus and pop up windows here
		//Only 1 can be active at a time
		private void dropDownMenu(int id)
		{
			if (dropDown)
			{
				if (cDropDown)
				{
					ddRect = new Rect(40, 55, 280, 160);
					GUI.Box(ddRect, "", cmSkins.dropDown);

					for (int i = 0; i < cList.Count; i++)
					{
						cScroll = GUI.BeginScrollView(ddRect, cScroll, new Rect(0, 0, 260, 25 * cList.Count));
						Rect r = new Rect(2, (25 * i) + 2, 250, 25);
						if (GUI.Button(r, cList[i].Name, cmSkins.configDropMenu))
						{
							setContractType(cList[i], contractModifierScenario.Instance.allowZero);
							cDropDown = false;
							dropDown = false;
						}
					GUI.EndScrollView();
					}
				}

				else if (pDropDown)
				{
					ddRect = new Rect(WindowRect.width - 365, 55, 280, 160);
					GUI.Box(ddRect, "", cmSkins.dropDown);

					for (int i = 0; i < pList.Count; i++)
					{
						pScroll = GUI.BeginScrollView(ddRect, pScroll, new Rect(0, 0, 260, 25 * pList.Count));
						Rect r = new Rect(2, (25 * i) + 2, 250, 25);
						if (GUI.Button(r, pList[i].Name, cmSkins.configDropMenu))
						{
							setParameterType(pList[i], contractModifierScenario.Instance.allowZero);
							pDropDown = false;
							dropDown = false;
						}
						GUI.EndScrollView();
					}
				}

				else if (zPopup)
				{
					ddRect = new Rect(WindowRect.width - 260, WindowRect.height - 130, 240, 130);
					GUI.Box(ddRect, "", cmSkins.dropDown);
					Rect r = new Rect(ddRect.x + 10, ddRect.y + 5, 220, 80);
					GUI.Label(r, "Warning:\nContract values set to 0.0% may no longer be adjustable", cmSkins.resetBox);

					r.x += 20;
					r.y += 60;
					r.height = 30;
					contractModifierScenario.Instance.CMNode.WarnedZero = GUI.Toggle(r, contractModifierScenario.Instance.CMNode.WarnedZero, "Do not show this warning", cmSkins.configToggle);

					r.x += 65;
					r.y += 30;
					r.width = 70;
					if (GUI.Button(r, "Confirm", cmSkins.resetButton))
					{
						dropDown = false;
						zPopup = false;
						contractModifierScenario.Instance.allowZero = true;
						allowZero = true;
					}
				}

				else if (rCPopup)
				{
					ddRect = new Rect(WindowRect.width - 300, WindowRect.height - 100, 280, 100);
					GUI.Box(ddRect, "", cmSkins.dropDown);
					Rect r = new Rect(ddRect.x + 5, ddRect.y + 5, 270, 70);
					GUI.Label(r, "Contract Type:\n<b>" + contractType.Name + "</b>\nWill Be Reset To Default Values", cmSkins.resetBox);
					r.x += 110;
					r.y += 60;
					r.width = 70;
					r.height = 30;
					if (GUI.Button(r, "Confirm", cmSkins.resetButton))
					{
						dropDown = false;
						rCPopup = false;
						resetContractToDefault();
					}
				}

				else if (rPPopup)
				{
					ddRect = new Rect(WindowRect.width - 300, WindowRect.height - 100, 280, 100);
					GUI.Box(ddRect, "", cmSkins.dropDown);
					Rect r = new Rect(ddRect.x + 5, ddRect.y + 5, 270, 80);
					GUI.Label(r, "Parameter Type:\n<b>" + paramType.Name + "</b>\nWill Be Reset To Default Values", cmSkins.resetBox);
					r.x += 110;
					r.y += 60;
					r.width = 70;
					r.height = 30;
					if (GUI.Button(r, "Confirm", cmSkins.resetButton))
					{
						dropDown = false;
						rCPopup = false;
						resetParameToDefault();
					}
				}

				else if (wPopup)
				{
					ddRect = new Rect(WindowRect.width - 260, WindowRect.height - 80, 240, 80);
					GUI.Box(ddRect, "", cmSkins.dropDown);
					Rect r = new Rect(ddRect.x + 10, ddRect.y + 5, 220, 45);
					GUI.Label(r, "Overwrite Default Config File With Current Values?", cmSkins.resetBox);
					r.x += 85;
					r.y += 40;
					r.width = 70;
					r.height = 30;
					if (GUI.Button(r, "Confirm", cmSkins.resetButton))
					{
						dropDown = false;
						wPopup = false;
						contractModifierScenario.Instance.CMNode.Save();
					}
				}

				else if (activePopup)
				{
					ddRect = new Rect(WindowRect.width - 300, WindowRect.height - 130, 280, 130);
					GUI.Box(ddRect, "", cmSkins.dropDown);
					Rect r = new Rect(ddRect.x + 5, ddRect.y + 5, 270, 60);
					GUI.Label(r, "Contract Duration Can Only Be Adjusted For Newly Offered Contracts", cmSkins.resetBox);

					r.x += 20;
					r.y += 60;
					r.height = 30;
					contractModifierScenario.Instance.CMNode.WarnedAlterActive = GUI.Toggle(r, contractModifierScenario.Instance.CMNode.WarnedAlterActive, "Do not show this warning", cmSkins.configToggle);

					r.x += 100;
					r.y += 30;
					r.width = 50;
					if (GUI.Button(r, "Confirm", cmSkins.resetButton))
					{
						dropDown = false;
						activePopup = false;
						contractModifierScenario.Instance.alterActive = true;
						alterActive = true;
					}
				}

				else if (toolbarPopup)
				{
					ddRect = new Rect(WindowRect.width - 300, WindowRect.height - 130, 280, 130);
					GUI.Box(ddRect, "", cmSkins.dropDown);
					Rect r = new Rect(ddRect.x + 5, ddRect.y + 5, 270, 60);
					GUI.Label(r, "Toolbar icon can only be reactived from the config file", cmSkins.resetBox);

					r.x += 20;
					r.y += 60;
					r.height = 30;
					contractModifierScenario.Instance.CMNode.WarnedToolbar = GUI.Toggle(r, contractModifierScenario.Instance.CMNode.WarnedToolbar, "Do not show this warning", cmSkins.configToggle);

					r.x += 100;
					r.y += 30;
					r.width = 50;
					if (GUI.Button(r, "Confirm", cmSkins.resetButton))
					{
						dropDown = false;
						toolbarPopup = false;
						contractModifierScenario.Instance.CMNode.ShowToolbar = false;
						showToolbar = false;
					}
				}

				else
					dropDown = false;
			}
		}

		//Label for horizontal sliders
		private void drawSliderLabel(Rect r, string min, string max, string mid = null)
		{
			Rect sr = new Rect(r.x, r.y + 9, 10, 20);
			drawLabel(sr, "|", true, true);
			if (!string.IsNullOrEmpty(mid))
			{
				sr.x += (r.width / 2);
				drawLabel(sr, "|", true, false);
				sr.x += ((r.width / 2) - 1);
			}
			else
				sr.x += (r.width - 7);
			drawLabel(sr, "|", true, false);
			sr.width = 40;
			sr.x -= r.width;
			sr.y += 11;
			drawLabel(sr, min, true, false);
			if (!string.IsNullOrEmpty(mid))
			{
				sr.x += (r.width / 2);
				drawLabel(sr, mid, true, false);
				sr.x += ((r.width / 2) - 10f);
			}
			else
				sr.x += (r.width + 62);
			drawLabel(sr, max, true, true);
		}

		//Label method for small font size
		private void drawLabel(Rect r, string txt, bool aligned, bool left)
		{
			if (txt.Length < 1)
				return;
			if (aligned)
			{
				Vector2 sz = cmSkins.smallLabel.CalcSize(new GUIContent(txt.Substring(0, 1)));
				r.x -= sz.x / 2;
				r.y -= sz.y / 2;
			}
			if (left)
				GUI.Label(r, txt, cmSkins.smallLabel);
			else
				GUI.Label(r, txt, cmSkins.smallLabel);
		}

		//Semi log scale slider for percentage adjustments
		private float logSlider (ref float f, float min, float max, Rect r, int round)
		{
			float newVal = f;
			if (!dropDown)
				f = GUI.HorizontalSlider(r, f, min, max).Mathf_Round(round);
			else
				GUI.HorizontalSlider(r, f, min, max);

			if (f >= -1 && f < -0.05)
				newVal = f + 1;
			else if (f >= -0.05 && f < 0.05)
			{
				f = 0;
				newVal = 1;
			}
			else if (f >= 0.05 && f < 1)
				newVal = (float)Math.Pow(10, f);
			else
				newVal = 10f;

			return newVal;
		}

		//Check here to see if any values have changed and update contracts accordingly
		//Only active when updating active contracts is allowed
		//the float[] is a Clone of the original because arrays are reference objects
		private void eventCheck(float newF, int pos, float[] originals, int type)
		{
			if (contractModifierScenario.Instance.alterActive)
			{
				if (Mathf.RoundToInt(originals[pos] * 100) != Mathf.RoundToInt(newF * 100))
				{
					if (type == 0)
					{
						contractModifierScenario.onContractChange.Fire(originals, contractType);
					}
					else if (type == 1)
					{
						contractModifierScenario.onParamChange.Fire(originals, paramType);
					}
				}
			}
		}

		//Reset all of the slider values for the newly selected contract type
		private void setContractType(contractTypeContainer c, bool allowZero)
		{
			contractType = c;
			cFRew = c.RewardFund.reverseLog(allowZero);
			cFAdv = c.AdvanceFund.reverseLog(allowZero);
			cFPen = c.PenaltyFund.reverseLog(allowZero);
			cRRew = c.RewardRep.reverseLog(allowZero);
			cRPen = c.PenaltyRep.reverseLog(allowZero);
			cSRew = c.RewardScience.reverseLog(allowZero);
			cDur = c.DurationTime.reverseLog(allowZero);
			cOffer = c.MaxOffer.reverseLog(allowZero);
			cActive = c.MaxActive.reverseLog(allowZero);
		}

		//Reset all of the slider values for the newly selected parameter type
		private void setParameterType(paramTypeContainer p, bool allowZero)
		{
			paramType = p;
			pFRew = p.RewardFund.reverseLog(allowZero);
			pFPen = p.PenaltyFund.reverseLog(allowZero);
			pRRew = p.RewardRep.reverseLog(allowZero);
			pRPen = p.PenaltyRep.reverseLog(allowZero);
			pSRew = p.RewardScience.reverseLog(allowZero);
		}

		//Reset the current contract type to its default values
		//Values always default to 100% for now; use config file later
		//Active contracts updated only if allowed
		private void resetContractToDefault()
		{
			float[] originals = (float[])contractType.ContractValues.Clone();
			contractType.RewardFund = contractType.DefaultFundReward;
			contractType.AdvanceFund = contractType.DefaultFundAdvance;
			contractType.PenaltyFund = contractType.DefaultFundPenalty;
			contractType.RewardRep = contractType.DefaultRepReward;
			contractType.PenaltyRep = contractType.DefaultRepPenalty;
			contractType.RewardScience = contractType.DefaultScienceReward;
			contractType.DurationTime = contractType.DefaultDuration;
			contractType.MaxOffer = contractType.DefaultMaxOffer;
			contractType.MaxActive = contractType.DefaultMaxActive;

			setContractType(contractType, contractModifierScenario.Instance.allowZero);
			if (contractModifierScenario.Instance.alterActive)
				contractModifierScenario.onContractChange.Fire(originals, contractType);
		}

		//Reset the current parameter type to its default values
		private void resetParameToDefault()
		{
			float[] originals = (float[])paramType.ParamValues.Clone();
			paramType.RewardFund = paramType.DefaultFundReward;
			paramType.PenaltyFund = paramType.DefaultFundPenalty;
			paramType.RewardRep = paramType.DefaultRepReward;
			paramType.PenaltyRep = paramType.DefaultRepPenalty;
			paramType.RewardScience = paramType.DefaultScienceReward;

			setParameterType(paramType, contractModifierScenario.Instance.allowZero);
			if (contractModifierScenario.Instance.alterActive)
				contractModifierScenario.onParamChange.Fire(originals, paramType);
		}
	}
}
