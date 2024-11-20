using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class MissionCheckPoint : MuObj
	{
		[BindGUI("AdditionalTime", Category = "MissionCheckPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _AdditionalTime
		{
			get
			{
				return this.spl__MissionCheckPointBancParam.AdditionalTime;
			}

			set
			{
				this.spl__MissionCheckPointBancParam.AdditionalTime = value;
			}
		}

		[BindGUI("IsLast", Category = "MissionCheckPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsLast
		{
			get
			{
				return this.spl__MissionCheckPointBancParam.IsLast;
			}

			set
			{
				this.spl__MissionCheckPointBancParam.IsLast = value;
			}
		}

		[BindGUI("Progress", Category = "MissionCheckPoint Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _Progress
		{
			get
			{
				return this.spl__MissionCheckPointBancParam.Progress;
			}

			set
			{
				this.spl__MissionCheckPointBancParam.Progress = value;
			}
		}

		[ByamlMember("spl__MissionCheckPointBancParam")]
		public Mu_spl__MissionCheckPointBancParam spl__MissionCheckPointBancParam { get; set; }

		public MissionCheckPoint() : base()
		{
			spl__MissionCheckPointBancParam = new Mu_spl__MissionCheckPointBancParam();

			Links = new List<Link>();
		}

		public MissionCheckPoint(MissionCheckPoint other) : base(other)
		{
			spl__MissionCheckPointBancParam = other.spl__MissionCheckPointBancParam.Clone();
		}

		public override MissionCheckPoint Clone()
		{
			return new MissionCheckPoint(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__MissionCheckPointBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
