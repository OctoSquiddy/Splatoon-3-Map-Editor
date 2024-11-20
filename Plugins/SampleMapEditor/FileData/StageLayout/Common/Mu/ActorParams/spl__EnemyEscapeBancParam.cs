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
    public class Mu_spl__EnemyEscapeBancParam
    {
        [ByamlMember]
        public bool IsBombEnabledOnEnterEscape { get; set; }

        [ByamlMember]
        public bool IsBombEnabledOnReachNode { get; set; }

        [ByamlMember]
        public float MaxOffsetToNode { get; set; }

        public Mu_spl__EnemyEscapeBancParam()
        {
            IsBombEnabledOnEnterEscape = false;
            IsBombEnabledOnReachNode = false;
            MaxOffsetToNode = 0.0f;
        }

        public Mu_spl__EnemyEscapeBancParam(Mu_spl__EnemyEscapeBancParam other)
        {
            IsBombEnabledOnEnterEscape = other.IsBombEnabledOnEnterEscape;
            IsBombEnabledOnReachNode = other.IsBombEnabledOnReachNode;
            MaxOffsetToNode = other.MaxOffsetToNode;
        }

        public Mu_spl__EnemyEscapeBancParam Clone()
        {
            return new Mu_spl__EnemyEscapeBancParam(this);
        }

        public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
        {
            BymlNode.DictionaryNode spl__EnemyEscapeBancParam = new BymlNode.DictionaryNode() { Name = "spl__EnemyEscapeBancParam" };

            if (SerializedActor["spl__EnemyEscapeBancParam"] != null)
            {
                spl__EnemyEscapeBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__EnemyEscapeBancParam"];
            }


            if (this.IsBombEnabledOnEnterEscape)
            {
                spl__EnemyEscapeBancParam.AddNode("IsBombEnabledOnEnterEscape", this.IsBombEnabledOnEnterEscape);
            }

            if (this.IsBombEnabledOnReachNode)
            {
                spl__EnemyEscapeBancParam.AddNode("IsBombEnabledOnReachNode", this.IsBombEnabledOnReachNode);
            }

            spl__EnemyEscapeBancParam.AddNode("MaxOffsetToNode", this.MaxOffsetToNode);

            if (SerializedActor["spl__EnemyEscapeBancParam"] == null)
            {
                SerializedActor.Nodes.Add(spl__EnemyEscapeBancParam);
            }
        }
    }
}
