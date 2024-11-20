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
	public class Mu_spl__DesignerObjBancParam
	{
		[ByamlMember]
		public string AnimName { get; set; }

		public Mu_spl__DesignerObjBancParam()
		{
			AnimName = "";
		}

		public Mu_spl__DesignerObjBancParam(Mu_spl__DesignerObjBancParam other)
		{
			AnimName = other.AnimName;
		}

		public Mu_spl__DesignerObjBancParam Clone()
		{
			return new Mu_spl__DesignerObjBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__DesignerObjBancParam = new BymlNode.DictionaryNode() { Name = "spl__DesignerObjBancParam" };

			if (SerializedActor["spl__DesignerObjBancParam"] != null)
			{
				spl__DesignerObjBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__DesignerObjBancParam"];
			}


			if (this.AnimName != "")
			{
				spl__DesignerObjBancParam.AddNode("AnimName", this.AnimName);
			}

			if (SerializedActor["spl__DesignerObjBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__DesignerObjBancParam);
			}
		}
	}
}
