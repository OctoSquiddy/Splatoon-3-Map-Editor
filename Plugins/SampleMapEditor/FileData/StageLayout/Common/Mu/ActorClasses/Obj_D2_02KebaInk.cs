using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_D2_02KebaInk : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "Obj_D2-02KebaInk Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		public Obj_D2_02KebaInk() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();

			Links = new List<Link>();
		}

		public Obj_D2_02KebaInk(Obj_D2_02KebaInk other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
		}

		public override Obj_D2_02KebaInk Clone()
		{
			return new Obj_D2_02KebaInk(this);
		}
		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
