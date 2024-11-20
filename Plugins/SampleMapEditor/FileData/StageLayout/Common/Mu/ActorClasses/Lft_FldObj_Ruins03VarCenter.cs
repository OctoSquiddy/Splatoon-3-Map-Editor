using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Lft_FldObj_Ruins03VarCenter : MuObj
	{
		[BindGUI("IsReverses", Category = "Lft_FldObj_Ruins03VarCenter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsReverses
		{
			get
			{
				return this.spl__LiftWithMoveAnimBancParam.IsReverses;
			}

			set
			{
				this.spl__LiftWithMoveAnimBancParam.IsReverses = value;
			}
		}

		[BindGUI("IsIncludeVArea", Category = "Lft_FldObj_Ruins03VarCenter Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "Lft_FldObj_Ruins03VarCenter Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "Lft_FldObj_Ruins03VarCenter Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("Accel", Category = "Lft_FldObj_Ruins03VarCenter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Accel
		{
			get
			{
				return this.spl__ailift__GachiInterlockBancParam.Accel;
			}

			set
			{
				this.spl__ailift__GachiInterlockBancParam.Accel = value;
			}
		}

		[BindGUI("MaxSpeed", Category = "Lft_FldObj_Ruins03VarCenter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MaxSpeed
		{
			get
			{
				return this.spl__ailift__GachiInterlockBancParam.MaxSpeed;
			}

			set
			{
				this.spl__ailift__GachiInterlockBancParam.MaxSpeed = value;
			}
		}

		[BindGUI("StopFrameNum", Category = "Lft_FldObj_Ruins03VarCenter Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _StopFrameNum
		{
			get
			{
				return this.spl__ailift__GachiInterlockBancParam.StopFrameNum;
			}

			set
			{
				this.spl__ailift__GachiInterlockBancParam.StopFrameNum = value;
			}
		}

		[ByamlMember("spl__LiftWithMoveAnimBancParam")]
		public Mu_spl__LiftWithMoveAnimBancParam spl__LiftWithMoveAnimBancParam { get; set; }

		[ByamlMember("spl__PaintBancParam")]
		public Mu_spl__PaintBancParam spl__PaintBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		[ByamlMember("spl__ailift__GachiInterlockBancParam")]
		public Mu_spl__ailift__GachiInterlockBancParam spl__ailift__GachiInterlockBancParam { get; set; }

		public Lft_FldObj_Ruins03VarCenter() : base()
		{
			spl__LiftWithMoveAnimBancParam = new Mu_spl__LiftWithMoveAnimBancParam();
			spl__PaintBancParam = new Mu_spl__PaintBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();
			spl__ailift__GachiInterlockBancParam = new Mu_spl__ailift__GachiInterlockBancParam();

			Links = new List<Link>();
		}

		public Lft_FldObj_Ruins03VarCenter(Lft_FldObj_Ruins03VarCenter other) : base(other)
		{
			spl__LiftWithMoveAnimBancParam = other.spl__LiftWithMoveAnimBancParam.Clone();
			spl__PaintBancParam = other.spl__PaintBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
			spl__ailift__GachiInterlockBancParam = other.spl__ailift__GachiInterlockBancParam.Clone();
		}

		public override Lft_FldObj_Ruins03VarCenter Clone()
		{
			return new Lft_FldObj_Ruins03VarCenter(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LiftWithMoveAnimBancParam.SaveParameterBank(SerializedActor);
			this.spl__PaintBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__GachiInterlockBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
