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
	public class Mu_spl__PaintTargetAreaBancParam
	{
		public Mu_spl__PaintTargetAreaBancParam()
		{
		}

		public Mu_spl__PaintTargetAreaBancParam(Mu_spl__PaintTargetAreaBancParam other)
		{
		}

		public Mu_spl__PaintTargetAreaBancParam Clone()
		{
			return new Mu_spl__PaintTargetAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PaintTargetAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__PaintTargetAreaBancParam" };

			if (SerializedActor["spl__PaintTargetAreaBancParam"] != null)
			{
				spl__PaintTargetAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PaintTargetAreaBancParam"];
			}


			if (SerializedActor["spl__PaintTargetAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PaintTargetAreaBancParam);
			}
		}
	}
}
