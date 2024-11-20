using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CompassWithNavigator : MuObj
	{
		[BindGUI("IsEndOnContactBasePos", Category = "CompassWithNavigator Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsEndOnContactBasePos
		{
			get
			{
				return this.spl__CompassBancParam.IsEndOnContactBasePos;
			}

			set
			{
				this.spl__CompassBancParam.IsEndOnContactBasePos = value;
			}
		}

		[BindGUI("MoveSpeed", Category = "CompassWithNavigator Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MoveSpeed
		{
			get
			{
				return this.spl__CompassBancParam.MoveSpeed;
			}

			set
			{
				this.spl__CompassBancParam.MoveSpeed = value;
			}
		}

		[ByamlMember("spl__CompassBancParam")]
		public Mu_spl__CompassBancParam spl__CompassBancParam { get; set; }

		public CompassWithNavigator() : base()
		{
			spl__CompassBancParam = new Mu_spl__CompassBancParam();

			Links = new List<Link>();
		}

		public CompassWithNavigator(CompassWithNavigator other) : base(other)
		{
			spl__CompassBancParam = other.spl__CompassBancParam.Clone();
		}

		public override CompassWithNavigator Clone()
		{
			return new CompassWithNavigator(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CompassBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
