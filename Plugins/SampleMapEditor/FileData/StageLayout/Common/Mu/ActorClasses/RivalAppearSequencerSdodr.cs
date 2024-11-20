using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class RivalAppearSequencerSdodr : MuObj
	{
		[BindGUI("TotalSpawnNumWave0", Category = "RivalAppearSequencerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _TotalSpawnNumWave0
		{
			get
			{
				return this.spl__RivalAppearSequencerBancParamSdodr.TotalSpawnNumWave0;
			}

			set
			{
				this.spl__RivalAppearSequencerBancParamSdodr.TotalSpawnNumWave0 = value;
			}
		}

		[BindGUI("TotalSpawnNumWave1", Category = "RivalAppearSequencerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _TotalSpawnNumWave1
		{
			get
			{
				return this.spl__RivalAppearSequencerBancParamSdodr.TotalSpawnNumWave1;
			}

			set
			{
				this.spl__RivalAppearSequencerBancParamSdodr.TotalSpawnNumWave1 = value;
			}
		}

		[BindGUI("TotalSpawnNumWave2", Category = "RivalAppearSequencerSdodr Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _TotalSpawnNumWave2
		{
			get
			{
				return this.spl__RivalAppearSequencerBancParamSdodr.TotalSpawnNumWave2;
			}

			set
			{
				this.spl__RivalAppearSequencerBancParamSdodr.TotalSpawnNumWave2 = value;
			}
		}

		[ByamlMember("spl__RivalAppearSequencerBancParamSdodr")]
		public Mu_spl__RivalAppearSequencerBancParamSdodr spl__RivalAppearSequencerBancParamSdodr { get; set; }

		public RivalAppearSequencerSdodr() : base()
		{
			spl__RivalAppearSequencerBancParamSdodr = new Mu_spl__RivalAppearSequencerBancParamSdodr();

			Links = new List<Link>();
		}

		public RivalAppearSequencerSdodr(RivalAppearSequencerSdodr other) : base(other)
		{
			spl__RivalAppearSequencerBancParamSdodr = other.spl__RivalAppearSequencerBancParamSdodr.Clone();
		}

		public override RivalAppearSequencerSdodr Clone()
		{
			return new RivalAppearSequencerSdodr(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__RivalAppearSequencerBancParamSdodr.SaveParameterBank(SerializedActor);
		}
	}
}
