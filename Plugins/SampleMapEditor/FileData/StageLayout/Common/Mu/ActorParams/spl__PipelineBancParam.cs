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
	public class Mu_spl__PipelineBancParam
	{
		[ByamlMember]
		public ulong LinkToRailPoint { get; set; }

		public Mu_spl__PipelineBancParam()
		{
			LinkToRailPoint = 0;
		}

		public Mu_spl__PipelineBancParam(Mu_spl__PipelineBancParam other)
		{
			LinkToRailPoint = other.LinkToRailPoint;
		}

		public Mu_spl__PipelineBancParam Clone()
		{
			return new Mu_spl__PipelineBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__PipelineBancParam = new BymlNode.DictionaryNode() { Name = "spl__PipelineBancParam" };

			if (SerializedActor["spl__PipelineBancParam"] != null)
			{
				spl__PipelineBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PipelineBancParam"];
			}


			spl__PipelineBancParam.AddNode("LinkToRailPoint", this.LinkToRailPoint);

			if (SerializedActor["spl__PipelineBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__PipelineBancParam);
			}
		}
	}
}
