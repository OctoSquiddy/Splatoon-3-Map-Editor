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
	public class Mu_spl__PaintBancParam
	{
		[ByamlMember]
		public bool IsIncludeVArea { get; set; }

		public Mu_spl__PaintBancParam()
		{
			IsIncludeVArea = false;
		}

		public Mu_spl__PaintBancParam(Mu_spl__PaintBancParam other)
		{
			IsIncludeVArea = other.IsIncludeVArea;
		}

		public Mu_spl__PaintBancParam Clone()
		{
			return new Mu_spl__PaintBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PaintBancParam = new BymlNode.DictionaryNode() { Name = "spl__PaintBancParam" };

			if (SerializedActor["spl__PaintBancParam"] != null)
			{
				spl__PaintBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PaintBancParam"];
			}


			if (this.IsIncludeVArea)
			{
				spl__PaintBancParam.AddNode("IsIncludeVArea", this.IsIncludeVArea);
			}

			if (SerializedActor["spl__PaintBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PaintBancParam);
			}
		}
	}
}
