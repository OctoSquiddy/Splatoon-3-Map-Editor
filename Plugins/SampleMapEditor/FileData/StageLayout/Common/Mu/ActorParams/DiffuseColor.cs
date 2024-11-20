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
	public class Mu_DiffuseColor
	{
		[ByamlMember]
		public float A { get; set; }

		[ByamlMember]
		public float R { get; set; }

		[ByamlMember]
		public float G { get; set; }

		[ByamlMember]
		public float B { get; set; }

		public Mu_DiffuseColor()
		{
			A = 0.0f;
			R = 0.0f;
			G = 0.0f;
			B = 0.0f;
		}

		public Mu_DiffuseColor(Mu_DiffuseColor other)
		{
			A = other.A;
			R = other.R;
			G = other.G;
			B = other.B;
		}

		public Mu_DiffuseColor Clone()
		{
			return new Mu_DiffuseColor(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			if (this.A == 0.0f && this.B == 0.0f && this.G == 0.0f && this.R == 0.0f) return;

			BymlNode.DictionaryNode DiffuseColor = new BymlNode.DictionaryNode() { Name = "DiffuseColor" };

			if (SerializedActor["DiffuseColor"] != null)
			{
				DiffuseColor = (BymlNode.DictionaryNode)SerializedActor["DiffuseColor"];
			}

			DiffuseColor.AddNode("A", this.A);

			DiffuseColor.AddNode("R", this.R);

			DiffuseColor.AddNode("G", this.G);

			DiffuseColor.AddNode("B", this.B);

			if (SerializedActor["DiffuseColor"] == null)
			{
				SerializedActor.Nodes.Add(DiffuseColor);
			}
		}
	}
}
