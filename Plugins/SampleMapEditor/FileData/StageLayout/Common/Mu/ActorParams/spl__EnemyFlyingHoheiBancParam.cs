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
	public class Mu_spl__EnemyFlyingHoheiBancParam
	{
		[ByamlMember]
		public float ExtendFootholdDelaySec { get; set; }

		[ByamlMember]
		public float LifeSpanSec { get; set; }

		public Mu_spl__EnemyFlyingHoheiBancParam()
		{
			ExtendFootholdDelaySec = 0.0f;
			LifeSpanSec = 0.0f;
		}

		public Mu_spl__EnemyFlyingHoheiBancParam(Mu_spl__EnemyFlyingHoheiBancParam other)
		{
			ExtendFootholdDelaySec = other.ExtendFootholdDelaySec;
			LifeSpanSec = other.LifeSpanSec;
		}

		public Mu_spl__EnemyFlyingHoheiBancParam Clone()
		{
			return new Mu_spl__EnemyFlyingHoheiBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyFlyingHoheiBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyFlyingHoheiBancParam" };

			if (SerializedActor["spl__EnemyFlyingHoheiBancParam"] != null)
			{
				spl__EnemyFlyingHoheiBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyFlyingHoheiBancParam"];
			}


			spl__EnemyFlyingHoheiBancParam.AddNode("ExtendFootholdDelaySec", this.ExtendFootholdDelaySec);

			spl__EnemyFlyingHoheiBancParam.AddNode("LifeSpanSec", this.LifeSpanSec);

			if (SerializedActor["spl__EnemyFlyingHoheiBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyFlyingHoheiBancParam);
			}
		}
	}
}
