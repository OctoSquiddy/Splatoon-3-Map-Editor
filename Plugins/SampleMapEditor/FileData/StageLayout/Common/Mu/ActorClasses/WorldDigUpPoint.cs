using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class WorldDigUpPoint : MuObj
	{
		[BindGUI("Type", Category = "WorldDigUpPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Type
		{
			get
			{
				return this.spl__WorldDigUpPointBancParam.Type;
			}

			set
			{
				this.spl__WorldDigUpPointBancParam.Type = value;
			}
		}

		[ByamlMember("spl__ItemDropBancParam")]
		public Mu_spl__ItemDropBancParam spl__ItemDropBancParam { get; set; }

		[ByamlMember("spl__WorldDigUpPointBancParam")]
		public Mu_spl__WorldDigUpPointBancParam spl__WorldDigUpPointBancParam { get; set; }

		public WorldDigUpPoint() : base()
		{
			spl__ItemDropBancParam = new Mu_spl__ItemDropBancParam();
			spl__WorldDigUpPointBancParam = new Mu_spl__WorldDigUpPointBancParam();

			Links = new List<Link>();
		}

		public WorldDigUpPoint(WorldDigUpPoint other) : base(other)
		{
			spl__ItemDropBancParam = other.spl__ItemDropBancParam.Clone();
			spl__WorldDigUpPointBancParam = other.spl__WorldDigUpPointBancParam.Clone();
		}

		public override WorldDigUpPoint Clone()
		{
			return new WorldDigUpPoint(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ItemDropBancParam.SaveParameterBank(SerializedActor);
			this.spl__WorldDigUpPointBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
