using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SoundShape_Point : MuObj
	{
		[BindGUI("Name", Category = "SoundShape_Point Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		public SoundShape_Point() : base()
		{
			game__SoundShapeBancParam = new Mu_game__SoundShapeBancParam();

			Links = new List<Link>();
		}

		public SoundShape_Point(SoundShape_Point other) : base(other)
		{
			game__SoundShapeBancParam = other.game__SoundShapeBancParam.Clone();
		}

		public override SoundShape_Point Clone()
		{
			return new SoundShape_Point(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__SoundShapeBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
