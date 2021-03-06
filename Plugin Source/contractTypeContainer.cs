﻿#region license
/*The MIT License (MIT)
Contract Type Container - An object to store variables for customizing contract rewards/penalties

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
using Contracts.Templates;
using ContractModifier.Framework;
using UnityEngine;

namespace ContractModifier
{
	/// <summary>
	/// A container to information about contract reward values
	/// </summary>
	public class contractTypeContainer : DMCM_ConfigNodeStorage
	{
		[Persistent]
		private string typeName = "";
		[Persistent]
		private float fundReward = 1.0f;
		[Persistent]
		private float fundPenalty = 1.0f;
		[Persistent]
		private float fundAdvance = 1.0f;
		[Persistent]
		private float repReward = 1.0f;
		[Persistent]
		private float repPenalty = 1.0f;
		[Persistent]
		private float scienceReward = 1.0f;
		[Persistent]
		private float durationTime = 1.0f;
		[Persistent]
		private float maxOffer = 10;
		[Persistent]
		private float maxActive = 10;
		[Persistent]
		private bool contractConfiguratorType = false;

		private float defaultFundReward = 1.0f;
		private float defaultFundAdvance = 1.0f;
		private float defaultFundPenalty = 1.0f;
		private float defaultRepReward = 1.0f;
		private float defaultRepPenalty = 1.0f;
		private float defaultScienceReward = 1.0f;
		private float defaultDuration = 1.0f;
		private float defaultMaxOffer = 10f;
		private float defaultMaxActive = 10f;

		private float[] contractValues = new float[9];
		private Type contractType;
		private string name = "";
		private bool generic = false;

		internal contractTypeContainer (Type CType)
		{
			contractType = CType;
			typeName = CType.Name;
			name = displayName(typeName);

			contractTypeContainer global = ContractValuesNode.getCType("GlobalSettings");
			if (global != null)
				setValuesToGlobal(global);
		}

		internal contractTypeContainer (string n, bool cconfig)
		{
			typeName = n;
			name = displayName(n);
			if (typeName == "GlobalSettings")
				generic = true;
			else
			{
				contractConfiguratorType = cconfig;
				if (!contractConfiguratorType)
					contractType = ContractValuesNode.getContractType(typeName);
				else
					contractType = ContractValuesNode.getContractType("ConfiguredContract");

				contractTypeContainer global = ContractValuesNode.getCType("GlobalSettings");
				if (global != null)
					setValuesToGlobal(global);
			}
		}

		public contractTypeContainer()
		{

		}

		public override void OnDecodeFromConfigNode()
		{
			loadFromNode();
		}

		public override void OnEncodeToConfigNode()
		{
			maxOffer *= 10;
			maxActive *= 10;
		}

		internal bool loadFromNode()
		{
			name = displayName(typeName);
			defaultFundReward = RewardFund = fundReward;
			defaultFundAdvance = AdvanceFund = fundAdvance;
			defaultFundPenalty = PenaltyFund = fundPenalty;
			defaultRepReward = RewardRep = repReward;
			defaultRepPenalty = PenaltyRep = repPenalty;
			defaultScienceReward = RewardScience = scienceReward;
			defaultDuration = DurationTime = durationTime;
			defaultMaxOffer = MaxOffer = (maxOffer / 10f);
			defaultMaxActive = MaxActive = (maxActive / 10f);

			if (typeName == "GlobalSettings")
				generic = true;
			else
			{
				if (!contractConfiguratorType)
					contractType = ContractValuesNode.getContractType(typeName);
				else
					contractType = ContractValuesNode.getContractType("ConfiguredContract");
			}
			return true;
		}

		private void setValuesToGlobal(contractTypeContainer global)
		{
			this.defaultFundReward = this.fundReward = global.fundReward;
			this.defaultFundAdvance = this.fundAdvance = global.fundAdvance;
			this.defaultFundPenalty = this.fundPenalty = global.fundPenalty;
			this.defaultRepReward = this.repReward = global.repReward;
			this.defaultRepPenalty = this.repPenalty = global.repPenalty;
			this.defaultScienceReward = this.scienceReward = global.scienceReward;
			this.defaultDuration = this.durationTime = global.durationTime;
			this.defaultMaxOffer = this.maxOffer = global.maxOffer;
			this.defaultMaxActive = this.maxActive = global.maxActive;
		}

		private string displayName (string s)
		{
			return Regex.Replace(s, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
		}

		public Type ContractType
		{
			get { return contractType; }
		}

		public bool Generic
		{
			get { return generic; }
		}

		public bool CConfigType
		{
			get { return contractConfiguratorType; }
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

		public float AdvanceFund
		{
			get { return fundAdvance; }
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
					fundAdvance = value;
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

		public float DurationTime
		{
			get { return durationTime; }
			set
			{
				if (value > 0 && value <= 10)
				{
					if (value <= 0.1f)
						value = 0.1f;

					durationTime = value;
				}
			}
		}

		public float MaxOffer
		{
			get { return maxOffer; }
			set
			{
				if (value >= 0 && value <= 10)
				{
					if (contractConfiguratorType)
					{
						if (value <= 0.1f)
							value = 0.1f;
					}

					if (value <= 1)
						maxOffer = value.Mathf_Round(1);
					else
						maxOffer = value.Mathf_Round(2);
				}
			}
		}

		public float MaxActive
		{
			get { return maxActive; }
			set
			{
				if (value >= 0 && value <= 10)
				{
					if (contractConfiguratorType)
					{
						if (value <= 0.1f)
							value = 0.1f;
					}

					if (value <= 1)
						maxActive = value.Mathf_Round(1);
					else
						maxActive = value.Mathf_Round(2);
				}
			}
		}

		public float DefaultFundReward
		{
			get { return defaultFundReward; }
		}

		public float DefaultFundAdvance
		{
			get { return defaultFundAdvance; }
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

		public float DefaultDuration
		{
			get { return defaultDuration; }
		}

		public float DefaultMaxOffer
		{
			get { return defaultMaxOffer; }
		}

		public float DefaultMaxActive
		{
			get { return defaultMaxActive; }
		}

		public float[] ContractValues
		{
			get
			{
				contractValues[0] = fundReward;
				contractValues[1] = fundAdvance;
				contractValues[2] = fundPenalty;
				contractValues[3] = repReward;
				contractValues[4] = repPenalty;
				contractValues[5] = scienceReward;
				contractValues[6] = durationTime;
				contractValues[7] = maxOffer;
				contractValues[8] = maxActive;
				return contractValues;
			}
		}


	}
}
