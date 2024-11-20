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
	public class Mu_spl__RivalAppearPointBancParam
	{
		[ByamlMember]
		public int EscapeRestHpBorder { get; set; }

		[ByamlMember]
		public bool IsSearchGroundOnSuperJump { get; set; }

		[ByamlMember]
		public bool IsShotSwitch { get; set; }

		[ByamlMember]
		public bool IsStrongLook { get; set; }

		[ByamlMember]
		public string MainWeapon { get; set; }

		[ByamlMember]
		public string SpawnType { get; set; }

		[ByamlMember]
		public float SpecialChargeSec { get; set; }

		[ByamlMember]
		public float SpecialReChargeSec { get; set; }

		[ByamlMember]
		public string SpecialWeapon { get; set; }

		[ByamlMember]
		public string SubWeapon { get; set; }

		[ByamlMember]
		public string SwitchType { get; set; }

		public Mu_spl__RivalAppearPointBancParam()
		{
			EscapeRestHpBorder = 0;
			IsSearchGroundOnSuperJump = false;
			IsShotSwitch = false;
			IsStrongLook = false;
			MainWeapon = "";
			SpawnType = "";
			SpecialChargeSec = 0.0f;
			SpecialReChargeSec = 0.0f;
			SpecialWeapon = "";
			SubWeapon = "";
			SwitchType = "";
		}

		public Mu_spl__RivalAppearPointBancParam(Mu_spl__RivalAppearPointBancParam other)
		{
			EscapeRestHpBorder = other.EscapeRestHpBorder;
			IsSearchGroundOnSuperJump = other.IsSearchGroundOnSuperJump;
			IsShotSwitch = other.IsShotSwitch;
			IsStrongLook = other.IsStrongLook;
			MainWeapon = other.MainWeapon;
			SpawnType = other.SpawnType;
			SpecialChargeSec = other.SpecialChargeSec;
			SpecialReChargeSec = other.SpecialReChargeSec;
			SpecialWeapon = other.SpecialWeapon;
			SubWeapon = other.SubWeapon;
			SwitchType = other.SwitchType;
		}

		public Mu_spl__RivalAppearPointBancParam Clone()
		{
			return new Mu_spl__RivalAppearPointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__RivalAppearPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__RivalAppearPointBancParam" };

			if (SerializedActor["spl__RivalAppearPointBancParam"] != null)
			{
				spl__RivalAppearPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RivalAppearPointBancParam"];
			}


			spl__RivalAppearPointBancParam.AddNode("EscapeRestHpBorder", this.EscapeRestHpBorder);

			if (this.IsSearchGroundOnSuperJump)
			{
				spl__RivalAppearPointBancParam.AddNode("IsSearchGroundOnSuperJump", this.IsSearchGroundOnSuperJump);
			}

			if (this.IsShotSwitch)
			{
				spl__RivalAppearPointBancParam.AddNode("IsShotSwitch", this.IsShotSwitch);
			}

			if (this.IsStrongLook)
			{
				spl__RivalAppearPointBancParam.AddNode("IsStrongLook", this.IsStrongLook);
			}

			if (this.MainWeapon != "")
			{
				spl__RivalAppearPointBancParam.AddNode("MainWeapon", this.MainWeapon);
			}

			if (this.SpawnType != "")
			{
				spl__RivalAppearPointBancParam.AddNode("SpawnType", this.SpawnType);
			}

			spl__RivalAppearPointBancParam.AddNode("SpecialChargeSec", this.SpecialChargeSec);

			spl__RivalAppearPointBancParam.AddNode("SpecialReChargeSec", this.SpecialReChargeSec);

			if (this.SpecialWeapon != "")
			{
				spl__RivalAppearPointBancParam.AddNode("SpecialWeapon", this.SpecialWeapon);
			}

			if (this.SubWeapon != "")
			{
				spl__RivalAppearPointBancParam.AddNode("SubWeapon", this.SubWeapon);
			}

			if (this.SwitchType != "")
			{
				spl__RivalAppearPointBancParam.AddNode("SwitchType", this.SwitchType);
			}

			if (SerializedActor["spl__RivalAppearPointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__RivalAppearPointBancParam);
			}
		}
	}
}
