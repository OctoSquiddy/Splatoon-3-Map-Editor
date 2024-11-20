using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplRandomDeactivator : MuObj
	{
		[BindGUI("ActorTag", Category = "SplRandomDeactivator Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ActorTag
		{
			get
			{
				return this.spl__RandomDeactivatorBancParam.ActorTag;
			}

			set
			{
				this.spl__RandomDeactivatorBancParam.ActorTag = value;
			}
		}

		[BindGUI("MaxActiveNum", Category = "SplRandomDeactivator Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _MaxActiveNum
		{
			get
			{
				return this.spl__RandomDeactivatorBancParam.MaxActiveNum;
			}

			set
			{
				this.spl__RandomDeactivatorBancParam.MaxActiveNum = value;
			}
		}

		[ByamlMember("spl__RandomDeactivatorBancParam")]
		public Mu_spl__RandomDeactivatorBancParam spl__RandomDeactivatorBancParam { get; set; }

		public SplRandomDeactivator() : base()
		{
			spl__RandomDeactivatorBancParam = new Mu_spl__RandomDeactivatorBancParam();

			Links = new List<Link>();
		}

		public SplRandomDeactivator(SplRandomDeactivator other) : base(other)
		{
			spl__RandomDeactivatorBancParam = other.spl__RandomDeactivatorBancParam.Clone();
		}

		public override SplRandomDeactivator Clone()
		{
			return new SplRandomDeactivator(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__RandomDeactivatorBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
