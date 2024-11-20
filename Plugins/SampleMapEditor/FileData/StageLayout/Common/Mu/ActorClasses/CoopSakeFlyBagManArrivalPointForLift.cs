using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopSakeFlyBagManArrivalPointForLift : MuObj
	{
		[ByamlMember("spl__CoopSakeFlyBagManArrivalPointForLiftParam")]
		public Mu_spl__CoopSakeFlyBagManArrivalPointForLiftParam spl__CoopSakeFlyBagManArrivalPointForLiftParam { get; set; }

		public CoopSakeFlyBagManArrivalPointForLift() : base()
		{
			spl__CoopSakeFlyBagManArrivalPointForLiftParam = new Mu_spl__CoopSakeFlyBagManArrivalPointForLiftParam();

			Links = new List<Link>();
		}

		public CoopSakeFlyBagManArrivalPointForLift(CoopSakeFlyBagManArrivalPointForLift other) : base(other)
		{
			spl__CoopSakeFlyBagManArrivalPointForLiftParam = other.spl__CoopSakeFlyBagManArrivalPointForLiftParam.Clone();
		}

		public override CoopSakeFlyBagManArrivalPointForLift Clone()
		{
			return new CoopSakeFlyBagManArrivalPointForLift(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopSakeFlyBagManArrivalPointForLiftParam.SaveParameterBank(SerializedActor);
		}
	}
}
