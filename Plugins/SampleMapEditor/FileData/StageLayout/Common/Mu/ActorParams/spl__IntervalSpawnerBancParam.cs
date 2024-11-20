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
	public class Mu_spl__IntervalSpawnerBancParam
	{
		[ByamlMember]
		public float DelaySec { get; set; }

		[ByamlMember]
		public int DropIkuraEnemyLimitNum { get; set; }

		[ByamlMember]
		public float IntervalSec { get; set; }

		[ByamlMember]
		public bool IsAlwaysOpen { get; set; }

		[ByamlMember]
		public ulong LinkToRailForMove { get; set; }

		[ByamlMember]
		public int ReserveNum { get; set; }

		public Mu_spl__IntervalSpawnerBancParam()
		{
			DelaySec = 0.0f;
			DropIkuraEnemyLimitNum = 0;
			IntervalSec = 0.0f;
			IsAlwaysOpen = false;
			LinkToRailForMove = 0;
			ReserveNum = 0;
		}

		public Mu_spl__IntervalSpawnerBancParam(Mu_spl__IntervalSpawnerBancParam other)
		{
			DelaySec = other.DelaySec;
			DropIkuraEnemyLimitNum = other.DropIkuraEnemyLimitNum;
			IntervalSec = other.IntervalSec;
			IsAlwaysOpen = other.IsAlwaysOpen;
			LinkToRailForMove = other.LinkToRailForMove;
			ReserveNum = other.ReserveNum;
		}

		public Mu_spl__IntervalSpawnerBancParam Clone()
		{
			return new Mu_spl__IntervalSpawnerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__IntervalSpawnerBancParam = new BymlNode.DictionaryNode() { Name = "spl__IntervalSpawnerBancParam" };

			if (SerializedActor["spl__IntervalSpawnerBancParam"] != null)
			{
				spl__IntervalSpawnerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__IntervalSpawnerBancParam"];
			}

			if (this.DelaySec > 0.0f)
				spl__IntervalSpawnerBancParam.AddNode("DelaySec", this.DelaySec);

            if (this.DropIkuraEnemyLimitNum > 0)
                spl__IntervalSpawnerBancParam.AddNode("DropIkuraEnemyLimitNum", this.DropIkuraEnemyLimitNum);

            if (this.IntervalSec > 0.0f)
                spl__IntervalSpawnerBancParam.AddNode("IntervalSec", this.IntervalSec);

			if (this.IsAlwaysOpen)
			{
				spl__IntervalSpawnerBancParam.AddNode("IsAlwaysOpen", this.IsAlwaysOpen);
			}

            if (this.LinkToRailForMove != 0)
                spl__IntervalSpawnerBancParam.AddNode("LinkToRailForMove", this.LinkToRailForMove);

            if (this.DropIkuraEnemyLimitNum > 0)
                spl__IntervalSpawnerBancParam.AddNode("ReserveNum", this.ReserveNum);

			if (SerializedActor["spl__IntervalSpawnerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__IntervalSpawnerBancParam);
			}
		}
	}
}
