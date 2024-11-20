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
	public class Mu_spl__EntryLiftBancParamSdodr
	{
		[ByamlMember]
		public bool IsShowGetOffUI { get; set; }

		public Mu_spl__EntryLiftBancParamSdodr()
		{
			IsShowGetOffUI = false;
		}

		public Mu_spl__EntryLiftBancParamSdodr(Mu_spl__EntryLiftBancParamSdodr other)
		{
			IsShowGetOffUI = other.IsShowGetOffUI;
		}

		public Mu_spl__EntryLiftBancParamSdodr Clone()
		{
			return new Mu_spl__EntryLiftBancParamSdodr(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EntryLiftBancParamSdodr = new BymlNode.DictionaryNode() { Name = "spl__EntryLiftBancParamSdodr" };

			if (SerializedActor["spl__EntryLiftBancParamSdodr"] != null)
			{
				spl__EntryLiftBancParamSdodr = (BymlNode.DictionaryNode)SerializedActor["spl__EntryLiftBancParamSdodr"];
			}


			if (this.IsShowGetOffUI)
			{
				spl__EntryLiftBancParamSdodr.AddNode("IsShowGetOffUI", this.IsShowGetOffUI);
			}

			if (SerializedActor["spl__EntryLiftBancParamSdodr"] == null)
			{
				SerializedActor.Nodes.Add(spl__EntryLiftBancParamSdodr);
			}
		}
	}
}
