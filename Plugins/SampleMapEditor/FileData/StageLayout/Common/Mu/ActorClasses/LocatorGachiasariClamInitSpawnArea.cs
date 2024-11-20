using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorGachiasariClamInitSpawnArea : MuObj
	{
		[BindGUI("Weight", Category = "LocatorGachiasariClamInitSpawnArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _Weight
		{
			get
			{
				return this.spl__LocatorGachiasariClamInitSpawnAreaBancParam.Weight;
			}

			set
			{
				this.spl__LocatorGachiasariClamInitSpawnAreaBancParam.Weight = value;
			}
		}

		[ByamlMember("spl__LocatorGachiasariClamInitSpawnAreaBancParam")]
		public Mu_spl__LocatorGachiasariClamInitSpawnAreaBancParam spl__LocatorGachiasariClamInitSpawnAreaBancParam { get; set; }

		public LocatorGachiasariClamInitSpawnArea() : base()
		{
			spl__LocatorGachiasariClamInitSpawnAreaBancParam = new Mu_spl__LocatorGachiasariClamInitSpawnAreaBancParam();

			Links = new List<Link>();
		}

		public LocatorGachiasariClamInitSpawnArea(LocatorGachiasariClamInitSpawnArea other) : base(other)
		{
			spl__LocatorGachiasariClamInitSpawnAreaBancParam = other.spl__LocatorGachiasariClamInitSpawnAreaBancParam.Clone();
		}

		public override LocatorGachiasariClamInitSpawnArea Clone()
		{
			return new LocatorGachiasariClamInitSpawnArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorGachiasariClamInitSpawnAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
