using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SoundFxArea : MuObj
	{
		[BindGUI("Bus", Category = "SoundFxArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Bus
		{
			get
			{
				return this.game__SoundFxAreaBancParam.Bus;
			}

			set
			{
				this.game__SoundFxAreaBancParam.Bus = value;
			}
		}

		[BindGUI("Margin", Category = "SoundFxArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Margin
		{
			get
			{
				return this.game__SoundFxAreaBancParam.Margin;
			}

			set
			{
				this.game__SoundFxAreaBancParam.Margin = value;
			}
		}

		[BindGUI("OcclusionMax", Category = "SoundFxArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _OcclusionMax
		{
			get
			{
				return this.game__SoundFxAreaBancParam.OcclusionMax;
			}

			set
			{
				this.game__SoundFxAreaBancParam.OcclusionMax = value;
			}
		}

		[ByamlMember("game__SoundFxAreaBancParam")]
		public Mu_game__SoundFxAreaBancParam game__SoundFxAreaBancParam { get; set; }

		public SoundFxArea() : base()
		{
			game__SoundFxAreaBancParam = new Mu_game__SoundFxAreaBancParam();

			Links = new List<Link>();
		}

		public SoundFxArea(SoundFxArea other) : base(other)
		{
			game__SoundFxAreaBancParam = other.game__SoundFxAreaBancParam.Clone();
		}

		public override SoundFxArea Clone()
		{
			return new SoundFxArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__SoundFxAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
