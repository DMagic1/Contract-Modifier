#region license
/*The MIT License (MIT)
Parameter Type Container - An object to store variables for customizing contract parameter rewards/penalties

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
using System.Text.RegularExpressions;

using Contracts;
using Contracts.Parameters;
using ContractModifier.Framework;
using UnityEngine;

namespace ContractModifier
{
	public class paramTypeContainer : DMCM_ConfigNodeStorage
	{
		[Persistent]
		private string typeName = "";
		[Persistent]
		private float fundReward = 1.0f;
		[Persistent]
		private float fundPenalty = 1.0f;
		[Persistent]
		private float repReward = 1.0f;
		[Persistent]
		private float repPenalty = 1.0f;
		[Persistent]
		private float scienceReward = 1.0f;
		[Persistent]
		private bool contractConfiguratorType = false;

		private float defaultFundReward = 1.0f;
		private float defaultFundPenalty = 1.0f;
		private float defaultRepReward = 1.0f;
		private float defaultRepPenalty = 1.0f;
		private float defaultScienceReward = 1.0f;

		private Type paramType = null;
		private ContractParameter param = null;
		private string name = "";
		private float[] paramValues = new float[5];
		private bool generic = false;

		internal paramTypeContainer (Type PType)
		{
			paramType = PType;
			try
			{
				param = (ContractParameter)Activator.CreateInstance(PType);
			}
			catch (Exception e)
			{
				DMCM_MBE.LogFormatted("This Parameter Type: {0} Does Not Have An Empty Constructor And Will Be Skipped: {1}", PType.Name, e);
				return;
			}
			typeName = PType.Name;
			name = displayName(typeName);

			paramTypeContainer global = cmConfigLoad.TopNode.getPType("GlobalSettings");
			if (global != null)
				setValuesToGlobal(global);

			Type t = ContractValuesNode.getParameterType("ContractConfiguratorParameter");
			if (t != null)
			{
				if (PType.IsSubclassOf(t))
					contractConfiguratorType = true;
			}
		}

		internal paramTypeContainer (string n)
		{
			typeName = n;
			name = displayName(n);
			if (typeName == "GlobalSettings")
				generic = true;
			else
			{

				paramType = ContractValuesNode.getParameterType(typeName);

				paramTypeContainer global = cmConfigLoad.TopNode.getPType("GlobalSettings");
				if (global != null)
					setValuesToGlobal(global);
			}
		}

		public paramTypeContainer()
		{

		}

		public override void OnDecodeFromConfigNode()
		{
			loadFromNode();
		}

		internal bool loadFromNode()
		{
			name = displayName(typeName);
			defaultFundReward = fundReward = fundReward.returnNonZero();
			defaultFundPenalty = fundPenalty = fundPenalty.returnNonZero();
			defaultRepReward = repReward = repReward.returnNonZero();
			defaultRepPenalty = repPenalty = repPenalty.returnNonZero();
			defaultScienceReward = scienceReward = scienceReward.returnNonZero();

			if (typeName == "GlobalSettings")
				generic = true;
			else
				paramType = ContractValuesNode.getParameterType(typeName);
			return true;
		}

		private void setValuesToGlobal(paramTypeContainer global)
		{
			this.defaultFundReward = this.fundReward = global.fundReward;
			this.defaultFundPenalty = this.fundPenalty = global.fundPenalty;
			this.defaultRepReward = this.repReward = global.repReward;
			this.defaultRepPenalty = this.repPenalty = global.repPenalty;
			this.defaultScienceReward = this.scienceReward = global.scienceReward;
		}

		private string displayName(string s)
		{
			return Regex.Replace(s, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
		}

		public ContractParameter Param
		{
			get { return param; }
		}

		public bool Generic
		{
			get { return generic; }
		}

		public bool CConfigType
		{
			get { return contractConfiguratorType; }
		}

		public Type ParamType
		{
			get { return paramType; }
		}

		public string Name
		{
			get { return name; }
		}

		public string TypeName
		{
			get { return typeName; }
		}

		public float RewardFund
		{
			get { return fundReward; }
			set
			{
				if (value >= 0 && value <= 10)
				{
					if (contractModifierScenario.Instance != null)
					{
						if (!contractModifierScenario.Instance.allowZero && value == 0.00f)
							value = 0.001f;
					}
					if (value == 0.00f)
						value = 0.0000000001f;
					fundReward = value;
				}
			}
		}

		public float PenaltyFund
		{
			get { return fundPenalty; }
			set
			{
				if (value >= 0 && value <= 10)
				{
					if (contractModifierScenario.Instance != null)
					{
						if (!contractModifierScenario.Instance.allowZero && value == 0.00f)
							value = 0.001f;
					}
					if (value == 0.00f)
						value = 0.0000000001f;
					fundPenalty = value;
				}
			}
		}

		public float RewardRep
		{
			get { return repReward; }
			set
			{
				if (value >= 0 && value <= 10)
				{
					if (contractModifierScenario.Instance != null)
					{
						if (!contractModifierScenario.Instance.allowZero && value == 0.00f)
							value = 0.001f;
					}
					if (value == 0.00f)
						value = 0.0000000001f;
					repReward = value;
				}
			}
		}

		public float PenaltyRep
		{
			get { return repPenalty; }
			set
			{
				if (value >= 0 && value <= 10)
				{
					if (contractModifierScenario.Instance != null)
					{
						if (!contractModifierScenario.Instance.allowZero && value == 0.00f)
							value = 0.001f;
					}
					if (value == 0.00f)
						value = 0.0000000001f;
					repPenalty = value;
				}
			}
		}

		public float RewardScience
		{
			get { return scienceReward; }
			set
			{
				if (value >= 0 && value <= 10)
				{
					if (contractModifierScenario.Instance != null)
					{
						if (!contractModifierScenario.Instance.allowZero && value == 0.00f)
							value = 0.001f;
					}
					if (value == 0.00f)
						value = 0.0000000001f;
					scienceReward = value;
				}
			}
		}

		public float DefaultFundReward
		{
			get { return defaultFundReward; }
		}

		public float DefaultFundPenalty
		{
			get { return defaultFundPenalty; }
		}

		public float DefaultRepReward
		{
			get { return defaultRepReward; }
		}

		public float DefaultRepPenalty
		{
			get { return defaultRepPenalty; }
		}

		public float DefaultScienceReward
		{
			get { return defaultScienceReward; }
		}

		public float[] ParamValues
		{
			get
			{
				paramValues[0] = fundReward;
				paramValues[1] = fundPenalty;
				paramValues[2] = repReward;
				paramValues[3] = repPenalty;
				paramValues[4] = scienceReward;
				return paramValues;
			}
		}
	}
}
