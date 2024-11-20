using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Pipeline : MuObj
	{
		[BindGUI("LinkToRailPoint", Category = "Pipeline Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _LinkToRailPoint
		{
			get
			{
				return this.spl__PipelineBancParam.LinkToRailPoint;
			}

			set
			{
				this.spl__PipelineBancParam.LinkToRailPoint = value;
			}
		}

		[ByamlMember("spl__PipelineBancParam")]
		public Mu_spl__PipelineBancParam spl__PipelineBancParam { get; set; }

		public Pipeline() : base()
		{
			spl__PipelineBancParam = new Mu_spl__PipelineBancParam();

			Links = new List<Link>();
		}

		public Pipeline(Pipeline other) : base(other)
		{
			spl__PipelineBancParam = other.spl__PipelineBancParam.Clone();
		}

		public override Pipeline Clone()
		{
			return new Pipeline(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__PipelineBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
