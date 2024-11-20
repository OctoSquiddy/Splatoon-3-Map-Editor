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
	public class Mu_spl__CullingAreaBancParam
	{
		[ByamlMember]
		public int Id { get; set; }

		public Mu_spl__CullingAreaBancParam()
		{
			Id = 0;
		}

		public Mu_spl__CullingAreaBancParam(Mu_spl__CullingAreaBancParam other)
		{
			Id = other.Id;
		}

		public Mu_spl__CullingAreaBancParam Clone()
		{
			return new Mu_spl__CullingAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CullingAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__CullingAreaBancParam" };

			if (SerializedActor["spl__CullingAreaBancParam"] != null)
			{
				spl__CullingAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CullingAreaBancParam"];
			}


			spl__CullingAreaBancParam.AddNode("Id", this.Id);

			if (SerializedActor["spl__CullingAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CullingAreaBancParam);
			}
		}
	}
}
