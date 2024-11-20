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
	public class Mu_spl__CirclePlacementObjBancParam
	{
		[ByamlMember]
		public bool IsStretch { get; set; }

		[ByamlMember]
		public float MaxBaseLength { get; set; }

		[ByamlMember]
		public float MinBaseLength { get; set; }

		[ByamlMember]
		public int ObjNum { get; set; }

		[ByamlMember]
		public float Radius { get; set; }

		[ByamlMember]
		public float RotateSpeedDegPerSec { get; set; }

		[ByamlMember]
		public int StopFrame { get; set; }

		[ByamlMember]
		public int StretchFrame { get; set; }

		public Mu_spl__CirclePlacementObjBancParam()
		{
			IsStretch = false;
			MaxBaseLength = 0.0f;
			MinBaseLength = 0.0f;
			ObjNum = 0;
			Radius = 0.0f;
			RotateSpeedDegPerSec = 0.0f;
			StopFrame = 0;
			StretchFrame = 0;
		}

		public Mu_spl__CirclePlacementObjBancParam(Mu_spl__CirclePlacementObjBancParam other)
		{
			IsStretch = other.IsStretch;
			MaxBaseLength = other.MaxBaseLength;
			MinBaseLength = other.MinBaseLength;
			ObjNum = other.ObjNum;
			Radius = other.Radius;
			RotateSpeedDegPerSec = other.RotateSpeedDegPerSec;
			StopFrame = other.StopFrame;
			StretchFrame = other.StretchFrame;
		}

		public Mu_spl__CirclePlacementObjBancParam Clone()
		{
			return new Mu_spl__CirclePlacementObjBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CirclePlacementObjBancParam = new BymlNode.DictionaryNode() { Name = "spl__CirclePlacementObjBancParam" };

			if (SerializedActor["spl__CirclePlacementObjBancParam"] != null)
			{
				spl__CirclePlacementObjBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CirclePlacementObjBancParam"];
			}


			if (this.IsStretch)
			{
				spl__CirclePlacementObjBancParam.AddNode("IsStretch", this.IsStretch);
			}

			spl__CirclePlacementObjBancParam.AddNode("MaxBaseLength", this.MaxBaseLength);

			spl__CirclePlacementObjBancParam.AddNode("MinBaseLength", this.MinBaseLength);

			spl__CirclePlacementObjBancParam.AddNode("ObjNum", this.ObjNum);

			spl__CirclePlacementObjBancParam.AddNode("Radius", this.Radius);

			spl__CirclePlacementObjBancParam.AddNode("RotateSpeedDegPerSec", this.RotateSpeedDegPerSec);

			spl__CirclePlacementObjBancParam.AddNode("StopFrame", this.StopFrame);

			spl__CirclePlacementObjBancParam.AddNode("StretchFrame", this.StretchFrame);

			if (SerializedActor["spl__CirclePlacementObjBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CirclePlacementObjBancParam);
			}
		}
	}
}
