using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Lft_Bridge_AbstractRotateToggle : MuObj
	{
		[BindGUI("FreeRotateVelDegPerSec", Category = "Lft_Bridge_AbstractRotateToggle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public OpenTK.Vector3 _FreeRotateVelDegPerSec
		{
			get
			{
				return new OpenTK.Vector3(
					this.game__SequentialRotateParam.FreeRotateVelDegPerSec.X,
					this.game__SequentialRotateParam.FreeRotateVelDegPerSec.Y,
					this.game__SequentialRotateParam.FreeRotateVelDegPerSec.Z);
			}

			set
			{
				this.game__SequentialRotateParam.FreeRotateVelDegPerSec = new ByamlVector3F(value.X, value.Y, value.Z);
			}
		}

		[BindGUI("OneTimeBreakTime", Category = "Lft_Bridge_AbstractRotateToggle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _OneTimeBreakTime
		{
			get
			{
				return this.game__SequentialRotateParam.OneTimeBreakTime;
			}

			set
			{
				this.game__SequentialRotateParam.OneTimeBreakTime = value;
			}
		}

		[BindGUI("OneTimeReverseBreakTime", Category = "Lft_Bridge_AbstractRotateToggle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _OneTimeReverseBreakTime
		{
			get
			{
				return this.game__SequentialRotateParam.OneTimeReverseBreakTime;
			}

			set
			{
				this.game__SequentialRotateParam.OneTimeReverseBreakTime = value;
			}
		}

		[BindGUI("OneTimeRotateAngleDeg", Category = "Lft_Bridge_AbstractRotateToggle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public OpenTK.Vector3 _OneTimeRotateAngleDeg
		{
			get
			{
				return new OpenTK.Vector3(
					this.game__SequentialRotateParam.OneTimeRotateAngleDeg.X,
					this.game__SequentialRotateParam.OneTimeRotateAngleDeg.Y,
					this.game__SequentialRotateParam.OneTimeRotateAngleDeg.Z);
			}

			set
			{
				this.game__SequentialRotateParam.OneTimeRotateAngleDeg = new ByamlVector3F(value.X, value.Y, value.Z);
			}
		}

		[BindGUI("OneTimeRotateTime", Category = "Lft_Bridge_AbstractRotateToggle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _OneTimeRotateTime
		{
			get
			{
				return this.game__SequentialRotateParam.OneTimeRotateTime;
			}

			set
			{
				this.game__SequentialRotateParam.OneTimeRotateTime = value;
			}
		}

		[BindGUI("OneTimeRotateType", Category = "Lft_Bridge_AbstractRotateToggle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _OneTimeRotateType
		{
			get
			{
				return this.game__SequentialRotateParam.OneTimeRotateType;
			}

			set
			{
				this.game__SequentialRotateParam.OneTimeRotateType = value;
			}
		}

		[BindGUI("WaitTime", Category = "Lft_Bridge_AbstractRotateToggle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _WaitTime
		{
			get
			{
				return this.game__SequentialRotateParam.WaitTime;
			}

			set
			{
				this.game__SequentialRotateParam.WaitTime = value;
			}
		}

		[BindGUI("IsIncludeVArea", Category = "Lft_Bridge_AbstractRotateToggle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsIncludeVArea
		{
			get
			{
				return this.spl__PaintBancParam.IsIncludeVArea;
			}

			set
			{
				this.spl__PaintBancParam.IsIncludeVArea = value;
			}
		}

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "Lft_Bridge_AbstractRotateToggle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEnableMoveChatteringPrevent
		{
			get
			{
				return this.spl__ailift__AILiftBancParam.IsEnableMoveChatteringPrevent;
			}

			set
			{
				this.spl__ailift__AILiftBancParam.IsEnableMoveChatteringPrevent = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "Lft_Bridge_AbstractRotateToggle Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__ailift__AILiftBancParam.ToRailPoint;
			}

			set
			{
				this.spl__ailift__AILiftBancParam.ToRailPoint = value;
			}
		}

		[ByamlMember("game__SequentialRotateParam")]
		public Mu_game__SequentialRotateParam game__SequentialRotateParam { get; set; }

		[ByamlMember("spl__PaintBancParam")]
		public Mu_spl__PaintBancParam spl__PaintBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public Lft_Bridge_AbstractRotateToggle() : base()
		{
			game__SequentialRotateParam = new Mu_game__SequentialRotateParam();
			spl__PaintBancParam = new Mu_spl__PaintBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public Lft_Bridge_AbstractRotateToggle(Lft_Bridge_AbstractRotateToggle other) : base(other)
		{
			game__SequentialRotateParam = other.game__SequentialRotateParam.Clone();
			spl__PaintBancParam = other.spl__PaintBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override Lft_Bridge_AbstractRotateToggle Clone()
		{
			return new Lft_Bridge_AbstractRotateToggle(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__SequentialRotateParam.SaveParameterBank(SerializedActor);
			this.spl__PaintBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
