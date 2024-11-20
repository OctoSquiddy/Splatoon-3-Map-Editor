using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplConcreteSpawnerSdodr : MuObj
	{
		[BindGUI("IsSpawnDirSpecified", Category = "SplConcreteSpawnerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsSpawnDirSpecified
		{
			get
			{
				return this.spl__ConcreteSpawnerSdodrHelperBancParam.IsSpawnDirSpecified;
			}

			set
			{
				this.spl__ConcreteSpawnerSdodrHelperBancParam.IsSpawnDirSpecified = value;
			}
		}

		[ByamlMember("spl__ConcreteSpawnerSdodrHelperBancParam")]
		public Mu_spl__ConcreteSpawnerSdodrHelperBancParam spl__ConcreteSpawnerSdodrHelperBancParam { get; set; }

		public SplConcreteSpawnerSdodr() : base()
		{
			spl__ConcreteSpawnerSdodrHelperBancParam = new Mu_spl__ConcreteSpawnerSdodrHelperBancParam();

			Links = new List<Link>();
		}

		public SplConcreteSpawnerSdodr(SplConcreteSpawnerSdodr other) : base(other)
		{
			spl__ConcreteSpawnerSdodrHelperBancParam = other.spl__ConcreteSpawnerSdodrHelperBancParam.Clone();
		}

		public override SplConcreteSpawnerSdodr Clone()
		{
			return new SplConcreteSpawnerSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ConcreteSpawnerSdodrHelperBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
