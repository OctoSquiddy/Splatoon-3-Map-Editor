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
	public class Mu_spl__InkBarBancParam
	{
		[ByamlMember]
		public string BaseModelDisplayType { get; set; }

		[ByamlMember]
		public int DelayFrame { get; set; }

		[ByamlMember]
		public int DurationFrame { get; set; }

		[ByamlMember]
		public int IntervalFrame { get; set; }

		[ByamlMember]
		public bool IsKeepDuration { get; set; }

		[ByamlMember]
		public bool IsLightType { get; set; }

		[ByamlMember]
		public float Length { get; set; }

		[ByamlMember]
		public int MovingFrame { get; set; }

		public Mu_spl__InkBarBancParam()
		{
			BaseModelDisplayType = "";
			DelayFrame = 0;
			DurationFrame = 0;
			IntervalFrame = 0;
			IsKeepDuration = false;
			IsLightType = false;
			Length = 0.0f;
			MovingFrame = 0;
		}

		public Mu_spl__InkBarBancParam(Mu_spl__InkBarBancParam other)
		{
			BaseModelDisplayType = other.BaseModelDisplayType;
			DelayFrame = other.DelayFrame;
			DurationFrame = other.DurationFrame;
			IntervalFrame = other.IntervalFrame;
			IsKeepDuration = other.IsKeepDuration;
			IsLightType = other.IsLightType;
			Length = other.Length;
			MovingFrame = other.MovingFrame;
		}

		public Mu_spl__InkBarBancParam Clone()
		{
			return new Mu_spl__InkBarBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__InkBarBancParam = new BymlNode.DictionaryNode() { Name = "spl__InkBarBancParam" };

			if (SerializedActor["spl__InkBarBancParam"] != null)
			{
				spl__InkBarBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__InkBarBancParam"];
			}


			if (this.BaseModelDisplayType != "")
			{
				spl__InkBarBancParam.AddNode("BaseModelDisplayType", this.BaseModelDisplayType);
			}

			spl__InkBarBancParam.AddNode("DelayFrame", this.DelayFrame);

			spl__InkBarBancParam.AddNode("DurationFrame", this.DurationFrame);

			spl__InkBarBancParam.AddNode("IntervalFrame", this.IntervalFrame);

			if (this.IsKeepDuration)
			{
				spl__InkBarBancParam.AddNode("IsKeepDuration", this.IsKeepDuration);
			}

			if (this.IsLightType)
			{
				spl__InkBarBancParam.AddNode("IsLightType", this.IsLightType);
			}

			spl__InkBarBancParam.AddNode("Length", this.Length);

			spl__InkBarBancParam.AddNode("MovingFrame", this.MovingFrame);

			if (SerializedActor["spl__InkBarBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__InkBarBancParam);
			}
		}
	}
}
