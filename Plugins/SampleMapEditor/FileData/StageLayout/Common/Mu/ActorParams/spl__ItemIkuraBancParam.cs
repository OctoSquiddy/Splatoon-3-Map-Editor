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
	public class Mu_spl__ItemIkuraBancParam
	{
		[ByamlMember]
		public bool IsEnableRejectGroundCol { get; set; }

		public Mu_spl__ItemIkuraBancParam()
		{
			IsEnableRejectGroundCol = false;
		}

		public Mu_spl__ItemIkuraBancParam(Mu_spl__ItemIkuraBancParam other)
		{
			IsEnableRejectGroundCol = other.IsEnableRejectGroundCol;
		}

		public Mu_spl__ItemIkuraBancParam Clone()
		{
			return new Mu_spl__ItemIkuraBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ItemIkuraBancParam = new BymlNode.DictionaryNode() { Name = "spl__ItemIkuraBancParam" };

			if (SerializedActor["spl__ItemIkuraBancParam"] != null)
			{
				spl__ItemIkuraBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ItemIkuraBancParam"];
			}


			if (this.IsEnableRejectGroundCol)
			{
				spl__ItemIkuraBancParam.AddNode("IsEnableRejectGroundCol", this.IsEnableRejectGroundCol);
			}

			if (SerializedActor["spl__ItemIkuraBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ItemIkuraBancParam);
			}
		}
	}
}
