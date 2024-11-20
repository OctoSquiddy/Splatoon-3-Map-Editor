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
	public class Mu_spl__DamageConveyorBancParam
	{
		[ByamlMember]
		public bool IsHitPaint { get; set; }

		public Mu_spl__DamageConveyorBancParam()
		{
			IsHitPaint = false;
		}

		public Mu_spl__DamageConveyorBancParam(Mu_spl__DamageConveyorBancParam other)
		{
			IsHitPaint = other.IsHitPaint;
		}

		public Mu_spl__DamageConveyorBancParam Clone()
		{
			return new Mu_spl__DamageConveyorBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__DamageConveyorBancParam = new BymlNode.DictionaryNode() { Name = "spl__DamageConveyorBancParam" };

			if (SerializedActor["spl__DamageConveyorBancParam"] != null)
			{
				spl__DamageConveyorBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__DamageConveyorBancParam"];
			}


			if (this.IsHitPaint)
			{
				spl__DamageConveyorBancParam.AddNode("IsHitPaint", this.IsHitPaint);
			}

			if (SerializedActor["spl__DamageConveyorBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__DamageConveyorBancParam);
			}
		}
	}
}
