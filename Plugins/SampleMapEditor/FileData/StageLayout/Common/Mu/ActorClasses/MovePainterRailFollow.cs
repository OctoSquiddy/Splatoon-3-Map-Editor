using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class MovePainterRailFollow : MuObj
	{
		[BindGUI("LinkToRailPoint", Category = "MovePainterRailFollow Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _LinkToRailPoint
		{
			get
			{
				return this.spl__MovePainterRailFollowBancParam.LinkToRailPoint;
			}

			set
			{
				this.spl__MovePainterRailFollowBancParam.LinkToRailPoint = value;
			}
		}

		[BindGUI("MaxSpeedKeepSec", Category = "MovePainterRailFollow Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MaxSpeedKeepSec
		{
			get
			{
				return this.spl__MovePainterRailFollowBancParam.MaxSpeedKeepSec;
			}

			set
			{
				this.spl__MovePainterRailFollowBancParam.MaxSpeedKeepSec = value;
			}
		}

		[BindGUI("SpeedRate", Category = "MovePainterRailFollow Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _SpeedRate
		{
			get
			{
				return this.spl__MovePainterRailFollowBancParam.SpeedRate;
			}

			set
			{
				this.spl__MovePainterRailFollowBancParam.SpeedRate = value;
			}
		}

		[ByamlMember("spl__MovePainterRailFollowBancParam")]
		public Mu_spl__MovePainterRailFollowBancParam spl__MovePainterRailFollowBancParam { get; set; }

		public MovePainterRailFollow() : base()
		{
			spl__MovePainterRailFollowBancParam = new Mu_spl__MovePainterRailFollowBancParam();

			Links = new List<Link>();
		}

		public MovePainterRailFollow(MovePainterRailFollow other) : base(other)
		{
			spl__MovePainterRailFollowBancParam = other.spl__MovePainterRailFollowBancParam.Clone();
		}

		public override MovePainterRailFollow Clone()
		{
			return new MovePainterRailFollow(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__MovePainterRailFollowBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
