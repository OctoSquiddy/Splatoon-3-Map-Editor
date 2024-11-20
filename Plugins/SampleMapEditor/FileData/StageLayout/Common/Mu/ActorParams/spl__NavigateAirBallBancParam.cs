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
	public class Mu_spl__NavigateAirBallBancParam
	{
		[ByamlMember]
		public int DisappearFrame { get; set; }

		[ByamlMember]
		public bool IsFirst { get; set; }

		[ByamlMember]
		public int MoveFrame { get; set; }

		[ByamlMember]
		public int WaitFrame { get; set; }

		public Mu_spl__NavigateAirBallBancParam()
		{
			DisappearFrame = 0;
			IsFirst = false;
			MoveFrame = 0;
			WaitFrame = 0;
		}

		public Mu_spl__NavigateAirBallBancParam(Mu_spl__NavigateAirBallBancParam other)
		{
			DisappearFrame = other.DisappearFrame;
			IsFirst = other.IsFirst;
			MoveFrame = other.MoveFrame;
			WaitFrame = other.WaitFrame;
		}

		public Mu_spl__NavigateAirBallBancParam Clone()
		{
			return new Mu_spl__NavigateAirBallBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__NavigateAirBallBancParam = new BymlNode.DictionaryNode() { Name = "spl__NavigateAirBallBancParam" };

			if (SerializedActor["spl__NavigateAirBallBancParam"] != null)
			{
				spl__NavigateAirBallBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__NavigateAirBallBancParam"];
			}


			spl__NavigateAirBallBancParam.AddNode("DisappearFrame", this.DisappearFrame);

			if (this.IsFirst)
			{
				spl__NavigateAirBallBancParam.AddNode("IsFirst", this.IsFirst);
			}

			spl__NavigateAirBallBancParam.AddNode("MoveFrame", this.MoveFrame);

			spl__NavigateAirBallBancParam.AddNode("WaitFrame", this.WaitFrame);

			if (SerializedActor["spl__NavigateAirBallBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__NavigateAirBallBancParam);
			}
		}
	}
}
