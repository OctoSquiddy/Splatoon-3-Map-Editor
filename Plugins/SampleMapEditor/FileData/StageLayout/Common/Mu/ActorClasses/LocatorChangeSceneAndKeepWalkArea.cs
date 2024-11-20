using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorChangeSceneAndKeepWalkArea : MuObj
	{
		[BindGUI("ChangeSceneOrStageName", Category = "LocatorChangeSceneAndKeepWalkArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ChangeSceneOrStageName
		{
			get
			{
				return this.spl__LocatorChangeSceneAndKeepWalkAreaBancParam.ChangeSceneOrStageName;
			}

			set
			{
				this.spl__LocatorChangeSceneAndKeepWalkAreaBancParam.ChangeSceneOrStageName = value;
			}
		}

		[BindGUI("InFaderType", Category = "LocatorChangeSceneAndKeepWalkArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _InFaderType
		{
			get
			{
				return this.spl__LocatorChangeSceneAndKeepWalkAreaBancParam.InFaderType;
			}

			set
			{
				this.spl__LocatorChangeSceneAndKeepWalkAreaBancParam.InFaderType = value;
			}
		}

		[BindGUI("IsInformClear", Category = "LocatorChangeSceneAndKeepWalkArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsInformClear
		{
			get
			{
				return this.spl__LocatorChangeSceneAndKeepWalkAreaBancParam.IsInformClear;
			}

			set
			{
				this.spl__LocatorChangeSceneAndKeepWalkAreaBancParam.IsInformClear = value;
			}
		}

		[BindGUI("IsSquid", Category = "LocatorChangeSceneAndKeepWalkArea Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsSquid
		{
			get
			{
				return this.spl__LocatorChangeSceneAndKeepWalkAreaBancParam.IsSquid;
			}

			set
			{
				this.spl__LocatorChangeSceneAndKeepWalkAreaBancParam.IsSquid = value;
			}
		}

		[ByamlMember("spl__LocatorChangeSceneAndKeepWalkAreaBancParam")]
		public Mu_spl__LocatorChangeSceneAndKeepWalkAreaBancParam spl__LocatorChangeSceneAndKeepWalkAreaBancParam { get; set; }

		public LocatorChangeSceneAndKeepWalkArea() : base()
		{
			spl__LocatorChangeSceneAndKeepWalkAreaBancParam = new Mu_spl__LocatorChangeSceneAndKeepWalkAreaBancParam();

			Links = new List<Link>();
		}

		public LocatorChangeSceneAndKeepWalkArea(LocatorChangeSceneAndKeepWalkArea other) : base(other)
		{
			spl__LocatorChangeSceneAndKeepWalkAreaBancParam = other.spl__LocatorChangeSceneAndKeepWalkAreaBancParam.Clone();
		}

		public override LocatorChangeSceneAndKeepWalkArea Clone()
		{
			return new LocatorChangeSceneAndKeepWalkArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorChangeSceneAndKeepWalkAreaBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
