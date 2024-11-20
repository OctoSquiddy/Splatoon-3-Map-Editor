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
	public class Mu_spl__WorldLocationAreaParam
	{
		[ByamlMember]
		public string AreaType { get; set; }

		[ByamlMember]
		public string LocationName { get; set; }

		public Mu_spl__WorldLocationAreaParam()
		{
			AreaType = "";
			LocationName = "";
		}

		public Mu_spl__WorldLocationAreaParam(Mu_spl__WorldLocationAreaParam other)
		{
			AreaType = other.AreaType;
			LocationName = other.LocationName;
		}

		public Mu_spl__WorldLocationAreaParam Clone()
		{
			return new Mu_spl__WorldLocationAreaParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__WorldLocationAreaParam = new BymlNode.DictionaryNode() { Name = "spl__WorldLocationAreaParam" };

			if (SerializedActor["spl__WorldLocationAreaParam"] != null)
			{
				spl__WorldLocationAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__WorldLocationAreaParam"];
			}


			if (this.AreaType != "")
			{
				spl__WorldLocationAreaParam.AddNode("AreaType", this.AreaType);
			}

			if (this.LocationName != "")
			{
				spl__WorldLocationAreaParam.AddNode("LocationName", this.LocationName);
			}

			if (SerializedActor["spl__WorldLocationAreaParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__WorldLocationAreaParam);
			}
		}
	}
}
