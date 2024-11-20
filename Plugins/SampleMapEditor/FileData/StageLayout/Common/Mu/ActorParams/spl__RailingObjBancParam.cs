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
	public class Mu_spl__RailingObjBancParam
	{
		[ByamlMember]
		public int ObjNum { get; set; }

		[ByamlMember]
		public string ObjRailingType { get; set; }

		[ByamlMember]
		public string OffsetType { get; set; }

		[ByamlMember]
		public int OrderMargin { get; set; }

		[ByamlMember]
		public float OrderOffset { get; set; }

		[ByamlMember]
		public ByamlVector3F PosOffset { get; set; }

		[ByamlMember]
		public ulong ToRailPoint { get; set; }

		public Mu_spl__RailingObjBancParam()
		{
			ObjNum = 0;
			ObjRailingType = "";
			OffsetType = "";
			OrderMargin = 0;
			OrderOffset = 0.0f;
			PosOffset = new ByamlVector3F();
			ToRailPoint = 0;
		}

		public Mu_spl__RailingObjBancParam(Mu_spl__RailingObjBancParam other)
		{
			ObjNum = other.ObjNum;
			ObjRailingType = other.ObjRailingType;
			OffsetType = other.OffsetType;
			OrderMargin = other.OrderMargin;
			OrderOffset = other.OrderOffset;
			PosOffset = new ByamlVector3F(other.PosOffset.X, other.PosOffset.Y, other.PosOffset.Z);
			ToRailPoint = other.ToRailPoint;
		}

		public Mu_spl__RailingObjBancParam Clone()
		{
			return new Mu_spl__RailingObjBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__RailingObjBancParam = new BymlNode.DictionaryNode() { Name = "spl__RailingObjBancParam" };

			if (SerializedActor["spl__RailingObjBancParam"] != null)
			{
				spl__RailingObjBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__RailingObjBancParam"];
			}

			BymlNode.DictionaryNode PosOffset = new BymlNode.DictionaryNode() { Name = "PosOffset" };

			spl__RailingObjBancParam.AddNode("ObjNum", this.ObjNum);

			if (this.ObjRailingType != "")
			{
				spl__RailingObjBancParam.AddNode("ObjRailingType", this.ObjRailingType);
			}

			if (this.OffsetType != "")
			{
				spl__RailingObjBancParam.AddNode("OffsetType", this.OffsetType);
			}

			spl__RailingObjBancParam.AddNode("OrderMargin", this.OrderMargin);

			spl__RailingObjBancParam.AddNode("OrderOffset", this.OrderOffset);

			if (this.PosOffset.Z != 0.0f || this.PosOffset.Y != 0.0f || this.PosOffset.X != 0.0f)
			{
				PosOffset.AddNode("X", this.PosOffset.X);
				PosOffset.AddNode("Y", this.PosOffset.Y);
				PosOffset.AddNode("Z", this.PosOffset.Z);
				spl__RailingObjBancParam.Nodes.Add(PosOffset);
			}

			spl__RailingObjBancParam.AddNode("ToRailPoint", this.ToRailPoint);

			if (SerializedActor["spl__RailingObjBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__RailingObjBancParam);
			}
		}
	}
}
