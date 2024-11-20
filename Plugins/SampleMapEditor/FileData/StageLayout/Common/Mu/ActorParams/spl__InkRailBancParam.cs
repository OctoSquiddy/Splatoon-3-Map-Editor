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
	public class Mu_spl__InkRailBancParam
	{
		[ByamlMember]
		public bool IsOpen { get; set; }

		[ByamlMember]
		public ulong LinkToPoint { get; set; }

		[ByamlMember]
		public bool UseBase { get; set; }

		public Mu_spl__InkRailBancParam()
		{
			IsOpen = false;
			LinkToPoint = 0;
			UseBase = false;
		}

		public Mu_spl__InkRailBancParam(Mu_spl__InkRailBancParam other)
		{
			IsOpen = other.IsOpen;
			LinkToPoint = other.LinkToPoint;
			UseBase = other.UseBase;
		}

		public Mu_spl__InkRailBancParam Clone()
		{
			return new Mu_spl__InkRailBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__InkRailBancParam = new BymlNode.DictionaryNode() { Name = "spl__InkRailBancParam" };

			if (SerializedActor["spl__InkRailBancParam"] != null)
			{
				spl__InkRailBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__InkRailBancParam"];
			}


			if (this.IsOpen)
			{
				spl__InkRailBancParam.AddNode("IsOpen", this.IsOpen);
			}

			spl__InkRailBancParam.AddNode("LinkToPoint", this.LinkToPoint);

			if (this.UseBase)
			{
				spl__InkRailBancParam.AddNode("UseBase", this.UseBase);
			}

			if (SerializedActor["spl__InkRailBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__InkRailBancParam);
			}
		}
	}
}
