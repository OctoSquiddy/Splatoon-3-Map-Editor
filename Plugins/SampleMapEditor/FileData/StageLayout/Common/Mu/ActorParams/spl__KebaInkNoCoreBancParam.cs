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
	public class Mu_spl__KebaInkNoCoreBancParam
	{
		public Mu_spl__KebaInkNoCoreBancParam()
		{
		}

		public Mu_spl__KebaInkNoCoreBancParam(Mu_spl__KebaInkNoCoreBancParam other)
		{
		}

		public Mu_spl__KebaInkNoCoreBancParam Clone()
		{
			return new Mu_spl__KebaInkNoCoreBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__KebaInkNoCoreBancParam = new BymlNode.DictionaryNode() { Name = "spl__KebaInkNoCoreBancParam" };

			if (SerializedActor["spl__KebaInkNoCoreBancParam"] != null)
			{
				spl__KebaInkNoCoreBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__KebaInkNoCoreBancParam"];
			}


			if (SerializedActor["spl__KebaInkNoCoreBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__KebaInkNoCoreBancParam);
			}
		}
	}
}
