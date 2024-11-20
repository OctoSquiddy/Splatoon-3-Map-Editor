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
	public class Mu_spl__BombFlowerBancParam
	{
		[ByamlMember]
		public bool IsMiddlePaint { get; set; }

		[ByamlMember]
		public bool IsPaintNormalBaseY { get; set; }

		public Mu_spl__BombFlowerBancParam()
		{
			IsMiddlePaint = false;
			IsPaintNormalBaseY = false;
		}

		public Mu_spl__BombFlowerBancParam(Mu_spl__BombFlowerBancParam other)
		{
			IsMiddlePaint = other.IsMiddlePaint;
			IsPaintNormalBaseY = other.IsPaintNormalBaseY;
		}

		public Mu_spl__BombFlowerBancParam Clone()
		{
			return new Mu_spl__BombFlowerBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__BombFlowerBancParam = new BymlNode.DictionaryNode() { Name = "spl__BombFlowerBancParam" };

			if (SerializedActor["spl__BombFlowerBancParam"] != null)
			{
				spl__BombFlowerBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__BombFlowerBancParam"];
			}


			if (this.IsMiddlePaint)
			{
				spl__BombFlowerBancParam.AddNode("IsMiddlePaint", this.IsMiddlePaint);
			}

			if (this.IsPaintNormalBaseY)
			{
				spl__BombFlowerBancParam.AddNode("IsPaintNormalBaseY", this.IsPaintNormalBaseY);
			}

			if (SerializedActor["spl__BombFlowerBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__BombFlowerBancParam);
			}
		}
	}
}
