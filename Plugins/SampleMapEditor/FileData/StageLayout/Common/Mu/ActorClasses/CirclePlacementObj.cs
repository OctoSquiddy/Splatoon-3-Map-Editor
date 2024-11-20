using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CirclePlacementObj : MuObj
	{
		[BindGUI("IsStretch", Category = "CirclePlacementObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsStretch
		{
			get
			{
				return this.spl__CirclePlacementObjBancParam.IsStretch;
			}

			set
			{
				this.spl__CirclePlacementObjBancParam.IsStretch = value;
			}
		}

		[BindGUI("MaxBaseLength", Category = "CirclePlacementObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MaxBaseLength
		{
			get
			{
				return this.spl__CirclePlacementObjBancParam.MaxBaseLength;
			}

			set
			{
				this.spl__CirclePlacementObjBancParam.MaxBaseLength = value;
			}
		}

		[BindGUI("MinBaseLength", Category = "CirclePlacementObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MinBaseLength
		{
			get
			{
				return this.spl__CirclePlacementObjBancParam.MinBaseLength;
			}

			set
			{
				this.spl__CirclePlacementObjBancParam.MinBaseLength = value;
			}
		}

		[BindGUI("ObjNum", Category = "CirclePlacementObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _ObjNum
		{
			get
			{
				return this.spl__CirclePlacementObjBancParam.ObjNum;
			}

			set
			{
				this.spl__CirclePlacementObjBancParam.ObjNum = value;
			}
		}

		[BindGUI("Radius", Category = "CirclePlacementObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Radius
		{
			get
			{
				return this.spl__CirclePlacementObjBancParam.Radius;
			}

			set
			{
				this.spl__CirclePlacementObjBancParam.Radius = value;
			}
		}

		[BindGUI("RotateSpeedDegPerSec", Category = "CirclePlacementObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _RotateSpeedDegPerSec
		{
			get
			{
				return this.spl__CirclePlacementObjBancParam.RotateSpeedDegPerSec;
			}

			set
			{
				this.spl__CirclePlacementObjBancParam.RotateSpeedDegPerSec = value;
			}
		}

		[BindGUI("StopFrame", Category = "CirclePlacementObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _StopFrame
		{
			get
			{
				return this.spl__CirclePlacementObjBancParam.StopFrame;
			}

			set
			{
				this.spl__CirclePlacementObjBancParam.StopFrame = value;
			}
		}

		[BindGUI("StretchFrame", Category = "CirclePlacementObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _StretchFrame
		{
			get
			{
				return this.spl__CirclePlacementObjBancParam.StretchFrame;
			}

			set
			{
				this.spl__CirclePlacementObjBancParam.StretchFrame = value;
			}
		}

		[ByamlMember("spl__CirclePlacementObjBancParam")]
		public Mu_spl__CirclePlacementObjBancParam spl__CirclePlacementObjBancParam { get; set; }

		public CirclePlacementObj() : base()
		{
			spl__CirclePlacementObjBancParam = new Mu_spl__CirclePlacementObjBancParam();

			Links = new List<Link>();
		}

		public CirclePlacementObj(CirclePlacementObj other) : base(other)
		{
			spl__CirclePlacementObjBancParam = other.spl__CirclePlacementObjBancParam.Clone();
		}

		public override CirclePlacementObj Clone()
		{
			return new CirclePlacementObj(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CirclePlacementObjBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
