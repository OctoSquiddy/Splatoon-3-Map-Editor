using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class InkBar : MuObj
	{
		[BindGUI("IsFreeY", Category = "InkBar Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsFreeY
		{
			get
			{
				return this.spl__ActorMatrixBindableHelperBancParam.IsFreeY;
			}

			set
			{
				this.spl__ActorMatrixBindableHelperBancParam.IsFreeY = value;
			}
		}

		[BindGUI("BaseModelDisplayType", Category = "InkBar Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _BaseModelDisplayType
		{
			get
			{
				return this.spl__InkBarBancParam.BaseModelDisplayType;
			}

			set
			{
				this.spl__InkBarBancParam.BaseModelDisplayType = value;
			}
		}

		[BindGUI("DelayFrame", Category = "InkBar Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _DelayFrame
		{
			get
			{
				return this.spl__InkBarBancParam.DelayFrame;
			}

			set
			{
				this.spl__InkBarBancParam.DelayFrame = value;
			}
		}

		[BindGUI("DurationFrame", Category = "InkBar Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _DurationFrame
		{
			get
			{
				return this.spl__InkBarBancParam.DurationFrame;
			}

			set
			{
				this.spl__InkBarBancParam.DurationFrame = value;
			}
		}

		[BindGUI("IntervalFrame", Category = "InkBar Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _IntervalFrame
		{
			get
			{
				return this.spl__InkBarBancParam.IntervalFrame;
			}

			set
			{
				this.spl__InkBarBancParam.IntervalFrame = value;
			}
		}

		[BindGUI("IsKeepDuration", Category = "InkBar Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsKeepDuration
		{
			get
			{
				return this.spl__InkBarBancParam.IsKeepDuration;
			}

			set
			{
				this.spl__InkBarBancParam.IsKeepDuration = value;
			}
		}

		[BindGUI("IsLightType", Category = "InkBar Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsLightType
		{
			get
			{
				return this.spl__InkBarBancParam.IsLightType;
			}

			set
			{
				this.spl__InkBarBancParam.IsLightType = value;
			}
		}

		[BindGUI("Length", Category = "InkBar Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Length
		{
			get
			{
				return this.spl__InkBarBancParam.Length;
			}

			set
			{
				this.spl__InkBarBancParam.Length = value;
			}
		}

		[BindGUI("MovingFrame", Category = "InkBar Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _MovingFrame
		{
			get
			{
				return this.spl__InkBarBancParam.MovingFrame;
			}

			set
			{
				this.spl__InkBarBancParam.MovingFrame = value;
			}
		}

		[ByamlMember("spl__ActorMatrixBindableHelperBancParam")]
		public Mu_spl__ActorMatrixBindableHelperBancParam spl__ActorMatrixBindableHelperBancParam { get; set; }

		[ByamlMember("spl__InkBarBancParam")]
		public Mu_spl__InkBarBancParam spl__InkBarBancParam { get; set; }

		public InkBar() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__InkBarBancParam = new Mu_spl__InkBarBancParam();

			Links = new List<Link>();
		}

		public InkBar(InkBar other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__InkBarBancParam = other.spl__InkBarBancParam.Clone();
		}

		public override InkBar Clone()
		{
			return new InkBar(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__InkBarBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
