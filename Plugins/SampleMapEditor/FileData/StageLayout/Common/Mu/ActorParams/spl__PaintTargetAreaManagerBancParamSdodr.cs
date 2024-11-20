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
	public class Mu_spl__PaintTargetAreaManagerBancParamSdodr
	{
		[ByamlMember]
		public int CountCheckPoint0 { get; set; }

		[ByamlMember]
		public int CountCheckPoint1 { get; set; }

		[ByamlMember]
		public int CountFirst { get; set; }

		public Mu_spl__PaintTargetAreaManagerBancParamSdodr()
		{
			CountCheckPoint0 = 0;
			CountCheckPoint1 = 0;
			CountFirst = 0;
		}

		public Mu_spl__PaintTargetAreaManagerBancParamSdodr(Mu_spl__PaintTargetAreaManagerBancParamSdodr other)
		{
			CountCheckPoint0 = other.CountCheckPoint0;
			CountCheckPoint1 = other.CountCheckPoint1;
			CountFirst = other.CountFirst;
		}

		public Mu_spl__PaintTargetAreaManagerBancParamSdodr Clone()
		{
			return new Mu_spl__PaintTargetAreaManagerBancParamSdodr(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PaintTargetAreaManagerBancParamSdodr = new BymlNode.DictionaryNode() { Name = "spl__PaintTargetAreaManagerBancParamSdodr" };

			if (SerializedActor["spl__PaintTargetAreaManagerBancParamSdodr"] != null)
			{
				spl__PaintTargetAreaManagerBancParamSdodr = (BymlNode.DictionaryNode)SerializedActor["spl__PaintTargetAreaManagerBancParamSdodr"];
			}


			spl__PaintTargetAreaManagerBancParamSdodr.AddNode("CountCheckPoint0", this.CountCheckPoint0);

			spl__PaintTargetAreaManagerBancParamSdodr.AddNode("CountCheckPoint1", this.CountCheckPoint1);

			spl__PaintTargetAreaManagerBancParamSdodr.AddNode("CountFirst", this.CountFirst);

			if (SerializedActor["spl__PaintTargetAreaManagerBancParamSdodr"] == null)
			{
				SerializedActor.Nodes.Add(spl__PaintTargetAreaManagerBancParamSdodr);
			}
		}
	}
}
