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
	public class Mu_game__gfx__LocatorPrimitiveLightBancParam
	{
		[ByamlMember]
		public Mu_DiffuseColor Color { get; set; }

		[ByamlMember]
		public float Intensity { get; set; }

		public Mu_game__gfx__LocatorPrimitiveLightBancParam()
		{
			Color = new Mu_DiffuseColor();
			Intensity = 0.0f;
		}

		public Mu_game__gfx__LocatorPrimitiveLightBancParam(Mu_game__gfx__LocatorPrimitiveLightBancParam other)
		{
			Color = other.Color.Clone();
			Intensity = other.Intensity;
		}

		public Mu_game__gfx__LocatorPrimitiveLightBancParam Clone()
		{
			return new Mu_game__gfx__LocatorPrimitiveLightBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode game__gfx__LocatorPrimitiveLightBancParam = new BymlNode.DictionaryNode() { Name = "game__gfx__LocatorPrimitiveLightBancParam" };

			if (SerializedActor["game__gfx__LocatorPrimitiveLightBancParam"] != null)
			{
				game__gfx__LocatorPrimitiveLightBancParam = (BymlNode.DictionaryNode)SerializedActor["game__gfx__LocatorPrimitiveLightBancParam"];
			}

			BymlNode.DictionaryNode Color = new BymlNode.DictionaryNode() { Name = "Color" };

			if (this.Color.A != 0.0f || this.Color.B != 0.0f || this.Color.G != 0.0f || this.Color.R != 0.0f)
			{
				Color.AddNode("A", this.Color.A);
				Color.AddNode("B", this.Color.B);
				Color.AddNode("G", this.Color.G);
				Color.AddNode("R", this.Color.R);
				game__gfx__LocatorPrimitiveLightBancParam.Nodes.Add(Color);
			}

			game__gfx__LocatorPrimitiveLightBancParam.AddNode("Intensity", this.Intensity);

			if (SerializedActor["game__gfx__LocatorPrimitiveLightBancParam"] == null)
			{
				SerializedActor.Nodes.Add(game__gfx__LocatorPrimitiveLightBancParam);
			}
		}
	}
}
