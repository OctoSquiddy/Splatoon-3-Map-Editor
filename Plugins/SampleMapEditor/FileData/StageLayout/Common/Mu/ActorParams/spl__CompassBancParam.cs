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
	public class Mu_spl__CompassBancParam
	{
		[ByamlMember]
		public bool IsEndOnContactBasePos { get; set; }

		[ByamlMember]
		public float MoveSpeed { get; set; }

		public Mu_spl__CompassBancParam()
		{
			IsEndOnContactBasePos = false;
			MoveSpeed = 0.0f;
		}

		public Mu_spl__CompassBancParam(Mu_spl__CompassBancParam other)
		{
			IsEndOnContactBasePos = other.IsEndOnContactBasePos;
			MoveSpeed = other.MoveSpeed;
		}

		public Mu_spl__CompassBancParam Clone()
		{
			return new Mu_spl__CompassBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__CompassBancParam = new BymlNode.DictionaryNode() { Name = "spl__CompassBancParam" };

			if (SerializedActor["spl__CompassBancParam"] != null)
			{
				spl__CompassBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__CompassBancParam"];
			}


			if (this.IsEndOnContactBasePos)
			{
				spl__CompassBancParam.AddNode("IsEndOnContactBasePos", this.IsEndOnContactBasePos);
			}

			spl__CompassBancParam.AddNode("MoveSpeed", this.MoveSpeed);

			if (SerializedActor["spl__CompassBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__CompassBancParam);
			}
		}
	}
}
