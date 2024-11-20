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
	public class Mu_spl__ObjIdolSyncerBancParam
	{
		public Mu_spl__ObjIdolSyncerBancParam()
		{
		}

		public Mu_spl__ObjIdolSyncerBancParam(Mu_spl__ObjIdolSyncerBancParam other)
		{
		}

		public Mu_spl__ObjIdolSyncerBancParam Clone()
		{
			return new Mu_spl__ObjIdolSyncerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ObjIdolSyncerBancParam = new BymlNode.DictionaryNode() { Name = "spl__ObjIdolSyncerBancParam" };

			if (SerializedActor["spl__ObjIdolSyncerBancParam"] != null)
			{
				spl__ObjIdolSyncerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ObjIdolSyncerBancParam"];
			}


			if (SerializedActor["spl__ObjIdolSyncerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ObjIdolSyncerBancParam);
			}
		}
	}
}
