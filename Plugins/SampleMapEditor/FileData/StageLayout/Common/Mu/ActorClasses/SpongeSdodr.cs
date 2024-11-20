using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SpongeSdodr : MuObj
	{
		[BindGUI("Type", Category = "SpongeSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Type
		{
			get
			{
				return this.spl__SpongeBancParam.Type;
			}

			set
			{
				this.spl__SpongeBancParam.Type = value;
			}
		}

		[ByamlMember("spl__SpongeBancParam")]
		public Mu_spl__SpongeBancParam spl__SpongeBancParam { get; set; }

		public SpongeSdodr() : base()
		{
			spl__SpongeBancParam = new Mu_spl__SpongeBancParam();

			Links = new List<Link>();
		}

		public SpongeSdodr(SpongeSdodr other) : base(other)
		{
			spl__SpongeBancParam = other.spl__SpongeBancParam.Clone();
		}

		public override SpongeSdodr Clone()
		{
			return new SpongeSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SpongeBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
