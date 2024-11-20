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
	public class Mu_spl__PlazaSandBandLocatorParam
	{
		[ByamlMember]
		public string Part { get; set; }

		[ByamlMember]
		public string StageType { get; set; }

		public Mu_spl__PlazaSandBandLocatorParam()
		{
			Part = "";
			StageType = "";
		}

		public Mu_spl__PlazaSandBandLocatorParam(Mu_spl__PlazaSandBandLocatorParam other)
		{
			Part = other.Part;
			StageType = other.StageType;
		}

		public Mu_spl__PlazaSandBandLocatorParam Clone()
		{
			return new Mu_spl__PlazaSandBandLocatorParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PlazaSandBandLocatorParam = new BymlNode.DictionaryNode() { Name = "spl__PlazaSandBandLocatorParam" };

			if (SerializedActor["spl__PlazaSandBandLocatorParam"] != null)
			{
				spl__PlazaSandBandLocatorParam = (BymlNode.DictionaryNode)SerializedActor["spl__PlazaSandBandLocatorParam"];
			}


			if (this.Part != "")
			{
				spl__PlazaSandBandLocatorParam.AddNode("Part", this.Part);
			}

			if (this.StageType != "")
			{
				spl__PlazaSandBandLocatorParam.AddNode("StageType", this.StageType);
			}

			if (SerializedActor["spl__PlazaSandBandLocatorParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PlazaSandBandLocatorParam);
			}
		}
	}
}
