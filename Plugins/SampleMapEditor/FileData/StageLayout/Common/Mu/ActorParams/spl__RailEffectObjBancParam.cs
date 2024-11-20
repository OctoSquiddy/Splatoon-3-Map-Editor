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
	public class Mu_spl__RailEffectObjBancParam
	{
		public Mu_spl__RailEffectObjBancParam()
		{
		}

		public Mu_spl__RailEffectObjBancParam(Mu_spl__RailEffectObjBancParam other)
		{
		}

		public Mu_spl__RailEffectObjBancParam Clone()
		{
			return new Mu_spl__RailEffectObjBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__RailEffectObjBancParam = new BymlNode.DictionaryNode() { Name = "spl__RailEffectObjBancParam" };

			if (SerializedActor["spl__RailEffectObjBancParam"] != null)
			{
				spl__RailEffectObjBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RailEffectObjBancParam"];
			}


			if (SerializedActor["spl__RailEffectObjBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__RailEffectObjBancParam);
			}
		}
	}
}
