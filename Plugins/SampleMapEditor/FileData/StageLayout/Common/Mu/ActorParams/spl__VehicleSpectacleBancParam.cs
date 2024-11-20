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
	public class Mu_spl__VehicleSpectacleBancParam
	{
		public Mu_spl__VehicleSpectacleBancParam()
		{
		}

		public Mu_spl__VehicleSpectacleBancParam(Mu_spl__VehicleSpectacleBancParam other)
		{
		}

		public Mu_spl__VehicleSpectacleBancParam Clone()
		{
			return new Mu_spl__VehicleSpectacleBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__VehicleSpectacleBancParam = new BymlNode.DictionaryNode() { Name = "spl__VehicleSpectacleBancParam" };

			if (SerializedActor["spl__VehicleSpectacleBancParam"] != null)
			{
				spl__VehicleSpectacleBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__VehicleSpectacleBancParam"];
			}


			if (SerializedActor["spl__VehicleSpectacleBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__VehicleSpectacleBancParam);
			}
		}
	}
}
