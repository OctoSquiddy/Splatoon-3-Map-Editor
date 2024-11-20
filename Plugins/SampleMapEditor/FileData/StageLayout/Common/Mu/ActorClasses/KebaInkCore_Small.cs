using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class KebaInkCore_Small : MuObj
	{
		[BindGUI("IsFreeY", Category = "KebaInkCore_Small Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsFreeY
		{
			get
			{
				return this.spl__ActorMatrixBindableHelperBancParam.IsFreeY;
			}

			set
			{
				this.spl__ActorMatrixBindableHelperBancParam.IsFreeY = value;
			}
		}

		[BindGUI("IsDisableDynamicLoading", Category = "KebaInkCore_Small Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsDisableDynamicLoading
		{
			get
			{
				return this.spl__KebaInkCoreBancParam.IsDisableDynamicLoading;
			}

			set
			{
				this.spl__KebaInkCoreBancParam.IsDisableDynamicLoading = value;
			}
		}

		[BindGUI("IsForIkuraShootTutorial", Category = "KebaInkCore_Small Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsForIkuraShootTutorial
		{
			get
			{
				return this.spl__KebaInkCoreBancParam.IsForIkuraShootTutorial;
			}

			set
			{
				this.spl__KebaInkCoreBancParam.IsForIkuraShootTutorial = value;
			}
		}

		[BindGUI("IsNoCore", Category = "KebaInkCore_Small Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsNoCore
		{
			get
			{
				return this.spl__KebaInkCoreBancParam.IsNoCore;
			}

			set
			{
				this.spl__KebaInkCoreBancParam.IsNoCore = value;
			}
		}

		[BindGUI("IsSmallWorldLast", Category = "KebaInkCore_Small Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsSmallWorldLast
		{
			get
			{
				return this.spl__KebaInkCoreBancParam.IsSmallWorldLast;
			}

			set
			{
				this.spl__KebaInkCoreBancParam.IsSmallWorldLast = value;
			}
		}

		[BindGUI("NecessarySalmonRoe", Category = "KebaInkCore_Small Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _NecessarySalmonRoe
		{
			get
			{
				return this.spl__KebaInkCoreBancParam.NecessarySalmonRoe;
			}

			set
			{
				this.spl__KebaInkCoreBancParam.NecessarySalmonRoe = value;
			}
		}

		[ByamlMember("spl__ActorMatrixBindableHelperBancParam")]
		public Mu_spl__ActorMatrixBindableHelperBancParam spl__ActorMatrixBindableHelperBancParam { get; set; }

		[ByamlMember("spl__KebaInkCoreBancParam")]
		public Mu_spl__KebaInkCoreBancParam spl__KebaInkCoreBancParam { get; set; }

		public KebaInkCore_Small() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__KebaInkCoreBancParam = new Mu_spl__KebaInkCoreBancParam();

			Links = new List<Link>();
		}

		public KebaInkCore_Small(KebaInkCore_Small other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__KebaInkCoreBancParam = other.spl__KebaInkCoreBancParam.Clone();
		}

		public override KebaInkCore_Small Clone()
		{
			return new KebaInkCore_Small(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__KebaInkCoreBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
