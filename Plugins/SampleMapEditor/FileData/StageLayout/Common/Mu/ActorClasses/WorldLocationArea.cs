using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class WorldLocationArea : MuObj
	{
		[BindGUI("AreaType", Category = "WorldLocationArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _AreaType
		{
			get
			{
				return this.spl__WorldLocationAreaParam.AreaType;
			}

			set
			{
				this.spl__WorldLocationAreaParam.AreaType = value;
			}
		}

		[BindGUI("LocationName", Category = "WorldLocationArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _LocationName
		{
			get
			{
				return this.spl__WorldLocationAreaParam.LocationName;
			}

			set
			{
				this.spl__WorldLocationAreaParam.LocationName = value;
			}
		}

		[ByamlMember("spl__WorldLocationAreaParam")]
		public Mu_spl__WorldLocationAreaParam spl__WorldLocationAreaParam { get; set; }

		public WorldLocationArea() : base()
		{
			spl__WorldLocationAreaParam = new Mu_spl__WorldLocationAreaParam();

			Links = new List<Link>();
		}

		public WorldLocationArea(WorldLocationArea other) : base(other)
		{
			spl__WorldLocationAreaParam = other.spl__WorldLocationAreaParam.Clone();
		}

		public override WorldLocationArea Clone()
		{
			return new WorldLocationArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__WorldLocationAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
