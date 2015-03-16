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

using Contracts;
using ContractModifier.Framework;

namespace ContractModifier
{
	public static class cmAssemblyLoad
	{
		private static bool contractsWindowPlusContractLoaded = false;
		private static bool contractsWindowPlusParameterLoaded = false;
		private static bool contractConfiguratorCTLoaded = false;
		private static bool contractConfiguratorCCLoaded = false;

		public static bool ContractsWindowPlusContractLoaded
		{
			get { return contractsWindowPlusContractLoaded; }
		}

		public static bool ContractsWindowPlusParameterLoaded
		{
			get { return contractsWindowPlusParameterLoaded; }
		}

		public static bool ContractConfiguratorCTLoaded
		{
			get { return contractConfiguratorCTLoaded; }
		}

		public static bool ContractConfiguratorCCLoaded
		{
			get { return contractConfiguratorCCLoaded; }
		}

		private const string contractsWindowPlusTypeName = "ContractsWindow.contractUtils";
		private const string contractsPlusContractMethodName = "UpdateContractType";
		private const string contractsPlusParameterMethodName = "UpdateParameterType";
		private const string contractConfiguratorTypeName = "ContractConfigurator.ContractType";
		private const string contractConfiguratorTypeMethod = "AllValidContractTypeNames";
		private const string contractConfiguratorCCTypeName = "ContractConfigurator.ConfiguredContract";
		private const string contractConfiguratorCCTypeNameMethod = "contractTypeName";

		private delegate void ContractPlusUpdateContract(Type t);
		private delegate void ContractPlusUpdateParameter(Type t);
		private delegate string ContractConfiguratorTypeName(Contract c);

		private static ContractPlusUpdateContract _UpdateContract;
		private static ContractPlusUpdateParameter _UpdateParam;
		private static ContractConfiguratorTypeName _CCTypeName;

		internal static IEnumerable<string> ContractConfiguratorTypeNames = null;

		private static Type CPlusType;

		internal static void loadReflectionMethods()
		{
			contractsWindowPlusContractLoaded = checkForContractsWindowPlusContractUpdate();
			contractsWindowPlusParameterLoaded = checkForContractsWindowPlusParameterUpdate();
			contractConfiguratorCTLoaded = checkForContractConfiguratorType();
			contractConfiguratorCCLoaded = checkForContractConfiguratorTypeName();
		}

		internal static void UpdateContractValues(Type t)
		{
			if (t != null)
				_UpdateContract(t);
		}

		internal static void UpdateParameterValues(Type t)
		{
			if (t != null)
				_UpdateParam(t);
		}

		internal static string CCTypeName(Contract c)
		{
			return _CCTypeName(c);
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

		private static bool checkForContractConfiguratorType()
		{
			try
			{
				Type CConfigType = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
						.SingleOrDefault(t => t.FullName == contractConfiguratorTypeName);

				if (CConfigType == null)
				{
					DMCM_MBE.LogFormatted("Contract Configurator Type [{0}] Not Found", contractConfiguratorTypeName);
					return false;
				}

				PropertyInfo CConfigTypeProperty = CConfigType.GetProperty(contractConfiguratorTypeMethod);

				if (CConfigTypeProperty == null)
				{
					DMCM_MBE.LogFormatted("Contract Configurator Property [{0}] Not Loaded", contractConfiguratorTypeMethod);
					return false;
				}

				var names = CConfigTypeProperty.GetValue(null, null);

				IEnumerable<string> enumerableNames = (IEnumerable<string>)names;

				ContractConfiguratorTypeNames = enumerableNames;

				DMCM_MBE.LogFormatted("Contract Configurator Contract Type Names Assigned");

				return ContractConfiguratorTypeNames != null;
			}
			catch (Exception e)
			{
				DMCM_MBE.LogFormatted("Error While Loading Contract Configurator Types Property: {0}", e);
			}

			return false;
		}

		private static bool checkForContractConfiguratorTypeName()
		{
			if (_CCTypeName != null)
				return true;

			try
			{
				Type CConfigType = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
						.SingleOrDefault(t => t.FullName == contractConfiguratorCCTypeName);

				if (CConfigType == null)
				{
					DMCM_MBE.LogFormatted("Contract Configurator Type [{0}] Not Found", contractConfiguratorCCTypeName);
					return false;
				}

				MethodInfo CConfigNameMethod = CConfigType.GetMethod(contractConfiguratorCCTypeNameMethod, new Type[] { typeof(Contract) });

				if (CConfigNameMethod == null)
				{
					DMCM_MBE.LogFormatted("Contract Configurator Method [{0}] Not Loaded", contractConfiguratorCCTypeNameMethod);
					return false;
				}

				_CCTypeName = (ContractConfiguratorTypeName)Delegate.CreateDelegate(typeof(ContractConfiguratorTypeName), CConfigNameMethod);

				DMCM_MBE.LogFormatted("Contract Configurator Contract Type Name Method Assigned");

				return _CCTypeName != null;

			}
			catch (Exception e)
			{
				DMCM_MBE.LogFormatted("Error While Loading Contract Configurator Name Method: {0}", e);
			}

			return false;
		}

	}
}
