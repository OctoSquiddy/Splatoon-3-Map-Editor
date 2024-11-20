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
	public class Mu_spl__GeyserOnlineBancParam
	{
		[ByamlMember]
		public float KeepMaxHeightSec { get; set; }

		[ByamlMember]
		public float MaxHeight { get; set; }

		public Mu_spl__GeyserOnlineBancParam()
		{
			KeepMaxHeightSec = 0.0f;
			MaxHeight = 0.0f;
		}

		public Mu_spl__GeyserOnlineBancParam(Mu_spl__GeyserOnlineBancParam other)
		{
			KeepMaxHeightSec = other.KeepMaxHeightSec;
			MaxHeight = other.MaxHeight;
		}

		public Mu_spl__GeyserOnlineBancParam Clone()
		{
			return new Mu_spl__GeyserOnlineBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__GeyserOnlineBancParam = new BymlNode.DictionaryNode() { Name = "spl__GeyserOnlineBancParam" };

			if (SerializedActor["spl__GeyserOnlineBancParam"] != null)
			{
				spl__GeyserOnlineBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__GeyserOnlineBancParam"];
			}


			spl__GeyserOnlineBancParam.AddNode("KeepMaxHeightSec", this.KeepMaxHeightSec);

			spl__GeyserOnlineBancParam.AddNode("MaxHeight", this.MaxHeight);

			if (SerializedActor["spl__GeyserOnlineBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__GeyserOnlineBancParam);
			}
		}
	}
}
