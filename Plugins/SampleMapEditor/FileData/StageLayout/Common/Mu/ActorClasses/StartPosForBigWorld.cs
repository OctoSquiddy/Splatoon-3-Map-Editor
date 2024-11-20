using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class StartPosForBigWorld : MuObj
	{
		[BindGUI("StartPosType", Category = "StartPosForBigWorld Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _StartPosType
		{
			get
			{
				return this.spl__StartPosForBigWorldBancParam.StartPosType;
			}

			set
			{
				this.spl__StartPosForBigWorldBancParam.StartPosType = value;
			}
		}

		[ByamlMember("spl__StartPosForBigWorldBancParam")]
		public Mu_spl__StartPosForBigWorldBancParam spl__StartPosForBigWorldBancParam { get; set; }

		public StartPosForBigWorld() : base()
		{
			spl__StartPosForBigWorldBancParam = new Mu_spl__StartPosForBigWorldBancParam();

			Links = new List<Link>();
		}

		public StartPosForBigWorld(StartPosForBigWorld other) : base(other)
		{
			spl__StartPosForBigWorldBancParam = other.spl__StartPosForBigWorldBancParam.Clone();
		}

		public override StartPosForBigWorld Clone()
		{
			return new StartPosForBigWorld(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__StartPosForBigWorldBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
