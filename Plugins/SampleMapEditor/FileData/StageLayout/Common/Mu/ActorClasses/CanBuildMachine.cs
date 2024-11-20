using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CanBuildMachine : MuObj
	{
		[ByamlMember("spl__CanBuildMachineBancParam")]
		public Mu_spl__CanBuildMachineBancParam spl__CanBuildMachineBancParam { get; set; }

		public CanBuildMachine() : base()
		{
			spl__CanBuildMachineBancParam = new Mu_spl__CanBuildMachineBancParam();

			Links = new List<Link>();
		}

		public CanBuildMachine(CanBuildMachine other) : base(other)
		{
			spl__CanBuildMachineBancParam = other.spl__CanBuildMachineBancParam.Clone();
		}

		public override CanBuildMachine Clone()
		{
			return new CanBuildMachine(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CanBuildMachineBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
