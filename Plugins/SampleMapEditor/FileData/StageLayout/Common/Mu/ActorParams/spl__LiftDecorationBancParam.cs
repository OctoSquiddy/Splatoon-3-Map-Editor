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
	public class Mu_spl__LiftDecorationBancParam
	{
		public Mu_spl__LiftDecorationBancParam()
		{
		}

		public Mu_spl__LiftDecorationBancParam(Mu_spl__LiftDecorationBancParam other)
		{
		}

		public Mu_spl__LiftDecorationBancParam Clone()
		{
			return new Mu_spl__LiftDecorationBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LiftDecorationBancParam = new BymlNode.DictionaryNode() { Name = "spl__LiftDecorationBancParam" };

			if (SerializedActor["spl__LiftDecorationBancParam"] != null)
			{
				spl__LiftDecorationBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LiftDecorationBancParam"];
			}


			if (SerializedActor["spl__LiftDecorationBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LiftDecorationBancParam);
			}
		}
	}
}
