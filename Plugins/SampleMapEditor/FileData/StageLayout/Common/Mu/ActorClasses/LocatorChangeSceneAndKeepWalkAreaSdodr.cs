using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class LocatorChangeSceneAndKeepWalkAreaSdodr : MuObj
	{
		[BindGUI("ChangeSceneOrStageName", Category = "LocatorChangeSceneAndKeepWalkAreaSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ChangeSceneOrStageName
		{
			get
			{
				return this.spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam.ChangeSceneOrStageName;
			}

			set
			{
				this.spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam.ChangeSceneOrStageName = value;
			}
		}

		[BindGUI("InFaderType", Category = "LocatorChangeSceneAndKeepWalkAreaSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _InFaderType
		{
			get
			{
				return this.spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam.InFaderType;
			}

			set
			{
				this.spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam.InFaderType = value;
			}
		}

		[BindGUI("OutFaderType", Category = "LocatorChangeSceneAndKeepWalkAreaSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _OutFaderType
		{
			get
			{
				return this.spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam.OutFaderType;
			}

			set
			{
				this.spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam.OutFaderType = value;
			}
		}

		[ByamlMember("spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam")]
		public Mu_spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam { get; set; }

		public LocatorChangeSceneAndKeepWalkAreaSdodr() : base()
		{
			spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam = new Mu_spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam();

			Links = new List<Link>();
		}

		public LocatorChangeSceneAndKeepWalkAreaSdodr(LocatorChangeSceneAndKeepWalkAreaSdodr other) : base(other)
		{
			spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam = other.spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam.Clone();
		}

		public override LocatorChangeSceneAndKeepWalkAreaSdodr Clone()
		{
			return new LocatorChangeSceneAndKeepWalkAreaSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__LocatorChangeSceneAndKeepWalkAreaSdodrBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
