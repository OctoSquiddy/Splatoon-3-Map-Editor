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
	public class Mu_spl__AttackPermitNumChangeAreaBancParam
	{
		[ByamlMember]
		public int LimitNum { get; set; }

		public Mu_spl__AttackPermitNumChangeAreaBancParam()
		{
			LimitNum = 0;
		}

		public Mu_spl__AttackPermitNumChangeAreaBancParam(Mu_spl__AttackPermitNumChangeAreaBancParam other)
		{
			LimitNum = other.LimitNum;
		}

		public Mu_spl__AttackPermitNumChangeAreaBancParam Clone()
		{
			return new Mu_spl__AttackPermitNumChangeAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__AttackPermitNumChangeAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__AttackPermitNumChangeAreaBancParam" };

			if (SerializedActor["spl__AttackPermitNumChangeAreaBancParam"] != null)
			{
				spl__AttackPermitNumChangeAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__AttackPermitNumChangeAreaBancParam"];
			}


			spl__AttackPermitNumChangeAreaBancParam.AddNode("LimitNum", this.LimitNum);

			if (SerializedActor["spl__AttackPermitNumChangeAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__AttackPermitNumChangeAreaBancParam);
			}
		}
	}
}
