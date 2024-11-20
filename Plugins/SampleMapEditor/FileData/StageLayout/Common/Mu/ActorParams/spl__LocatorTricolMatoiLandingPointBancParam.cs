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
	public class Mu_spl__LocatorTricolMatoiLandingPointBancParam
	{
		[ByamlMember]
		public float Height { get; set; }

		[ByamlMember]
		public float TargetPaintRadius { get; set; }

		public Mu_spl__LocatorTricolMatoiLandingPointBancParam()
		{
			Height = 0.0f;
			TargetPaintRadius = 13.0f;
		}

		public Mu_spl__LocatorTricolMatoiLandingPointBancParam(Mu_spl__LocatorTricolMatoiLandingPointBancParam other)
		{
			Height = other.Height;
			TargetPaintRadius = other.TargetPaintRadius;
		}

		public Mu_spl__LocatorTricolMatoiLandingPointBancParam Clone()
		{
			return new Mu_spl__LocatorTricolMatoiLandingPointBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorTricolMatoiLandingPointBancParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorTricolMatoiLandingPointBancParam" };

			if (SerializedActor["spl__LocatorTricolMatoiLandingPointBancParam"] != null)
			{
				spl__LocatorTricolMatoiLandingPointBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorTricolMatoiLandingPointBancParam"];
			}

			if (this.Height > 0.0f) spl__LocatorTricolMatoiLandingPointBancParam.AddNode("Height", this.Height);

			spl__LocatorTricolMatoiLandingPointBancParam.AddNode("TargetPaintRadius", this.TargetPaintRadius);

			if (SerializedActor["spl__LocatorTricolMatoiLandingPointBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorTricolMatoiLandingPointBancParam);
			}
		}
	}
}
