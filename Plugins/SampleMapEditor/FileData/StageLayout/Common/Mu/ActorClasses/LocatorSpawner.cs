using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorSpawner : MuObj
	{
		[BindGUI("CameraPitDeg", Category = "LocatorSpawner Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _CameraPitDeg
		{
			get
			{
				return this.spl__LocatorSpawnerBancParam.CameraPitDeg;
			}

			set
			{
				this.spl__LocatorSpawnerBancParam.CameraPitDeg = value;
			}
		}

		[ByamlMember("spl__LocatorSpawnerBancParam")]
		public Mu_spl__LocatorSpawnerBancParam spl__LocatorSpawnerBancParam { get; set; }

		public LocatorSpawner() : base()
		{
			spl__LocatorSpawnerBancParam = new Mu_spl__LocatorSpawnerBancParam();

			Links = new List<Link>();
		}

		public LocatorSpawner(LocatorSpawner other) : base(other)
		{
			spl__LocatorSpawnerBancParam = other.spl__LocatorSpawnerBancParam.Clone();
		}

		public override LocatorSpawner Clone()
		{
			return new LocatorSpawner(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorSpawnerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
