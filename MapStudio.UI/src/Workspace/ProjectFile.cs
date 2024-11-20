using System;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using Newtonsoft.Json;
using GLFrameworkEngine;
using System.Globalization;

namespace MapStudio.UI
{
    public class ProjectFile
    {
        //Folder of the loaded files
        public string WorkingDirectory { get; set; }

        private string saveData = "";

        //Save date of project document
        public string SaveDate
        {
            get { return saveData; }
            set
            {
                saveData = value;
                DateTime = DateTime.ParseExact(value, "dd/MMM/yyyy-(hh:mm:ss)", CultureInfo.InvariantCulture);
            }
        }

        public DateTime DateTime;

        /// <summary>
        /// Gets or sets the original GUI scroll value of the outliner on the Y axis.
        /// </summary>
        public float OutlierScroll { get; set; }

        /// <summary>
        /// A list of files used by the project to load/save
        /// </summary>
        public List<string> FileAssets = new List<string>();

        //Editor properties
        public string SelectedWorkspace = "Default";
        public List<string> ActiveWorkspaces = new List<string>();
        public string ActiveEditor { get; set; }

        public bool UseCollisionDetection { get; set; } = true;

        //Camera settings
        public CameraSettings Camera = new CameraSettings();

        //Debug viewport shading
        public string ShadingMode { get; set; } = "Default";

        //Nodes with an ID linked.
        public Dictionary<int, NodeSettings> Nodes = new Dictionary<int, NodeSettings>();

        public static ProjectFile Load(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<ProjectFile>(json);
        }
            
        public void Save(string filePath)
        {
            saveData = DateTime.Now.ToString("dd/MMM/yyyy-(hh:mm:ss)");

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void ApplySettings(GLContext context, Workspace workspace)
        {
           // SelectedWorkspace = workspace.ActiveEditor.SubEditor;
            ShadingMode = DebugShaderRender.DebugRendering.ToString();

            var camera = context.Camera;
            this.UseCollisionDetection = context.EnableDropToCollision;
            this.Camera.RotationX = camera.RotationX;
            this.Camera.RotationY = camera.RotationY;
            this.Camera.Distance = camera.TargetDistance;
            this.Camera.PositionX = camera.TargetPosition.X;
            this.Camera.PositionY = camera.TargetPosition.Y;
            this.Camera.PositionZ = camera.TargetPosition.Z;
            this.Camera.FovDegrees = camera.FovDegrees;
            this.Camera.ZFar = camera.ZFar;
            this.Camera.ZNear = camera.ZNear;
        }

        public void LoadSettings(GLContext context, Workspace workspace)
        {
          //  workspace.ActiveEditor.SubEditor = SelectedWorkspace;
            DebugShaderRender.DebugRendering = Enum.Parse<DebugShaderRender.DebugRender>(ShadingMode);

            var camera = context.Camera;
            context.EnableDropToCollision = UseCollisionDetection;
            camera.RotationX = this.Camera.RotationX;
            camera.RotationY = this.Camera.RotationY;
            camera.TargetDistance = this.Camera.Distance;
            camera.TargetPosition = new Vector3(this.Camera.PositionX, this.Camera.PositionY, this.Camera.PositionZ);
            camera.FovDegrees = this.Camera.FovDegrees;
            camera.ZFar = this.Camera.ZFar;
            camera.ZNear = this.Camera.ZNear;
        }

        public class NodeSettings
        {
            public bool IsExpaned = false;
            public bool IsSelected = false;
        }

        public class CameraSettings
        {
            public float PositionX { get; set; }
            public float PositionY { get; set; }
            public float PositionZ { get; set; }

            public float Distance { get; set; }

            public float RotationX { get; set; }
            public float RotationY { get; set; }

            /// <summary>
            /// Gets or sets the camera controller mode to determine how the camera moves.
            /// </summary>
            public Camera.CameraMode Mode { get; set; } = GLFrameworkEngine.Camera.CameraMode.Inspect;

            /// <summary>
            /// Toggles orthographic projection.
            /// </summary>
            public bool IsOrthographic { get; set; } = false;

            /// <summary>
            /// Gets or sets the field of view in degrees.
            /// </summary>
            public float FovDegrees { get; set; } = 45;

            /// <summary>
            /// Gets or sets the camera move speed using key inputs.
            /// </summary>
            public float KeyMoveSpeed { get; set; } = 10.0f;

            /// <summary>
            /// Gets or sets the camera move speed during panning.
            /// </summary>
            public float PanSpeed { get; set; } = 1.0f;

            /// <summary>
            /// Gets or sets the camera move speed during zooming.
            /// </summary>
            public float ZoomSpeed { get; set; } = 1.0f;

            /// <summary>
            /// Gets or sets the z near projection.
            /// </summary>
            public float ZNear { get; set; } = 1.0f;

            /// <summary>
            /// Gets or sets the z far projection.
            /// </summary>
            public float ZFar { get; set; } = 100000.0f;
        }
    }
}
