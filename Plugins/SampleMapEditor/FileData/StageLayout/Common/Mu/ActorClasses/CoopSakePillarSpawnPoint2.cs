using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopSakePillarSpawnPoint2 : MuObj
	{
		[ByamlMember("spl__CoopSakePillarSpawnPointBancParam")]
		public Mu_spl__CoopSakePillarSpawnPointBancParam spl__CoopSakePillarSpawnPointBancParam { get; set; }

		public CoopSakePillarSpawnPoint2() : base()
		{
			spl__CoopSakePillarSpawnPointBancParam = new Mu_spl__CoopSakePillarSpawnPointBancParam();

			Links = new List<Link>();
		}

		public CoopSakePillarSpawnPoint2(CoopSakePillarSpawnPoint2 other) : base(other)
		{
			spl__CoopSakePillarSpawnPointBancParam = other.spl__CoopSakePillarSpawnPointBancParam.Clone();
		}

		public override CoopSakePillarSpawnPoint2 Clone()
		{
			return new CoopSakePillarSpawnPoint2(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopSakePillarSpawnPointBancParam.SaveParameterBank(SerializedActor);
		}
	}
}
