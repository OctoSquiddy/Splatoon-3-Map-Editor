using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class NpcVsJerry_HelmetRuins_00 : MuObj
	{
		[BindGUI("IsEnableMoveChatteringPrevent", Category = "NpcVsJerry_HelmetRuins_00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("ToRailPoint", Category = "NpcVsJerry_HelmetRuins_00 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__ailift__AILiftBancParam")]
		public Mu_spl__ailift__AILiftBancParam spl__ailift__AILiftBancParam { get; set; }

		public NpcVsJerry_HelmetRuins_00() : base()
		{
			spl__ailift__AILiftBancParam = new Mu_spl__ailift__AILiftBancParam();

			Links = new List<Link>();
		}

		public NpcVsJerry_HelmetRuins_00(NpcVsJerry_HelmetRuins_00 other) : base(other)
		{
			spl__ailift__AILiftBancParam = other.spl__ailift__AILiftBancParam.Clone();
		}

		public override NpcVsJerry_HelmetRuins_00 Clone()
		{
			return new NpcVsJerry_HelmetRuins_00(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ailift__AILiftBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
