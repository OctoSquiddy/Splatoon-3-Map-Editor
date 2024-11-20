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
	public class Mu_spl__MovePainterRailFollowBancParam
	{
		[ByamlMember]
		public ulong LinkToRailPoint { get; set; }

		[ByamlMember]
		public float MaxSpeedKeepSec { get; set; }

		[ByamlMember]
		public float SpeedRate { get; set; }

		public Mu_spl__MovePainterRailFollowBancParam()
		{
			LinkToRailPoint = 0;
			MaxSpeedKeepSec = 0.0f;
			SpeedRate = 0.0f;
		}

		public Mu_spl__MovePainterRailFollowBancParam(Mu_spl__MovePainterRailFollowBancParam other)
		{
			LinkToRailPoint = other.LinkToRailPoint;
			MaxSpeedKeepSec = other.MaxSpeedKeepSec;
			SpeedRate = other.SpeedRate;
		}

		public Mu_spl__MovePainterRailFollowBancParam Clone()
		{
			return new Mu_spl__MovePainterRailFollowBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__MovePainterRailFollowBancParam = new BymlNode.DictionaryNode() { Name = "spl__MovePainterRailFollowBancParam" };

			if (SerializedActor["spl__MovePainterRailFollowBancParam"] != null)
			{
				spl__MovePainterRailFollowBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MovePainterRailFollowBancParam"];
			}

			if (this.LinkToRailPoint != 0)
				spl__MovePainterRailFollowBancParam.AddNode("LinkToRailPoint", this.LinkToRailPoint);

            if (this.MaxSpeedKeepSec > 0.0f)
                spl__MovePainterRailFollowBancParam.AddNode("MaxSpeedKeepSec", this.MaxSpeedKeepSec);

            if (this.SpeedRate > 0.0f)
                spl__MovePainterRailFollowBancParam.AddNode("SpeedRate", this.SpeedRate);

			if (SerializedActor["spl__MovePainterRailFollowBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__MovePainterRailFollowBancParam);
			}
		}
	}
}
