using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	[ByamlObject]
	public class Mu_game__SequentialRotateParam
	{
		[ByamlMember]
		public ByamlVector3F FreeRotateVelDegPerSec { get; set; }

		[ByamlMember]
		public float OneTimeBreakTime { get; set; }

		[ByamlMember]
		public float OneTimeReverseBreakTime { get; set; }

		[ByamlMember]
		public ByamlVector3F OneTimeRotateAngleDeg { get; set; }

		[ByamlMember]
		public float OneTimeRotateTime { get; set; }

		[ByamlMember]
		public string OneTimeRotateType { get; set; }

		[ByamlMember]
		public float WaitTime { get; set; }

		public Mu_game__SequentialRotateParam()
		{
			FreeRotateVelDegPerSec = new ByamlVector3F();
			OneTimeBreakTime = 0.0f;
			OneTimeReverseBreakTime = 0.0f;
			OneTimeRotateAngleDeg = new ByamlVector3F();
			OneTimeRotateTime = 0.0f;
			OneTimeRotateType = "";
			WaitTime = 0.0f;
		}

		public Mu_game__SequentialRotateParam(Mu_game__SequentialRotateParam other)
		{
			FreeRotateVelDegPerSec = new ByamlVector3F(other.FreeRotateVelDegPerSec.X, other.FreeRotateVelDegPerSec.Y, other.FreeRotateVelDegPerSec.Z);
			OneTimeBreakTime = other.OneTimeBreakTime;
			OneTimeReverseBreakTime = other.OneTimeReverseBreakTime;
			OneTimeRotateAngleDeg = new ByamlVector3F(other.OneTimeRotateAngleDeg.X, other.OneTimeRotateAngleDeg.Y, other.OneTimeRotateAngleDeg.Z);
			OneTimeRotateTime = other.OneTimeRotateTime;
			OneTimeRotateType = other.OneTimeRotateType;
			WaitTime = other.WaitTime;
		}

		public Mu_game__SequentialRotateParam Clone()
		{
			return new Mu_game__SequentialRotateParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode game__SequentialRotateParam = new BymlNode.DictionaryNode() { Name = "game__SequentialRotateParam" };

			if (SerializedActor["game__SequentialRotateParam"] != null)
			{
				game__SequentialRotateParam = (BymlNode.DictionaryNode)SerializedActor["game__SequentialRotateParam"];
			}

			BymlNode.DictionaryNode FreeRotateVelDegPerSec = new BymlNode.DictionaryNode() { Name = "FreeRotateVelDegPerSec" };
			BymlNode.DictionaryNode OneTimeRotateAngleDeg = new BymlNode.DictionaryNode() { Name = "OneTimeRotateAngleDeg" };

			if (this.FreeRotateVelDegPerSec.Z != 0.0f || this.FreeRotateVelDegPerSec.Y != 0.0f || this.FreeRotateVelDegPerSec.X != 0.0f)
			{
				FreeRotateVelDegPerSec.AddNode("X", this.FreeRotateVelDegPerSec.X);
				FreeRotateVelDegPerSec.AddNode("Y", this.FreeRotateVelDegPerSec.Y);
				FreeRotateVelDegPerSec.AddNode("Z", this.FreeRotateVelDegPerSec.Z);
				game__SequentialRotateParam.Nodes.Add(FreeRotateVelDegPerSec);
			}

			if (this.OneTimeBreakTime > 0.0f)
				game__SequentialRotateParam.AddNode("OneTimeBreakTime", this.OneTimeBreakTime);

			if (this.OneTimeReverseBreakTime > 0.0f)
				game__SequentialRotateParam.AddNode("OneTimeReverseBreakTime", this.OneTimeReverseBreakTime);

			if (this.OneTimeRotateAngleDeg.Z != 0.0f || this.OneTimeRotateAngleDeg.Y != 0.0f || this.OneTimeRotateAngleDeg.X != 0.0f)
			{
				OneTimeRotateAngleDeg.AddNode("X", this.OneTimeRotateAngleDeg.X);
				OneTimeRotateAngleDeg.AddNode("Y", this.OneTimeRotateAngleDeg.Y);
				OneTimeRotateAngleDeg.AddNode("Z", this.OneTimeRotateAngleDeg.Z);
				game__SequentialRotateParam.Nodes.Add(OneTimeRotateAngleDeg);
			}

            if (this.OneTimeRotateTime > 0.0f)
                game__SequentialRotateParam.AddNode("OneTimeRotateTime", this.OneTimeRotateTime);

			if (this.OneTimeRotateType != "")
			{
				game__SequentialRotateParam.AddNode("OneTimeRotateType", this.OneTimeRotateType);
			}

            if (this.WaitTime > 0.0f)
                game__SequentialRotateParam.AddNode("WaitTime", this.WaitTime);

			if (SerializedActor["game__SequentialRotateParam"] == null)
			{
				SerializedActor.Nodes.Add(game__SequentialRotateParam);
			}
		}
	}
}
