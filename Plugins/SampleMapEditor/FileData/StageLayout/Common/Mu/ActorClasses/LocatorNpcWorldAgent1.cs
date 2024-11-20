using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorNpcWorldAgent1 : MuObj
	{
		[BindGUI("ConversationKey", Category = "LocatorNpcWorldAgent1 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ConversationKey
		{
			get
			{
				return this.spl__LocatorNpcWorldBancParam.ConversationKey;
			}

			set
			{
				this.spl__LocatorNpcWorldBancParam.ConversationKey = value;
			}
		}

		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "LocatorNpcWorldAgent1 Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActivateOnlyInBeingPerformer
		{
			get
			{
				return this.spl__LocatorNpcWorldBancParam.IsActivateOnlyInBeingPerformer;
			}

			set
			{
				this.spl__LocatorNpcWorldBancParam.IsActivateOnlyInBeingPerformer = value;
			}
		}

		[ByamlMember("spl__LocatorNpcWorldBancParam")]
		public Mu_spl__LocatorNpcWorldBancParam spl__LocatorNpcWorldBancParam { get; set; }

		public LocatorNpcWorldAgent1() : base()
		{
			spl__LocatorNpcWorldBancParam = new Mu_spl__LocatorNpcWorldBancParam();

			Links = new List<Link>();
		}

		public LocatorNpcWorldAgent1(LocatorNpcWorldAgent1 other) : base(other)
		{
			spl__LocatorNpcWorldBancParam = other.spl__LocatorNpcWorldBancParam.Clone();
		}

		public override LocatorNpcWorldAgent1 Clone()
		{
			return new LocatorNpcWorldAgent1(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorNpcWorldBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
