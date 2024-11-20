using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopParamHolderEventGeyser : MuObj
	{
		[BindGUI("ThDistanceWaterHigh", Category = "CoopParamHolderEventGeyser Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _ThDistanceWaterHigh
		{
			get
			{
				return this.spl__CoopParamHolderEventGeyserParam.ThDistanceWaterHigh;
			}

			set
			{
				this.spl__CoopParamHolderEventGeyserParam.ThDistanceWaterHigh = value;
			}
		}

		[BindGUI("ThDistanceWaterMid", Category = "CoopParamHolderEventGeyser Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _ThDistanceWaterMid
		{
			get
			{
				return this.spl__CoopParamHolderEventGeyserParam.ThDistanceWaterMid;
			}

			set
			{
				this.spl__CoopParamHolderEventGeyserParam.ThDistanceWaterMid = value;
			}
		}

		[ByamlMember("spl__CoopParamHolderEventGeyserParam")]
		public Mu_spl__CoopParamHolderEventGeyserParam spl__CoopParamHolderEventGeyserParam { get; set; }

		public CoopParamHolderEventGeyser() : base()
		{
			spl__CoopParamHolderEventGeyserParam = new Mu_spl__CoopParamHolderEventGeyserParam();

			Links = new List<Link>();
		}

		public CoopParamHolderEventGeyser(CoopParamHolderEventGeyser other) : base(other)
		{
			spl__CoopParamHolderEventGeyserParam = other.spl__CoopParamHolderEventGeyserParam.Clone();
		}

		public override CoopParamHolderEventGeyser Clone()
		{
			return new CoopParamHolderEventGeyser(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopParamHolderEventGeyserParam.SaveParameterBank(SerializedActor);
		}
	}
}
