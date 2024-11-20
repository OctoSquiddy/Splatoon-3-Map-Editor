using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Sdodr_Entrance : MuObj
	{
		[BindGUI("IsCameraReverse", Category = "Sdodr_Entrance Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("StartPosType", Category = "Sdodr_Entrance Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		public Sdodr_Entrance() : base()
		{
			spl__StartPosForSdodrWorldBancParam = new Mu_spl__StartPosForSdodrWorldBancParam();

			Links = new List<Link>();
		}

		public Sdodr_Entrance(Sdodr_Entrance other) : base(other)
		{
			spl__StartPosForSdodrWorldBancParam = other.spl__StartPosForSdodrWorldBancParam.Clone();
		}

		public override Sdodr_Entrance Clone()
		{
			return new Sdodr_Entrance(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__StartPosForSdodrWorldBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
