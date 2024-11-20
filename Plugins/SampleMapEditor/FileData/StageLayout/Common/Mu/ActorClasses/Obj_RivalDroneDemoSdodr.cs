using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_RivalDroneDemoSdodr : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "Obj_RivalDroneDemoSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActivateOnlyInBeingPerformer
		{
			get
			{
				return this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer;
			}

			set
			{
				this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer = value;
			}
		}

		[ByamlMember("game__EventPerformerBancParam")]
		public Mu_game__EventPerformerBancParam game__EventPerformerBancParam { get; set; }

		public Obj_RivalDroneDemoSdodr() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();

			Links = new List<Link>();
		}

		public Obj_RivalDroneDemoSdodr(Obj_RivalDroneDemoSdodr other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
		}

		public override Obj_RivalDroneDemoSdodr Clone()
		{
			return new Obj_RivalDroneDemoSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}