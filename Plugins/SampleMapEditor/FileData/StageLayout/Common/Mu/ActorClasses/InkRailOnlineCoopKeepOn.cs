using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class InkRailOnlineCoopKeepOn : MuObj
	{
		[BindGUI("IsOpen", Category = "InkRailOnlineCoopKeepOn Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("LinkToPoint", Category = "InkRailOnlineCoopKeepOn Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("UseBase", Category = "InkRailOnlineCoopKeepOn Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsActiveInHigh", Category = "InkRailOnlineCoopKeepOn Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActiveInHigh
		{
			get
			{
				return this.spl__InkRailCoopBancParam.IsActiveInHigh;
			}

			set
			{
				this.spl__InkRailCoopBancParam.IsActiveInHigh = value;
			}
		}

		[BindGUI("IsActiveInLow", Category = "InkRailOnlineCoopKeepOn Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActiveInLow
		{
			get
			{
				return this.spl__InkRailCoopBancParam.IsActiveInLow;
			}

			set
			{
				this.spl__InkRailCoopBancParam.IsActiveInLow = value;
			}
		}

		[BindGUI("IsActiveInMid", Category = "InkRailOnlineCoopKeepOn Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActiveInMid
		{
			get
			{
				return this.spl__InkRailCoopBancParam.IsActiveInMid;
			}

			set
			{
				this.spl__InkRailCoopBancParam.IsActiveInMid = value;
			}
		}

		[BindGUI("IsActiveInRelay", Category = "InkRailOnlineCoopKeepOn Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActiveInRelay
		{
			get
			{
				return this.spl__InkRailCoopBancParam.IsActiveInRelay;
			}

			set
			{
				this.spl__InkRailCoopBancParam.IsActiveInRelay = value;
			}
		}

		[ByamlMember("spl__InkRailBancParam")]
		public Mu_spl__InkRailBancParam spl__InkRailBancParam { get; set; }

		[ByamlMember("spl__InkRailCoopBancParam")]
		public Mu_spl__InkRailCoopBancParam spl__InkRailCoopBancParam { get; set; }

		public InkRailOnlineCoopKeepOn() : base()
		{
			spl__InkRailBancParam = new Mu_spl__InkRailBancParam();
			spl__InkRailCoopBancParam = new Mu_spl__InkRailCoopBancParam();

			Links = new List<Link>();
		}

		public InkRailOnlineCoopKeepOn(InkRailOnlineCoopKeepOn other) : base(other)
		{
			spl__InkRailBancParam = other.spl__InkRailBancParam.Clone();
			spl__InkRailCoopBancParam = other.spl__InkRailCoopBancParam.Clone();
		}

		public override InkRailOnlineCoopKeepOn Clone()
		{
			return new InkRailOnlineCoopKeepOn(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__InkRailBancParam.SaveParameterBank(SerializedActor);
			this.spl__InkRailCoopBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
