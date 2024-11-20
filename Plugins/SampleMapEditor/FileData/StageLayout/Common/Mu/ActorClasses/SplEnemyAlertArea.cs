using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplEnemyAlertArea : MuObj
	{
		[BindGUI("IsForceFindDirect", Category = "SplEnemyAlertArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsForceFindDirect
		{
			get
			{
				return this.spl__EnemyAlertAreaBancParam.IsForceFindDirect;
			}

			set
			{
				this.spl__EnemyAlertAreaBancParam.IsForceFindDirect = value;
			}
		}

		[BindGUI("IsForceFindHide", Category = "SplEnemyAlertArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsForceFindHide
		{
			get
			{
				return this.spl__EnemyAlertAreaBancParam.IsForceFindHide;
			}

			set
			{
				this.spl__EnemyAlertAreaBancParam.IsForceFindHide = value;
			}
		}

		[BindGUI("IsOneTime", Category = "SplEnemyAlertArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsOneTime
		{
			get
			{
				return this.spl__EnemyAlertAreaBancParam.IsOneTime;
			}

			set
			{
				this.spl__EnemyAlertAreaBancParam.IsOneTime = value;
			}
		}

		[ByamlMember("spl__EnemyAlertAreaBancParam")]
		public Mu_spl__EnemyAlertAreaBancParam spl__EnemyAlertAreaBancParam { get; set; }

		public SplEnemyAlertArea() : base()
		{
			spl__EnemyAlertAreaBancParam = new Mu_spl__EnemyAlertAreaBancParam();

			Links = new List<Link>();
		}

		public SplEnemyAlertArea(SplEnemyAlertArea other) : base(other)
		{
			spl__EnemyAlertAreaBancParam = other.spl__EnemyAlertAreaBancParam.Clone();
		}

		public override SplEnemyAlertArea Clone()
		{
			return new SplEnemyAlertArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__EnemyAlertAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
