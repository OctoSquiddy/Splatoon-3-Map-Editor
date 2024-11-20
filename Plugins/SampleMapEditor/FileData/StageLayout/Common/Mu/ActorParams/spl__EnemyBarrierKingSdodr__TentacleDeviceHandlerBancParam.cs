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
	public class Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam
	{
		public Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam()
		{
		}

		public Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam(Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam other)
		{
		}

		public Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam Clone()
		{
			return new Mu_spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam" };

			if (SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"] != null)
			{
				spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"];
			}


			if (SerializedActor["spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyBarrierKingSdodr__TentacleDeviceHandlerBancParam);
			}
		}
	}
}
