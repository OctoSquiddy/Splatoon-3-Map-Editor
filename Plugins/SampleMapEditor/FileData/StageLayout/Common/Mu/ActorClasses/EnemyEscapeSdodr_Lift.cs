using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class EnemyEscapeSdodr_Lift : MuObj
	{
		[BindGUI("IsBombEnabledOnEnterEscape", Category = "EnemyEscapeSdodr_Lift Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsBombEnabledOnEnterEscape
		{
			get
			{
				return this.spl__EnemyEscapeBancParam.IsBombEnabledOnEnterEscape;
			}

			set
			{
				this.spl__EnemyEscapeBancParam.IsBombEnabledOnEnterEscape = value;
			}
		}

		[BindGUI("IsBombEnabledOnReachNode", Category = "EnemyEscapeSdodr_Lift Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsBombEnabledOnReachNode
		{
			get
			{
				return this.spl__EnemyEscapeBancParam.IsBombEnabledOnReachNode;
			}

			set
			{
				this.spl__EnemyEscapeBancParam.IsBombEnabledOnReachNode = value;
			}
		}

		[BindGUI("MaxOffsetToNode", Category = "EnemyEscapeSdodr_Lift Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _MaxOffsetToNode
		{
			get
			{
				return this.spl__EnemyEscapeBancParam.MaxOffsetToNode;
			}

			set
			{
				this.spl__EnemyEscapeBancParam.MaxOffsetToNode = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "EnemyEscapeSdodr_Lift Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__GraphRailHelperBancParam.ToRailPoint;
			}

			set
			{
				this.spl__GraphRailHelperBancParam.ToRailPoint = value;
			}
		}

		[BindGUI("MidiLinkNoteNumber", Category = "EnemyEscapeSdodr_Lift Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _MidiLinkNoteNumber
		{
			get
			{
				return this.spl__MusicLinkBancParam.MidiLinkNoteNumber;
			}

			set
			{
				this.spl__MusicLinkBancParam.MidiLinkNoteNumber = value;
			}
		}

		[BindGUI("MidiLinkTrackName", Category = "EnemyEscapeSdodr_Lift Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _MidiLinkTrackName
		{
			get
			{
				return this.spl__MusicLinkBancParam.MidiLinkTrackName;
			}

			set
			{
				this.spl__MusicLinkBancParam.MidiLinkTrackName = value;
			}
		}

		[ByamlMember("spl__AttentionTargetingBancParam")]
		public Mu_spl__AttentionTargetingBancParam spl__AttentionTargetingBancParam { get; set; }

		[ByamlMember("spl__EnemyEscapeBancParam")]
		public Mu_spl__EnemyEscapeBancParam spl__EnemyEscapeBancParam { get; set; }

		[ByamlMember("spl__GraphRailHelperBancParam")]
		public Mu_spl__GraphRailHelperBancParam spl__GraphRailHelperBancParam { get; set; }

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		public EnemyEscapeSdodr_Lift() : base()
		{
			spl__AttentionTargetingBancParam = new Mu_spl__AttentionTargetingBancParam();
			spl__EnemyEscapeBancParam = new Mu_spl__EnemyEscapeBancParam();
			spl__GraphRailHelperBancParam = new Mu_spl__GraphRailHelperBancParam();
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();

			Links = new List<Link>();
		}

		public EnemyEscapeSdodr_Lift(EnemyEscapeSdodr_Lift other) : base(other)
		{
			spl__AttentionTargetingBancParam = other.spl__AttentionTargetingBancParam.Clone();
			spl__EnemyEscapeBancParam = other.spl__EnemyEscapeBancParam.Clone();
			spl__GraphRailHelperBancParam = other.spl__GraphRailHelperBancParam.Clone();
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
		}

		public override EnemyEscapeSdodr_Lift Clone()
		{
			return new EnemyEscapeSdodr_Lift(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__AttentionTargetingBancParam.SaveParameterBank(SerializedActor);
			this.spl__EnemyEscapeBancParam.SaveParameterBank(SerializedActor);
			this.spl__GraphRailHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
