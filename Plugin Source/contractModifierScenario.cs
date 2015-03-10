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
	public class contractModifierScenario : ScenarioModule
	{

		public static contractModifierScenario Instance
		{
			get
			{
				Game g = HighLogic.CurrentGame;
				try
				{
					var mod = g.scenarios.FirstOrDefault(m => m.moduleName == typeof(contractModifierScenario).Name);
					if (mod != null)
						return (contractModifierScenario)mod.moduleRef;
					else
						return null;
				}
				catch (Exception e)
				{
					DMCM_MBE.LogFormatted("[Contracts Modifier] Could not find Contracts Modifier Scenario Module: {0}", e);
					return null;
				}
			}
			private set { }
		}

		[KSPField(isPersistant = true)]
		public string version = "1.0";

		public bool allowZero;
		public bool alterActive;
		public bool stockToolbar;

		internal cmStockToolbar appLauncherButton;
		internal cmToolbar blizzyToolbarButton;
		internal contractConfig configWindow;

		public static EventData<float[], contractTypeContainer> onContractChange;
		public static EventData<float[], paramTypeContainer> onParamChange;

		private contractModifierNode cmNode;

		public contractModifierNode CMNode
		{
			get { return cmNode; }
		}

		private void Start()
		{
			if (onContractChange == null)
				onContractChange = new EventData<float[], contractTypeContainer>("onContractChange");
			if (onParamChange == null)
				onParamChange = new EventData<float[], paramTypeContainer>("onParamChange");
			onContractChange.Add(contractChanged);
			onParamChange.Add(paramChanged);
			GameEvents.Contract.onOffered.Add(contractOffered);

			if (cmNode.ShowToolbar)
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

		#region Save/Load

		public override void OnLoad(ConfigNode node)
		{
			cmNode = cmConfigLoad.TopNode;
			if (cmNode == null)
				cmNode = new contractModifierNode(cmConfigLoad.fileName);

			allowZero = cmNode.AllowZero;
			alterActive = cmNode.AlterActive;
			stockToolbar = cmNode.StockToolbar;

			bool.TryParse(node.GetValue("allowZero"), out allowZero);
			bool.TryParse(node.GetValue("alterActive"), out alterActive);
			bool.TryParse(node.GetValue("stockToolbar"), out stockToolbar);

			try
			{
				//Load the contract and parameter types
				foreach (AssemblyLoader.LoadedAssembly assembly in AssemblyLoader.loadedAssemblies)
				{
					Type[] assemblyTypes = assembly.assembly.GetTypes();
					foreach (Type t in assemblyTypes)
					{
						if (t.IsSubclassOf(typeof(Contract)))
						{
							if (t != typeof(Contract))
							{
								if (cmNode.getCType(t.Name) == null)
								{
									if (!cmNode.addToContractList(new contractTypeContainer(t)))
										DMCM_MBE.LogFormatted("Error During Contract Type Loading; [{0}] Cannot Be Added To Contract Type List", t.Name);
								}
							}
						}
					}
				}

				ConfigNode contractTypes = node.GetNode("Contracts_Modifier_Contract_Types");

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
				foreach (AssemblyLoader.LoadedAssembly assembly in AssemblyLoader.loadedAssemblies)
				{
					Type[] assemblyTypes = assembly.assembly.GetTypes();
					foreach (Type t in assemblyTypes)
					{
						if (t.IsSubclassOf(typeof(ContractParameter)))
						{
							if (t.Name == "OR" || t.Name == "XOR" || t.Name == "RecoverPart")
								continue;
							if (t.IsAbstract)
								continue;
							if (t.IsGenericType)
								continue;
							if (t != typeof(ContractParameter))
							{
								if (cmNode.getPType(t.Name) == null)
								{
									if (!cmNode.addToParamList(new paramTypeContainer(t)))
										DMCM_MBE.LogFormatted("Error During Parameter Type Loading; [{0}] Cannot Be Added To Parameter Type List", t.Name);
								}
							}
						}
					}
				}

				ConfigNode paramTypes = node.GetNode("Contracts_Window_Parameter_Types");

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

			if (cmNode.ShowToolbar)
			{
				//Start the window object
				try
				{
					configWindow = gameObject.AddComponent<contractConfig>();
				}
				catch (Exception e)
				{
					DMCM_MBE.LogFormatted("Contract Modifier Windows Cannot Be Started: {0}", e);
				}
			}

			if (cmNode.getCType("GlobalSettings") == null)
				cmNode.addToContractList(new contractTypeContainer("GlobalSettings"));

			if (cmNode.getPType("GlobalSettings") == null)
				cmNode.addToParamList(new paramTypeContainer("GlobalSettings"));
		}

		public override void OnSave(ConfigNode node)
		{
			node.AddValue("allowZero", allowZero);
			node.AddValue("alterActive", alterActive);
			node.AddValue("stockToolbar", stockToolbar);

			try
			{
				ConfigNode contractTypes = new ConfigNode("Contracts_Window_Contract_Types");

				for (int i = 0; i < cmNode.ContractTypeCount; i++ )
				{
					contractTypeContainer c = cmNode.getCType(i);
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
				ConfigNode paramTypes = new ConfigNode("Contracts_Window_Parameter_Types");

				for (int i = 0; i < cmNode.ParameterTypeCount; i++ )
				{
					paramTypeContainer p = cmNode.getPType(i);

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
			contractTypeContainer c = cmNode.getCType(type);

			if (c == null)
			{
				DMCM_MBE.LogFormatted("Contract Type Not Found; Removing Type From List");
				return;
			}

			string[] a = s.Split(',');

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

		private void stringParamParse(string s, string type)
		{
			paramTypeContainer p = cmNode.getPType(type);

			if (p == null)
			{
				DMCM_MBE.LogFormatted("Parameter Type Not Found; Removing Type From List");
				return;
			}
			string[] a = s.Split(',');

			p.RewardFund = stringFloatParse(a[0], true);
			p.PenaltyFund = stringFloatParse(a[1], true);
			p.RewardRep = stringFloatParse(a[2], true);
			p.PenaltyRep = stringFloatParse(a[3], true);
			p.RewardScience = stringFloatParse(a[4], true);
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

		private void contractOffered(Contract c)
		{
			Type contractT = c.GetType();
			contractTypeContainer cC = cmNode.getCType(contractT.Name);
			if (cC == null)
				return;

			if (cC.MaxActive < 10f || cC.MaxOffer < 10f)
			{
				var cList = ContractSystem.Instance.Contracts;
				int active = 0;
				int offered = 0;
				for (int i = 0; i < cList.Count; i++)
				{
					if (cList[i].GetType() == contractT)
					{
						if (cList[i].ContractState == Contract.State.Active)
							active++;
						else if (cList[i].ContractState == Contract.State.Offered)
							offered++;
					}
				}
				int remainingSlots = (int)(cC.MaxActive * 10) - active;
				if ((offered - 1) >= (int)(cC.MaxOffer * 10) && cC.MaxOffer < 10f)
				{
					c.Unregister();
					ContractSystem.Instance.Contracts.Remove(c);
				}
				else if ((offered - 1) >= remainingSlots && cC.MaxActive < 10f)
				{
					c.Unregister();
					ContractSystem.Instance.Contracts.Remove(c);
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

		private void paramChanged(float[] originals, paramTypeContainer p)
		{
			var cList = ContractSystem.Instance.Contracts;
			for (int i = 0; i < cList.Count; i++)
			{
				if (cList[i].ContractState == Contract.State.Active || cList[i].ContractState == Contract.State.Offered)
				{
					updateParameterValues(p, cList[i], originals);
				}
			}
		}

		private void contractChanged(float[] originals, contractTypeContainer c)
		{
			var cList = ContractSystem.Instance.Contracts;
			for (int i = 0; i < cList.Count; i++)
			{
				if (cList[i].ContractState == Contract.State.Active || cList[i].ContractState == Contract.State.Offered)
				{
					updateContractValues(c, cList[i], originals);
				}
			}
		}

		private void updateContractValues(contractTypeContainer cC, Contract c, float[] O)
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
				if (cParams.Count() > 0)
				{
					for (int i = 0; i < cParams.Count(); i++)
					{
						string name = cParams.ElementAt(i).GetType().Name;
						paramTypeContainer p = cmNode.getPType(name);
						if (p != null)
						{
							updateParameterValues(p, new List<ContractParameter>() { cParams.ElementAt(i) }, new float[5] { 1, 1, 1, 1, 1 });
						}
					}
				}
			}
		}

		#endregion

		public List<paramTypeContainer> setParamTypes(List<paramTypeContainer> pList)
		{
			pList = new List<paramTypeContainer>();
			List<paramTypeContainer> sortList = new List<paramTypeContainer>();
			for (int i = 0; i < cmNode.ParameterTypeCount; i++)
			{
				paramTypeContainer p = cmNode.getPType(i);
				if (p != null)
				{
					if (p.Generic && pList.Count == 0)
						pList.Add(p);
					else
						sortList.Add(p);
				}
			}

			if (sortList.Count > 0)
			{
				sortList.Sort((a, b) => string.Compare(a.Name, b.Name));
				pList.AddRange(sortList);
			}

			return pList;
		}

		public List<contractTypeContainer> setContractTypes(List<contractTypeContainer> cList)
		{
			cList = new List<contractTypeContainer>();
			List<contractTypeContainer> sortList = new List<contractTypeContainer>();
			for (int i = 0; i < cmNode.ContractTypeCount; i++)
			{
				contractTypeContainer c = cmNode.getCType(i);
				if (c != null)
				{
					if (c.Generic && cList.Count == 0)
						cList.Add(c);
					else
						sortList.Add(c);
				}
			}

			if (sortList.Count > 0)
			{
				sortList.Sort((a, b) => string.Compare(a.Name, b.Name));
				cList.AddRange(sortList);
			}

			return cList;
		}


	}
}
