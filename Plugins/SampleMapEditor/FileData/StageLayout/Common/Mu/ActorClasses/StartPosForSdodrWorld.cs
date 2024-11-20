using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class StartPosForSdodrWorld : MuObj
	{
		[BindGUI("IsCameraReverse", Category = "StartPosForSdodrWorld Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsCameraReverse
		{
			get
			{
				return this.spl__StartPosForSdodrWorldBancParam.IsCameraReverse;
			}

			set
			{
				this.spl__StartPosForSdodrWorldBancParam.IsCameraReverse = value;
			}
		}

		[BindGUI("StartPosType", Category = "StartPosForSdodrWorld Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _StartPosType
		{
			get
			{
				return this.spl__StartPosForSdodrWorldBancParam.StartPosType;
			}

			set
			{
				this.spl__StartPosForSdodrWorldBancParam.StartPosType = value;
			}
		}

		[ByamlMember("spl__StartPosForSdodrWorldBancParam")]
		public Mu_spl__StartPosForSdodrWorldBancParam spl__StartPosForSdodrWorldBancParam { get; set; }

		public StartPosForSdodrWorld() : base()
		{
			spl__StartPosForSdodrWorldBancParam = new Mu_spl__StartPosForSdodrWorldBancParam();

			Links = new List<Link>();
		}

		public StartPosForSdodrWorld(StartPosForSdodrWorld other) : base(other)
		{
			spl__StartPosForSdodrWorldBancParam = other.spl__StartPosForSdodrWorldBancParam.Clone();
		}

		public override StartPosForSdodrWorld Clone()
		{
			return new StartPosForSdodrWorld(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__StartPosForSdodrWorldBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
