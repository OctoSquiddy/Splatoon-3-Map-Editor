using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class GrindRailBigWorld : MuObj
	{
		[BindGUI("IsCameraReset", Category = "GrindRailBigWorld Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsCameraReset
		{
			get
			{
				return this.spl__GrindRailBancParam.IsCameraReset;
			}

			set
			{
				this.spl__GrindRailBancParam.IsCameraReset = value;
			}
		}

		[BindGUI("IsLightModel", Category = "GrindRailBigWorld Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsLightModel
		{
			get
			{
				return this.spl__GrindRailBancParam.IsLightModel;
			}

			set
			{
				this.spl__GrindRailBancParam.IsLightModel = value;
			}
		}

		[BindGUI("IsOpen", Category = "GrindRailBigWorld Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsOpen
		{
			get
			{
				return this.spl__GrindRailBancParam.IsOpen;
			}

			set
			{
				this.spl__GrindRailBancParam.IsOpen = value;
			}
		}

		[BindGUI("IsUseBase", Category = "GrindRailBigWorld Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsUseBase
		{
			get
			{
				return this.spl__GrindRailBancParam.IsUseBase;
			}

			set
			{
				this.spl__GrindRailBancParam.IsUseBase = value;
			}
		}

		[BindGUI("LinkToRail", Category = "GrindRailBigWorld Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _LinkToRail
		{
			get
			{
				return this.spl__GrindRailBancParam.LinkToRail;
			}

			set
			{
				this.spl__GrindRailBancParam.LinkToRail = value;
			}
		}

		[ByamlMember("spl__GrindRailBancParam")]
		public Mu_spl__GrindRailBancParam spl__GrindRailBancParam { get; set; }

		public GrindRailBigWorld() : base()
		{
			spl__GrindRailBancParam = new Mu_spl__GrindRailBancParam();

			Links = new List<Link>();
		}

		public GrindRailBigWorld(GrindRailBigWorld other) : base(other)
		{
			spl__GrindRailBancParam = other.spl__GrindRailBancParam.Clone();
		}

		public override GrindRailBigWorld Clone()
		{
			return new GrindRailBigWorld(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__GrindRailBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
