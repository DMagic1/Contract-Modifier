#region license
/*The MIT License (MIT)
Contract Type Container - An object to store variables for customizing contract rewards/penalties

Copyright (c) 2014 DMagic

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
		private float maxOffer = 100;
		[Persistent]
		private float maxActive = 100;

		private float[] contractValues = new float[9];
		private ConfigNode contractTypeNode;
		private Type contractType;
		private string name = "";
		private Contract contractC = null;
		private bool generic = false;

		internal contractTypeContainer (Type CType)
		{
			contractType = CType;
			try
			{
			contractC = (Contract)Activator.CreateInstance(CType);
			}
			catch (Exception e)
			{
				DMCM_MBE.LogFormatted("This Contract Type: {0} Does Not Have An Empty Constructor And Will Be Skipped: {1]", CType.Name, e);
				
			}
			typeName = CType.Name;
			name = displayName(typeName);
			maxOffer /= 10f;
			maxActive /= 10f;
			contractTypeNode = this.AsConfigNode;
			if (typeName == "GlobalSettings")
				generic = true;
		}

		internal contractTypeContainer (string n)
		{
			typeName = n;
			name = displayName(n);
			maxOffer /= 10f;
			maxActive /= 10f;
			contractTypeNode = this.AsConfigNode;
			if (typeName == "GlobalSettings")
				generic = true;
		}

		public contractTypeContainer()
		{

		}

		public override void OnDecodeFromConfigNode()
		{
			loadFromNode(true);
		}

		internal bool loadFromNode(bool zero)
		{
			name = displayName(typeName);
			fundReward = fundReward.returnNonZero(zero);
			fundAdvance = fundAdvance.returnNonZero(zero);
			fundPenalty = fundPenalty.returnNonZero(zero);
			repReward = repReward.returnNonZero(zero);
			repPenalty = repPenalty.returnNonZero(zero);
			scienceReward = scienceReward.returnNonZero(zero);
			durationTime = durationTime.returnNonZero(zero);
			maxOffer /= 10;
			maxActive /= 10;
			if (typeName == "GlobalSettings")
				generic = true;
			return true;
		}

		private string displayName (string s)
		{
			return Regex.Replace(s, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
		}

		public Contract ContractC
		{
			get { return contractC; }
		}

		public ConfigNode ContractTypeNode
		{
			get { return contractTypeNode; }
		}

		public Type ContractType
		{
			get { return contractType; }
		}

		public bool Generic
		{
			get { return generic; }
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
					if (contractModifierScenario.Instance != null)
					{
						if (!contractModifierScenario.Instance.allowZero && value == 0.00f)
							value = 0.001f;
					}
					if (value == 0.00f)
						value = 0.0000000001f;
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
					maxOffer = value;
			}
		}

		public float MaxActive
		{
			get { return maxActive; }
			set
			{
				if (value >= 0 && value <= 10)
					maxActive = value;
			}
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
