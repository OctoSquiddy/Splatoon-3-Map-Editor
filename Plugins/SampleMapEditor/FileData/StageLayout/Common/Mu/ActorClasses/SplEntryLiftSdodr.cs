using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplEntryLiftSdodr : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "SplEntryLiftSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActivateOnlyInBeingPerformer
		{
			get
			{
				return this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer;
			}

			set
			{
				this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer = value;
			}
		}

		[BindGUI("IsShowGetOffUI", Category = "SplEntryLiftSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsShowGetOffUI
		{
			get
			{
				return this.spl__EntryLiftBancParamSdodr.IsShowGetOffUI;
			}

			set
			{
				this.spl__EntryLiftBancParamSdodr.IsShowGetOffUI = value;
			}
		}

		[ByamlMember("game__EventPerformerBancParam")]
		public Mu_game__EventPerformerBancParam game__EventPerformerBancParam { get; set; }

		[ByamlMember("spl__EntryLiftBancParamSdodr")]
		public Mu_spl__EntryLiftBancParamSdodr spl__EntryLiftBancParamSdodr { get; set; }

		public SplEntryLiftSdodr() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__EntryLiftBancParamSdodr = new Mu_spl__EntryLiftBancParamSdodr();

			Links = new List<Link>();
		}

		public SplEntryLiftSdodr(SplEntryLiftSdodr other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__EntryLiftBancParamSdodr = other.spl__EntryLiftBancParamSdodr.Clone();
		}

		public override SplEntryLiftSdodr Clone()
		{
			return new SplEntryLiftSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__EntryLiftBancParamSdodr.SaveParameterBank(SerializedActor);
		}
	}
}
