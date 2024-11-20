using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;

namespace SampleMapEditor
{
    [ByamlObject]
    public class MuRail : UnitObject, IByamlSerializable, IStageReferencable, INotifyPropertyChanged /*, IByamlSerializable, IStageReferencable*/
    {
        [ByamlMember]
        [BindGUI("Gyaml", Category = "Rail", ColumnIndex = 0, Control = BindControl.Default)]
        public string Gyaml { get; set; }

        [ByamlMember]
        [BindGUI("Hash", Category = "Rail", ColumnIndex = 0, Control = BindControl.Default)]
        public ulong Hash { get; set; }

        [ByamlMember]
        [BindGUI("Is Closed", Category = "Rail", ColumnIndex = 0, Control = BindControl.Default)]
        public bool IsClosed { get; set; }

        [ByamlMember]
        [BindGUI("Layer",
            new object[28] { "None", "Cmn", "Pnt", "Var", "Vlf", "Vgl", "Vcl", "Tcl", "Low", "Mid", "High", "Day",
                "DayOrSuntet", "Sunset", "Night", "FestFirstHalf", "FestSecondHalf", "FestAnnounce", "FestWaitingForResult",
                "FestNone", "BigRun", "Sound", "Area01", "Area02", "Area03", "Area04", "Area05", "Area06" },

            new string[28] { "None", "Common", "Turf War", "Splat Zones", "Tower Control", "Rainmaker", "Clam Blitz",
                "Tricolor Turf War", "Salmon Run Low Tide", "Salmon Run Mid Tide", "Salmon Run High Tide", "Day",
                "Either Day or Sunset", "Sunset", "Night", "First Half of the Splatfest", "Second Half of the Splatfest",
                "Splatfest Announcement", "Splatfest Waiting for Results", "Outside of Splatfest", "Big Run", "Sound",
                "Future Utopia Island", "Cozy & Safe Factory", "Cryogenic Hopetown", "Landfill Dreamland",
                "Eco-Forest Treehills", "Happiness Research Lab" },

            Category = "OBJECT", ColumnIndex = 0, Control = BindControl.ComboBox)]
        public string Layer { get; set; }

        [ByamlMember]
        public List<MuRailPoint> Points { get; set; }

        private ByamlVector3F _rotation;

        /// <summary>
        /// Gets or sets the rotation of the object in radians.
        /// </summary>
        [ByamlMember]
        public ByamlVector3F Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// Gets or sets the rotation of the object in degrees.
        /// </summary>
        [BindGUI("Rotation", Category = "Rail", ColumnIndex = 0, Control = BindControl.Default)]
        public OpenTK.Vector3 RotationDegrees
        {
            get { return new OpenTK.Vector3(_rotation.X, _rotation.Y, _rotation.Z) * Toolbox.Core.STMath.Rad2Deg; }
            set
            {
                var radians = value * Toolbox.Core.STMath.Deg2Rad;
                if (_rotation.X != radians.X || _rotation.Y != radians.Y || _rotation.Z != radians.Z)
                {
                    _rotation = new ByamlVector3F(radians.X, radians.Y, radians.Z);
                    //NotifyPropertyChanged("RotateDegrees");
                    NotifyPropertyChanged("Rotate");
                }
            }
        }

        [ByamlMember]
        [BindGUI("Name", Category = "Rail", ColumnIndex = 0, Control = BindControl.Default)]
        public string Name { get; set; }

        // Constructor
        public MuRail()
        {
            Gyaml = "LiftRail";
            Layer = "None";
            Name = "LiftRail";
            IsClosed = false;
            Points = new List<MuRailPoint>();
        }

        public MuRail(MuRail other)
        {
            Gyaml = other.Gyaml;
            Layer = other.Layer;
            Name = other.Name;
            IsClosed = other.IsClosed;
            Rotation = new ByamlVector3F(other.Rotation.X, other.Rotation.Y, other.Rotation.Z);

            Points = new List<MuRailPoint>();
            for (int i = 0; i < other.Points.Count; i++)
                Points.Add(other.Points[i].Clone());
        }

        public MuRail Clone()
        {
            return new MuRail(this);
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
