using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using static SampleMapEditor.BgmArea;

namespace SampleMapEditor
{
    [ByamlObject]
    public class MuRailPoint : SpatialObject, IByamlSerializable, IStageReferencable, INotifyPropertyChanged
    {
        [ByamlMember]
        public ulong Hash { get; set; }

        [ByamlMember]
        public ByamlVector3F Control0 { get; set; }

        [ByamlMember]
        public ByamlVector3F Control1 { get; set; }

        public bool HasBeenEditedWithSelector = false;

        public bool hasControlPoints { get; set; }

        [ByamlObject]
        public class Mu_game__LiftGraphRailNodeParam
        {
            [ByamlMember]
            public float BreakTime { get; set; }

            [ByamlMember]
            public ByamlVector3F Rotation { get; set; }

            public Mu_game__LiftGraphRailNodeParam()
            {
                BreakTime = 0.0f;
            }

            public Mu_game__LiftGraphRailNodeParam(Mu_game__LiftGraphRailNodeParam other)
            {
                BreakTime = other.BreakTime;
                Rotation = new ByamlVector3F(other.Rotation.X, other.Rotation.Y, other.Rotation.Z);
            }

            public Mu_game__LiftGraphRailNodeParam Clone()
            {
                return new Mu_game__LiftGraphRailNodeParam(this);
            }
        }

        [ByamlObject]
        public class Mu_spl__GachiyaguraRailNodeParam
        {
            [ByamlMember]
            public int CheckPointHP { get; set; }

            [ByamlMember]
            public string FillUpType { get; set; }

            [ByamlMember]
            public ByamlVector3F PositionOffset { get; set; }

            [ByamlMember]
            public ByamlVector3F RotationDeg { get; set; }

            public Mu_spl__GachiyaguraRailNodeParam()
            {
                CheckPointHP = 0;
                FillUpType = "";
            }

            public Mu_spl__GachiyaguraRailNodeParam(Mu_spl__GachiyaguraRailNodeParam other)
            {
                CheckPointHP = other.CheckPointHP;
                FillUpType = other.FillUpType;
                PositionOffset = new ByamlVector3F(other.PositionOffset.X, other.PositionOffset.Y, other.PositionOffset.Z);
                RotationDeg = new ByamlVector3F(other.RotationDeg.X, other.RotationDeg.Y, other.RotationDeg.Z);
            }

            public Mu_spl__GachiyaguraRailNodeParam Clone()
            {
                return new Mu_spl__GachiyaguraRailNodeParam(this);
            }
        }

        [ByamlMember("game__LiftGraphRailNodeParam")]
        public Mu_game__LiftGraphRailNodeParam game__LiftGraphRailNodeParam { get; set; }

        [ByamlMember("spl__GachiyaguraRailNodeParam")]
        public Mu_spl__GachiyaguraRailNodeParam spl__GachiyaguraRailNodeParam { get; set; }

        public MuRailPoint()
        {
            hasControlPoints = false;
            game__LiftGraphRailNodeParam = new Mu_game__LiftGraphRailNodeParam();
            spl__GachiyaguraRailNodeParam = new Mu_spl__GachiyaguraRailNodeParam();
        }

        public MuRailPoint(MuRailPoint other)
        {
            Translate = new ByamlVector3F(other.Translate.X, other.Translate.Y, other.Translate.Z);
            Rotate = new ByamlVector3F(other.Rotate.X, other.Rotate.Y, other.Rotate.Z);
            Scale = new ByamlVector3F(other.Scale.X, other.Scale.Y, other.Scale.Z);

            Control0 = new ByamlVector3F(other.Control0.X, other.Control0.Y, other.Control0.Z);
            Control1 = new ByamlVector3F(other.Control1.X, other.Control1.Y, other.Control1.Z);

            hasControlPoints = other.hasControlPoints;
            game__LiftGraphRailNodeParam = other.game__LiftGraphRailNodeParam.Clone();
            spl__GachiyaguraRailNodeParam = other.spl__GachiyaguraRailNodeParam.Clone();
        }

        public MuRailPoint Clone()
        {
            return new MuRailPoint(this);
        }

        // Methods
        public virtual void DeserializeByaml(IDictionary<string, object> dictionary)
        {

        }

        public virtual void SerializeByaml(IDictionary<string, object> dictionary)
        {

        }

        public virtual void DeserializeReferences(StageDefinition stageDefinition)
        {

        }

        public virtual void SerializeReferences(StageDefinition stageDefinition)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
