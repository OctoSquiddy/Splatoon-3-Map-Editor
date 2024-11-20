using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class MovePainterFixedDirection : MuObj
	{
		[BindGUI("IsWallUpEnabled", Category = "MovePainterFixedDirection Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsWallUpEnabled
		{
			get
			{
				return this.spl__MovePainterBancParam.IsWallUpEnabled;
			}

			set
			{
				this.spl__MovePainterBancParam.IsWallUpEnabled = value;
			}
		}

		[BindGUI("MaxSpeedKeepSec", Category = "MovePainterFixedDirection Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MaxSpeedKeepSec
		{
			get
			{
				return this.spl__MovePainterBancParam.MaxSpeedKeepSec;
			}

			set
			{
				this.spl__MovePainterBancParam.MaxSpeedKeepSec = value;
			}
		}

		[BindGUI("SpeedRate", Category = "MovePainterFixedDirection Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _SpeedRate
		{
			get
			{
				return this.spl__MovePainterBancParam.SpeedRate;
			}

			set
			{
				this.spl__MovePainterBancParam.SpeedRate = value;
			}
		}

		[ByamlMember("spl__MovePainterBancParam")]
		public Mu_spl__MovePainterBancParam spl__MovePainterBancParam { get; set; }

		public MovePainterFixedDirection() : base()
		{
			spl__MovePainterBancParam = new Mu_spl__MovePainterBancParam();

			Links = new List<Link>();
		}

		public MovePainterFixedDirection(MovePainterFixedDirection other) : base(other)
		{
			spl__MovePainterBancParam = other.spl__MovePainterBancParam.Clone();
		}

		public override MovePainterFixedDirection Clone()
		{
			return new MovePainterFixedDirection(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__MovePainterBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
