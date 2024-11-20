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
	public class Mu_spl__ActionAreaSwitchForPlayerBancParam
	{
		[ByamlMember]
		public bool EnableMsnArmorDieToZero { get; set; }

		[ByamlMember]
		public bool EnableMsnArmorGetFromZero { get; set; }

		[ByamlMember]
		public bool EnablePlayerSpecialStart { get; set; }

		[ByamlMember]
		public bool EnableSpecialFullSuperLanding { get; set; }

		public Mu_spl__ActionAreaSwitchForPlayerBancParam()
		{
			EnableMsnArmorDieToZero = false;
			EnableMsnArmorGetFromZero = false;
			EnablePlayerSpecialStart = false;
			EnableSpecialFullSuperLanding = false;
		}

		public Mu_spl__ActionAreaSwitchForPlayerBancParam(Mu_spl__ActionAreaSwitchForPlayerBancParam other)
		{
			EnableMsnArmorDieToZero = other.EnableMsnArmorDieToZero;
			EnableMsnArmorGetFromZero = other.EnableMsnArmorGetFromZero;
			EnablePlayerSpecialStart = other.EnablePlayerSpecialStart;
			EnableSpecialFullSuperLanding = other.EnableSpecialFullSuperLanding;
		}

		public Mu_spl__ActionAreaSwitchForPlayerBancParam Clone()
		{
			return new Mu_spl__ActionAreaSwitchForPlayerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ActionAreaSwitchForPlayerBancParam = new BymlNode.DictionaryNode() { Name = "spl__ActionAreaSwitchForPlayerBancParam" };

			if (SerializedActor["spl__ActionAreaSwitchForPlayerBancParam"] != null)
			{
				spl__ActionAreaSwitchForPlayerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ActionAreaSwitchForPlayerBancParam"];
			}


			if (this.EnableMsnArmorDieToZero)
			{
				spl__ActionAreaSwitchForPlayerBancParam.AddNode("EnableMsnArmorDieToZero", this.EnableMsnArmorDieToZero);
			}

			if (this.EnableMsnArmorGetFromZero)
			{
				spl__ActionAreaSwitchForPlayerBancParam.AddNode("EnableMsnArmorGetFromZero", this.EnableMsnArmorGetFromZero);
			}

			if (this.EnablePlayerSpecialStart)
			{
				spl__ActionAreaSwitchForPlayerBancParam.AddNode("EnablePlayerSpecialStart", this.EnablePlayerSpecialStart);
			}

			if (this.EnableSpecialFullSuperLanding)
			{
				spl__ActionAreaSwitchForPlayerBancParam.AddNode("EnableSpecialFullSuperLanding", this.EnableSpecialFullSuperLanding);
			}

			if (SerializedActor["spl__ActionAreaSwitchForPlayerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ActionAreaSwitchForPlayerBancParam);
			}
		}
	}
}
