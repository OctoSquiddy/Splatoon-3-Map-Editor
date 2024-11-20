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
	public class Mu_spl__InkRailCoopBancParam
	{
		[ByamlMember]
		public bool IsActiveInHigh { get; set; }

		[ByamlMember]
		public bool IsActiveInLow { get; set; }

		[ByamlMember]
		public bool IsActiveInMid { get; set; }

		[ByamlMember]
		public bool IsActiveInRelay { get; set; }

		public Mu_spl__InkRailCoopBancParam()
		{
			IsActiveInHigh = false;
			IsActiveInLow = false;
			IsActiveInMid = false;
			IsActiveInRelay = false;
		}

		public Mu_spl__InkRailCoopBancParam(Mu_spl__InkRailCoopBancParam other)
		{
			IsActiveInHigh = other.IsActiveInHigh;
			IsActiveInLow = other.IsActiveInLow;
			IsActiveInMid = other.IsActiveInMid;
			IsActiveInRelay = other.IsActiveInRelay;
		}

		public Mu_spl__InkRailCoopBancParam Clone()
		{
			return new Mu_spl__InkRailCoopBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__InkRailCoopBancParam = new BymlNode.DictionaryNode() { Name = "spl__InkRailCoopBancParam" };

			if (SerializedActor["spl__InkRailCoopBancParam"] != null)
			{
				spl__InkRailCoopBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__InkRailCoopBancParam"];
			}


			if (this.IsActiveInHigh)
			{
				spl__InkRailCoopBancParam.AddNode("IsActiveInHigh", this.IsActiveInHigh);
			}

			if (this.IsActiveInLow)
			{
				spl__InkRailCoopBancParam.AddNode("IsActiveInLow", this.IsActiveInLow);
			}

			if (this.IsActiveInMid)
			{
				spl__InkRailCoopBancParam.AddNode("IsActiveInMid", this.IsActiveInMid);
			}

			if (this.IsActiveInRelay)
			{
				spl__InkRailCoopBancParam.AddNode("IsActiveInRelay", this.IsActiveInRelay);
			}

			if (SerializedActor["spl__InkRailCoopBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__InkRailCoopBancParam);
			}
		}
	}
}
