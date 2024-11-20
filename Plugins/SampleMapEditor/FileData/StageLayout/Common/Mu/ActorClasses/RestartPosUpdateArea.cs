using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class RestartPosUpdateArea : MuObj
	{
		[ByamlMember("spl__RestartPosUpdateAreaBancParam")]
		public Mu_spl__RestartPosUpdateAreaBancParam spl__RestartPosUpdateAreaBancParam { get; set; }

		public RestartPosUpdateArea() : base()
		{
			spl__RestartPosUpdateAreaBancParam = new Mu_spl__RestartPosUpdateAreaBancParam();

			Links = new List<Link>();
		}

		public RestartPosUpdateArea(RestartPosUpdateArea other) : base(other)
		{
			spl__RestartPosUpdateAreaBancParam = other.spl__RestartPosUpdateAreaBancParam.Clone();
		}

		public override RestartPosUpdateArea Clone()
		{
			return new RestartPosUpdateArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__RestartPosUpdateAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
