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
	public class Mu_spl__LiftWithMoveAnimBancParam
	{
		[ByamlMember]
		public bool IsReverses { get; set; }

		public Mu_spl__LiftWithMoveAnimBancParam()
		{
			IsReverses = false;
		}

		public Mu_spl__LiftWithMoveAnimBancParam(Mu_spl__LiftWithMoveAnimBancParam other)
		{
			IsReverses = other.IsReverses;
		}

		public Mu_spl__LiftWithMoveAnimBancParam Clone()
		{
			return new Mu_spl__LiftWithMoveAnimBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LiftWithMoveAnimBancParam = new BymlNode.DictionaryNode() { Name = "spl__LiftWithMoveAnimBancParam" };

			if (SerializedActor["spl__LiftWithMoveAnimBancParam"] != null)
			{
				spl__LiftWithMoveAnimBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LiftWithMoveAnimBancParam"];
			}


			if (this.IsReverses)
			{
				spl__LiftWithMoveAnimBancParam.AddNode("IsReverses", this.IsReverses);
			}

			if (SerializedActor["spl__LiftWithMoveAnimBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LiftWithMoveAnimBancParam);
			}
		}
	}
}
