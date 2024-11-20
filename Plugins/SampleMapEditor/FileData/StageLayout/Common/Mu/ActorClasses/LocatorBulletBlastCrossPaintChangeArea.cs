using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorBulletBlastCrossPaintChangeArea : MuObj
	{
		[BindGUI("ForceAreaBottom", Category = "LocatorBulletBlastCrossPaintChangeArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _ForceAreaBottom
		{
			get
			{
				return this.spl__LocatorBulletBlastCrossPaintChangeAreaParam.ForceAreaBottom;
			}

			set
			{
				this.spl__LocatorBulletBlastCrossPaintChangeAreaParam.ForceAreaBottom = value;
			}
		}

		[BindGUI("RotateDegree", Category = "LocatorBulletBlastCrossPaintChangeArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _RotateDegree
		{
			get
			{
				return this.spl__LocatorBulletBlastCrossPaintChangeAreaParam.RotateDegree;
			}

			set
			{
				this.spl__LocatorBulletBlastCrossPaintChangeAreaParam.RotateDegree = value;
			}
		}

		[ByamlMember("spl__LocatorBulletBlastCrossPaintChangeAreaParam")]
		public Mu_spl__LocatorBulletBlastCrossPaintChangeAreaParam spl__LocatorBulletBlastCrossPaintChangeAreaParam { get; set; }

		public LocatorBulletBlastCrossPaintChangeArea() : base()
		{
			spl__LocatorBulletBlastCrossPaintChangeAreaParam = new Mu_spl__LocatorBulletBlastCrossPaintChangeAreaParam();

			Links = new List<Link>();
		}

		public LocatorBulletBlastCrossPaintChangeArea(LocatorBulletBlastCrossPaintChangeArea other) : base(other)
		{
			spl__LocatorBulletBlastCrossPaintChangeAreaParam = other.spl__LocatorBulletBlastCrossPaintChangeAreaParam.Clone();
		}

		public override LocatorBulletBlastCrossPaintChangeArea Clone()
		{
			return new LocatorBulletBlastCrossPaintChangeArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorBulletBlastCrossPaintChangeAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
