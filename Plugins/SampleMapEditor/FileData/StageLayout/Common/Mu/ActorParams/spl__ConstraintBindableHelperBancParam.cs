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
	public class Mu_spl__ConstraintBindableHelperBancParam
	{
		[ByamlMember]
		public string ConstraintType { get; set; }

		[ByamlMember]
		public string FreeAxis { get; set; }

		[ByamlMember]
		public bool IsBindToWorld { get; set; }

		[ByamlMember]
		public bool IsNoHitBetweenBodies { get; set; }

		public Mu_spl__ConstraintBindableHelperBancParam()
		{
			ConstraintType = "";
			FreeAxis = "";
			IsBindToWorld = false;
			IsNoHitBetweenBodies = false;
		}

		public Mu_spl__ConstraintBindableHelperBancParam(Mu_spl__ConstraintBindableHelperBancParam other)
		{
			ConstraintType = other.ConstraintType;
			FreeAxis = other.FreeAxis;
			IsBindToWorld = other.IsBindToWorld;
			IsNoHitBetweenBodies = other.IsNoHitBetweenBodies;
		}

		public Mu_spl__ConstraintBindableHelperBancParam Clone()
		{
			return new Mu_spl__ConstraintBindableHelperBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__ConstraintBindableHelperBancParam = new BymlNode.DictionaryNode() { Name = "spl__ConstraintBindableHelperBancParam" };

			if (SerializedActor["spl__ConstraintBindableHelperBancParam"] != null)
			{
				spl__ConstraintBindableHelperBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__ConstraintBindableHelperBancParam"];
			}


			if (this.ConstraintType != "")
			{
				spl__ConstraintBindableHelperBancParam.AddNode("ConstraintType", this.ConstraintType);
			}

			if (this.FreeAxis != "")
			{
				spl__ConstraintBindableHelperBancParam.AddNode("FreeAxis", this.FreeAxis);
			}

			if (this.IsBindToWorld)
			{
				spl__ConstraintBindableHelperBancParam.AddNode("IsBindToWorld", this.IsBindToWorld);
			}

			if (this.IsNoHitBetweenBodies)
			{
				spl__ConstraintBindableHelperBancParam.AddNode("IsNoHitBetweenBodies", this.IsNoHitBetweenBodies);
			}

			if (SerializedActor["spl__ConstraintBindableHelperBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__ConstraintBindableHelperBancParam);
			}
		}
	}
}
