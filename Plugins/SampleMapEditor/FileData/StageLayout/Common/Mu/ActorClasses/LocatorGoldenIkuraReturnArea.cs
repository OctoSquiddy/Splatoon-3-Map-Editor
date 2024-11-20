using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorGoldenIkuraReturnArea : MuObj
	{
		[BindGUI("EdgeA", Category = "LocatorGoldenIkuraReturnArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public OpenTK.Vector3 _EdgeA
		{
			get
			{
				return new OpenTK.Vector3(
					this.spl__LocatorGoldenIkuraReturnOutSegment.EdgeA.X,
					this.spl__LocatorGoldenIkuraReturnOutSegment.EdgeA.Y,
					this.spl__LocatorGoldenIkuraReturnOutSegment.EdgeA.Z);
			}

			set
			{
				this.spl__LocatorGoldenIkuraReturnOutSegment.EdgeA = new ByamlVector3F(value.X, value.Y, value.Z);
			}
		}

		[BindGUI("EdgeB", Category = "LocatorGoldenIkuraReturnArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public OpenTK.Vector3 _EdgeB
		{
			get
			{
				return new OpenTK.Vector3(
					this.spl__LocatorGoldenIkuraReturnOutSegment.EdgeB.X,
					this.spl__LocatorGoldenIkuraReturnOutSegment.EdgeB.Y,
					this.spl__LocatorGoldenIkuraReturnOutSegment.EdgeB.Z);
			}

			set
			{
				this.spl__LocatorGoldenIkuraReturnOutSegment.EdgeB = new ByamlVector3F(value.X, value.Y, value.Z);
			}
		}

		[ByamlMember("spl__LocatorGoldenIkuraReturnOutSegment")]
		public Mu_spl__LocatorGoldenIkuraReturnOutSegment spl__LocatorGoldenIkuraReturnOutSegment { get; set; }

		public LocatorGoldenIkuraReturnArea() : base()
		{
			spl__LocatorGoldenIkuraReturnOutSegment = new Mu_spl__LocatorGoldenIkuraReturnOutSegment();

			Links = new List<Link>();
		}

		public LocatorGoldenIkuraReturnArea(LocatorGoldenIkuraReturnArea other) : base(other)
		{
			spl__LocatorGoldenIkuraReturnOutSegment = other.spl__LocatorGoldenIkuraReturnOutSegment.Clone();
		}

		public override LocatorGoldenIkuraReturnArea Clone()
		{
			return new LocatorGoldenIkuraReturnArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorGoldenIkuraReturnOutSegment.SaveParameterBank(SerializedActor);
		}
	}
}
