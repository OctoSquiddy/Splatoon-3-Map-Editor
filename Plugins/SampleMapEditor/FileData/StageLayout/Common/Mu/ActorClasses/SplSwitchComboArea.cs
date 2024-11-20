using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplSwitchComboArea : MuObj
	{
		[BindGUI("CheckActorName0", Category = "SplSwitchComboArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _CheckActorName0
		{
			get
			{
				return this.spl__SwitchComboAreaBancParam.CheckActorName0;
			}

			set
			{
				this.spl__SwitchComboAreaBancParam.CheckActorName0 = value;
			}
		}

		[BindGUI("CheckActorName1", Category = "SplSwitchComboArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _CheckActorName1
		{
			get
			{
				return this.spl__SwitchComboAreaBancParam.CheckActorName1;
			}

			set
			{
				this.spl__SwitchComboAreaBancParam.CheckActorName1 = value;
			}
		}

		[BindGUI("CheckActorName2", Category = "SplSwitchComboArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _CheckActorName2
		{
			get
			{
				return this.spl__SwitchComboAreaBancParam.CheckActorName2;
			}

			set
			{
				this.spl__SwitchComboAreaBancParam.CheckActorName2 = value;
			}
		}

		[BindGUI("ComboNum", Category = "SplSwitchComboArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _ComboNum
		{
			get
			{
				return this.spl__SwitchComboAreaBancParam.ComboNum;
			}

			set
			{
				this.spl__SwitchComboAreaBancParam.ComboNum = value;
			}
		}

		[BindGUI("ComboTime", Category = "SplSwitchComboArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _ComboTime
		{
			get
			{
				return this.spl__SwitchComboAreaBancParam.ComboTime;
			}

			set
			{
				this.spl__SwitchComboAreaBancParam.ComboTime = value;
			}
		}

		[ByamlMember("spl__SwitchComboAreaBancParam")]
		public Mu_spl__SwitchComboAreaBancParam spl__SwitchComboAreaBancParam { get; set; }

		public SplSwitchComboArea() : base()
		{
			spl__SwitchComboAreaBancParam = new Mu_spl__SwitchComboAreaBancParam();

			Links = new List<Link>();
		}

		public SplSwitchComboArea(SplSwitchComboArea other) : base(other)
		{
			spl__SwitchComboAreaBancParam = other.spl__SwitchComboAreaBancParam.Clone();
		}

		public override SplSwitchComboArea Clone()
		{
			return new SplSwitchComboArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SwitchComboAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
