using System;
using System.Collections.Generic;
using System.Linq;

using ContractModifier.Framework;

namespace ContractModifier
{
	public class contractModifierNode : DMCM_ConfigNodeStorage
	{
		[Persistent]
		public bool showToolbar = true;
		[Persistent]
		public string version = "";

		private Dictionary<string, contractTypeContainer> masterContractList = new Dictionary<string, contractTypeContainer>();
		private Dictionary<string, paramTypeContainer> masterParamList = new Dictionary<string, paramTypeContainer>();

		private ConfigNode topNode;

		public ConfigNode TopNode
		{
			get { return topNode; }
			internal set { topNode = value; }
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
			return masterContractList.ElementAt(i).Value;
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
			return masterParamList.ElementAt(i).Value;
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
			if (!masterContractList.ContainsKey(c.Name))
			{
				masterContractList.Add(c.Name, c);
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
			if (!masterParamList.ContainsKey(p.Name))
			{
				masterParamList.Add(p.Name, p);
				return true;
			}
			else
			{
				LogFormatted("Parameter Type Container Dictionary Already Has Parameter Of This Type; Skipping...");
				return false;
			}
		}
	}

	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class cmConfigLoad : DMCM_MBE
	{
		private static contractModifierNode topConfigNode;
		private const string fileName = "contractModifierConfig.cfg";

		public static contractModifierNode TopNode
		{
			get { return topConfigNode; }
		}

		protected override void Start()
		{
			topConfigNode = new contractModifierNode();
			topConfigNode.FilePath = fileName;

			loadConfigFile();
		}

		private void loadConfigFile()
		{
			if (topConfigNode.Load())
			{
				topConfigNode.TopNode = topConfigNode.AsConfigNode;
				if (topConfigNode.TopNode != null)
				{
					bool.TryParse(topConfigNode.TopNode.GetValue("showToolbar"), out topConfigNode.showToolbar);
					topConfigNode.version = topConfigNode.TopNode.GetValue("version");

					foreach (ConfigNode cType in topConfigNode.TopNode.GetNodes("CONTRACT_TYPE_CONFIG"))
					{
						if (cType != null)
						{
							string name;
							float fRew, fAdv, fPen, rRew, rPen, sRew, mOff, mAct;

							if (cType.HasValue("name"))
								name = cType.GetValue("name");
							else
								continue;

							fRew = stringFloatParse(cType.GetValue("fundsReward"), 1.0f);
							fAdv = stringFloatParse(cType.GetValue("fundsAdvance"), 1.0f);
							fPen = stringFloatParse(cType.GetValue("fundsPenalty"), 1.0f);
							rRew = stringFloatParse(cType.GetValue("repReward"), 1.0f);
							rPen = stringFloatParse(cType.GetValue("repPenalty"), 1.0f);
							sRew = stringFloatParse(cType.GetValue("scienceReward"), 1.0f);
							mOff = stringFloatParse(cType.GetValue("maxOffered"), 100.0f);
							mAct = stringFloatParse(cType.GetValue("maxActive"), 100.0f);

							contractTypeContainer cCont = new contractTypeContainer(name, fRew, fAdv, fPen, rRew, rPen, sRew, mOff, mAct, cType);

							topConfigNode.addToContractList(cCont);
						}
					}

					foreach (ConfigNode pType in topConfigNode.TopNode.GetNodes("PARAMATER_TYPES_CONFIG"))
					{
						if (pType != null)
						{
							string name;
							float fRew, fPen, rRew, rPen, sRew;

							if (pType.HasValue("name"))
								name = pType.GetValue("name");
							else
								continue;

							fRew = stringFloatParse(pType.GetValue("fundsReward"), 1.0f);
							fPen = stringFloatParse(pType.GetValue("fundsPenalty"), 1.0f);
							rRew = stringFloatParse(pType.GetValue("repReward"), 1.0f);
							rPen = stringFloatParse(pType.GetValue("repPenalty"), 1.0f);
							sRew = stringFloatParse(pType.GetValue("scienceReward"), 1.0f);

							paramTypeContainer pCont = new paramTypeContainer(name, fRew, fPen, rRew, rPen, sRew, pType);

							topConfigNode.addToParamList(pCont);
						}
					}
				}
			}
		}

		private float stringFloatParse(string s, float defVal)
		{
			float f;
			if (float.TryParse(s, out f)) return f;
			return defVal;
		}
	}
}
