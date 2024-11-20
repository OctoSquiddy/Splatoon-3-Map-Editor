using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class DummyTextArea : MuObj
	{
		[BindGUI("DummyText", Category = "DummyTextArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _DummyText
		{
			get
			{
				return this.spl__DummyTextAreaBancParam.DummyText;
			}

			set
			{
				this.spl__DummyTextAreaBancParam.DummyText = value;
			}
		}

		[ByamlMember("spl__DummyTextAreaBancParam")]
		public Mu_spl__DummyTextAreaBancParam spl__DummyTextAreaBancParam { get; set; }

		public DummyTextArea() : base()
		{
			spl__DummyTextAreaBancParam = new Mu_spl__DummyTextAreaBancParam();

			Links = new List<Link>();
		}

		public DummyTextArea(DummyTextArea other) : base(other)
		{
			spl__DummyTextAreaBancParam = other.spl__DummyTextAreaBancParam.Clone();
		}

		public override DummyTextArea Clone()
		{
			return new DummyTextArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__DummyTextAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
