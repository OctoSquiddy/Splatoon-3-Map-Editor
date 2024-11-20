using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SoundDryControlArea : MuObj
	{
		[BindGUI("Margin", Category = "SoundDryControlArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Margin
		{
			get
			{
				return this.spl__SoundDryControlAreaParam.Margin;
			}

			set
			{
				this.spl__SoundDryControlAreaParam.Margin = value;
			}
		}

		[BindGUI("Scale", Category = "SoundDryControlArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Scale
		{
			get
			{
				return this.spl__SoundDryControlAreaParam.Scale;
			}

			set
			{
				this.spl__SoundDryControlAreaParam.Scale = value;
			}
		}

		[ByamlMember("spl__SoundDryControlAreaParam")]
		public Mu_spl__SoundDryControlAreaParam spl__SoundDryControlAreaParam { get; set; }

		public SoundDryControlArea() : base()
		{
			spl__SoundDryControlAreaParam = new Mu_spl__SoundDryControlAreaParam();

			Links = new List<Link>();
		}

		public SoundDryControlArea(SoundDryControlArea other) : base(other)
		{
			spl__SoundDryControlAreaParam = other.spl__SoundDryControlAreaParam.Clone();
		}

		public override SoundDryControlArea Clone()
		{
			return new SoundDryControlArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SoundDryControlAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
