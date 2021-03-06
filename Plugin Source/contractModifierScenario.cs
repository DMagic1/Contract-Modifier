﻿#region license
/*The MIT License (MIT)
Contract Modifier Scenario : A scenario module to store data for the addon and control save/load

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
using ContractModifier.Toolbar;
using ContractModifier.Framework;
using UnityEngine;

namespace ContractModifier
{
	[KSPScenario(ScenarioCreationOptions.AddToExistingCareerGames | ScenarioCreationOptions.AddToNewCareerGames, GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.TRACKSTATION, GameScenes.SPACECENTER)]
	public class contractModifierScenario : ScenarioModule
	{
		private static contractModifierScenario instance;

		public static contractModifierScenario Instance
		{
			get { return instance; }
		}

		public bool allowZero = false;
		public bool alterActive = false;
		public bool stockToolbar = true;
		public bool disableToolbar = false;
		public bool warnedZero = false;
		public bool warnedToolbar = false;
		public bool warnedAlterActive = false;
		private bool disableSaveLoading = false;

		internal cmStockToolbar appLauncherButton;
		internal cmToolbar blizzyToolbarButton;
		internal contractConfig configWindow;

		public static EventData<float[], contractTypeContainer> onContractChange;
		public static EventData<float[], paramTypeContainer> onParamChange;

		private ContractValuesNode cmNode;

		public ContractValuesNode CMNode
		{
			get { return cmNode; }
		}

		public override void OnAwake()
		{
			instance = this;
		}

		private void Start()
		{
			if (onContractChange == null)
				onContractChange = new EventData<float[], contractTypeContainer>("onContractChange");
			if (onParamChange == null)
				onParamChange = new EventData<float[], paramTypeContainer>("onParamChange");
			onContractChange.Add(contractChanged);
			onParamChange.Add(paramChanged);
			GameEvents.Contract.onAccepted.Add(contractAccepted);
			GameEvents.Contract.onOffered.Add(contractOffered);
			GameEvents.Contract.onContractsListChanged.Add(contractListUpdate);

			if (!disableToolbar)
			{
				if (stockToolbar || !ToolbarManager.ToolbarAvailable)
				{
					appLauncherButton = gameObject.AddComponent<cmStockToolbar>();
					if (blizzyToolbarButton != null)
						Destroy(blizzyToolbarButton);
				}
				else if (ToolbarManager.ToolbarAvailable && !stockToolbar)
				{
					blizzyToolbarButton = gameObject.AddComponent<cmToolbar>();
					if (appLauncherButton != null)
						Destroy(appLauncherButton);
				}
			}
		}

		private void OnDestroy()
		{
			if (configWindow != null)
				Destroy(configWindow);
			if (appLauncherButton != null)
				Destroy(appLauncherButton);
			if (blizzyToolbarButton != null)
				Destroy(blizzyToolbarButton);

			onContractChange.Remove(contractChanged);
			onParamChange.Remove(paramChanged);
			GameEvents.Contract.onAccepted.Remove(contractAccepted);
			GameEvents.Contract.onOffered.Remove(contractOffered);
			GameEvents.Contract.onContractsListChanged.Remove(contractListUpdate);
		}

		#region Save/Load

		public override void OnLoad(ConfigNode node)
		{
			cmNode = cmConfigLoad.TopNode;
			if (cmNode == null)
				cmNode = new ContractValuesNode(cmConfigLoad.fileName);

			if (cmNode != null)
			{
				disableSaveLoading = cmNode.DisableSaveSpecificValues;
				allowZero = cmNode.AllowZero;
				alterActive = cmNode.AlterActive;
				stockToolbar = cmNode.StockToolbar;
				disableToolbar = cmNode.DisableToolbar;
				warnedAlterActive = cmNode.WarnedAlterActive;
				warnedToolbar = cmNode.WarnedToolbar;
				warnedZero = cmNode.WarnedZero;
			}

			if (!disableSaveLoading)
			{
				bool.TryParse(node.GetValue("allowZero"), out allowZero);
				bool.TryParse(node.GetValue("alterActive"), out alterActive);
				if (!bool.TryParse(node.GetValue("stockToolbar"), out stockToolbar))
					stockToolbar = true;
				bool.TryParse(node.GetValue("warnedAlterActive"), out warnedAlterActive);
				bool.TryParse(node.GetValue("warnedToolbar"), out warnedToolbar);
				bool.TryParse(node.GetValue("warnedZero"), out warnedZero);

				try
				{
					ConfigNode contractTypes = node.GetNode("Contract_Types");

					if (contractTypes != null)
					{
						foreach (ConfigNode contractType in contractTypes.GetNodes("Contract_Type"))
						{
							if (contractType != null)
							{
								string contractTypeName = contractType.GetValue("TypeName");
								string valuesString = contractType.GetValue("ContractValues");
								stringContractParse(valuesString, contractTypeName);
							}
						}
					}
				}
				catch (Exception e)
				{
					DMCM_MBE.LogFormatted("Contract Type List Cannot Be Generated Or Loaded: {0}", e);
				}

				try
				{
					ConfigNode paramTypes = node.GetNode("Parameter_Types");

					if (paramTypes != null)
					{
						foreach (ConfigNode paramType in paramTypes.GetNodes("Parameter_Type"))
						{
							if (paramType != null)
							{
								string paramTypeName = paramType.GetValue("TypeName");
								string valuesString = paramType.GetValue("ParameterValues");
								stringParamParse(valuesString, paramTypeName);
							}
						}
					}
				}
				catch (Exception e)
				{
					DMCM_MBE.LogFormatted("Parameter Type List Cannot Be Generated Or Loaded: {0}", e);
				}
			}
			else
				DMCM_MBE.LogFormatted("All save-specific settings disabled; values loaded from config file...");

			//Start the window object
			if (!disableToolbar)
			{
				try
				{
					configWindow = gameObject.AddComponent<contractConfig>();
				}
				catch (Exception e)
				{
					DMCM_MBE.LogFormatted("Contract Modifier Windows Cannot Be Started: {0}", e);
				}
			}

			if (ContractValuesNode.getCType("GlobalSettings") == null)
				cmNode.addToContractList(new contractTypeContainer("GlobalSettings", false));

			if (ContractValuesNode.getPType("GlobalSettings") == null)
				cmNode.addToParamList(new paramTypeContainer("GlobalSettings"));
		}

		public override void OnSave(ConfigNode node)
		{
			if (!disableSaveLoading)
			{
				node.AddValue("allowZero", allowZero);
				node.AddValue("alterActive", alterActive);
				node.AddValue("stockToolbar", stockToolbar);
				node.AddValue("warnedAlterActive", warnedAlterActive);
				node.AddValue("warnedToolbar", warnedToolbar);
				node.AddValue("warnedZero", warnedZero);

				try
				{
					ConfigNode contractTypes = new ConfigNode("Contract_Types");

					for (int i = 0; i < ContractValuesNode.ContractTypeCount; i++)
					{
						contractTypeContainer c = ContractValuesNode.getCType(i);
						if (c != null)
						{
							ConfigNode contractType = new ConfigNode("Contract_Type");

							contractType.AddValue("TypeName", c.TypeName);
							contractType.AddValue("ContractValues", stringConcat(c));

							contractTypes.AddNode(contractType);
						}
					}

					node.AddNode(contractTypes);
				}
				catch (Exception e)
				{
					DMCM_MBE.LogFormatted("Contract Types Cannot Be Saved: {0}", e);
				}

				//Save values for each parameter type
				try
				{
					ConfigNode paramTypes = new ConfigNode("Parameter_Types");

					for (int i = 0; i < ContractValuesNode.ParameterTypeCount; i++)
					{
						paramTypeContainer p = ContractValuesNode.getPType(i);

						if (p != null)
						{
							ConfigNode paramType = new ConfigNode("Parameter_Type");

							paramType.AddValue("TypeName", p.TypeName);
							paramType.AddValue("ParameterValues", stringConcat(p));

							paramTypes.AddNode(paramType);
						}
					}

					node.AddNode(paramTypes);
				}
				catch (Exception e)
				{
					DMCM_MBE.LogFormatted("Parameter Type List Cannot Be Saved: {0}", e);
				}
			}
		}

		#endregion

		#region Save/Load Utilities

		private string stringConcat(contractTypeContainer c)
		{
			string[] s = new string[9];
			s[0] = c.RewardFund.ToString("N3");
			s[1] = c.AdvanceFund.ToString("N3");
			s[2] = c.PenaltyFund.ToString("N3");
			s[3] = c.RewardRep.ToString("N3");
			s[4] = c.PenaltyRep.ToString("N3");
			s[5] = c.RewardScience.ToString("N3");
			s[6] = c.DurationTime.ToString("N3");
			s[7] = c.MaxOffer.ToString("N1");
			s[8] = c.MaxActive.ToString("N1");
			return string.Join(",", s);
		}

		private string stringConcat(paramTypeContainer p)
		{
			string[] s = new string[5];
			s[0] = p.RewardFund.ToString("N3");
			s[1] = p.PenaltyFund.ToString("N3");
			s[2] = p.RewardRep.ToString("N3");
			s[3] = p.PenaltyRep.ToString("N3");
			s[4] = p.RewardScience.ToString("N3");
			return string.Join(",", s);
		}

		private void stringContractParse(string s, string type)
		{
			if (string.IsNullOrEmpty(s))
				return;

			contractTypeContainer c = ContractValuesNode.getCType(type, false);

			if (c == null)
			{
				DMCM_MBE.LogFormatted("Contract Type Not Found; Removing Type From List");
				return;
			}

			string[] a = s.Split(',');

			try
			{
				c.RewardFund = stringFloatParse(a[0], true);
				c.AdvanceFund = stringFloatParse(a[1], true);
				c.PenaltyFund = stringFloatParse(a[2], true);
				c.RewardRep = stringFloatParse(a[3], true);
				c.PenaltyRep = stringFloatParse(a[4], true);
				c.RewardScience = stringFloatParse(a[5], true);
				c.DurationTime = stringFloatParse(a[6], true);
				c.MaxOffer = stringFloatParse(a[7], false);
				c.MaxActive = stringFloatParse(a[8], false);
			}
			catch (Exception e)
			{
				DMCM_MBE.LogFormatted("Error While Loading Contract Values...\n{0}", e);
			}
		}

		private void stringParamParse(string s, string type)
		{
			if (string.IsNullOrEmpty(s))
				return;

			paramTypeContainer p = ContractValuesNode.getPType(type, false);

			if (p == null)
			{
				DMCM_MBE.LogFormatted("Parameter Type Not Found; Removing Type From List");
				return;
			}
			string[] a = s.Split(',');

			try
			{
				p.RewardFund = stringFloatParse(a[0], true);
				p.PenaltyFund = stringFloatParse(a[1], true);
				p.RewardRep = stringFloatParse(a[2], true);
				p.PenaltyRep = stringFloatParse(a[3], true);
				p.RewardScience = stringFloatParse(a[4], true);
			}
			catch (Exception e)
			{
				DMCM_MBE.LogFormatted("Error While Loading Parameter Values...\n{0}", e);
			}
		}

		private float stringFloatParse(string s, bool b)
		{
			float f;
			if (float.TryParse(s, out f)) return f;
			if (b)
				return 1;
			else
				return 10;
		}

		#endregion

		#region contract Events

		private List<Contract> offeredContractList = new List<Contract>();

		private void contractListUpdate()
		{
			foreach (Contract c in offeredContractList)
			{
				Type contractT = c.GetType();
				contractTypeContainer cC = null;

				if (contractT.Name == "ConfiguredContract")
				{
					if (cmAssemblyLoad.ContractConfiguratorCCLoaded)
					{
						string name = cmAssemblyLoad.CCTypeName(c);
						cC = ContractValuesNode.getCType(name);
					}
					else
					{
						DMCM_MBE.LogFormatted("Contract Configurator Contract Type Detected, But Type Name Can't Be Determined; CC Possibly Out Of Date...");
						continue;
					}
				}
				else
				{
					cC = ContractValuesNode.getCType(contractT.Name);
				}

				if (cC == null)
					continue;

				if (cC.ContractType == null)
					continue;

				if (cC.MaxActive < 10f || cC.MaxOffer < 10f)
				{
					var cList = ContractSystem.Instance.Contracts;
					int active = 0;
					int offered = 0;
					for (int i = 0; i < cList.Count; i++)
					{
						if (cList[i].GetType().Name == "ConfiguredContract")
						{
							if (!cC.CConfigType)
								continue;
							if (cmAssemblyLoad.ContractConfiguratorCCLoaded)
							{
								string name = cmAssemblyLoad.CCTypeName(cList[i]);
								if (cC.TypeName == name)
								{
									if (cList[i].ContractState == Contract.State.Active)
										active++;
									else if (cList[i].ContractState == Contract.State.Offered)
										offered++;
								}
							}
						}
						else
						{
							if (cList[i].GetType() == contractT)
							{
								if (cList[i].ContractState == Contract.State.Active)
									active++;
								else if (cList[i].ContractState == Contract.State.Offered)
									offered++;
							}
						}
					}
					int remainingSlots = (int)(cC.MaxActive * 10) - active;
					if ((offered - 1) >= (int)(cC.MaxOffer * 10) && cC.MaxOffer < 10f)
					{
						c.Unregister();
						float repLoss = HighLogic.CurrentGame.Parameters.Career.RepLossDeclined;
						HighLogic.CurrentGame.Parameters.Career.RepLossDeclined = 0f;
						c.IgnoresWeight = true;
						c.Decline();
						HighLogic.CurrentGame.Parameters.Career.RepLossDeclined = repLoss;
						ContractSystem.Instance.Contracts.Remove(c);
						DMCM_MBE.LogFormatted("Contract Type [{0}] Over The Offer Limit, Blocking Offer", contractT.Name);
					}
					else if ((offered - 1) >= remainingSlots && cC.MaxActive < 10f)
					{
						c.Unregister();
						float repLoss = HighLogic.CurrentGame.Parameters.Career.RepLossDeclined;
						HighLogic.CurrentGame.Parameters.Career.RepLossDeclined = 0f;
						c.IgnoresWeight = true;
						c.Decline();
						HighLogic.CurrentGame.Parameters.Career.RepLossDeclined = repLoss;
						ContractSystem.Instance.Contracts.Remove(c);
						DMCM_MBE.LogFormatted("Contract Type [{0}] Over The Active Limit, Blocking Offer", contractT.Name);
					}
					else
					{
						updateContractValues(cC, c, new float[9] { 1, 1, 1, 1, 1, 1, 1, 1, 1 });
						updateParameterValues(c);
					}
				}
				else
				{
					updateContractValues(cC, c, new float[9] { 1, 1, 1, 1, 1, 1, 1, 1, 1 });
					updateParameterValues(c);
				}
			}

			if (offeredContractList.Count > 0)
			{
				offeredContractList.Clear();
				GameEvents.Contract.onContractsListChanged.Fire();
			}
		}

		private void contractAccepted(Contract c)
		{
			Type contractT = c.GetType();

			if (contractT.Name == "ConfiguredContract")
			{
				if (cmAssemblyLoad.ContractConfiguratorCCLoaded)
				{
					string name = cmAssemblyLoad.CCTypeName(c);
					contractTypeContainer cC = ContractValuesNode.getCType(name);

					if (cC == null)
						return;

					if (cC.ContractType == null)
						return;

					updateContractValues(cC, c, new float[9] { 1, 1, 1, 1, 1, 1, 1, 1, 1 });
					updateParameterValues(c);
				}
				else
				{
					DMCM_MBE.LogFormatted("Contract Configurator Contract Type Detected, But Type Name Can't Be Determined; CC Possibly Out Of Date...");
					return;
				}
			}
		}

		private void contractOffered(Contract c)
		{
			if (!ContractSystem.Instance.Contracts.Contains(c))
				offeredContractList.Add(c);
		}

		private void paramChanged(float[] originals, paramTypeContainer p)
		{
			if (p.ParamType == null)
				return;

			var cList = ContractSystem.Instance.Contracts;
			for (int i = 0; i < cList.Count; i++)
			{
				if (cList[i].ContractState == Contract.State.Active || cList[i].ContractState == Contract.State.Offered)
					updateParameterValues(p, cList[i], originals);
			}

			if (cmAssemblyLoad.ContractsWindowPlusParameterLoaded && !p.CConfigType)
				cmAssemblyLoad.UpdateParameterValues(p.ParamType);
		}

		private void contractChanged(float[] originals, contractTypeContainer c)
		{
			if (c.ContractType == null)
				return;

			var cList = ContractSystem.Instance.Contracts;
			for (int i = 0; i < cList.Count; i++)
			{
				if (cList[i].ContractState == Contract.State.Active || cList[i].ContractState == Contract.State.Offered)
					updateContractValues(c, cList[i], originals);
			}

			if (cmAssemblyLoad.ContractsWindowPlusContractLoaded && !c.CConfigType)
				cmAssemblyLoad.UpdateContractValues(c.ContractType);
		}

		private void updateContractValues(contractTypeContainer cC, Contract c, float[] O)
		{
			if (cC.CConfigType)
			{
				if (cmAssemblyLoad.ContractConfiguratorCCLoaded)
				{
					string name = cmAssemblyLoad.CCTypeName(c);
					if (cC.TypeName == name)
					{
						c.FundsCompletion = (c.FundsCompletion / O[0]) * cC.RewardFund;
						c.FundsAdvance = (c.FundsAdvance / O[1]) * cC.AdvanceFund;
						c.FundsFailure = (c.FundsFailure / O[2]) * cC.PenaltyFund;
						c.ReputationCompletion = (c.ReputationCompletion / O[3]) * cC.RewardRep;
						c.ReputationFailure = (c.ReputationFailure / O[4]) * cC.PenaltyRep;
						c.ScienceCompletion = (c.ScienceCompletion / O[5]) * cC.RewardScience;
						c.TimeDeadline = (c.TimeDeadline / O[6]) * cC.DurationTime;
					}
				}
			}
			else
			{
				if (c.GetType() == cC.ContractType)
				{
					c.FundsCompletion = (c.FundsCompletion / O[0]) * cC.RewardFund;
					c.FundsAdvance = (c.FundsAdvance / O[1]) * cC.AdvanceFund;
					c.FundsFailure = (c.FundsFailure / O[2]) * cC.PenaltyFund;
					c.ReputationCompletion = (c.ReputationCompletion / O[3]) * cC.RewardRep;
					c.ReputationFailure = (c.ReputationFailure / O[4]) * cC.PenaltyRep;
					c.ScienceCompletion = (c.ScienceCompletion / O[5]) * cC.RewardScience;
					c.TimeDeadline = (c.TimeDeadline / O[6]) * cC.DurationTime;
				}
			}
		}

		private void updateParameterValues(paramTypeContainer pC, List<ContractParameter> pL, float[] O)
		{
			foreach (ContractParameter p in pL)
			{
				if (p.GetType() == pC.ParamType)
				{
					p.FundsCompletion = (p.FundsCompletion / O[0]) * pC.RewardFund;
					p.FundsFailure = (p.FundsFailure / O[1]) * pC.PenaltyFund;
					p.ReputationCompletion = (p.ReputationCompletion / O[2]) * pC.RewardRep;
					p.ReputationFailure = (p.ReputationFailure / O[3]) * pC.PenaltyRep;
					p.ScienceCompletion = (p.ScienceCompletion / O[4]) * pC.RewardScience;
				}
			}
		}

		private void updateParameterValues(paramTypeContainer pC, Contract c, float[] originals)
		{
			List<ContractParameter> modifyList = new List<ContractParameter>();
			var cParams = c.AllParameters;
			for (int i = 0; i < cParams.Count(); i++)
			{
				if (cParams.ElementAt(i).GetType() == pC.ParamType)
					modifyList.Add(cParams.ElementAt(i));
			}
			if (modifyList.Count > 0)
				updateParameterValues(pC, modifyList, originals);
		}

		private void updateParameterValues(Contract c)
		{
			if (ContractSystem.Instance.Contracts.Contains(c))
			{
				var cParams = c.AllParameters;
				for (int i = 0; i < cParams.Count(); i++)
				{
					string name = cParams.ElementAt(i).GetType().Name;
					paramTypeContainer p = ContractValuesNode.getPType(name);
					if (p != null)
					{
						updateParameterValues(p, new List<ContractParameter>() { cParams.ElementAt(i) }, new float[5] { 1, 1, 1, 1, 1 });
					}
				}
			}
		}

		#endregion

		#region Internal Methods

		public List<paramTypeContainer> setParamTypes(List<paramTypeContainer> pList)
		{
			pList = new List<paramTypeContainer>();
			List<paramTypeContainer> sortList = new List<paramTypeContainer>();
			List<paramTypeContainer> pConfiguratorList = new List<paramTypeContainer>();
			for (int i = 0; i < ContractValuesNode.ParameterTypeCount; i++)
			{
				paramTypeContainer p = ContractValuesNode.getPType(i);
				if (p != null)
				{
					if (p.Generic && pList.Count == 0)
						pList.Add(p);
					else if (p.CConfigType)
						pConfiguratorList.Add(p);
					else
						sortList.Add(p);
				}
			}

			if (sortList.Count > 0)
			{
				sortList.Sort((a, b) => string.Compare(a.Name, b.Name));
				pList.AddRange(sortList);
			}

			if (pConfiguratorList.Count > 0)
			{
				pConfiguratorList.Sort((a, b) => string.Compare(a.Name, b.Name));
				pList.AddRange(pConfiguratorList);
			}

			return pList;
		}

		public List<contractTypeContainer> setContractTypes(List<contractTypeContainer> cList)
		{
			cList = new List<contractTypeContainer>();
			List<contractTypeContainer> sortList = new List<contractTypeContainer>();
			List<contractTypeContainer> cConfiguratorList = new List<contractTypeContainer>();
			for (int i = 0; i < ContractValuesNode.ContractTypeCount; i++)
			{
				contractTypeContainer c = ContractValuesNode.getCType(i);
				if (c != null)
				{
					if (c.Generic && cList.Count == 0)
						cList.Add(c);
					else if (c.CConfigType)
						cConfiguratorList.Add(c);
					else
						sortList.Add(c);
				}
			}

			if (sortList.Count > 0)
			{
				sortList.Sort((a, b) => string.Compare(a.Name, b.Name));
				cList.AddRange(sortList);
			}

			if (cConfiguratorList.Count > 0)
			{
				cConfiguratorList.Sort((a, b) => string.Compare(a.Name, b.Name));
				cList.AddRange(cConfiguratorList);
			}

			return cList;
		}

		#endregion

	}
}
