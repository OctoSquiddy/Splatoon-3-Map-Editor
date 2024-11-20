using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Kbi_NoCoreParent : MuObj
	{
		[ByamlMember("spl__KebaInkNoCoreBancParam")]
		public Mu_spl__KebaInkNoCoreBancParam spl__KebaInkNoCoreBancParam { get; set; }

		public Kbi_NoCoreParent() : base()
		{
			spl__KebaInkNoCoreBancParam = new Mu_spl__KebaInkNoCoreBancParam();

			Links = new List<Link>();
		}

		public Kbi_NoCoreParent(Kbi_NoCoreParent other) : base(other)
		{
			spl__KebaInkNoCoreBancParam = other.spl__KebaInkNoCoreBancParam.Clone();
		}

		public override Kbi_NoCoreParent Clone()
		{
			return new Kbi_NoCoreParent(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__KebaInkNoCoreBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
