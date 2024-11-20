using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class RailingObj : MuObj
	{
		[BindGUI("ObjNum", Category = "RailingObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _ObjNum
		{
			get
			{
				return this.spl__RailingObjBancParam.ObjNum;
			}

			set
			{
				this.spl__RailingObjBancParam.ObjNum = value;
			}
		}

		[BindGUI("ObjRailingType", Category = "RailingObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ObjRailingType
		{
			get
			{
				return this.spl__RailingObjBancParam.ObjRailingType;
			}

			set
			{
				this.spl__RailingObjBancParam.ObjRailingType = value;
			}
		}

		[BindGUI("OffsetType", Category = "RailingObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _OffsetType
		{
			get
			{
				return this.spl__RailingObjBancParam.OffsetType;
			}

			set
			{
				this.spl__RailingObjBancParam.OffsetType = value;
			}
		}

		[BindGUI("OrderMargin", Category = "RailingObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _OrderMargin
		{
			get
			{
				return this.spl__RailingObjBancParam.OrderMargin;
			}

			set
			{
				this.spl__RailingObjBancParam.OrderMargin = value;
			}
		}

		[BindGUI("OrderOffset", Category = "RailingObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _OrderOffset
		{
			get
			{
				return this.spl__RailingObjBancParam.OrderOffset;
			}

			set
			{
				this.spl__RailingObjBancParam.OrderOffset = value;
			}
		}

		[BindGUI("PosOffset", Category = "RailingObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public OpenTK.Vector3 _PosOffset
		{
			get
			{
				return new OpenTK.Vector3(
					this.spl__RailingObjBancParam.PosOffset.X,
					this.spl__RailingObjBancParam.PosOffset.Y,
					this.spl__RailingObjBancParam.PosOffset.Z);
			}

			set
			{
				this.spl__RailingObjBancParam.PosOffset = new ByamlVector3F(value.X, value.Y, value.Z);
			}
		}

		[BindGUI("ToRailPoint", Category = "RailingObj Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__RailingObjBancParam.ToRailPoint;
			}

			set
			{
				this.spl__RailingObjBancParam.ToRailPoint = value;
			}
		}

		[ByamlMember("spl__RailingObjBancParam")]
		public Mu_spl__RailingObjBancParam spl__RailingObjBancParam { get; set; }

		public RailingObj() : base()
		{
			spl__RailingObjBancParam = new Mu_spl__RailingObjBancParam();

			Links = new List<Link>();
		}

		public RailingObj(RailingObj other) : base(other)
		{
			spl__RailingObjBancParam = other.spl__RailingObjBancParam.Clone();
		}

		public override RailingObj Clone()
		{
			return new RailingObj(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__RailingObjBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
