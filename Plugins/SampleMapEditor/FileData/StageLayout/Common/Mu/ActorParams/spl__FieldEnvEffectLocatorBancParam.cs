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
	public class Mu_spl__FieldEnvEffectLocatorBancParam
	{
		[ByamlMember]
		public string KeyName { get; set; }

		public Mu_spl__FieldEnvEffectLocatorBancParam()
		{
			KeyName = "";
		}

		public Mu_spl__FieldEnvEffectLocatorBancParam(Mu_spl__FieldEnvEffectLocatorBancParam other)
		{
			KeyName = other.KeyName;
		}

		public Mu_spl__FieldEnvEffectLocatorBancParam Clone()
		{
			return new Mu_spl__FieldEnvEffectLocatorBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__FieldEnvEffectLocatorBancParam = new BymlNode.DictionaryNode() { Name = "spl__FieldEnvEffectLocatorBancParam" };

			if (SerializedActor["spl__FieldEnvEffectLocatorBancParam"] != null)
			{
				spl__FieldEnvEffectLocatorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__FieldEnvEffectLocatorBancParam"];
			}


			if (this.KeyName != "")
			{
				spl__FieldEnvEffectLocatorBancParam.AddNode("KeyName", this.KeyName);
			}

			if (SerializedActor["spl__FieldEnvEffectLocatorBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__FieldEnvEffectLocatorBancParam);
			}
		}
	}
}
