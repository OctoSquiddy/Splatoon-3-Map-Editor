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
	public class Mu_spl__EnemyAlertAreaBancParam
	{
		[ByamlMember]
		public bool IsForceFindDirect { get; set; }

		[ByamlMember]
		public bool IsForceFindHide { get; set; }

		[ByamlMember]
		public bool IsOneTime { get; set; }

		public Mu_spl__EnemyAlertAreaBancParam()
		{
			IsForceFindDirect = false;
			IsForceFindHide = false;
			IsOneTime = false;
		}

		public Mu_spl__EnemyAlertAreaBancParam(Mu_spl__EnemyAlertAreaBancParam other)
		{
			IsForceFindDirect = other.IsForceFindDirect;
			IsForceFindHide = other.IsForceFindHide;
			IsOneTime = other.IsOneTime;
		}

		public Mu_spl__EnemyAlertAreaBancParam Clone()
		{
			return new Mu_spl__EnemyAlertAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__EnemyAlertAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyAlertAreaBancParam" };

			if (SerializedActor["spl__EnemyAlertAreaBancParam"] != null)
			{
				spl__EnemyAlertAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyAlertAreaBancParam"];
			}


			if (this.IsForceFindDirect)
			{
				spl__EnemyAlertAreaBancParam.AddNode("IsForceFindDirect", this.IsForceFindDirect);
			}

			if (this.IsForceFindHide)
			{
				spl__EnemyAlertAreaBancParam.AddNode("IsForceFindHide", this.IsForceFindHide);
			}

			if (this.IsOneTime)
			{
				spl__EnemyAlertAreaBancParam.AddNode("IsOneTime", this.IsOneTime);
			}

			if (SerializedActor["spl__EnemyAlertAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__EnemyAlertAreaBancParam);
			}
		}
	}
}
