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
	public class Mu_spl__SoundDryControlAreaParam
	{
		[ByamlMember]
		public float Margin { get; set; }

		[ByamlMember]
		public float Scale { get; set; }

		public Mu_spl__SoundDryControlAreaParam()
		{
			Margin = 0.0f;
			Scale = 0.0f;
		}

		public Mu_spl__SoundDryControlAreaParam(Mu_spl__SoundDryControlAreaParam other)
		{
			Margin = other.Margin;
			Scale = other.Scale;
		}

		public Mu_spl__SoundDryControlAreaParam Clone()
		{
			return new Mu_spl__SoundDryControlAreaParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__SoundDryControlAreaParam = new BymlNode.DictionaryNode() { Name = "spl__SoundDryControlAreaParam" };

			if (SerializedActor["spl__SoundDryControlAreaParam"] != null)
			{
				spl__SoundDryControlAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__SoundDryControlAreaParam"];
			}


			spl__SoundDryControlAreaParam.AddNode("Margin", this.Margin);

			spl__SoundDryControlAreaParam.AddNode("Scale", this.Scale);

			if (SerializedActor["spl__SoundDryControlAreaParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__SoundDryControlAreaParam);
			}
		}
	}
}
