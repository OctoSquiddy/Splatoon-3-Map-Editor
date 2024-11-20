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
    public class Mu_spl__PropellerOnlineBancParam
    {
        [ByamlMember]
        public float MoveAcc { get; set; }

        [ByamlMember]
        public float MoveReturnAcc { get; set; }

        [ByamlMember]
        public float MoveReturnSpeedMax { get; set; }

        [ByamlMember]
        public float MoveSpeedMax { get; set; }

        [ByamlMember]
        public float MoveVelChargeFull { get; set; }

        [ByamlMember]
        public int WaitFrameEndRail { get; set; }

        public Mu_spl__PropellerOnlineBancParam()
        {
            MoveAcc = 0.0f;
            MoveReturnAcc = 0.0f;
            MoveReturnSpeedMax = 0.0f;
            MoveSpeedMax = 0.0f;
            MoveVelChargeFull = 0.0f;
            WaitFrameEndRail = 0;
        }

        public Mu_spl__PropellerOnlineBancParam(Mu_spl__PropellerOnlineBancParam other)
        {
            MoveAcc = other.MoveAcc;
            MoveReturnAcc = other.MoveReturnAcc;
            MoveReturnSpeedMax = other.MoveReturnSpeedMax;
            MoveSpeedMax = other.MoveSpeedMax;
            MoveVelChargeFull = other.MoveVelChargeFull;
            WaitFrameEndRail = other.WaitFrameEndRail;
        }

        public Mu_spl__PropellerOnlineBancParam Clone()
        {
            return new Mu_spl__PropellerOnlineBancParam(this);
        }

        public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
        {
            BymlNode.DictionaryNode spl__PropellerOnlineBancParam = new BymlNode.DictionaryNode() { Name = "spl__PropellerOnlineBancParam" };

            if (SerializedActor["spl__PropellerOnlineBancParam"] != null)
            {
                spl__PropellerOnlineBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__PropellerOnlineBancParam"];
            }


            spl__PropellerOnlineBancParam.AddNode("MoveAcc", this.MoveAcc);

            spl__PropellerOnlineBancParam.AddNode("MoveReturnAcc", this.MoveReturnAcc);

            spl__PropellerOnlineBancParam.AddNode("MoveReturnSpeedMax", this.MoveReturnSpeedMax);

            spl__PropellerOnlineBancParam.AddNode("MoveSpeedMax", this.MoveSpeedMax);

            spl__PropellerOnlineBancParam.AddNode("MoveVelChargeFull", this.MoveVelChargeFull);

            spl__PropellerOnlineBancParam.AddNode("WaitFrameEndRail", this.WaitFrameEndRail);

            if (SerializedActor["spl__PropellerOnlineBancParam"] == null)
            {
                SerializedActor.Nodes.Add(spl__PropellerOnlineBancParam);
            }
        }
    }
}
