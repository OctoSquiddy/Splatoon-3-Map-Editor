using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SplAutoWarpPointWithChangeScene : MuObj
	{
		[BindGUI("ChangeSceneOrStageName", Category = "SplAutoWarpPointWithChangeScene Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _ChangeSceneOrStageName
		{
			get
			{
				return this.spl__AutoWarpPointBancParam.ChangeSceneOrStageName;
			}

			set
			{
				this.spl__AutoWarpPointBancParam.ChangeSceneOrStageName = value;
			}
		}

		[BindGUI("IsDokanWarpCameraYQuick", Category = "SplAutoWarpPointWithChangeScene Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsDokanWarpCameraYQuick
		{
			get
			{
				return this.spl__AutoWarpPointBancParam.IsDokanWarpCameraYQuick;
			}

			set
			{
				this.spl__AutoWarpPointBancParam.IsDokanWarpCameraYQuick = value;
			}
		}

		[BindGUI("IsDokanWarpTypeLong", Category = "SplAutoWarpPointWithChangeScene Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsDokanWarpTypeLong
		{
			get
			{
				return this.spl__AutoWarpPointBancParam.IsDokanWarpTypeLong;
			}

			set
			{
				this.spl__AutoWarpPointBancParam.IsDokanWarpTypeLong = value;
			}
		}

		[ByamlMember("spl__AutoWarpPointBancParam")]
		public Mu_spl__AutoWarpPointBancParam spl__AutoWarpPointBancParam { get; set; }

		public SplAutoWarpPointWithChangeScene() : base()
		{
			spl__AutoWarpPointBancParam = new Mu_spl__AutoWarpPointBancParam();

			Links = new List<Link>();
		}

		public SplAutoWarpPointWithChangeScene(SplAutoWarpPointWithChangeScene other) : base(other)
		{
			spl__AutoWarpPointBancParam = other.spl__AutoWarpPointBancParam.Clone();
		}

		public override SplAutoWarpPointWithChangeScene Clone()
		{
			return new SplAutoWarpPointWithChangeScene(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__AutoWarpPointBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
