using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorPlayerForceStainArea : MuObj
	{
		[BindGUI("StainRt_Body_Human", Category = "LocatorPlayerForceStainArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _StainRt_Body_Human
		{
			get
			{
				return this.spl__LocatorPlayerForceStainAreaParam.StainRt_Body_Human;
			}

			set
			{
				this.spl__LocatorPlayerForceStainAreaParam.StainRt_Body_Human = value;
			}
		}

		[BindGUI("StainRt_Body_Squid", Category = "LocatorPlayerForceStainArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _StainRt_Body_Squid
		{
			get
			{
				return this.spl__LocatorPlayerForceStainAreaParam.StainRt_Body_Squid;
			}

			set
			{
				this.spl__LocatorPlayerForceStainAreaParam.StainRt_Body_Squid = value;
			}
		}

		[BindGUI("StainRt_Bottom", Category = "LocatorPlayerForceStainArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _StainRt_Bottom
		{
			get
			{
				return this.spl__LocatorPlayerForceStainAreaParam.StainRt_Bottom;
			}

			set
			{
				this.spl__LocatorPlayerForceStainAreaParam.StainRt_Bottom = value;
			}
		}

		[BindGUI("StainRt_Clothes", Category = "LocatorPlayerForceStainArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _StainRt_Clothes
		{
			get
			{
				return this.spl__LocatorPlayerForceStainAreaParam.StainRt_Clothes;
			}

			set
			{
				this.spl__LocatorPlayerForceStainAreaParam.StainRt_Clothes = value;
			}
		}

		[BindGUI("StainRt_Head", Category = "LocatorPlayerForceStainArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _StainRt_Head
		{
			get
			{
				return this.spl__LocatorPlayerForceStainAreaParam.StainRt_Head;
			}

			set
			{
				this.spl__LocatorPlayerForceStainAreaParam.StainRt_Head = value;
			}
		}

		[BindGUI("StainRt_Shoes", Category = "LocatorPlayerForceStainArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _StainRt_Shoes
		{
			get
			{
				return this.spl__LocatorPlayerForceStainAreaParam.StainRt_Shoes;
			}

			set
			{
				this.spl__LocatorPlayerForceStainAreaParam.StainRt_Shoes = value;
			}
		}

		[ByamlMember("spl__LocatorPlayerForceStainAreaParam")]
		public Mu_spl__LocatorPlayerForceStainAreaParam spl__LocatorPlayerForceStainAreaParam { get; set; }

		public LocatorPlayerForceStainArea() : base()
		{
			spl__LocatorPlayerForceStainAreaParam = new Mu_spl__LocatorPlayerForceStainAreaParam();

			Links = new List<Link>();
		}

		public LocatorPlayerForceStainArea(LocatorPlayerForceStainArea other) : base(other)
		{
			spl__LocatorPlayerForceStainAreaParam = other.spl__LocatorPlayerForceStainAreaParam.Clone();
		}

		public override LocatorPlayerForceStainArea Clone()
		{
			return new LocatorPlayerForceStainArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorPlayerForceStainAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
