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
	public class Mu_spl__CullingOcclusionAreaParam
	{
		[ByamlMember]
		public float Addition { get; set; }

		public Mu_spl__CullingOcclusionAreaParam()
		{
			Addition = 0.0f;
		}

		public Mu_spl__CullingOcclusionAreaParam(Mu_spl__CullingOcclusionAreaParam other)
		{
			Addition = other.Addition;
		}

		public Mu_spl__CullingOcclusionAreaParam Clone()
		{
			return new Mu_spl__CullingOcclusionAreaParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CullingOcclusionAreaParam = new BymlNode.DictionaryNode() { Name = "spl__CullingOcclusionAreaParam" };

			if (SerializedActor["spl__CullingOcclusionAreaParam"] != null)
			{
				spl__CullingOcclusionAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__CullingOcclusionAreaParam"];
			}


			spl__CullingOcclusionAreaParam.AddNode("Addition", this.Addition);

			if (SerializedActor["spl__CullingOcclusionAreaParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CullingOcclusionAreaParam);
			}
		}
	}
}
