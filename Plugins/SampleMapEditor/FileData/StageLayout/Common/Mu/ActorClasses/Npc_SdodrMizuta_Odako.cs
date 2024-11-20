using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class Npc_SdodrMizuta_Odako : MuObj
	{
		[BindGUI("IsActivateOnlyInBeingPerformer", Category = "Npc_SdodrMizuta_Odako Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsActivateOnlyInBeingPerformer
		{
			get
			{
				return this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer;
			}

			set
			{
				this.game__EventPerformerBancParam.IsActivateOnlyInBeingPerformer = value;
			}
		}

		[BindGUI("MidiLinkNoteNumber", Category = "Npc_SdodrMizuta_Odako Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("MidiLinkTrackName", Category = "Npc_SdodrMizuta_Odako Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsInitEventVisible", Category = "Npc_SdodrMizuta_Odako Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsInitEventVisible
		{
			get
			{
				return this.spl__NpcSdodrBancParam.IsInitEventVisible;
			}

			set
			{
				this.spl__NpcSdodrBancParam.IsInitEventVisible = value;
			}
		}

		[BindGUI("IsKinematicWait", Category = "Npc_SdodrMizuta_Odako Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsKinematicWait
		{
			get
			{
				return this.spl__NpcSdodrBancParam.IsKinematicWait;
			}

			set
			{
				this.spl__NpcSdodrBancParam.IsKinematicWait = value;
			}
		}

		[BindGUI("IsTakeOverVisible", Category = "Npc_SdodrMizuta_Odako Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsTakeOverVisible
		{
			get
			{
				return this.spl__NpcSdodrBancParam.IsTakeOverVisible;
			}

			set
			{
				this.spl__NpcSdodrBancParam.IsTakeOverVisible = value;
			}
		}

		[BindGUI("WaitASCommand", Category = "Npc_SdodrMizuta_Odako Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public string _WaitASCommand
		{
			get
			{
				return this.spl__NpcSdodrBancParam.WaitASCommand;
			}

			set
			{
				this.spl__NpcSdodrBancParam.WaitASCommand = value;
			}
		}

		[ByamlMember("game__EventPerformerBancParam")]
		public Mu_game__EventPerformerBancParam game__EventPerformerBancParam { get; set; }

		[ByamlMember("spl__MusicLinkBancParam")]
		public Mu_spl__MusicLinkBancParam spl__MusicLinkBancParam { get; set; }

		[ByamlMember("spl__NpcSdodrBancParam")]
		public Mu_spl__NpcSdodrBancParam spl__NpcSdodrBancParam { get; set; }

		public Npc_SdodrMizuta_Odako() : base()
		{
			game__EventPerformerBancParam = new Mu_game__EventPerformerBancParam();
			spl__MusicLinkBancParam = new Mu_spl__MusicLinkBancParam();
			spl__NpcSdodrBancParam = new Mu_spl__NpcSdodrBancParam();

			Links = new List<Link>();
		}

		public Npc_SdodrMizuta_Odako(Npc_SdodrMizuta_Odako other) : base(other)
		{
			game__EventPerformerBancParam = other.game__EventPerformerBancParam.Clone();
			spl__MusicLinkBancParam = other.spl__MusicLinkBancParam.Clone();
			spl__NpcSdodrBancParam = other.spl__NpcSdodrBancParam.Clone();
		}

		public override Npc_SdodrMizuta_Odako Clone()
		{
			return new Npc_SdodrMizuta_Odako(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.game__EventPerformerBancParam.SaveParameterBank(SerializedActor);
			this.spl__MusicLinkBancParam.SaveParameterBank(SerializedActor);
			this.spl__NpcSdodrBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
