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
	public class Mu_spl__ExclamationMarkEmitObjBancParam
	{
		public Mu_spl__ExclamationMarkEmitObjBancParam()
		{
		}

		public Mu_spl__ExclamationMarkEmitObjBancParam(Mu_spl__ExclamationMarkEmitObjBancParam other)
		{
		}

		public Mu_spl__ExclamationMarkEmitObjBancParam Clone()
		{
			return new Mu_spl__ExclamationMarkEmitObjBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ExclamationMarkEmitObjBancParam = new BymlNode.DictionaryNode() { Name = "spl__ExclamationMarkEmitObjBancParam" };

			if (SerializedActor["spl__ExclamationMarkEmitObjBancParam"] != null)
			{
				spl__ExclamationMarkEmitObjBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ExclamationMarkEmitObjBancParam"];
			}


			if (SerializedActor["spl__ExclamationMarkEmitObjBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ExclamationMarkEmitObjBancParam);
			}
		}
	}
}
