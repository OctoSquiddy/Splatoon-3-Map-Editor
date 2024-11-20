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
	public class Mu_spl__BlowoutsBancParam
	{
		[ByamlMember]
		public float MaxLength { get; set; }

		public Mu_spl__BlowoutsBancParam()
		{
			MaxLength = 0.0f;
		}

		public Mu_spl__BlowoutsBancParam(Mu_spl__BlowoutsBancParam other)
		{
			MaxLength = other.MaxLength;
		}

		public Mu_spl__BlowoutsBancParam Clone()
		{
			return new Mu_spl__BlowoutsBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__BlowoutsBancParam = new BymlNode.DictionaryNode() { Name = "spl__BlowoutsBancParam" };

			if (SerializedActor["spl__BlowoutsBancParam"] != null)
			{
				spl__BlowoutsBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__BlowoutsBancParam"];
			}


			spl__BlowoutsBancParam.AddNode("MaxLength", this.MaxLength);

			if (SerializedActor["spl__BlowoutsBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__BlowoutsBancParam);
			}
		}
	}
}
