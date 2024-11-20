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
	public class Mu_spl__EnemyStampBancParam
	{
		[ByamlMember]
		public bool IsFirstDirectlyAttack { get; set; }

		public Mu_spl__EnemyStampBancParam()
		{
			IsFirstDirectlyAttack = false;
		}

		public Mu_spl__EnemyStampBancParam(Mu_spl__EnemyStampBancParam other)
		{
			IsFirstDirectlyAttack = other.IsFirstDirectlyAttack;
		}

		public Mu_spl__EnemyStampBancParam Clone()
		{
			return new Mu_spl__EnemyStampBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyStampBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyStampBancParam" };

			if (SerializedActor["spl__EnemyStampBancParam"] != null)
			{
				spl__EnemyStampBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyStampBancParam"];
			}


			if (this.IsFirstDirectlyAttack)
			{
				spl__EnemyStampBancParam.AddNode("IsFirstDirectlyAttack", this.IsFirstDirectlyAttack);
			}

			if (SerializedActor["spl__EnemyStampBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyStampBancParam);
			}
		}
	}
}
