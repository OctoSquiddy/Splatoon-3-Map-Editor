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
	public class Mu_spl__MissionCheckPointBancParam
	{
		[ByamlMember]
		public int AdditionalTime { get; set; }

		[ByamlMember]
		public bool IsLast { get; set; }

		[ByamlMember]
		public int Progress { get; set; }

		public Mu_spl__MissionCheckPointBancParam()
		{
			AdditionalTime = 0;
			IsLast = false;
			Progress = 0;
		}

		public Mu_spl__MissionCheckPointBancParam(Mu_spl__MissionCheckPointBancParam other)
		{
			AdditionalTime = other.AdditionalTime;
			IsLast = other.IsLast;
			Progress = other.Progress;
		}

		public Mu_spl__MissionCheckPointBancParam Clone()
		{
			return new Mu_spl__MissionCheckPointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__MissionCheckPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__MissionCheckPointBancParam" };

			if (SerializedActor["spl__MissionCheckPointBancParam"] != null)
			{
				spl__MissionCheckPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__MissionCheckPointBancParam"];
			}

			if (this.AdditionalTime > 0)
				spl__MissionCheckPointBancParam.AddNode("AdditionalTime", this.AdditionalTime);

			if (this.IsLast)
			{
				spl__MissionCheckPointBancParam.AddNode("IsLast", this.IsLast);
			}

            if (this.Progress > 0)
                spl__MissionCheckPointBancParam.AddNode("Progress", this.Progress);

			if (SerializedActor["spl__MissionCheckPointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__MissionCheckPointBancParam);
			}
		}
	}
}
