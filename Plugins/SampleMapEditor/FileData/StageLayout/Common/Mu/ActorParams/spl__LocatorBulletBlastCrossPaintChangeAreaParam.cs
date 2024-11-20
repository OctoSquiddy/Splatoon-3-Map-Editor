using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	[ByamlObject]
	public class Mu_spl__LocatorBulletBlastCrossPaintChangeAreaParam
	{
		[ByamlMember]
		public bool ForceAreaBottom { get; set; }

		[ByamlMember]
		public float RotateDegree { get; set; }

		public Mu_spl__LocatorBulletBlastCrossPaintChangeAreaParam()
		{
			ForceAreaBottom = false;
			RotateDegree = 0.0f;
		}

		public Mu_spl__LocatorBulletBlastCrossPaintChangeAreaParam(Mu_spl__LocatorBulletBlastCrossPaintChangeAreaParam other)
		{
			ForceAreaBottom = other.ForceAreaBottom;
			RotateDegree = other.RotateDegree;
		}

		public Mu_spl__LocatorBulletBlastCrossPaintChangeAreaParam Clone()
		{
			return new Mu_spl__LocatorBulletBlastCrossPaintChangeAreaParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__LocatorBulletBlastCrossPaintChangeAreaParam = new BymlNode.DictionaryNode() { Name = "spl__LocatorBulletBlastCrossPaintChangeAreaParam" };

			if (SerializedActor["spl__LocatorBulletBlastCrossPaintChangeAreaParam"] != null)
			{
				spl__LocatorBulletBlastCrossPaintChangeAreaParam = (BymlNode.DictionaryNode)SerializedActor["spl__LocatorBulletBlastCrossPaintChangeAreaParam"];
			}


			if (this.ForceAreaBottom)
			{
				spl__LocatorBulletBlastCrossPaintChangeAreaParam.AddNode("ForceAreaBottom", this.ForceAreaBottom);
			}

			spl__LocatorBulletBlastCrossPaintChangeAreaParam.AddNode("RotateDegree", this.RotateDegree);

			if (SerializedActor["spl__LocatorBulletBlastCrossPaintChangeAreaParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__LocatorBulletBlastCrossPaintChangeAreaParam);
			}
		}
	}
}
