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
	public class Mu_spl__LocatorNpcPlazaFixedBancParam
	{
		[ByamlMember]
		public string ASCommand { get; set; }

		[ByamlMember]
		public string BindActor { get; set; }

		[ByamlMember]
		public string BindBone { get; set; }

		[ByamlMember]
		public bool ForMiniGame { get; set; }

		[ByamlMember]
		public bool LookPlayer { get; set; }

		[ByamlMember]
		public string SourceType { get; set; }

		[ByamlMember]
		public int VariationId { get; set; }

		public Mu_spl__LocatorNpcPlazaFixedBancParam()
		{
			ASCommand = "";
			BindActor = "";
			BindBone = "";
			ForMiniGame = false;
			LookPlayer = false;
			SourceType = "";
			VariationId = 0;
		}

		public Mu_spl__LocatorNpcPlazaFixedBancParam(Mu_spl__LocatorNpcPlazaFixedBancParam other)
		{
			ASCommand = other.ASCommand;
			BindActor = other.BindActor;
			BindBone = other.BindBone;
			ForMiniGame = other.ForMiniGame;
			LookPlayer = other.LookPlayer;
			SourceType = other.SourceType;
			VariationId = other.VariationId;
		}

		public Mu_spl__LocatorNpcPlazaFixedBancParam Clone()
		{
			return new Mu_spl__LocatorNpcPlazaFixedBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorNpcPlazaFixedBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorNpcPlazaFixedBancParam" };

			if (SerializedActor["spl__LocatorNpcPlazaFixedBancParam"] != null)
			{
				spl__LocatorNpcPlazaFixedBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorNpcPlazaFixedBancParam"];
			}


			if (this.ASCommand != "")
			{
				spl__LocatorNpcPlazaFixedBancParam.AddNode("ASCommand", this.ASCommand);
			}

			if (this.BindActor != "")
			{
				spl__LocatorNpcPlazaFixedBancParam.AddNode("BindActor", this.BindActor);
			}

			if (this.BindBone != "")
			{
				spl__LocatorNpcPlazaFixedBancParam.AddNode("BindBone", this.BindBone);
			}

			if (this.ForMiniGame)
			{
				spl__LocatorNpcPlazaFixedBancParam.AddNode("ForMiniGame", this.ForMiniGame);
			}

			if (this.LookPlayer)
			{
				spl__LocatorNpcPlazaFixedBancParam.AddNode("LookPlayer", this.LookPlayer);
			}

			if (this.SourceType != "")
			{
				spl__LocatorNpcPlazaFixedBancParam.AddNode("SourceType", this.SourceType);
			}

			spl__LocatorNpcPlazaFixedBancParam.AddNode("VariationId", this.VariationId);

			if (SerializedActor["spl__LocatorNpcPlazaFixedBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorNpcPlazaFixedBancParam);
			}
		}
	}
}
