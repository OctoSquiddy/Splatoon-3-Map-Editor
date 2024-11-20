using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorNpcWorldCommander : MuObj
	{
		[BindGUI("ConversationKey", Category = "LocatorNpcWorldCommander Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "LocatorNpcWorldCommander Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		public LocatorNpcWorldCommander() : base()
		{
			spl__LocatorNpcWorldBancParam = new Mu_spl__LocatorNpcWorldBancParam();

			Links = new List<Link>();
		}

		public LocatorNpcWorldCommander(LocatorNpcWorldCommander other) : base(other)
		{
			spl__LocatorNpcWorldBancParam = other.spl__LocatorNpcWorldBancParam.Clone();
		}

		public override LocatorNpcWorldCommander Clone()
		{
			return new LocatorNpcWorldCommander(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorNpcWorldBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
