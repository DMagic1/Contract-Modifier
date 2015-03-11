#region license
/*The MIT License (MIT)
Contract Modifier Config Load : A MonoBehaviour to load settings and contract types at startup

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

namespace ContractModifier
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class cmConfigLoad : DMCM_MBE
	{
		private static ContractValuesNode topConfigNode;
		internal const string fileName = "ContractModifierConfig.cfg";

		public static ContractValuesNode TopNode
		{
			get { return topConfigNode; }
		}

		protected override void Start()
		{
			topConfigNode = new ContractValuesNode(fileName);
			loadCurrentContractTypes();
			loadCurrentParameterTypes();
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
								if (topConfigNode.getCType(t.Name) == null)
								{
									if (!topConfigNode.addToContractList(new contractTypeContainer(t)))
										DMCM_MBE.LogFormatted("Error During Contract Type Loading; [{0}] Cannot Be Added To Contract Type List", t.Name);
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DMCM_MBE.LogFormatted("Error loading contract types: {0}", e);
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
							if (t.Name == "OR" || t.Name == "XOR" || t.Name == "RecoverPart")
								continue;
							if (t.IsAbstract)
								continue;
							if (t.IsGenericType)
								continue;
							if (t != typeof(ContractParameter))
							{
								if (topConfigNode.getPType(t.Name) == null)
								{
									if (!topConfigNode.addToParamList(new paramTypeContainer(t)))
										DMCM_MBE.LogFormatted("Error During Parameter Type Loading; [{0}] Cannot Be Added To Parameter Type List", t.Name);
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DMCM_MBE.LogFormatted("Error loading parameter types: {0}", e);
			}
		}
	}
}
