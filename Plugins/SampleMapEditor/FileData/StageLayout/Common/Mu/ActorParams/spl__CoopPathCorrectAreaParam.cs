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
	public class Mu_spl__CoopPathCorrectAreaParam
	{
		public Mu_spl__CoopPathCorrectAreaParam()
		{
		}

		public Mu_spl__CoopPathCorrectAreaParam(Mu_spl__CoopPathCorrectAreaParam other)
		{
		}

		public Mu_spl__CoopPathCorrectAreaParam Clone()
		{
			return new Mu_spl__CoopPathCorrectAreaParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CoopPathCorrectAreaParam = new BymlNode.DictionaryNode() { Name = "spl__CoopPathCorrectAreaParam" };

			if (SerializedActor["spl__CoopPathCorrectAreaParam"] != null)
			{
				spl__CoopPathCorrectAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__CoopPathCorrectAreaParam"];
			}


			if (SerializedActor["spl__CoopPathCorrectAreaParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CoopPathCorrectAreaParam);
			}
		}
	}
}
