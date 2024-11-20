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
	public class Mu_spl__InkHideHelperBancParam
	{
		[ByamlMember]
		public string HideMode { get; set; }

		public Mu_spl__InkHideHelperBancParam()
		{
			HideMode = "";
		}

		public Mu_spl__InkHideHelperBancParam(Mu_spl__InkHideHelperBancParam other)
		{
			HideMode = other.HideMode;
		}

		public Mu_spl__InkHideHelperBancParam Clone()
		{
			return new Mu_spl__InkHideHelperBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__InkHideHelperBancParam = new BymlNode.DictionaryNode() { Name = "spl__InkHideHelperBancParam" };

			if (SerializedActor["spl__InkHideHelperBancParam"] != null)
			{
				spl__InkHideHelperBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__InkHideHelperBancParam"];
			}


			if (this.HideMode != "")
			{
				spl__InkHideHelperBancParam.AddNode("HideMode", this.HideMode);
			}

			if (SerializedActor["spl__InkHideHelperBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__InkHideHelperBancParam);
			}
		}
	}
}
