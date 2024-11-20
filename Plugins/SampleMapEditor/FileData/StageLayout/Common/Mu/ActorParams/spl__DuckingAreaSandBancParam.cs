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
	public class Mu_spl__DuckingAreaSandBancParam
	{
		[ByamlMember]
		public string DuckingName { get; set; }

		[ByamlMember]
		public float Margin { get; set; }

		[ByamlMember]
		public string StageType { get; set; }

		public Mu_spl__DuckingAreaSandBancParam()
		{
			DuckingName = "";
			Margin = 0.0f;
			StageType = "";
		}

		public Mu_spl__DuckingAreaSandBancParam(Mu_spl__DuckingAreaSandBancParam other)
		{
			DuckingName = other.DuckingName;
			Margin = other.Margin;
			StageType = other.StageType;
		}

		public Mu_spl__DuckingAreaSandBancParam Clone()
		{
			return new Mu_spl__DuckingAreaSandBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__DuckingAreaSandBancParam = new BymlNode.DictionaryNode() { Name = "spl__DuckingAreaSandBancParam" };

			if (SerializedActor["spl__DuckingAreaSandBancParam"] != null)
			{
				spl__DuckingAreaSandBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__DuckingAreaSandBancParam"];
			}


			if (this.DuckingName != "")
			{
				spl__DuckingAreaSandBancParam.AddNode("DuckingName", this.DuckingName);
			}

			spl__DuckingAreaSandBancParam.AddNode("Margin", this.Margin);

			if (this.StageType != "")
			{
				spl__DuckingAreaSandBancParam.AddNode("StageType", this.StageType);
			}

			if (SerializedActor["spl__DuckingAreaSandBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__DuckingAreaSandBancParam);
			}
		}
	}
}
