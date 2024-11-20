using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopSakePillarSpawnPoint1 : MuObj
	{
		[ByamlMember("spl__CoopSakePillarSpawnPointBancParam")]
		public Mu_spl__CoopSakePillarSpawnPointBancParam spl__CoopSakePillarSpawnPointBancParam { get; set; }

		public CoopSakePillarSpawnPoint1() : base()
		{
			spl__CoopSakePillarSpawnPointBancParam = new Mu_spl__CoopSakePillarSpawnPointBancParam();

			Links = new List<Link>();
		}

		public CoopSakePillarSpawnPoint1(CoopSakePillarSpawnPoint1 other) : base(other)
		{
			spl__CoopSakePillarSpawnPointBancParam = other.spl__CoopSakePillarSpawnPointBancParam.Clone();
		}

		public override CoopSakePillarSpawnPoint1 Clone()
		{
			return new CoopSakePillarSpawnPoint1(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopSakePillarSpawnPointBancParam.SaveParameterBank(SerializedActor);
		}
	}
}