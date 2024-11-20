using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Obj_LobbyProjector : MuObj
	{
		[ByamlMember("spl__HologramProjectorBancParam")]
		public Mu_spl__HologramProjectorBancParam spl__HologramProjectorBancParam { get; set; }

		public Obj_LobbyProjector() : base()
		{
			spl__HologramProjectorBancParam = new Mu_spl__HologramProjectorBancParam();

			Links = new List<Link>();
		}

		public Obj_LobbyProjector(Obj_LobbyProjector other) : base(other)
		{
			spl__HologramProjectorBancParam = other.spl__HologramProjectorBancParam.Clone();
		}

		public override Obj_LobbyProjector Clone()
		{
			return new Obj_LobbyProjector(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__HologramProjectorBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
