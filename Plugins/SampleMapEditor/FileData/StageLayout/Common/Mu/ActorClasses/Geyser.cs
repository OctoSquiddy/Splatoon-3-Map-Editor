using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Geyser : MuObj
	{
		[BindGUI("MaxHeight", Category = "Geyser Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MaxHeight
		{
			get
			{
				return this.spl__GeyserBancParam.MaxHeight;
			}

			set
			{
				this.spl__GeyserBancParam.MaxHeight = value;
			}
		}

		[BindGUI("IsEnableMoveChatteringPrevent", Category = "Geyser Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "Geyser Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__GeyserBancParam")]
		public Mu_spl__GeyserBancParam spl__GeyserBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public Geyser() : base()
		{
			spl__GeyserBancParam = new Mu_spl__GeyserBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public Geyser(Geyser other) : base(other)
		{
			spl__GeyserBancParam = other.spl__GeyserBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override Geyser Clone()
		{
			return new Geyser(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__GeyserBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
