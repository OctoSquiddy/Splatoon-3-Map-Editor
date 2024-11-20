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
	public class Mu_spl__LocatorTricolTenjinSpawnerLandingPointBancParam
	{
		[ByamlMember]
		public float PitchDegree { get; set; }

		[ByamlMember]
		public int SpanFrame { get; set; }

		[ByamlMember]
		public float SpeedBase { get; set; }

		[ByamlMember]
		public float SpeedRandom { get; set; }

		[ByamlMember]
		public int WaitFrame { get; set; }

		[ByamlMember]
		public float YawDegreeRandom { get; set; }

		public Mu_spl__LocatorTricolTenjinSpawnerLandingPointBancParam()
		{
			PitchDegree = 0.0f;
			SpanFrame = 0;
			SpeedBase = 0.0f;
			SpeedRandom = 0.0f;
			WaitFrame = 0;
			YawDegreeRandom = 0.0f;
		}

		public Mu_spl__LocatorTricolTenjinSpawnerLandingPointBancParam(Mu_spl__LocatorTricolTenjinSpawnerLandingPointBancParam other)
		{
			PitchDegree = other.PitchDegree;
			SpanFrame = other.SpanFrame;
			SpeedBase = other.SpeedBase;
			SpeedRandom = other.SpeedRandom;
			WaitFrame = other.WaitFrame;
			YawDegreeRandom = other.YawDegreeRandom;
		}

		public Mu_spl__LocatorTricolTenjinSpawnerLandingPointBancParam Clone()
		{
			return new Mu_spl__LocatorTricolTenjinSpawnerLandingPointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorTricolTenjinSpawnerLandingPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorTricolTenjinSpawnerLandingPointBancParam" };

			if (SerializedActor["spl__LocatorTricolTenjinSpawnerLandingPointBancParam"] != null)
			{
				spl__LocatorTricolTenjinSpawnerLandingPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorTricolTenjinSpawnerLandingPointBancParam"];
			}


			spl__LocatorTricolTenjinSpawnerLandingPointBancParam.AddNode("PitchDegree", this.PitchDegree);

			spl__LocatorTricolTenjinSpawnerLandingPointBancParam.AddNode("SpanFrame", this.SpanFrame);

			spl__LocatorTricolTenjinSpawnerLandingPointBancParam.AddNode("SpeedBase", this.SpeedBase);

			spl__LocatorTricolTenjinSpawnerLandingPointBancParam.AddNode("SpeedRandom", this.SpeedRandom);

			spl__LocatorTricolTenjinSpawnerLandingPointBancParam.AddNode("WaitFrame", this.WaitFrame);

			spl__LocatorTricolTenjinSpawnerLandingPointBancParam.AddNode("YawDegreeRandom", this.YawDegreeRandom);

			if (SerializedActor["spl__LocatorTricolTenjinSpawnerLandingPointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorTricolTenjinSpawnerLandingPointBancParam);
			}
		}
	}
}
