using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class InkRailSdodr : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "InkRailSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActivateOnlyInBeingPerformer
		{
			get
			{
				return this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer;
			}

			set
			{
				this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer = value;
			}
		}

		[BindGUI("IsOpen", Category = "InkRailSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("LinkToPoint", Category = "InkRailSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("UseBase", Category = "InkRailSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("game__EventPerformerBancParam")]
		public Mu_game__EventPerformerBancParam game__EventPerformerBancParam { get; set; }

		[ByamlMember("spl__InkRailBancParam")]
		public Mu_spl__InkRailBancParam spl__InkRailBancParam { get; set; }

		public InkRailSdodr() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__InkRailBancParam = new Mu_spl__InkRailBancParam();

			Links = new List<Link>();
		}

		public InkRailSdodr(InkRailSdodr other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__InkRailBancParam = other.spl__InkRailBancParam.Clone();
		}

		public override InkRailSdodr Clone()
		{
			return new InkRailSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__InkRailBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
