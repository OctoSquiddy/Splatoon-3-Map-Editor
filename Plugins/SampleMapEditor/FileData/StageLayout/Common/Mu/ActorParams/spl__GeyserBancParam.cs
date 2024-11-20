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
	public class Mu_spl__GeyserBancParam
	{
		[ByamlMember]
		public float MaxHeight { get; set; }

		public Mu_spl__GeyserBancParam()
		{
			MaxHeight = 0.0f;
		}

		public Mu_spl__GeyserBancParam(Mu_spl__GeyserBancParam other)
		{
			MaxHeight = other.MaxHeight;
		}

		public Mu_spl__GeyserBancParam Clone()
		{
			return new Mu_spl__GeyserBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__GeyserBancParam = new BymlNode.DictionaryNode() { Name = "spl__GeyserBancParam" };

			if (SerializedActor["spl__GeyserBancParam"] != null)
			{
				spl__GeyserBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__GeyserBancParam"];
			}


			spl__GeyserBancParam.AddNode("MaxHeight", this.MaxHeight);

			if (SerializedActor["spl__GeyserBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__GeyserBancParam);
			}
		}
	}
}
