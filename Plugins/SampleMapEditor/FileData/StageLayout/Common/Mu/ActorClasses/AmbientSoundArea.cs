using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class AmbientSoundArea : MuObj
	{
		[BindGUI("IsSetPos", Category = "AmbientSoundArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsSetPos
		{
			get
			{
				return this.game__AmbientSoundAreaBancParam.IsSetPos;
			}

			set
			{
				this.game__AmbientSoundAreaBancParam.IsSetPos = value;
			}
		}

		[BindGUI("Key", Category = "AmbientSoundArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Key
		{
			get
			{
				return this.game__AmbientSoundAreaBancParam.Key;
			}

			set
			{
				this.game__AmbientSoundAreaBancParam.Key = value;
			}
		}

		[BindGUI("Margin", Category = "AmbientSoundArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Margin
		{
			get
			{
				return this.game__AmbientSoundAreaBancParam.Margin;
			}

			set
			{
				this.game__AmbientSoundAreaBancParam.Margin = value;
			}
		}

		[ByamlMember("game__AmbientSoundAreaBancParam")]
		public Mu_game__AmbientSoundAreaBancParam game__AmbientSoundAreaBancParam { get; set; }

		public AmbientSoundArea() : base()
		{
			game__AmbientSoundAreaBancParam = new Mu_game__AmbientSoundAreaBancParam();

			Links = new List<Link>();
		}

		public AmbientSoundArea(AmbientSoundArea other) : base(other)
		{
			game__AmbientSoundAreaBancParam = other.game__AmbientSoundAreaBancParam.Clone();
		}

		public override AmbientSoundArea Clone()
		{
			return new AmbientSoundArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__AmbientSoundAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
