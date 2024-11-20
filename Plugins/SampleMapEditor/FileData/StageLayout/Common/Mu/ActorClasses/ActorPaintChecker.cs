using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class ActorPaintChecker : MuObj
	{
		[BindGUI("OffRate", Category = "ActorPaintChecker Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _OffRate
		{
			get
			{
				return this.spl__ActorPaintCheckerBancParam.OffRate;
			}

			set
			{
				this.spl__ActorPaintCheckerBancParam.OffRate = value;
			}
		}

		[BindGUI("OnRate", Category = "ActorPaintChecker Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _OnRate
		{
			get
			{
				return this.spl__ActorPaintCheckerBancParam.OnRate;
			}

			set
			{
				this.spl__ActorPaintCheckerBancParam.OnRate = value;
			}
		}

		[ByamlMember("spl__ActorPaintCheckerBancParam")]
		public Mu_spl__ActorPaintCheckerBancParam spl__ActorPaintCheckerBancParam { get; set; }

		public ActorPaintChecker() : base()
		{
			spl__ActorPaintCheckerBancParam = new Mu_spl__ActorPaintCheckerBancParam();

			Links = new List<Link>();
		}

		public ActorPaintChecker(ActorPaintChecker other) : base(other)
		{
			spl__ActorPaintCheckerBancParam = other.spl__ActorPaintCheckerBancParam.Clone();
		}

		public override ActorPaintChecker Clone()
		{
			return new ActorPaintChecker(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorPaintCheckerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
