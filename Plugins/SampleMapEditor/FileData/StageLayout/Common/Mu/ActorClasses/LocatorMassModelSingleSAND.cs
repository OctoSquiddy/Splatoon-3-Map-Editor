using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorMassModelSingleSAND : MuObj
	{
		[BindGUI("EnableMoveOnGround", Category = "LocatorMassModelSingleSAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _EnableMoveOnGround
		{
			get
			{
				return this.spl__gfx__LocatorMassModelSANDBancParam.EnableMoveOnGround;
			}

			set
			{
				this.spl__gfx__LocatorMassModelSANDBancParam.EnableMoveOnGround = value;
			}
		}

		[BindGUI("ForcePseudoCollision", Category = "LocatorMassModelSingleSAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _ForcePseudoCollision
		{
			get
			{
				return this.spl__gfx__LocatorMassModelSANDBancParam.ForcePseudoCollision;
			}

			set
			{
				this.spl__gfx__LocatorMassModelSANDBancParam.ForcePseudoCollision = value;
			}
		}

		[BindGUI("ForceTeamType", Category = "LocatorMassModelSingleSAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ForceTeamType
		{
			get
			{
				return this.spl__gfx__LocatorMassModelSANDBancParam.ForceTeamType;
			}

			set
			{
				this.spl__gfx__LocatorMassModelSANDBancParam.ForceTeamType = value;
			}
		}

		[BindGUI("ModelType", Category = "LocatorMassModelSingleSAND Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ModelType
		{
			get
			{
				return this.spl__gfx__LocatorMassModelSANDBancParam.ModelType;
			}

			set
			{
				this.spl__gfx__LocatorMassModelSANDBancParam.ModelType = value;
			}
		}

		[ByamlMember("spl__gfx__LocatorMassModelSANDBancParam")]
		public Mu_spl__gfx__LocatorMassModelSANDBancParam spl__gfx__LocatorMassModelSANDBancParam { get; set; }

		public LocatorMassModelSingleSAND() : base()
		{
			spl__gfx__LocatorMassModelSANDBancParam = new Mu_spl__gfx__LocatorMassModelSANDBancParam();

			Links = new List<Link>();
		}

		public LocatorMassModelSingleSAND(LocatorMassModelSingleSAND other) : base(other)
		{
			spl__gfx__LocatorMassModelSANDBancParam = other.spl__gfx__LocatorMassModelSANDBancParam.Clone();
		}

		public override LocatorMassModelSingleSAND Clone()
		{
			return new LocatorMassModelSingleSAND(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__gfx__LocatorMassModelSANDBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
