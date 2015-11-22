#region license
/*The MIT License (MIT)
Contract Values Node - An object to store persistent data from the config file

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
using ContractModifier.Framework;

namespace ContractModifier
{
	/// <summary>
	/// A storage object for loading and saving data from a config file
	/// </summary>
	public class ContractValuesNode : DMCM_ConfigNodeStorage
	{
		[Persistent]
		private bool disableToolbar = false;
		[Persistent]
		private bool disableSaveSpecificValues = false;
		[Persistent]
		private bool allowZero = false;
		[Persistent]
		private bool alterActive = false;
		[Persistent]
		private bool stockToolbar = true;
		[Persistent]
		private bool warnedZero = false;
		[Persistent]
		private bool warnedToolbar = false;
		[Persistent]
		private bool warnedAlterActive = false;
		[Persistent]
		private List<contractTypeContainer> ContractTypeConfigs = new List<contractTypeContainer>();
		[Persistent]
		private List<paramTypeContainer> ParameterTypeConfigs = new List<paramTypeContainer>();

		private static Dictionary<string, contractTypeContainer> masterContractList = new Dictionary<string, contractTypeContainer>();
		private static Dictionary<string, paramTypeContainer> masterParamList = new Dictionary<string, paramTypeContainer>();

		private static Dictionary<string, Type> contractTypes = new Dictionary<string, Type>();
		private static Dictionary<string, Type> parameterTypes = new Dictionary<string, Type>();

		public override void OnDecodeFromConfigNode()
		{
			try
			{
				masterContractList = ContractTypeConfigs.ToDictionary(a => a.TypeName, a => a);
			}
			catch (Exception e)
			{
				LogFormatted("Error while loading contract container list; possibly a duplicate entry: {0}", e);
			}

			try
			{
				masterParamList = ParameterTypeConfigs.ToDictionary(a => a.TypeName, a => a);
			}
			catch (Exception e)
			{
				LogFormatted("Error while loading contract container list; possibly a duplicate entry: {0}", e);
			}
		}

		public override void OnEncodeToConfigNode()
		{
			try
			{
				ContractTypeConfigs = masterContractList.Values.ToList();
			}
			catch (Exception e)
			{
				LogFormatted("Error while saving contract container list: {0}", e);
			}

			try
			{
				ParameterTypeConfigs = masterParamList.Values.ToList();
			}
			catch (Exception e)
			{
				LogFormatted("Error while saving parameter container list: {0}", e);
			}

			if (contractModifierScenario.Instance != null)
			{
				disableToolbar = contractModifierScenario.Instance.disableToolbar;
				stockToolbar = contractModifierScenario.Instance.stockToolbar;
				allowZero = contractModifierScenario.Instance.allowZero;
				alterActive = contractModifierScenario.Instance.alterActive;
				warnedAlterActive = contractModifierScenario.Instance.warnedAlterActive;
				warnedToolbar = contractModifierScenario.Instance.warnedToolbar;
				warnedZero = contractModifierScenario.Instance.warnedZero;
			}
		}

		internal ContractValuesNode(string filePath)
		{
			FilePath = filePath;

			loadCurrentContractTypes();
			loadCurrentParameterTypes();

			Load();

			if (cmAssemblyLoad.ContractConfiguratorCTLoaded)
				loadCConfigTypes();

			checkAllContractTypes();
			checkAllParamTypes();
		}

		public static int ContractTypeCount
		{
			get { return masterContractList.Count; }
		}

		public static int ParameterTypeCount
		{
			get { return masterParamList.Count; }
		}

		public static contractTypeContainer getCType(int i)
		{
			return masterContractList.ElementAtOrDefault(i).Value;
		}

		public static contractTypeContainer getCType(string s, bool warn = true)
		{
			if (masterContractList.ContainsKey(s))
				return masterContractList[s];
			else if (warn)
				LogFormatted("No Contract Type Of Name: [{0}] Found...", s);
				
			return null;
		}

		public static paramTypeContainer getPType(int i)
		{
			return masterParamList.ElementAtOrDefault(i).Value;
		}

		public static paramTypeContainer getPType(string s, bool warn = true)
		{
			if (masterParamList.ContainsKey(s))
				return masterParamList[s];
			else if (warn)
				LogFormatted("No Parameter Type Of Name [{0}] Found...", s);

			return null;
		}

		public bool addToContractList(contractTypeContainer c)
		{
			if (!masterContractList.ContainsKey(c.TypeName))
			{
				masterContractList.Add(c.TypeName, c);
				return true;
			}
			else
			{
				LogFormatted("Contract Type Container Dictionary Already Has Contract Of This Type; Skipping...");
				return false;
			}
		}

		public bool addToParamList(paramTypeContainer p)
		{
			if (!masterParamList.ContainsKey(p.TypeName))
			{
				masterParamList.Add(p.TypeName, p);
				return true;
			}
			else
			{
				LogFormatted("Parameter Type Container Dictionary Already Has Parameter Of This Type; Skipping...");
				return false;
			}
		}

		private void loadCurrentContractTypes()
		{
			try
			{
				foreach (AssemblyLoader.LoadedAssembly assembly in AssemblyLoader.loadedAssemblies)
				{
					Type[] assemblyTypes = assembly.assembly.GetTypes();
					foreach (Type t in assemblyTypes)
					{
						if (t.IsSubclassOf(typeof(Contract)))
						{
							if (t != typeof(Contract))
							{
								if (!contractTypes.ContainsKey(t.Name))
									contractTypes.Add(t.Name, t);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				LogFormatted("Error loading contract types: {0}", e);
			}
		}

		private void checkAllContractTypes()
		{
			foreach (Type t in contractTypes.Values)
			{
				if (t.Name == "ConfiguredContract")
					continue;
				if (getCType(t.Name, false) == null)
				{
					if (!addToContractList(new contractTypeContainer(t)))
						LogFormatted("Error During Contract Type Loading; [{0}] Cannot Be Added To Contract Type List", t.Name);
				}
			}
		}

		private void loadCurrentParameterTypes()
		{
			try
			{
				foreach (AssemblyLoader.LoadedAssembly assembly in AssemblyLoader.loadedAssemblies)
				{
					Type[] assemblyTypes = assembly.assembly.GetTypes();
					foreach (Type t in assemblyTypes)
					{
						if (t.IsSubclassOf(typeof(ContractParameter)))
						{
							if (t != typeof(ContractParameter))
							{
								if (!parameterTypes.ContainsKey(t.Name))
									parameterTypes.Add(t.Name, t);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				LogFormatted("Error loading parameter types: {0}", e);
			}
		}

		private void checkAllParamTypes()
		{
			foreach(Type t in parameterTypes.Values)
			{
				if (t.Name == "OR" || t.Name == "XOR" || t.Name == "AlwaysTrue" || t.Name == "Any" || t.Name == "All")
					continue;
				if (t.IsAbstract)
					continue;
				if (t.IsGenericType)
					continue;
				if (t.IsSealed)
					continue;
				if (getPType(t.Name, false) == null)
				{
					if (!addToParamList(new paramTypeContainer(t)))
						LogFormatted("Error During Parameter Type Loading; [{0}] Cannot Be Added To Parameter Type List", t.Name);
				}
			}
		}

		private void loadCConfigTypes()
		{
			foreach (string s in cmAssemblyLoad.ContractConfiguratorTypeNames)
			{
				if (getCType(s, false) == null)
				{
					if (!addToContractList(new contractTypeContainer(s, true)))
						LogFormatted("Error During Contract Type Loading; [{0}] Cannot Be Added To Contract Type List", s);
				}
			}
		}

		public static Type getContractType(string name, bool warn = true)
		{
			if (contractTypes.ContainsKey(name))
				return contractTypes[name];
			else if (warn)				
				LogFormatted("Cannot find Contract Type of name: {0}", name);

			return null;
		}

		public static Type getParameterType(string name, bool warn = true)
		{
			if (parameterTypes.ContainsKey(name))
				return parameterTypes[name];
			else if (warn)
				LogFormatted("Cannot find Parameter Type of name: {0}", name);

			return null;
		}

		public bool DisableToolbar
		{
			get { return disableToolbar; }
		}

		public bool DisableSaveSpecificValues
		{
			get { return disableSaveSpecificValues; }
		}

		public bool AllowZero
		{
			get { return allowZero; }
		}

		public bool AlterActive
		{
			get { return alterActive; }
		}

		public bool StockToolbar
		{
			get { return stockToolbar; }
		}

		public bool WarnedZero
		{
			get { return warnedZero; }
		}

		public bool WarnedAlterActive
		{
			get { return warnedAlterActive; }
		}

		public bool WarnedToolbar
		{
			get { return warnedToolbar; }
		}
	}
}
