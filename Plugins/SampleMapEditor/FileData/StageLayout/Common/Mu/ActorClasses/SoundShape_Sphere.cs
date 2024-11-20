using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SoundShape_Sphere : MuObj
	{
		[BindGUI("Name", Category = "SoundShape_Sphere Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _Name
		{
			get
			{
				return this.game__SoundShapeBancParam.Name;
			}

			set
			{
				this.game__SoundShapeBancParam.Name = value;
			}
		}

		[ByamlMember("game__SoundShapeBancParam")]
		public Mu_game__SoundShapeBancParam game__SoundShapeBancParam { get; set; }

		public SoundShape_Sphere() : base()
		{
			game__SoundShapeBancParam = new Mu_game__SoundShapeBancParam();

			Links = new List<Link>();
		}

		public SoundShape_Sphere(SoundShape_Sphere other) : base(other)
		{
			game__SoundShapeBancParam = other.game__SoundShapeBancParam.Clone();
		}

		public override SoundShape_Sphere Clone()
		{
			return new SoundShape_Sphere(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__SoundShapeBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
