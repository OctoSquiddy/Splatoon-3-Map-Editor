using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class BombFlowerParent : MuObj
	{
		[BindGUI("IsFreeY", Category = "BombFlowerParent Properties", ColumnIndex = 0, Control = BindControl.Default)]
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

		[BindGUI("IsMiddlePaint", Category = "BombFlowerParent Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsMiddlePaint
		{
			get
			{
				return this.spl__BombFlowerBancParam.IsMiddlePaint;
			}

			set
			{
				this.spl__BombFlowerBancParam.IsMiddlePaint = value;
			}
		}

		[BindGUI("IsPaintNormalBaseY", Category = "BombFlowerParent Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public bool _IsPaintNormalBaseY
		{
			get
			{
				return this.spl__BombFlowerBancParam.IsPaintNormalBaseY;
			}

			set
			{
				this.spl__BombFlowerBancParam.IsPaintNormalBaseY = value;
			}
		}

		[ByamlMember("spl__ActorMatrixBindableHelperBancParam")]
		public Mu_spl__ActorMatrixBindableHelperBancParam spl__ActorMatrixBindableHelperBancParam { get; set; }

		[ByamlMember("spl__BombFlowerBancParam")]
		public Mu_spl__BombFlowerBancParam spl__BombFlowerBancParam { get; set; }

		public BombFlowerParent() : base()
		{
			spl__ActorMatrixBindableHelperBancParam = new Mu_spl__ActorMatrixBindableHelperBancParam();
			spl__BombFlowerBancParam = new Mu_spl__BombFlowerBancParam();

			Links = new List<Link>();
		}

		public BombFlowerParent(BombFlowerParent other) : base(other)
		{
			spl__ActorMatrixBindableHelperBancParam = other.spl__ActorMatrixBindableHelperBancParam.Clone();
			spl__BombFlowerBancParam = other.spl__BombFlowerBancParam.Clone();
		}

		public override BombFlowerParent Clone()
		{
			return new BombFlowerParent(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__ActorMatrixBindableHelperBancParam.SaveParameterBank(SerializedActor);
			this.spl__BombFlowerBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
