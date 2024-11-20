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
	public class Mu_spl__MovePainterBancParam
	{
		[ByamlMember]
		public bool IsWallUpEnabled { get; set; }

		[ByamlMember]
		public float MaxSpeedKeepSec { get; set; }

		[ByamlMember]
		public float SpeedRate { get; set; }

		public Mu_spl__MovePainterBancParam()
		{
			IsWallUpEnabled = false;
			MaxSpeedKeepSec = 0.0f;
			SpeedRate = 0.0f;
		}

		public Mu_spl__MovePainterBancParam(Mu_spl__MovePainterBancParam other)
		{
			IsWallUpEnabled = other.IsWallUpEnabled;
			MaxSpeedKeepSec = other.MaxSpeedKeepSec;
			SpeedRate = other.SpeedRate;
		}

		public Mu_spl__MovePainterBancParam Clone()
		{
			return new Mu_spl__MovePainterBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__MovePainterBancParam = new BymlNode.DictionaryNode() { Name = "spl__MovePainterBancParam" };

			if (SerializedActor["spl__MovePainterBancParam"] != null)
			{
				spl__MovePainterBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MovePainterBancParam"];
			}


			if (this.IsWallUpEnabled)
			{
				spl__MovePainterBancParam.AddNode("IsWallUpEnabled", this.IsWallUpEnabled);
			}

			spl__MovePainterBancParam.AddNode("MaxSpeedKeepSec", this.MaxSpeedKeepSec);

			spl__MovePainterBancParam.AddNode("SpeedRate", this.SpeedRate);

			if (SerializedActor["spl__MovePainterBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__MovePainterBancParam);
			}
		}
	}
}
