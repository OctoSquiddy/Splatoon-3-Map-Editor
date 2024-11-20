using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	[ByamlObject]
	public class Mu_spl__gfx__LocatorMassModelSANDBancParam
	{
		[ByamlMember]
		public bool EnableMoveOnGround { get; set; }

		[ByamlMember]
		public bool ForcePseudoCollision { get; set; }

		[ByamlMember]
		public string ForceTeamType { get; set; }

		[ByamlMember]
		public string ModelType { get; set; }

		public Mu_spl__gfx__LocatorMassModelSANDBancParam()
		{
			EnableMoveOnGround = false;
			ForcePseudoCollision = false;
			ForceTeamType = "";
			ModelType = "";
		}

		public Mu_spl__gfx__LocatorMassModelSANDBancParam(Mu_spl__gfx__LocatorMassModelSANDBancParam other)
		{
			EnableMoveOnGround = other.EnableMoveOnGround;
			ForcePseudoCollision = other.ForcePseudoCollision;
			ForceTeamType = other.ForceTeamType;
			ModelType = other.ModelType;
		}

		public Mu_spl__gfx__LocatorMassModelSANDBancParam Clone()
		{
			return new Mu_spl__gfx__LocatorMassModelSANDBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__gfx__LocatorMassModelSANDBancParam = new BymlNode.DictionaryNode() { Name = "spl__gfx__LocatorMassModelSANDBancParam" };

			if (SerializedActor["spl__gfx__LocatorMassModelSANDBancParam"] != null)
			{
				spl__gfx__LocatorMassModelSANDBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__gfx__LocatorMassModelSANDBancParam"];
			}


			if (this.EnableMoveOnGround)
			{
				spl__gfx__LocatorMassModelSANDBancParam.AddNode("EnableMoveOnGround", this.EnableMoveOnGround);
			}

			if (this.ForcePseudoCollision)
			{
				spl__gfx__LocatorMassModelSANDBancParam.AddNode("ForcePseudoCollision", this.ForcePseudoCollision);
			}

			if (this.ForceTeamType != "")
			{
				spl__gfx__LocatorMassModelSANDBancParam.AddNode("ForceTeamType", this.ForceTeamType);
			}

			if (this.ModelType != "")
			{
				spl__gfx__LocatorMassModelSANDBancParam.AddNode("ModelType", this.ModelType);
			}

			if (SerializedActor["spl__gfx__LocatorMassModelSANDBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__gfx__LocatorMassModelSANDBancParam);
			}
		}
	}
}
