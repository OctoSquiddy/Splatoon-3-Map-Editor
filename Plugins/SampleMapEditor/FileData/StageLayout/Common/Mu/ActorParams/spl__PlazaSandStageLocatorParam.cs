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
	public class Mu_spl__PlazaSandStageLocatorParam
	{
		[ByamlMember]
		public string StageType { get; set; }

		public Mu_spl__PlazaSandStageLocatorParam()
		{
			StageType = "";
		}

		public Mu_spl__PlazaSandStageLocatorParam(Mu_spl__PlazaSandStageLocatorParam other)
		{
			StageType = other.StageType;
		}

		public Mu_spl__PlazaSandStageLocatorParam Clone()
		{
			return new Mu_spl__PlazaSandStageLocatorParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PlazaSandStageLocatorParam = new BymlNode.DictionaryNode() { Name = "spl__PlazaSandStageLocatorParam" };

			if (SerializedActor["spl__PlazaSandStageLocatorParam"] != null)
			{
				spl__PlazaSandStageLocatorParam = (BymlNode.DictionaryNode)SerializedActor["spl__PlazaSandStageLocatorParam"];
			}


			if (this.StageType != "")
			{
				spl__PlazaSandStageLocatorParam.AddNode("StageType", this.StageType);
			}

			if (SerializedActor["spl__PlazaSandStageLocatorParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PlazaSandStageLocatorParam);
			}
		}
	}
}
