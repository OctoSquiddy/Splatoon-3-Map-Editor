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
	public class Mu_spl__AutoWarpPointBancParam
	{
		[ByamlMember]
		public string ChangeSceneOrStageName { get; set; }

		[ByamlMember]
		public bool IsDokanWarpCameraYQuick { get; set; }

		[ByamlMember]
		public bool IsDokanWarpTypeLong { get; set; }

		public Mu_spl__AutoWarpPointBancParam()
		{
			ChangeSceneOrStageName = "";
			IsDokanWarpCameraYQuick = false;
			IsDokanWarpTypeLong = false;
		}

		public Mu_spl__AutoWarpPointBancParam(Mu_spl__AutoWarpPointBancParam other)
		{
			ChangeSceneOrStageName = other.ChangeSceneOrStageName;
			IsDokanWarpCameraYQuick = other.IsDokanWarpCameraYQuick;
			IsDokanWarpTypeLong = other.IsDokanWarpTypeLong;
		}

		public Mu_spl__AutoWarpPointBancParam Clone()
		{
			return new Mu_spl__AutoWarpPointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__AutoWarpPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__AutoWarpPointBancParam" };

			if (SerializedActor["spl__AutoWarpPointBancParam"] != null)
			{
				spl__AutoWarpPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__AutoWarpPointBancParam"];
			}


			if (this.ChangeSceneOrStageName != "")
			{
				spl__AutoWarpPointBancParam.AddNode("ChangeSceneOrStageName", this.ChangeSceneOrStageName);
			}

			if (this.IsDokanWarpCameraYQuick)
			{
				spl__AutoWarpPointBancParam.AddNode("IsDokanWarpCameraYQuick", this.IsDokanWarpCameraYQuick);
			}

			if (this.IsDokanWarpTypeLong)
			{
				spl__AutoWarpPointBancParam.AddNode("IsDokanWarpTypeLong", this.IsDokanWarpTypeLong);
			}

			if (SerializedActor["spl__AutoWarpPointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__AutoWarpPointBancParam);
			}
		}
	}
}
