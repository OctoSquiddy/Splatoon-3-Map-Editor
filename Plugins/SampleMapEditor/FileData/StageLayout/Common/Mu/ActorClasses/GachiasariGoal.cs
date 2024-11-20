using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class GachiasariGoal : MuObj
	{
		[BindGUI("DegAxisY_SpawnFromGoal", Category = "GachiasariGoal Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _DegAxisY_SpawnFromGoal
		{
			get
			{
				return this.spl__GachiasariGoalBancParam.DegAxisY_SpawnFromGoal;
			}

			set
			{
				this.spl__GachiasariGoalBancParam.DegAxisY_SpawnFromGoal = value;
			}
		}

		[BindGUI("Height", Category = "GachiasariGoal Properties", ColumnIndex = 0, Control = BindControl.Default)]
		public float _Height
		{
			get
			{
				return this.spl__GachiasariGoalBancParam.Height;
			}

			set
			{
				this.spl__GachiasariGoalBancParam.Height = value;
			}
		}

		[ByamlMember("spl__GachiasariGoalBancParam")]
		public Mu_spl__GachiasariGoalBancParam spl__GachiasariGoalBancParam { get; set; }

		public GachiasariGoal() : base()
		{
			spl__GachiasariGoalBancParam = new Mu_spl__GachiasariGoalBancParam();

			Links = new List<Link>();
		}

		public GachiasariGoal(GachiasariGoal other) : base(other)
		{
			spl__GachiasariGoalBancParam = other.spl__GachiasariGoalBancParam.Clone();
		}

		public override GachiasariGoal Clone()
		{
			return new GachiasariGoal(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__GachiasariGoalBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
