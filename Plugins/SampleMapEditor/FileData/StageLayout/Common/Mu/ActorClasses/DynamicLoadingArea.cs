using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class DynamicLoadingArea : MuObj
	{
		[BindGUI("Id", Category = "DynamicLoadingArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _Id
		{
			get
			{
				return this.spl__DynamicLoadingAreaBancParam.Id;
			}

			set
			{
				this.spl__DynamicLoadingAreaBancParam.Id = value;
			}
		}

		[BindGUI("Layer", Category = "DynamicLoadingArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Layer
		{
			get
			{
				return this.spl__DynamicLoadingAreaBancParam.Layer;
			}

			set
			{
				this.spl__DynamicLoadingAreaBancParam.Layer = value;
			}
		}

		[ByamlMember("spl__DynamicLoadingAreaBancParam")]
		public Mu_spl__DynamicLoadingAreaBancParam spl__DynamicLoadingAreaBancParam { get; set; }

		public DynamicLoadingArea() : base()
		{
			spl__DynamicLoadingAreaBancParam = new Mu_spl__DynamicLoadingAreaBancParam();

			Links = new List<Link>();
		}

		public DynamicLoadingArea(DynamicLoadingArea other) : base(other)
		{
			spl__DynamicLoadingAreaBancParam = other.spl__DynamicLoadingAreaBancParam.Clone();
		}

		public override DynamicLoadingArea Clone()
		{
			return new DynamicLoadingArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__DynamicLoadingAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
