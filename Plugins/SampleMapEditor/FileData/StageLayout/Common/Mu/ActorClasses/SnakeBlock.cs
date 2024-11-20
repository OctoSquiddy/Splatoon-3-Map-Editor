using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class SnakeBlock : MuObj
	{
		[BindGUI("DamagePerExtension", Category = "SnakeBlock Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _DamagePerExtension
		{
			get
			{
				return this.spl__SnakeBlockBancParam.DamagePerExtension;
			}

			set
			{
				this.spl__SnakeBlockBancParam.DamagePerExtension = value;
			}
		}

		[BindGUI("ShrinkNoDamageFrame", Category = "SnakeBlock Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public int _ShrinkNoDamageFrame
		{
			get
			{
				return this.spl__SnakeBlockBancParam.ShrinkNoDamageFrame;
			}

			set
			{
				this.spl__SnakeBlockBancParam.ShrinkNoDamageFrame = value;
			}
		}

		[BindGUI("ToRailPoint", Category = "SnakeBlock Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public ulong _ToRailPoint
		{
			get
			{
				return this.spl__SnakeBlockBancParam.ToRailPoint;
			}

			set
			{
				this.spl__SnakeBlockBancParam.ToRailPoint = value;
			}
		}

		[ByamlMember("spl__SnakeBlockBancParam")]
		public Mu_spl__SnakeBlockBancParam spl__SnakeBlockBancParam { get; set; }

		public SnakeBlock() : base()
		{
			spl__SnakeBlockBancParam = new Mu_spl__SnakeBlockBancParam();

			Links = new List<Link>();
		}

		public SnakeBlock(SnakeBlock other) : base(other)
		{
			spl__SnakeBlockBancParam = other.spl__SnakeBlockBancParam.Clone();
		}

		public override SnakeBlock Clone()
		{
			return new SnakeBlock(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__SnakeBlockBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
