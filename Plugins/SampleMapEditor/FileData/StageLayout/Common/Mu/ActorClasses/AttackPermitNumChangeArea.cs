using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class AttackPermitNumChangeArea : MuObj
	{
		[BindGUI("LimitNum", Category = "AttackPermitNumChangeArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _LimitNum
		{
			get
			{
				return this.spl__AttackPermitNumChangeAreaBancParam.LimitNum;
			}

			set
			{
				this.spl__AttackPermitNumChangeAreaBancParam.LimitNum = value;
			}
		}

		[ByamlMember("spl__AttackPermitNumChangeAreaBancParam")]
		public Mu_spl__AttackPermitNumChangeAreaBancParam spl__AttackPermitNumChangeAreaBancParam { get; set; }

		public AttackPermitNumChangeArea() : base()
		{
			spl__AttackPermitNumChangeAreaBancParam = new Mu_spl__AttackPermitNumChangeAreaBancParam();

			Links = new List<Link>();
		}

		public AttackPermitNumChangeArea(AttackPermitNumChangeArea other) : base(other)
		{
			spl__AttackPermitNumChangeAreaBancParam = other.spl__AttackPermitNumChangeAreaBancParam.Clone();
		}

		public override AttackPermitNumChangeArea Clone()
		{
			return new AttackPermitNumChangeArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__AttackPermitNumChangeAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
