using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorRandomDigUpPointSpawnArea : MuObj
	{
		[BindGUI("MaxSpawnNum", Category = "LocatorRandomDigUpPointSpawnArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _MaxSpawnNum
		{
			get
			{
				return this.spl__RandomDigUpPointSpawnAreaBancParam.MaxSpawnNum;
			}

			set
			{
				this.spl__RandomDigUpPointSpawnAreaBancParam.MaxSpawnNum = value;
			}
		}

		[BindGUI("MinSpawnNum", Category = "LocatorRandomDigUpPointSpawnArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _MinSpawnNum
		{
			get
			{
				return this.spl__RandomDigUpPointSpawnAreaBancParam.MinSpawnNum;
			}

			set
			{
				this.spl__RandomDigUpPointSpawnAreaBancParam.MinSpawnNum = value;
			}
		}

		[ByamlMember("spl__RandomDigUpPointSpawnAreaBancParam")]
		public Mu_spl__RandomDigUpPointSpawnAreaBancParam spl__RandomDigUpPointSpawnAreaBancParam { get; set; }

		public LocatorRandomDigUpPointSpawnArea() : base()
		{
			spl__RandomDigUpPointSpawnAreaBancParam = new Mu_spl__RandomDigUpPointSpawnAreaBancParam();

			Links = new List<Link>();
		}

		public LocatorRandomDigUpPointSpawnArea(LocatorRandomDigUpPointSpawnArea other) : base(other)
		{
			spl__RandomDigUpPointSpawnAreaBancParam = other.spl__RandomDigUpPointSpawnAreaBancParam.Clone();
		}

		public override LocatorRandomDigUpPointSpawnArea Clone()
		{
			return new LocatorRandomDigUpPointSpawnArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__RandomDigUpPointSpawnAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
