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
	public class Mu_spl__KebaInkCoreBancParam
	{
		[ByamlMember]
		public bool IsDisableDynamicLoading { get; set; }

		[ByamlMember]
		public bool IsForIkuraShootTutorial { get; set; }

		[ByamlMember]
		public bool IsNoCore { get; set; }

		[ByamlMember]
		public bool IsSmallWorldLast { get; set; }

		[ByamlMember]
		public int NecessarySalmonRoe { get; set; }

		public Mu_spl__KebaInkCoreBancParam()
		{
			IsDisableDynamicLoading = false;
			IsForIkuraShootTutorial = false;
			IsNoCore = false;
			IsSmallWorldLast = false;
			NecessarySalmonRoe = 0;
		}

		public Mu_spl__KebaInkCoreBancParam(Mu_spl__KebaInkCoreBancParam other)
		{
			IsDisableDynamicLoading = other.IsDisableDynamicLoading;
			IsForIkuraShootTutorial = other.IsForIkuraShootTutorial;
			IsNoCore = other.IsNoCore;
			IsSmallWorldLast = other.IsSmallWorldLast;
			NecessarySalmonRoe = other.NecessarySalmonRoe;
		}

		public Mu_spl__KebaInkCoreBancParam Clone()
		{
			return new Mu_spl__KebaInkCoreBancParam(this);
		}

		public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
		{
			BymlNode.DictionaryNode spl__KebaInkCoreBancParam = new BymlNode.DictionaryNode() { Name = "spl__KebaInkCoreBancParam" };

			if (SerializedActor["spl__KebaInkCoreBancParam"] != null)
			{
				spl__KebaInkCoreBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__KebaInkCoreBancParam"];
			}


			if (this.IsDisableDynamicLoading)
			{
				spl__KebaInkCoreBancParam.AddNode("IsDisableDynamicLoading", this.IsDisableDynamicLoading);
			}

			if (this.IsForIkuraShootTutorial)
			{
				spl__KebaInkCoreBancParam.AddNode("IsForIkuraShootTutorial", this.IsForIkuraShootTutorial);
			}

			if (this.IsNoCore)
			{
				spl__KebaInkCoreBancParam.AddNode("IsNoCore", this.IsNoCore);
			}

			if (this.IsSmallWorldLast)
			{
				spl__KebaInkCoreBancParam.AddNode("IsSmallWorldLast", this.IsSmallWorldLast);
			}

			spl__KebaInkCoreBancParam.AddNode("NecessarySalmonRoe", this.NecessarySalmonRoe);

			if (SerializedActor["spl__KebaInkCoreBancParam"] == null)
			{
				SerializedActor.Nodes.Add(spl__KebaInkCoreBancParam);
			}
		}
	}
}
