using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CullingArea : MuObj
	{
		[BindGUI("Id", Category = "CullingArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _Id
		{
			get
			{
				return this.spl__CullingAreaBancParam.Id;
			}

			set
			{
				this.spl__CullingAreaBancParam.Id = value;
			}
		}

		[ByamlMember("spl__CullingAreaBancParam")]
		public Mu_spl__CullingAreaBancParam spl__CullingAreaBancParam { get; set; }

		public CullingArea() : base()
		{
			spl__CullingAreaBancParam = new Mu_spl__CullingAreaBancParam();

			Links = new List<Link>();
		}

		public CullingArea(CullingArea other) : base(other)
		{
			spl__CullingAreaBancParam = other.spl__CullingAreaBancParam.Clone();
		}

		public override CullingArea Clone()
		{
			return new CullingArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CullingAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
