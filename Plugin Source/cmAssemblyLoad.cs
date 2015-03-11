#region license
/*The MIT License (MIT)
Contract Modifier Assembly Load - Assign some reflection methods and info based on other installed addons

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
using System.Reflection;

using ContractModifier.Framework;

namespace ContractModifier
{
	public static class cmAssemblyLoad
	{
		private static bool contractsWindowPlusContractLoaded = false;
		private static bool contractsWindowPlusParameterLoaded = false;
		private static bool contractConfiguratorLoaded = false;

		public static bool ContractsWindowPlusContractLoaded
		{
			get { return contractsWindowPlusContractLoaded; }
		}

		public static bool ContractsWindowPlusParameterLoaded
		{
			get { return contractsWindowPlusParameterLoaded; }
		}

		public static bool ContractConfiguratorLoaded
		{
			get { return contractConfiguratorLoaded; }
		}

		private const string contractsWindowPlusTypeName = "ContractsWindow.contractUtils";
		private const string contractsPlusContractMethodName = "UpdateContractType";
		private const string contractsPlusParameterMethodName = "UpdateParameterType";
		private const string contractConfiguratorTypeName = "ContractConfigurator.ContractType";
		private const string contractConfiguratorTypeMethod = "AllValidContractTypes";

		private delegate void ContractPlusUpdateContract(Type t);
		private delegate void ContractPlusUpdateParameter(Type t);
		private delegate void ContractConfiguratorTypes();

		private static ContractPlusUpdateContract _UpdateContract;
		private static ContractPlusUpdateParameter _UpdateParam;
		private static ContractConfiguratorTypes _CCTypes;

		private static Type CPlusType;

		private static Type CConfigType;

		internal static void loadReflectionMethods()
		{
			contractsWindowPlusContractLoaded = checkForContractsWindowPlusContractUpdate();
			contractsWindowPlusParameterLoaded = checkForContractsWindowPlusParameterUpdate();
			contractConfiguratorLoaded = checkForContractConfigurator();
		}

		internal static void UpdateContractValues(Type t)
		{
			_UpdateContract(t);
		}

		internal static void UpdateParameterValues(Type t)
		{
			_UpdateParam(t);
		}

		private static bool checkForContractsWindowPlusContractUpdate()
		{
			if (_UpdateContract != null)
				return true;

			try
			{
				CPlusType = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
					.SingleOrDefault(t => t.FullName == contractsWindowPlusTypeName);

				if (CPlusType == null)
				{
					DMCM_MBE.LogFormatted("Contracts Window + Type Not Found");
					return false;
				}

				MethodInfo CPlusContractMethod = CPlusType.GetMethod(contractsPlusContractMethodName, new Type[] { typeof(Type) });

				if (CPlusContractMethod == null)
				{
					DMCM_MBE.LogFormatted("Contracts Window + Contract Update Method Not Found");
					return false;
				}

				_UpdateContract = (ContractPlusUpdateContract)Delegate.CreateDelegate(typeof(ContractPlusUpdateContract), CPlusContractMethod);

				DMCM_MBE.LogFormatted("Contracts Window + Contract Update Method Assigned");

				return _UpdateContract != null;
			}
			catch (Exception e)
			{
				DMCM_MBE.LogFormatted("Error While Loading Contracts Window Plus Contract Reflection Methods: {0}", e);
			}

			return false;
		}

		private static bool checkForContractsWindowPlusParameterUpdate()
		{
			if (_UpdateParam != null)
				return true;

			try
			{
				if (CPlusType == null)
				{
					DMCM_MBE.LogFormatted("Contracts Window + Type Not Found");
					return false;
				}

				MethodInfo CPlusParameterMethod = CPlusType.GetMethod(contractsPlusParameterMethodName, new Type[] { typeof(Type) });

				if (CPlusParameterMethod == null)
				{
					DMCM_MBE.LogFormatted("Contracts Window + Parameter Update Method Not Found");
					return false;
				}

				_UpdateParam = (ContractPlusUpdateParameter)Delegate.CreateDelegate(typeof(ContractPlusUpdateParameter), CPlusParameterMethod);

				DMCM_MBE.LogFormatted("Contracts Window + Parameter Update Method Assigned");

				return _UpdateContract != null;
			}
			catch (Exception e)
			{
				DMCM_MBE.LogFormatted("Error While Loading Contracts Window Plus Parameter Reflection Methods: {0}", e);
			}

			return false;
		}

		private static bool checkForContractConfigurator()
		{
			if (_CCTypes != null)
				return true;

			try
			{
				CConfigType = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
						.SingleOrDefault(t => t.FullName == contractConfiguratorTypeName);

				if (CConfigType == null)
				{
					DMCM_MBE.LogFormatted("Contract Configurator Type Not Found");
					return false;
				}

				PropertyInfo CConfigTypeProperty = CConfigType.GetProperty(contractConfiguratorTypeMethod);

				if (CConfigTypeProperty == null)
				{
					DMCM_MBE.LogFormatted("Contract Configurator Method Not Loaded");
					return false;
				}

				FieldInfo ccTypeName = CConfigType.GetField("name");

				CConfigType.InvokeMember(contractConfiguratorTypeMethod, BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, null, null);

				int counter = 0;

				while (true)
				{
					try
					{
						CConfigTypeProperty.GetValue(null, new object[] { counter });
						counter++;
					}
					catch
					{
						break;
					}
				}
				for (int i = 0; i < counter; i++)
				{
					var ccType = CConfigTypeProperty.GetValue(null, new object[] { i });
					var ccName = ccTypeName.GetValue(ccType);

					string name = (string)ccName;
				}
			}
			catch (Exception e)
			{
				DMCM_MBE.LogFormatted("Error While Loading Contract Configurator Method: {0}", e);
			}

			return false;
		}
	}
}
