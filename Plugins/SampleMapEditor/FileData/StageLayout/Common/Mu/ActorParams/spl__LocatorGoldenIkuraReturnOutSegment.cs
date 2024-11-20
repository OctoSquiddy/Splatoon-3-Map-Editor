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
	public class Mu_spl__LocatorGoldenIkuraReturnOutSegment
	{
		[ByamlMember]
		public ByamlVector3F EdgeA { get; set; }

		[ByamlMember]
		public ByamlVector3F EdgeB { get; set; }

		public Mu_spl__LocatorGoldenIkuraReturnOutSegment()
		{
			EdgeA = new ByamlVector3F();
			EdgeB = new ByamlVector3F();
		}

		public Mu_spl__LocatorGoldenIkuraReturnOutSegment(Mu_spl__LocatorGoldenIkuraReturnOutSegment other)
		{
			EdgeA = new ByamlVector3F(other.EdgeA.X, other.EdgeA.Y, other.EdgeA.Z);
			EdgeB = new ByamlVector3F(other.EdgeB.X, other.EdgeB.Y, other.EdgeB.Z);
		}

		public Mu_spl__LocatorGoldenIkuraReturnOutSegment Clone()
		{
			return new Mu_spl__LocatorGoldenIkuraReturnOutSegment(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorGoldenIkuraReturnOutSegment = new BymlNode.DictionaryNode() { Name = "spl__LocatorGoldenIkuraReturnOutSegment" };

			if (SerializedActor["spl__LocatorGoldenIkuraReturnOutSegment"] != null)
			{
				spl__LocatorGoldenIkuraReturnOutSegment = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorGoldenIkuraReturnOutSegment"];
			}

			BymlNode.DictionaryNode EdgeA = new BymlNode.DictionaryNode() { Name = "EdgeA" };
			BymlNode.DictionaryNode EdgeB = new BymlNode.DictionaryNode() { Name = "EdgeB" };

			if (this.EdgeA.Z != 0.0f || this.EdgeA.Y != 0.0f || this.EdgeA.X != 0.0f)
			{
				EdgeA.AddNode("X", this.EdgeA.X);
				EdgeA.AddNode("Y", this.EdgeA.Y);
				EdgeA.AddNode("Z", this.EdgeA.Z);
				spl__LocatorGoldenIkuraReturnOutSegment.Nodes.Add(EdgeA);
			}

			if (this.EdgeB.Z != 0.0f || this.EdgeB.Y != 0.0f || this.EdgeB.X != 0.0f)
			{
				EdgeB.AddNode("X", this.EdgeB.X);
				EdgeB.AddNode("Y", this.EdgeB.Y);
				EdgeB.AddNode("Z", this.EdgeB.Z);
				spl__LocatorGoldenIkuraReturnOutSegment.Nodes.Add(EdgeB);
			}

			if (SerializedActor["spl__LocatorGoldenIkuraReturnOutSegment"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorGoldenIkuraReturnOutSegment);
			}
		}
	}
}
