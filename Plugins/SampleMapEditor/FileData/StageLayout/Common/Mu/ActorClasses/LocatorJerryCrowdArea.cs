using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorJerryCrowdArea : MuObj
	{
		[BindGUI("PlayerHumanMoveRt", Category = "LocatorJerryCrowdArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _PlayerHumanMoveRt
		{
			get
			{
				return this.spl__LocatorJerryCrowdAreaParam.PlayerHumanMoveRt;
			}

			set
			{
				this.spl__LocatorJerryCrowdAreaParam.PlayerHumanMoveRt = value;
			}
		}

		[ByamlMember("spl__LocatorJerryCrowdAreaParam")]
		public Mu_spl__LocatorJerryCrowdAreaParam spl__LocatorJerryCrowdAreaParam { get; set; }

		public LocatorJerryCrowdArea() : base()
		{
			spl__LocatorJerryCrowdAreaParam = new Mu_spl__LocatorJerryCrowdAreaParam();

			Links = new List<Link>();
		}

		public LocatorJerryCrowdArea(LocatorJerryCrowdArea other) : base(other)
		{
			spl__LocatorJerryCrowdAreaParam = other.spl__LocatorJerryCrowdAreaParam.Clone();
		}

		public override LocatorJerryCrowdArea Clone()
		{
			return new LocatorJerryCrowdArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorJerryCrowdAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
