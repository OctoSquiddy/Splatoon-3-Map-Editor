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
	public class Mu_spl__LocatorPlayerPatchAreaParam
	{
		[ByamlMember]
		public float MoveResistXZ_Th { get; set; }

		[ByamlMember]
		public float MoveResistXZ_Val { get; set; }

		[ByamlMember]
		public bool NiceBallHoverAltitudeInflate { get; set; }

		[ByamlMember]
		public bool NoAirFall { get; set; }

		public Mu_spl__LocatorPlayerPatchAreaParam()
		{
			MoveResistXZ_Th = 0.0f;
			MoveResistXZ_Val = 0.0f;
			NiceBallHoverAltitudeInflate = false;
			NoAirFall = false;
		}

		public Mu_spl__LocatorPlayerPatchAreaParam(Mu_spl__LocatorPlayerPatchAreaParam other)
		{
			MoveResistXZ_Th = other.MoveResistXZ_Th;
			MoveResistXZ_Val = other.MoveResistXZ_Val;
			NiceBallHoverAltitudeInflate = other.NiceBallHoverAltitudeInflate;
			NoAirFall = other.NoAirFall;
		}

		public Mu_spl__LocatorPlayerPatchAreaParam Clone()
		{
			return new Mu_spl__LocatorPlayerPatchAreaParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorPlayerPatchAreaParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorPlayerPatchAreaParam" };

			if (SerializedActor["spl__LocatorPlayerPatchAreaParam"] != null)
			{
				spl__LocatorPlayerPatchAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorPlayerPatchAreaParam"];
			}


			spl__LocatorPlayerPatchAreaParam.AddNode("MoveResistXZ_Th", this.MoveResistXZ_Th);

			spl__LocatorPlayerPatchAreaParam.AddNode("MoveResistXZ_Val", this.MoveResistXZ_Val);

			if (this.NiceBallHoverAltitudeInflate)
			{
				spl__LocatorPlayerPatchAreaParam.AddNode("NiceBallHoverAltitudeInflate", this.NiceBallHoverAltitudeInflate);
			}

			if (this.NoAirFall)
			{
				spl__LocatorPlayerPatchAreaParam.AddNode("NoAirFall", this.NoAirFall);
			}

			if (SerializedActor["spl__LocatorPlayerPatchAreaParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorPlayerPatchAreaParam);
			}
		}
	}
}
