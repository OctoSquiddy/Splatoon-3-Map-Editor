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
    public class Mu_spl__gfx__LocatorSpotLightBancParam
    {
        [ByamlMember]
        public float AngleDamp { get; set; }

        [ByamlMember]
        public Mu_DiffuseColor DiffuseColor { get; set; }

        [ByamlMember]
        public float DistDamp { get; set; }

        [ByamlMember]
        public float Intensity { get; set; }

        [ByamlMember]
        public bool IsEnable { get; set; }

        [ByamlMember]
        public string TurnOnType { get; set; }

        public Mu_spl__gfx__LocatorSpotLightBancParam()
        {
            AngleDamp = 0.0f;
            DiffuseColor = new Mu_DiffuseColor();
            DistDamp = 0.0f;
            Intensity = 0.0f;
            IsEnable = false;
            TurnOnType = "";
        }

        public Mu_spl__gfx__LocatorSpotLightBancParam(Mu_spl__gfx__LocatorSpotLightBancParam other)
        {
            AngleDamp = other.AngleDamp;
            DiffuseColor = other.DiffuseColor.Clone();
            DistDamp = other.DistDamp;
            Intensity = other.Intensity;
            IsEnable = other.IsEnable;
            TurnOnType = other.TurnOnType;
        }

        public Mu_spl__gfx__LocatorSpotLightBancParam Clone()
        {
            return new Mu_spl__gfx__LocatorSpotLightBancParam(this);
        }

        public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
        {
            BymlNode.DictionaryNode spl__gfx__LocatorSpotLightBancParam = new BymlNode.DictionaryNode() { Name = "spl__gfx__LocatorSpotLightBancParam" };

            if (SerializedActor["spl__gfx__LocatorSpotLightBancParam"] != null)
            {
                spl__gfx__LocatorSpotLightBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__gfx__LocatorSpotLightBancParam"];
            }

            BymlNode.DictionaryNode DiffuseColor = new BymlNode.DictionaryNode() { Name = "DiffuseColor" };

            if (this.AngleDamp > 0.0f)
            {
                spl__gfx__LocatorSpotLightBancParam.AddNode("AngleDamp", this.AngleDamp);
            }

            if (this.DiffuseColor.A != 0.0f || this.DiffuseColor.B != 0.0f || this.DiffuseColor.G != 0.0f || this.DiffuseColor.R != 0.0f)
            {
                DiffuseColor.AddNode("A", this.DiffuseColor.A);
                DiffuseColor.AddNode("B", this.DiffuseColor.B);
                DiffuseColor.AddNode("G", this.DiffuseColor.G);
                DiffuseColor.AddNode("R", this.DiffuseColor.R);
                spl__gfx__LocatorSpotLightBancParam.Nodes.Add(DiffuseColor);
            }

            if (this.DistDamp > 0.0f)
            {
                spl__gfx__LocatorSpotLightBancParam.AddNode("DistDamp", this.DistDamp);
            }

            if (this.Intensity > 0.0f)
            {
                spl__gfx__LocatorSpotLightBancParam.AddNode("Intensity", this.Intensity);
            }

            if (this.IsEnable)
            {
                spl__gfx__LocatorSpotLightBancParam.AddNode("IsEnable", this.IsEnable);
            }

            if (this.TurnOnType != "")
            {
                spl__gfx__LocatorSpotLightBancParam.AddNode("TurnOnType", this.TurnOnType);
            }

            if (SerializedActor["spl__gfx__LocatorSpotLightBancParam"] == null)
            {
                SerializedActor.Nodes.Add(spl__gfx__LocatorSpotLightBancParam);
            }
        }
    }
}
