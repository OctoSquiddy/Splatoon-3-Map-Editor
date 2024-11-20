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
	public class Mu_spl__LocatorSpawnerBancParam
	{
		[ByamlMember]
		public float CameraPitDeg { get; set; }

		public Mu_spl__LocatorSpawnerBancParam()
		{
			CameraPitDeg = 0.0f;
		}

		public Mu_spl__LocatorSpawnerBancParam(Mu_spl__LocatorSpawnerBancParam other)
		{
			CameraPitDeg = other.CameraPitDeg;
		}

		public Mu_spl__LocatorSpawnerBancParam Clone()
		{
			return new Mu_spl__LocatorSpawnerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorSpawnerBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorSpawnerBancParam" };

			if (SerializedActor["spl__LocatorSpawnerBancParam"] != null)
			{
				spl__LocatorSpawnerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorSpawnerBancParam"];
			}


			spl__LocatorSpawnerBancParam.AddNode("CameraPitDeg", this.CameraPitDeg);

			if (SerializedActor["spl__LocatorSpawnerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorSpawnerBancParam);
			}
		}
	}
}
