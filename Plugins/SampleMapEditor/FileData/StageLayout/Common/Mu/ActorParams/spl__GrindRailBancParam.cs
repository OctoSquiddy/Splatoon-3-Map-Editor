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
	public class Mu_spl__GrindRailBancParam
	{
		[ByamlMember]
		public bool IsCameraReset { get; set; }

		[ByamlMember]
		public bool IsLightModel { get; set; }

		[ByamlMember]
		public bool IsOpen { get; set; }

		[ByamlMember]
		public bool IsUseBase { get; set; }

		[ByamlMember]
		public ulong LinkToRail { get; set; }

		public Mu_spl__GrindRailBancParam()
		{
			IsCameraReset = false;
			IsLightModel = false;
			IsOpen = false;
			IsUseBase = false;
			LinkToRail = 0;
		}

		public Mu_spl__GrindRailBancParam(Mu_spl__GrindRailBancParam other)
		{
			IsCameraReset = other.IsCameraReset;
			IsLightModel = other.IsLightModel;
			IsOpen = other.IsOpen;
			IsUseBase = other.IsUseBase;
			LinkToRail = other.LinkToRail;
		}

		public Mu_spl__GrindRailBancParam Clone()
		{
			return new Mu_spl__GrindRailBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__GrindRailBancParam = new BymlNode.DictionaryNode() { Name = "spl__GrindRailBancParam" };

			if (SerializedActor["spl__GrindRailBancParam"] != null)
			{
				spl__GrindRailBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__GrindRailBancParam"];
			}


			if (this.IsCameraReset)
			{
				spl__GrindRailBancParam.AddNode("IsCameraReset", this.IsCameraReset);
			}

			if (this.IsLightModel)
			{
				spl__GrindRailBancParam.AddNode("IsLightModel", this.IsLightModel);
			}

			if (this.IsOpen)
			{
				spl__GrindRailBancParam.AddNode("IsOpen", this.IsOpen);
			}

			if (this.IsUseBase)
			{
				spl__GrindRailBancParam.AddNode("IsUseBase", this.IsUseBase);
			}

			spl__GrindRailBancParam.AddNode("LinkToRail", this.LinkToRail);

			if (SerializedActor["spl__GrindRailBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__GrindRailBancParam);
			}
		}
	}
}
