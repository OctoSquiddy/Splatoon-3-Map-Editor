using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Npc_SdodrHime : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "Npc_SdodrHime Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsEventOnly", Category = "Npc_SdodrHime Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEventOnly
		{
			get
			{
				return this.spl__EventActorStateBancParam.IsEventOnly;
			}

			set
			{
				this.spl__EventActorStateBancParam.IsEventOnly = value;
			}
		}

		[BindGUI("IsInitEventVisible", Category = "Npc_SdodrHime Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsInitEventVisible
		{
			get
			{
				return this.spl__NpcSdodrBancParam.IsInitEventVisible;
			}

			set
			{
				this.spl__NpcSdodrBancParam.IsInitEventVisible = value;
			}
		}

		[BindGUI("IsKinematicWait", Category = "Npc_SdodrHime Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsKinematicWait
		{
			get
			{
				return this.spl__NpcSdodrBancParam.IsKinematicWait;
			}

			set
			{
				this.spl__NpcSdodrBancParam.IsKinematicWait = value;
			}
		}

		[BindGUI("IsTakeOverVisible", Category = "Npc_SdodrHime Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsTakeOverVisible
		{
			get
			{
				return this.spl__NpcSdodrBancParam.IsTakeOverVisible;
			}

			set
			{
				this.spl__NpcSdodrBancParam.IsTakeOverVisible = value;
			}
		}

		[BindGUI("WaitASCommand", Category = "Npc_SdodrHime Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _WaitASCommand
		{
			get
			{
				return this.spl__NpcSdodrBancParam.WaitASCommand;
			}

			set
			{
				this.spl__NpcSdodrBancParam.WaitASCommand = value;
			}
		}

		[ByamlMember("game__EventPerformerBancParam")]
		public Mu_game__EventPerformerBancParam game__EventPerformerBancParam { get; set; }

		[ByamlMember("spl__EventActorStateBancParam")]
		public Mu_spl__EventActorStateBancParam spl__EventActorStateBancParam { get; set; }

		[ByamlMember("spl__NpcSdodrBancParam")]
		public Mu_spl__NpcSdodrBancParam spl__NpcSdodrBancParam { get; set; }

		public Npc_SdodrHime() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();
			spl__NpcSdodrBancParam = new Mu_spl__NpcSdodrBancParam();

			Links = new List<Link>();
		}

		public Npc_SdodrHime(Npc_SdodrHime other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
			spl__NpcSdodrBancParam = other.spl__NpcSdodrBancParam.Clone();
		}

		public override Npc_SdodrHime Clone()
		{
			return new Npc_SdodrHime(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
			this.spl__NpcSdodrBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
