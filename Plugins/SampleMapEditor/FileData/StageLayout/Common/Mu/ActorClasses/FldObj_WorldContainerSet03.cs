using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class FldObj_WorldContainerSet03 : MuObj
	{
		[BindGUI("IsEventOnly", Category = "FldObj_WorldContainerSet03 Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[ByamlMember("spl__EventActorStateBancParam")]
		public Mu_spl__EventActorStateBancParam spl__EventActorStateBancParam { get; set; }

		public FldObj_WorldContainerSet03() : base()
		{
			spl__EventActorStateBancParam = new Mu_spl__EventActorStateBancParam();

			Links = new List<Link>();
		}

		public FldObj_WorldContainerSet03(FldObj_WorldContainerSet03 other) : base(other)
		{
			spl__EventActorStateBancParam = other.spl__EventActorStateBancParam.Clone();
		}

		public override FldObj_WorldContainerSet03 Clone()
		{
			return new FldObj_WorldContainerSet03(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EventActorStateBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
