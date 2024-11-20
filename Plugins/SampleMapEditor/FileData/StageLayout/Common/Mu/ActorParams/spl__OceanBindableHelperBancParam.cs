using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	[ByamlObject]
	public class Mu_spl__OceanBindableHelperBancParam
	{
		[ByamlMember]
		public bool IsOceanBind { get; set; }

		[ByamlMember]
		public float Ratio { get; set; }

		public Mu_spl__OceanBindableHelperBancParam()
		{
			IsOceanBind = false;
			Ratio = 0.0f;
		}

		public Mu_spl__OceanBindableHelperBancParam(Mu_spl__OceanBindableHelperBancParam other)
		{
			IsOceanBind = other.IsOceanBind;
			Ratio = other.Ratio;
		}

		public Mu_spl__OceanBindableHelperBancParam Clone()
		{
			return new Mu_spl__OceanBindableHelperBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__OceanBindableHelperBancParam = new BymlNode.DictionaryNode() { Name = "spl__OceanBindableHelperBancParam" };

			if (SerializedActor["spl__OceanBindableHelperBancParam"] != null)
			{
				spl__OceanBindableHelperBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__OceanBindableHelperBancParam"];
			}


			if (this.IsOceanBind)
			{
				spl__OceanBindableHelperBancParam.AddNode("IsOceanBind", this.IsOceanBind);
			}

			spl__OceanBindableHelperBancParam.AddNode("Ratio", this.Ratio);

			if (SerializedActor["spl__OceanBindableHelperBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__OceanBindableHelperBancParam);
			}
		}
	}
}
