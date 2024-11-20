using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Gachiyagura_4M : MuObj
	{
		[BindGUI("ToRailPoint", Category = "Gachiyagura_4M Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__GachiyaguraBancParam.ToRailPoint;
			}

			set
			{
				this.spl__GachiyaguraBancParam.ToRailPoint = value;
			}
		}

		[ByamlMember("spl__GachiyaguraBancParam")]
		public Mu_spl__GachiyaguraBancParam spl__GachiyaguraBancParam { get; set; }

		public Gachiyagura_4M() : base()
		{
			spl__GachiyaguraBancParam = new Mu_spl__GachiyaguraBancParam();

			Links = new List<Link>();
		}

		public Gachiyagura_4M(Gachiyagura_4M other) : base(other)
		{
			spl__GachiyaguraBancParam = other.spl__GachiyaguraBancParam.Clone();
		}

		public override Gachiyagura_4M Clone()
		{
			return new Gachiyagura_4M(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__GachiyaguraBancParam.SaveParameterBank(SerializedActor);
		}
	}
}