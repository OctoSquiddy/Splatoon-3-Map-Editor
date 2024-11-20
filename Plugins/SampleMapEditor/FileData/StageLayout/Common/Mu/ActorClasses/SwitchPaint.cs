using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SwitchPaint : MuObj
	{
		[BindGUI("IsEnableMoveChatteringPrevent", Category = "SwitchPaint Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "SwitchPaint Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__SwitchPaintBancParam")]
		public Mu_spl__SwitchPaintBancParam spl__SwitchPaintBancParam { get; set; }

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public SwitchPaint() : base()
		{
			spl__SwitchPaintBancParam = new Mu_spl__SwitchPaintBancParam();
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public SwitchPaint(SwitchPaint other) : base(other)
		{
			spl__SwitchPaintBancParam = other.spl__SwitchPaintBancParam.Clone();
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override SwitchPaint Clone()
		{
			return new SwitchPaint(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SwitchPaintBancParam.SaveParameterBank(SerializedActor);
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}