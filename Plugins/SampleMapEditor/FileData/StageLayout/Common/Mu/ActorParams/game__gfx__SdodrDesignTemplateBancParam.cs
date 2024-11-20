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
	public class Mu_game__gfx__SdodrDesignTemplateBancParam
	{
		[ByamlMember]
		public bool CreateBlister { get; set; }

		[ByamlMember]
		public string PaintColAttr { get; set; }

		public Mu_game__gfx__SdodrDesignTemplateBancParam()
		{
			CreateBlister = false;
			PaintColAttr = "";
		}

		public Mu_game__gfx__SdodrDesignTemplateBancParam(Mu_game__gfx__SdodrDesignTemplateBancParam other)
		{
			CreateBlister = other.CreateBlister;
			PaintColAttr = other.PaintColAttr;
		}

		public Mu_game__gfx__SdodrDesignTemplateBancParam Clone()
		{
			return new Mu_game__gfx__SdodrDesignTemplateBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode game__gfx__SdodrDesignTemplateBancParam = new BymlNode.DictionaryNode() { Name = "game__gfx__SdodrDesignTemplateBancParam" };

			if (SerializedActor["game__gfx__SdodrDesignTemplateBancParam"] != null)
			{
				game__gfx__SdodrDesignTemplateBancParam = (BymlNode.DictionaryNode)SerializedActor["game__gfx__SdodrDesignTemplateBancParam"];
			}


			if (this.CreateBlister)
			{
				game__gfx__SdodrDesignTemplateBancParam.AddNode("CreateBlister", this.CreateBlister);
			}

			if (this.PaintColAttr != "")
			{
				game__gfx__SdodrDesignTemplateBancParam.AddNode("PaintColAttr", this.PaintColAttr);
			}

			if (SerializedActor["game__gfx__SdodrDesignTemplateBancParam"] == null)
			{
				SerializedActor.Nodes.Add(game__gfx__SdodrDesignTemplateBancParam);
			}
		}
	}
}
