using System;
using System.Collections.Generic;
using System.Linq;

using ContractModifier.Framework;

namespace ContractModifier
{
	public class ContractValuesNode : DMCM_ConfigNodeStorage
	{
		[Persistent]
		private string version = "";
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
			LogFormatted_DebugOnly("Start On Decode Method");
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
			LogFormatted_DebugOnly("Create new config object");
			FilePath = filePath;

			if (Load())
			{
				LogFormatted_DebugOnly("Loaded From File: {0}", filePath);
				topNode = this.AsConfigNode;
			}
			else
			{
				LogFormatted_DebugOnly("Failed to load from file: {0}", filePath);
				topNode = new ConfigNode("ContractValuesNode");
			}
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
			LogFormatted_DebugOnly("Start config loader");
			topConfigNode = new ContractValuesNode(fileName);
		}
	}
}
