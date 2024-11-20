using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class InkRail : MuObj
	{
		[BindGUI("IsOpen", Category = "InkRail Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsOpen
		{
			get
			{
				return this.spl__InkRailBancParam.IsOpen;
			}

			set
			{
				this.spl__InkRailBancParam.IsOpen = value;
			}
		}

		[BindGUI("LinkToPoint", Category = "InkRail Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _LinkToPoint
		{
			get
			{
				return this.spl__InkRailBancParam.LinkToPoint;
			}

			set
			{
				this.spl__InkRailBancParam.LinkToPoint = value;
			}
		}

		[BindGUI("UseBase", Category = "InkRail Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _UseBase
		{
			get
			{
				return this.spl__InkRailBancParam.UseBase;
			}

			set
			{
				this.spl__InkRailBancParam.UseBase = value;
			}
		}

		[ByamlMember("spl__InkRailBancParam")]
		public Mu_spl__InkRailBancParam spl__InkRailBancParam { get; set; }

		public InkRail() : base()
		{
			spl__InkRailBancParam = new Mu_spl__InkRailBancParam();

			Links = new List<Link>();
		}

		public InkRail(InkRail other) : base(other)
		{
			spl__InkRailBancParam = other.spl__InkRailBancParam.Clone();
		}

		public override InkRail Clone()
		{
			return new InkRail(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__InkRailBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
