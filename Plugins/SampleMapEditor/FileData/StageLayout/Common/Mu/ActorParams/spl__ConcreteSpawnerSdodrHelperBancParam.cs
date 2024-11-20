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
	public class Mu_spl__ConcreteSpawnerSdodrHelperBancParam
	{
		[ByamlMember]
		public bool IsSpawnDirSpecified { get; set; }

		public Mu_spl__ConcreteSpawnerSdodrHelperBancParam()
		{
			IsSpawnDirSpecified = false;
		}

		public Mu_spl__ConcreteSpawnerSdodrHelperBancParam(Mu_spl__ConcreteSpawnerSdodrHelperBancParam other)
		{
			IsSpawnDirSpecified = other.IsSpawnDirSpecified;
		}

		public Mu_spl__ConcreteSpawnerSdodrHelperBancParam Clone()
		{
			return new Mu_spl__ConcreteSpawnerSdodrHelperBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ConcreteSpawnerSdodrHelperBancParam = new BymlNode.DictionaryNode() { Name = "spl__ConcreteSpawnerSdodrHelperBancParam" };

			if (SerializedActor["spl__ConcreteSpawnerSdodrHelperBancParam"] != null)
			{
				spl__ConcreteSpawnerSdodrHelperBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ConcreteSpawnerSdodrHelperBancParam"];
			}


			if (this.IsSpawnDirSpecified)
			{
				spl__ConcreteSpawnerSdodrHelperBancParam.AddNode("IsSpawnDirSpecified", this.IsSpawnDirSpecified);
			}

			if (SerializedActor["spl__ConcreteSpawnerSdodrHelperBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ConcreteSpawnerSdodrHelperBancParam);
			}
		}
	}
}
