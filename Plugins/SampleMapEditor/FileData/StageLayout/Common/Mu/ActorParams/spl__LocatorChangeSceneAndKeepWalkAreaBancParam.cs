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
	public class Mu_spl__LocatorChangeSceneAndKeepWalkAreaBancParam
	{
		[ByamlMember]
		public string ChangeSceneOrStageName { get; set; }

		[ByamlMember]
		public string InFaderType { get; set; }

		[ByamlMember]
		public bool IsInformClear { get; set; }

		[ByamlMember]
		public bool IsSquid { get; set; }

		public Mu_spl__LocatorChangeSceneAndKeepWalkAreaBancParam()
		{
			ChangeSceneOrStageName = "";
			InFaderType = "";
			IsInformClear = false;
			IsSquid = false;
		}

		public Mu_spl__LocatorChangeSceneAndKeepWalkAreaBancParam(Mu_spl__LocatorChangeSceneAndKeepWalkAreaBancParam other)
		{
			ChangeSceneOrStageName = other.ChangeSceneOrStageName;
			InFaderType = other.InFaderType;
			IsInformClear = other.IsInformClear;
			IsSquid = other.IsSquid;
		}

		public Mu_spl__LocatorChangeSceneAndKeepWalkAreaBancParam Clone()
		{
			return new Mu_spl__LocatorChangeSceneAndKeepWalkAreaBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorChangeSceneAndKeepWalkAreaBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorChangeSceneAndKeepWalkAreaBancParam" };

			if (SerializedActor["spl__LocatorChangeSceneAndKeepWalkAreaBancParam"] != null)
			{
				spl__LocatorChangeSceneAndKeepWalkAreaBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorChangeSceneAndKeepWalkAreaBancParam"];
			}


			if (this.ChangeSceneOrStageName != "")
			{
				spl__LocatorChangeSceneAndKeepWalkAreaBancParam.AddNode("ChangeSceneOrStageName", this.ChangeSceneOrStageName);
			}

			if (this.InFaderType != "")
			{
				spl__LocatorChangeSceneAndKeepWalkAreaBancParam.AddNode("InFaderType", this.InFaderType);
			}

			if (this.IsInformClear)
			{
				spl__LocatorChangeSceneAndKeepWalkAreaBancParam.AddNode("IsInformClear", this.IsInformClear);
			}

			if (this.IsSquid)
			{
				spl__LocatorChangeSceneAndKeepWalkAreaBancParam.AddNode("IsSquid", this.IsSquid);
			}

			if (SerializedActor["spl__LocatorChangeSceneAndKeepWalkAreaBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorChangeSceneAndKeepWalkAreaBancParam);
			}
		}
	}
}
