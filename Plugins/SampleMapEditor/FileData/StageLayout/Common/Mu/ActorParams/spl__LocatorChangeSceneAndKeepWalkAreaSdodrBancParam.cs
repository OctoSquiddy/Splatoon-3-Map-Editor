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
	public class Mu_spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam
	{
		[ByamlMember]
		public string ChangeSceneOrStageName { get; set; }

		[ByamlMember]
		public string InFaderType { get; set; }

		[ByamlMember]
		public string OutFaderType { get; set; }

		public Mu_spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam()
		{
			ChangeSceneOrStageName = "";
			InFaderType = "";
			OutFaderType = "";
		}

		public Mu_spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam(Mu_spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam other)
		{
			ChangeSceneOrStageName = other.ChangeSceneOrStageName;
			InFaderType = other.InFaderType;
			OutFaderType = other.OutFaderType;
		}

		public Mu_spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam Clone()
		{
			return new Mu_spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam" };

			if (SerializedActor["spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam"] != null)
			{
				spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam"];
			}


			if (this.ChangeSceneOrStageName != "")
			{
				spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam.AddNode("ChangeSceneOrStageName", this.ChangeSceneOrStageName);
			}

			if (this.InFaderType != "")
			{
				spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam.AddNode("InFaderType", this.InFaderType);
			}

			if (this.OutFaderType != "")
			{
				spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam.AddNode("OutFaderType", this.OutFaderType);
			}

			if (SerializedActor["spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam);
			}
		}
	}
}
