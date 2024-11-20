using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CullingOcclusionArea : MuObj
	{
		[BindGUI("Addition", Category = "CullingOcclusionArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Addition
		{
			get
			{
				return this.spl__CullingOcclusionAreaParam.Addition;
			}

			set
			{
				this.spl__CullingOcclusionAreaParam.Addition = value;
			}
		}

		[ByamlMember("spl__CullingOcclusionAreaParam")]
		public Mu_spl__CullingOcclusionAreaParam spl__CullingOcclusionAreaParam { get; set; }

		public CullingOcclusionArea() : base()
		{
			spl__CullingOcclusionAreaParam = new Mu_spl__CullingOcclusionAreaParam();

			Links = new List<Link>();
		}

		public CullingOcclusionArea(CullingOcclusionArea other) : base(other)
		{
			spl__CullingOcclusionAreaParam = other.spl__CullingOcclusionAreaParam.Clone();
		}

		public override CullingOcclusionArea Clone()
		{
			return new CullingOcclusionArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CullingOcclusionAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
