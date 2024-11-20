using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Wheatley.io.BYML;

namespace SampleMapEditor
{
	public class CoopPathCorrectArea : MuObj
	{
		[ByamlMember("spl__CoopPathCorrectAreaParam")]
		public Mu_spl__CoopPathCorrectAreaParam spl__CoopPathCorrectAreaParam { get; set; }

		public CoopPathCorrectArea() : base()
		{
			spl__CoopPathCorrectAreaParam = new Mu_spl__CoopPathCorrectAreaParam();

			Links = new List<Link>();
		}

		public CoopPathCorrectArea(CoopPathCorrectArea other) : base(other)
		{
			spl__CoopPathCorrectAreaParam = other.spl__CoopPathCorrectAreaParam.Clone();
		}

		public override CoopPathCorrectArea Clone()
		{
			return new CoopPathCorrectArea(this);
		}

		public override void SaveAdditionalParameters(BymlNode.DictionaryNode SerializedActor)
		{
			this.spl__CoopPathCorrectAreaParam.SaveParameterBank(SerializedActor);
		}
	}
}
