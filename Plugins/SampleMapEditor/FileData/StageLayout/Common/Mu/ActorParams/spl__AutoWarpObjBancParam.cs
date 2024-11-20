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
	public class Mu_spl__AutoWarpObjBancParam
	{
		public Mu_spl__AutoWarpObjBancParam()
		{
		}

		public Mu_spl__AutoWarpObjBancParam(Mu_spl__AutoWarpObjBancParam other)
		{
		}

		public Mu_spl__AutoWarpObjBancParam Clone()
		{
			return new Mu_spl__AutoWarpObjBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__AutoWarpObjBancParam = new BymlNode.DictionaryNode() { Name = "spl__AutoWarpObjBancParam" };

			if (SerializedActor["spl__AutoWarpObjBancParam"] != null)
			{
				spl__AutoWarpObjBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__AutoWarpObjBancParam"];
			}


			if (SerializedActor["spl__AutoWarpObjBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__AutoWarpObjBancParam);
			}
		}
	}
}
