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

using ContractModifier.Framework;

namespace ContractModifier
{
	public class ContractValuesNode : DMCM_ConfigNodeStorage
	{
		[Persistent]
		private bool showToolbar = true;
		[Persistent]
		private bool allowZero = false;
		[Persistent]
		private bool alterActive = true;
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

		private Dictionary<string, contractTypeContainer> masterContractList = new Dictionary<string, contractTypeContainer>();
		private Dictionary<string, paramTypeContainer> masterParamList = new Dictionary<string, paramTypeContainer>();

		public override void OnDecodeFromConfigNode()
		{
			try
			{
				masterContractList = ContractTypeConfigs.ToDictionary(a => a.TypeName, a => a);
				LogFormatted_DebugOnly("Contract Dict Loaded; {0} Objects", masterContractList.Count);
			}
			catch (Exception e)
			{
				LogFormatted("Error while loading contract container list; possibly a duplicate entry: {0}", e);
			}

			try
			{
				masterParamList = ParameterTypeConfigs.ToDictionary(a => a.TypeName, a => a);
				LogFormatted_DebugOnly("Parameter Dict Loaded; {0} Objects", masterParamList.Count);
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
		}

		internal ContractValuesNode(string filePath)
		{
			FilePath = filePath;

			if (Load())
				topNode = this.AsConfigNode;
			else
				topNode = new ConfigNode("ContractValuesNode");
		}

		private ConfigNode topNode;

		public ConfigNode TopNode
		{
			get { return topNode; }
		}

		public int ContractTypeCount
		{
			get { return masterContractList.Count; }
		}

		public int ParameterTypeCount
		{
			get { return masterParamList.Count; }
		}

		public contractTypeContainer getCType(int i)
		{
			return masterContractList.ElementAtOrDefault(i).Value;
		}

		public contractTypeContainer getCType(string s)
		{
			if (masterContractList.ContainsKey(s))
				return masterContractList[s];
			else
			{
				LogFormatted("No Contract Type Of Name: [{0}] Found...", s);
				return null;
			}
		}

		public paramTypeContainer getPType(int i)
		{
			return masterParamList.ElementAtOrDefault(i).Value;
		}

		public paramTypeContainer getPType(string s)
		{
			if (masterParamList.ContainsKey(s))
				return masterParamList[s];
			else
			{
				LogFormatted("No Parameter Type Of Name [{0}] Found...", s);
				return null;
			}
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

		public bool ShowToolbar
		{
			get { return showToolbar; }
			internal set { showToolbar = value; }
		}

		public bool AllowZero
		{
			get { return allowZero; }
			internal set { allowZero = value; }
		}

		public bool AlterActive
		{
			get { return alterActive; }
			internal set { alterActive = value; }
		}

		public bool StockToolbar
		{
			get { return stockToolbar; }
			internal set { stockToolbar = value; }
		}

		public bool WarnedZero
		{
			get { return warnedZero; }
			internal set { warnedZero = value; }
		}

		public bool WarnedAlterActive
		{
			get { return warnedAlterActive; }
			internal set { warnedAlterActive = value; }
		}

		public bool WarnedToolbar
		{
			get { return warnedToolbar; }
			internal set { warnedToolbar = value; }
		}
	}
}
