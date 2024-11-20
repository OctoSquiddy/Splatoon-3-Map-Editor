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
	public class Mu_spl__NpcSdodrBancParam
	{
		[ByamlMember]
		public bool IsInitEventVisible { get; set; }

		[ByamlMember]
		public bool IsKinematicWait { get; set; }

		[ByamlMember]
		public bool IsTakeOverVisible { get; set; }

		[ByamlMember]
		public string WaitASCommand { get; set; }

		public Mu_spl__NpcSdodrBancParam()
		{
			IsInitEventVisible = false;
			IsKinematicWait = false;
			IsTakeOverVisible = false;
			WaitASCommand = "";
		}

		public Mu_spl__NpcSdodrBancParam(Mu_spl__NpcSdodrBancParam other)
		{
			IsInitEventVisible = other.IsInitEventVisible;
			IsKinematicWait = other.IsKinematicWait;
			IsTakeOverVisible = other.IsTakeOverVisible;
			WaitASCommand = other.WaitASCommand;
		}

		public Mu_spl__NpcSdodrBancParam Clone()
		{
			return new Mu_spl__NpcSdodrBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__NpcSdodrBancParam = new BymlNode.DictionaryNode() { Name = "spl__NpcSdodrBancParam" };

			if (SerializedActor["spl__NpcSdodrBancParam"] != null)
			{
				spl__NpcSdodrBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__NpcSdodrBancParam"];
			}


			if (this.IsInitEventVisible)
			{
				spl__NpcSdodrBancParam.AddNode("IsInitEventVisible", this.IsInitEventVisible);
			}

			if (this.IsKinematicWait)
			{
				spl__NpcSdodrBancParam.AddNode("IsKinematicWait", this.IsKinematicWait);
			}

			if (this.IsTakeOverVisible)
			{
				spl__NpcSdodrBancParam.AddNode("IsTakeOverVisible", this.IsTakeOverVisible);
			}

			if (this.WaitASCommand != "")
			{
				spl__NpcSdodrBancParam.AddNode("WaitASCommand", this.WaitASCommand);
			}

			if (SerializedActor["spl__NpcSdodrBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__NpcSdodrBancParam);
			}
		}
	}
}
