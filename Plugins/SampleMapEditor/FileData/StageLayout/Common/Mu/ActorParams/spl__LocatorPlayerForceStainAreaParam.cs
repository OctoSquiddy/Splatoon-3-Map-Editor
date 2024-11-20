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
	public class Mu_spl__LocatorPlayerForceStainAreaParam
	{
		[ByamlMember]
		public float StainRt_Body_Human { get; set; }

		[ByamlMember]
		public float StainRt_Body_Squid { get; set; }

		[ByamlMember]
		public float StainRt_Bottom { get; set; }

		[ByamlMember]
		public float StainRt_Clothes { get; set; }

		[ByamlMember]
		public float StainRt_Head { get; set; }

		[ByamlMember]
		public float StainRt_Shoes { get; set; }

		public Mu_spl__LocatorPlayerForceStainAreaParam()
		{
			StainRt_Body_Human = 0.0f;
			StainRt_Body_Squid = 0.0f;
			StainRt_Bottom = 0.0f;
			StainRt_Clothes = 0.0f;
			StainRt_Head = 0.0f;
			StainRt_Shoes = 0.0f;
		}

		public Mu_spl__LocatorPlayerForceStainAreaParam(Mu_spl__LocatorPlayerForceStainAreaParam other)
		{
			StainRt_Body_Human = other.StainRt_Body_Human;
			StainRt_Body_Squid = other.StainRt_Body_Squid;
			StainRt_Bottom = other.StainRt_Bottom;
			StainRt_Clothes = other.StainRt_Clothes;
			StainRt_Head = other.StainRt_Head;
			StainRt_Shoes = other.StainRt_Shoes;
		}

		public Mu_spl__LocatorPlayerForceStainAreaParam Clone()
		{
			return new Mu_spl__LocatorPlayerForceStainAreaParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorPlayerForceStainAreaParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorPlayerForceStainAreaParam" };

			if (SerializedActor["spl__LocatorPlayerForceStainAreaParam"] != null)
			{
				spl__LocatorPlayerForceStainAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorPlayerForceStainAreaParam"];
			}


			spl__LocatorPlayerForceStainAreaParam.AddNode("StainRt_Body_Human", this.StainRt_Body_Human);

			spl__LocatorPlayerForceStainAreaParam.AddNode("StainRt_Body_Squid", this.StainRt_Body_Squid);

			spl__LocatorPlayerForceStainAreaParam.AddNode("StainRt_Bottom", this.StainRt_Bottom);

			spl__LocatorPlayerForceStainAreaParam.AddNode("StainRt_Clothes", this.StainRt_Clothes);

			spl__LocatorPlayerForceStainAreaParam.AddNode("StainRt_Head", this.StainRt_Head);

			spl__LocatorPlayerForceStainAreaParam.AddNode("StainRt_Shoes", this.StainRt_Shoes);

			if (SerializedActor["spl__LocatorPlayerForceStainAreaParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorPlayerForceStainAreaParam);
			}
		}
	}
}
