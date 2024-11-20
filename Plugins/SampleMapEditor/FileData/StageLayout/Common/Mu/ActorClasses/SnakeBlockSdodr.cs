using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SnakeBlockSdodr : MuObj
	{
		[BindGUI("ToRailPoint", Category = "SnakeBlockSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__SnakeBlockSdodrBancParam.ToRailPoint;
			}

			set
			{
				this.spl__SnakeBlockSdodrBancParam.ToRailPoint = value;
			}
		}

		[ByamlMember("spl__SnakeBlockSdodrBancParam")]
		public Mu_spl__SnakeBlockSdodrBancParam spl__SnakeBlockSdodrBancParam { get; set; }

		public SnakeBlockSdodr() : base()
		{
			spl__SnakeBlockSdodrBancParam = new Mu_spl__SnakeBlockSdodrBancParam();

			Links = new List<Link>();
		}

		public SnakeBlockSdodr(SnakeBlockSdodr other) : base(other)
		{
			spl__SnakeBlockSdodrBancParam = other.spl__SnakeBlockSdodrBancParam.Clone();
		}

		public override SnakeBlockSdodr Clone()
		{
			return new SnakeBlockSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SnakeBlockSdodrBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
