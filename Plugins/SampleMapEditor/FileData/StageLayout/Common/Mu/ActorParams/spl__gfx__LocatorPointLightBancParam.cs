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
    public class Mu_spl__gfx__LocatorPointLightBancParam
    {
        [ByamlMember]
        public float DampParam { get; set; }

        [ByamlMember]
        public bool DebugDisplay { get; set; }

        [ByamlMember]
        public Mu_DiffuseColor DiffuseColor { get; set; }

        [ByamlMember]
        public float Intensity { get; set; }

        [ByamlMember]
        public bool IsEnable { get; set; }

        [ByamlMember]
        public string TurnOnType { get; set; }

        public Mu_spl__gfx__LocatorPointLightBancParam()
        {
            DampParam = 0.0f;
            DebugDisplay = false;
            DiffuseColor = new Mu_DiffuseColor();
            Intensity = 0.0f;
            IsEnable = false;
            TurnOnType = "";
        }

        public Mu_spl__gfx__LocatorPointLightBancParam(Mu_spl__gfx__LocatorPointLightBancParam other)
        {
            DampParam = other.DampParam;
            DebugDisplay = other.DebugDisplay;
            DiffuseColor = other.DiffuseColor.Clone();
            Intensity = other.Intensity;
            IsEnable = other.IsEnable;
            TurnOnType = other.TurnOnType;
        }

        public Mu_spl__gfx__LocatorPointLightBancParam Clone()
        {
            return new Mu_spl__gfx__LocatorPointLightBancParam(this);
        }

        public void SaveParameterBank(BymlNode.DictionaryNode SerializedActor)
        {
            BymlNode.DictionaryNode spl__gfx__LocatorPointLightBancParam = new BymlNode.DictionaryNode() { Name = "spl__gfx__LocatorPointLightBancParam" };

            if (SerializedActor["spl__gfx__LocatorPointLightBancParam"] != null)
            {
                spl__gfx__LocatorPointLightBancParam = (BymlNode.DictionaryNode)SerializedActor["spl__gfx__LocatorPointLightBancParam"];
            }

            BymlNode.DictionaryNode DiffuseColor = new BymlNode.DictionaryNode() { Name = "DiffuseColor" };

            if (this.DampParam > 0.0f)
            {
                spl__gfx__LocatorPointLightBancParam.AddNode("DampParam", this.DampParam);
            }

            if (this.DebugDisplay)
            {
                spl__gfx__LocatorPointLightBancParam.AddNode("DebugDisplay", this.DebugDisplay);
            }

            if (this.DiffuseColor.A != 0.0f || this.DiffuseColor.B != 0.0f || this.DiffuseColor.G != 0.0f || this.DiffuseColor.R != 0.0f)
            {
                DiffuseColor.AddNode("A", this.DiffuseColor.A);
                DiffuseColor.AddNode("B", this.DiffuseColor.B);
                DiffuseColor.AddNode("G", this.DiffuseColor.G);
                DiffuseColor.AddNode("R", this.DiffuseColor.R);
                spl__gfx__LocatorPointLightBancParam.Nodes.Add(DiffuseColor);
            }
            if (this.Intensity > 0.0f)
            {
                spl__gfx__LocatorPointLightBancParam.AddNode("Intensity", this.Intensity);
            }
                

            if (this.IsEnable)
            {
                spl__gfx__LocatorPointLightBancParam.AddNode("IsEnable", this.IsEnable);
            }

            if (this.TurnOnType != "")
            {
                spl__gfx__LocatorPointLightBancParam.AddNode("TurnOnType", this.TurnOnType);
            }

            if (SerializedActor["spl__gfx__LocatorPointLightBancParam"] == null)
            {
                SerializedActor.Nodes.Add(spl__gfx__LocatorPointLightBancParam);
            }
        }
    }
}
