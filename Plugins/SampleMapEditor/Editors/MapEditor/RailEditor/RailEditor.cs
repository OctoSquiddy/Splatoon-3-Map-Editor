using GLFrameworkEngine;
using GLFrameworkEngine.UI;
using ImGuiNET;
using IONET.Collada.Core.Scene;
using IONET.Collada.Core.Transform;
using IONET.Collada.FX.Rendering;
using MapStudio.UI;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;
using Toolbox.Core.ViewModels;
using static Toolbox.Core.Runtime;

namespace SampleMapEditor.LayoutEditor
{
    public class RailEditor : ILayoutEditor//, UIEditToolMenu
    {
        public string Name => "Rail Editor";
        public StageLayoutPlugin MapEditor { get; set; }

        public ObjectEditor ObjectEditor { get; set; }

        private IToolWindowDrawer PathSettingsWindow;

        public IToolWindowDrawer ToolWindowDrawer => PathSettingsWindow;

        public List<IDrawable> Renderers { get; set; } = new List<IDrawable>();

        public NodeBase Root { get; set; }

        public List<MenuItemModel> MenuItems { get; set; } = new List<MenuItemModel>();

        public bool IsActive { get; set; }

        
        private GlobalSettings.PathColor Color;

        public RailEditor(StageLayoutPlugin editor, GlobalSettings.PathColor color, List<MuRail> rails, ObjectEditor objectEditor)
        {
            MapEditor = editor;
            ObjectEditor = objectEditor;

            PathSettingsWindow = new RailPathToolSettings(this);
            Root = new NodeBase(Name) { HasCheckBox = true };
            MapEditorIcons.ReloadIcons(Root, typeof(MuRail));

            Color = color;
            ReloadPaths(rails);

            var addMenu = new MenuItemModel(this.Name);
            GLContext.ActiveContext.Scene.MenuItemsAdd.Add(addMenu);

            addMenu.MenuItems.Add(new MenuItemModel("ADD_NORMAL", CreateLinearPath));
            addMenu.MenuItems.Add(new MenuItemModel("ADD_BEZIER", CreateBezierPathStandard));
            addMenu.MenuItems.Add(new MenuItemModel("ADD_CIRCLE", CreateBezierPathCircle));

            MenuItems = new List<MenuItemModel>();
            MenuItems.AddRange(addMenu.MenuItems);
        }

        public void DrawEditMenuBar()       // DIRECT COPY AND PASTE // MAY NEED LOOKED OVER
        {
            foreach (var item in MenuItems)
                ImGuiHelper.LoadMenuItem(item);

            var selected = (RenderablePath)this.Renderers.FirstOrDefault(x =>
                ((RenderablePath)x).IsSelected ||
                ((RenderablePath)x).EditMode);

            if (selected != null)
            {
                if (ImGui.Button(selected.EditMode ? "Object Mode" : "Edit Mode"))
                {
                    GLContext.ActiveContext.Scene.ToggleEditMode();
                }
            }

            if (selected != null && selected.EditMode)
            {
                Workspace.ActiveWorkspace.ViewportWindow.DrawPathDropdown();
            }
        }


        public void DrawHelpWindow()
        {
            if (ImGuiNET.ImGui.CollapsingHeader("Paths", ImGuiNET.ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGuiHelper.BoldTextLabel(InputSettings.INPUT.Scene.EditMode, "Edit Selected Path.");
                ImGuiHelper.BoldTextLabel(InputSettings.INPUT.Scene.Extrude, "Extrude Point.");
            }
        }



        public void ReloadPath(MuRail rail)
        {
            CreatePathObject(rail);
        }


        //public void ReloadPaths(IEnumerable<PathBase<TPath, TPoint>> groups)
        public void ReloadPaths(List<MuRail> rails)
        {
            if (rails != null)
            {
                Root.Children.Clear();
                foreach (var rail in rails)
                    CreatePathObject(rail);
            }
        }



        public void OnSave(StageDefinition stage)
        {
            stage.Rails = new List<MuRail>();

            foreach (var path in Root.Children)
            {
                var rail = path.Tag as MuRail;
                rail.Points.Clear();    //rail.Points.Clear();

                foreach (RenderablePath.PointNode point in path.Children)
                {
                    ApplyPoint(point.Point, rail, point.Tag as MuRailPoint);
                }

                /*if (path.Tag is Path)
                    course.Paths.Add((Path)path.Tag);
                if (path.Tag is ObjPath)
                    course.ObjPaths.Add((ObjPath)path.Tag);
                if (path.Tag is JugemPath)
                    course.JugemPaths.Add((JugemPath)path.Tag);*/

                if (path.Tag is MuRail)
                    stage.Rails.Add((MuRail)path.Tag);
            }
        }


        private void ApplyPoint(RenderablePathPoint renderPoint, MuRail rail, MuRailPoint point)
        {
            rail.Points.Add(point);

            point.Translate = new ByamlVector3F(
                renderPoint.Transform.Position.X / 10.0f,
                renderPoint.Transform.Position.Y / 10.0f,
                renderPoint.Transform.Position.Z / 10.0f);

            point.Rotate = new ByamlVector3F(
                renderPoint.Transform.Rotation.X,
                renderPoint.Transform.Rotation.Y,
                renderPoint.Transform.Rotation.Z);

            if (renderPoint.Transform.Scale != Vector3.One)
            {
                point.Scale = new ByamlVector3F(
                    renderPoint.Transform.Scale.X,
                    renderPoint.Transform.Scale.Y,
                    renderPoint.Transform.Scale.Z);
            }

            if (renderPoint.ParentPath.InterpolationMode == RenderablePath.Interpolation.Bezier)
            {
                var pathPoint = point as MuRailPoint;

                pathPoint.Control0 = new ByamlVector3F(
                    renderPoint.ControlPoint1.Transform.Position.X / 10.0f,
                    renderPoint.ControlPoint1.Transform.Position.Y / 10.0f,
                    renderPoint.ControlPoint1.Transform.Position.Z / 10.0f);

                pathPoint.Control1 = new ByamlVector3F(
                    renderPoint.ControlPoint2.Transform.Position.X / 10.0f,
                    renderPoint.ControlPoint2.Transform.Position.Y / 10.0f,
                    renderPoint.ControlPoint2.Transform.Position.Z / 10.0f);
            }
        }

        private void CreatePathObject(MuRail rail)
        {
            RenderablePath renderer = null;
            //Reload existing rails
            foreach (RenderablePath render in Renderers)
            {
                if (render.UINode.Tag == rail)
                    renderer = render;
            }
            if (renderer == null)
            {
                renderer = new RenderablePath();
                this.Add(renderer);
            }

            //Reset points incase the path gets reloaded
            renderer.PathPoints.Clear();
            renderer.UINode.Children.Clear();

            LoadPath(renderer, rail);
        }



        private void PreparePath(RenderablePath path)
        {
            //Make the path create instances for property types
            path.PointUITagType = typeof(MuRailPoint);
            path.PathUITagType = typeof(MuRail);
            path.GetPointColor = (RenderablePathPoint pt) =>
            {
                var tag = pt.UINode.Tag;
                if (tag is MuRailPoint)
                {
                    var pointData = tag as MuRailPoint;
                    /*if (pointData.Prm1 != 0 || pointData.Prm2 != 0)
                        return new Vector4(1, 1, 0, 1);*/
                }
                return path.PointColor;
            };

            //Create the path tag instace for created paths
            path.UINode.Tag = Activator.CreateInstance(path.PathUITagType);

            // Initialize the MuRail with proper Hash and InstanceID
            var rail = path.UINode.Tag as MuRail;
            if (rail != null)
            {
                var (allHashes, allInstanceIDs) = CollectAllHashesAndInstanceIDs();

                // Generate unique InstanceID
                string instanceID = GenInstanceID();
                while (allInstanceIDs.Contains(instanceID))
                    instanceID = GenInstanceID();
                rail.InstanceID = instanceID;

                // Generate unique Hash
                ulong hash = GenHash();
                while (allHashes.Contains(hash))
                    hash = GenHash();
                rail.Hash = hash;
            }

            MapEditorIcons.ReloadIcons(path.UINode, typeof(MuRail));

            // Setup the path default settings
            path.ConnectHoveredPoints = false;
            path.PointColor = new Vector4(Color.PointColor.X, Color.PointColor.Y, Color.PointColor.Z, 1.0f);
            path.ArrowColor = new Vector4(Color.ArrowColor.X, Color.ArrowColor.Y, Color.ArrowColor.Z, 1.0f);
            path.LineColor = new Vector4(Color.LineColor.X, Color.LineColor.Y, Color.LineColor.Z, 1.0f);
            // Only connect by the next points
            path.AutoConnectByNext = true;

            // Debug color changing setting for updating real time
            Color.OnColorChanged += delegate
            {
                path.PointColor = new Vector4(Color.PointColor.X, Color.PointColor.Y, Color.PointColor.Z, 1.0f);
                path.ArrowColor = new Vector4(Color.ArrowColor.X, Color.ArrowColor.Y, Color.ArrowColor.Z, 1.0f);
                path.LineColor = new Vector4(Color.LineColor.X, Color.LineColor.Y, Color.LineColor.Z, 1.0f);
            };
            path.AddCallback += delegate
            {
                Root.AddChild(path.UINode);
                Renderers.Add(path);
            };
            path.RemoveCallback += delegate
            {
                Root.Children.Remove(path.UINode);
                Renderers.Remove(path);
            };
            SetupProperties(path);
        }

        unsafe private void SetupProperties(RenderablePath path)
        {
            if (path.UINode.Tag is MuRail) // ???
            {
                //Update the renderer when properties are updated
                var pathProp = path.UINode.Tag as MuRail;

                // Draw Edit Data
                path.UINode.TagUI.UIDrawer = null;
                path.UINode.TagUI.UIDrawer += delegate
                {
                    if (ImGui.CollapsingHeader($"Path Points", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        if (ImGui.Button($"   {IconManager.ADD_ICON}   ###Path Points"))
                        {
                            InsertNewPathNodeInkRail(path, pathProp);
                        }

                        ImGui.BeginGroup();
                        for (int i = 0; i < pathProp.Points.Count; i++)
                        {
                            if (ImGui.Button($"   {IconManager.DELETE_ICON}   ###Path Points.{i}"))
                            {
                                if (pathProp.Points.Count > 2)
                                    RemovePathNodeInkRail(path, pathProp, i);
                            }

                            ImGui.SameLine();

                            if (ImGui.Button($"...###Path Points2.{i}"))
                            {
                                EditTransformObjectSelector(path, pathProp, i);
                            }
                        }
                        ImGui.EndGroup();

                        ImGui.SameLine();
                        int numColumns = 2;

                        ImGui.BeginGroup();
                        ImGui.BeginColumns("##" + "Points" + numColumns.ToString(), numColumns);
                        for (int i = 0; i < pathProp.Points.Count; i++)
                        {
                            float colwidth = 0.0f;

                            if (pathProp.Points[i].HasBeenEditedWithSelector)
                            {
                                ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.2f, 0.65f, 0.32f, 1.0f));
                            }

                            ImGui.AlignTextToFramePadding();
                            ImGui.Text($"Point {i}");
                            ImGui.NextColumn();

                            if (pathProp.Points[i].HasBeenEditedWithSelector)
                            {
                                ImGui.PopStyleColor();
                            }

                            ulong inputValue = pathProp.Points[i].Hash;
                            ulong* ulongptr = &inputValue;

                            colwidth = ImGui.GetColumnWidth();
                            ImGui.PushItemWidth(colwidth - 6);

                            if (ImGui.InputScalar($"###PointBox.hash{i}", ImGuiDataType.U64, (IntPtr)ulongptr))
                            {
                                pathProp.Points[i].Hash = (ulong)inputValue;
                            }

                            ImGui.PopItemWidth();
                            ImGui.NextColumn();                         
                        }
                        ImGui.EndColumns();
                        ImGui.EndGroup();
                    }

                    var gui = new MapObjectUI();
                    gui.RenderRail(pathProp, Workspace.ActiveWorkspace.GetSelected().Select(x => x.Tag));
                };

                pathProp.PropertyChanged += delegate
                {
                    path.Loop = pathProp.IsClosed;

                    bool IsBezierAtSomePoint = false;
                    for (int i = 0; i < pathProp.Points.Count; i++)
                    {
                        if (!pathProp.Points[i].Control0.Equals(new ByamlVector3F(0.0f, 0.0f, 0.0f)) && !pathProp.Points[i].Control1.Equals(new ByamlVector3F(0.0f, 0.0f, 0.0f)))
                        {
                            IsBezierAtSomePoint = true;
                            break;
                        }
                    }

                    if (IsBezierAtSomePoint)
                        path.InterpolationMode = RenderablePath.Interpolation.Bezier;
                    else
                        path.InterpolationMode = RenderablePath.Interpolation.Linear;

                    GLContext.ActiveContext.UpdateViewport = true;
                };
            }
        }

        public void ReloadEditor()
        {
            foreach (RenderablePath path in Renderers)
            {
                foreach (var part in path.PathPoints)
                    part.CanSelect = true; // !IsBaked;
            }
        }

        private void LoadPath(RenderablePath renderable, MuRail rail)
        {
            if (rail == null)
                return;

            renderable.PathPoints.Clear();

            if (rail is MuRail)
                renderable.Loop = (rail as MuRail).IsClosed;

            //Set the tag information of the path node
            renderable.UINode.Tag = rail;
            //Setup the tag properties
            SetupProperties(renderable);

            bool IsBezierAtSomePoint = false;
            for (int i = 0; i < rail.Points.Count; i++)
            {
                var pt = rail.Points[i];
                RenderablePathPoint point = CreatePathPoint(renderable, pt);

                //Configure path points
                if (pt is MuRailPoint)
                {
                    var pathPt = pt as MuRailPoint;
                    //Configure control points if used
                    if (IsBezierAtSomePoint || (!pathPt.Control0.Equals(new ByamlVector3F(0.0f, 0.0f, 0.0f)) && !pathPt.Control1.Equals(new ByamlVector3F(0.0f, 0.0f, 0.0f))))
                    {
                        //Set each control handle position in world space while updating all matrices
                        point.ControlPoint1.Transform.Position = new Vector3(pathPt.Control0.X * 10.0f, pathPt.Control0.Y * 10.0f, pathPt.Control0.Z * 10.0f);
                        point.ControlPoint2.Transform.Position = new Vector3(pathPt.Control1.X * 10.0f, pathPt.Control1.Y * 10.0f, pathPt.Control1.Z * 10.0f);
                        point.UpdateMatrices();

                        IsBezierAtSomePoint = true;
                    }
                }

                //Add the point to the list
                renderable.AddPoint(point);

                //Set the tag information of the node
                point.UINode.Tag = pt;

                // Connect each point from the previous point
                if (i != 0)
                {
                    renderable.PathPoints[i - 1].AddChild(point);
                }
            }

            if (IsBezierAtSomePoint)
            {
                renderable.InterpolationMode = RenderablePath.Interpolation.Bezier;
            }
            else
            {
                renderable.InterpolationMode = RenderablePath.Interpolation.Linear;
            }
        }

        unsafe private RenderablePathPoint CreatePathPoint(RenderablePath renderable, MuRailPoint pt)
        {
            var position = new Vector3(pt.Translate.X * 10.0f, pt.Translate.Y * 10.0f, pt.Translate.Z * 10.0f);
            var point = renderable.CreatePoint(position);

            // Use the point's own rotation, not the rail's rotation
            point.Transform.RotationEuler = new Vector3(pt.Rotate.X, pt.Rotate.Y, pt.Rotate.Z);

            // Use the point's own scale (default to 1,1,1 if not set)
            float scaleX = pt.Scale.X != 0 ? pt.Scale.X : 1.0f;
            float scaleY = pt.Scale.Y != 0 ? pt.Scale.Y : 1.0f;
            float scaleZ = pt.Scale.Z != 0 ? pt.Scale.Z : 1.0f;
            point.Transform.Scale = new Vector3(scaleX, scaleY, scaleZ);

            point.Transform.UpdateMatrix(true);

            // Draw Edit Data
            point.UINode.TagUI.UIDrawer = null;
            point.UINode.TagUI.UIDrawer += delegate
            {
                var mPoint = (point.UINode as RenderablePath.PointNode).Point;
                var points = mPoint.ParentPath.GetSelectedPoints();

                ImguiBinder.LoadProperties(mPoint.Transform, (sender, e) =>
                {
                    var handler = (ImguiBinder.PropertyChangedCustomArgs)e;
                    var type = mPoint.Transform.GetType().GetProperty(handler.Name);

                    List<IRevertable> revertables = new List<IRevertable>();
                    foreach (var pt in points)
                    {
                        var editTransform = pt.Transform;
                        revertables.Add(new TransformUndo(new TransformInfo(editTransform)));

                        type.SetValue(editTransform, sender);
                        editTransform.UpdateMatrix(true);
                    }
                    GLContext.ActiveContext.Scene.AddToUndo(revertables);
                    GLContext.ActiveContext.UpdateViewport = true;
                });

                if (ImGui.CollapsingHeader($"{TranslationSource.GetText("CONTROL_POINTS")}", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    bool updated = false;

                    ImGui.Columns(2);

                    ImGui.Text($"Has Control Points");
                    ImGui.NextColumn();

                    float colwidth = ImGui.GetColumnWidth();
                    ImGui.SetColumnOffset(1, ImGui.GetWindowWidth() * 0.25f);
                    ImGui.PushItemWidth(colwidth);

                    bool hasctpoints = pt.hasControlPoints;
                    if (ImGui.Checkbox("##HashControlPoints", ref hasctpoints))
                    {
                        pt.hasControlPoints = hasctpoints;
                    }
                    ImGui.PopItemWidth();
                    ImGui.NextColumn();

                    ImGui.Text($"Point 1");
                    ImGui.NextColumn();

                    ImGui.SetColumnOffset(1, ImGui.GetWindowWidth() * 0.25f);
                    ImGui.PushItemWidth(colwidth);

                    updated |= ImGuiHelper.InputTKVector3($"##point1", mPoint.ControlPoint1.Transform, "Position");
                    ImGui.PopItemWidth();
                    ImGui.NextColumn();

                    ImGui.Text($"Point 2");
                    ImGui.NextColumn();

                    ImGui.SetColumnOffset(1, ImGui.GetWindowWidth() * 0.25f);
                    ImGui.PushItemWidth(colwidth);
                    updated |= ImGuiHelper.InputTKVector3($"##point2", mPoint.ControlPoint2.Transform, "Position");
                    ImGui.PopItemWidth();
                    ImGui.NextColumn();

                    ImGui.Columns(1);

                    if (updated)
                    {
                        point.ControlPoint1.Transform.UpdateMatrix(true);
                        point.ControlPoint2.Transform.UpdateMatrix(true);
                        GLContext.ActiveContext.UpdateViewport = true;
                    }
                }

                if (ImGui.CollapsingHeader($"Properties", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    int numColumns = 2;
                    float colwidth = 0.0f;

                    ImGui.BeginColumns("##" + "Properties" + numColumns.ToString(), numColumns);

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Hash");
                    ImGui.NextColumn();

                    ulong inputValue = pt.Hash;
                    ulong* ulongptr = &inputValue;

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.InputScalar($"###PathPointHashBox", ImGuiDataType.U64, (IntPtr)ulongptr))
                    {
                        pt.Hash = (ulong)inputValue;
                    }

                    ImGui.NextColumn();

                    ImGui.PopItemWidth();
                    ImGui.EndColumns();
                }

                if (ImGui.CollapsingHeader($"Tower Control Properties", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    int numColumns = 2;
                    float colwidth = 0.0f;

                    ImGui.BeginColumns("##" + "TCProperties" + numColumns.ToString(), numColumns);

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Checkpoint HP");
                    ImGui.NextColumn();

                    int inputValue = pt.spl__GachiyaguraRailNodeParam.CheckPointHP;

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.InputInt("###CheckpointHP", ref inputValue))
                    {
                        pt.spl__GachiyaguraRailNodeParam.CheckPointHP = inputValue;
                    }

                    ImGui.NextColumn();

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Fill Up Type");
                    ImGui.NextColumn();

                    string inputString = pt.spl__GachiyaguraRailNodeParam.FillUpType;

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.InputText("###FillUpType", ref inputString, 0x1000))
                    {
                        pt.spl__GachiyaguraRailNodeParam.FillUpType = inputString;
                    }

                    ImGui.NextColumn();

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Position Offset");
                    ImGui.NextColumn();

                    System.Numerics.Vector3 inputOffset = new System.Numerics.Vector3(pt.spl__GachiyaguraRailNodeParam.PositionOffset.X * 10.0f,
                        pt.spl__GachiyaguraRailNodeParam.PositionOffset.Y * 10.0f, pt.spl__GachiyaguraRailNodeParam.PositionOffset.Z * 10.0f);

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.DragFloat3("###PositionOffset", ref inputOffset))
                    {
                        pt.spl__GachiyaguraRailNodeParam.PositionOffset = new ByamlVector3F(inputOffset.X / 10.0f, inputOffset.Y / 10.0f, inputOffset.Z / 10.0f);
                    }

                    ImGui.NextColumn();

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Rotation Degree");
                    ImGui.NextColumn();

                    inputOffset = new System.Numerics.Vector3(pt.spl__GachiyaguraRailNodeParam.RotationDeg.X,
                        pt.spl__GachiyaguraRailNodeParam.RotationDeg.Y, pt.spl__GachiyaguraRailNodeParam.RotationDeg.Z);

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.DragFloat3("###RotationDeg", ref inputOffset))
                    {
                        pt.spl__GachiyaguraRailNodeParam.RotationDeg = new ByamlVector3F(inputOffset.X, inputOffset.Y, inputOffset.Z);
                    }

                    ImGui.NextColumn();

                    ImGui.PopItemWidth();
                    ImGui.EndColumns();
                }

                if (ImGui.CollapsingHeader($"Rail Point Properties", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    int numColumns = 2;
                    float colwidth = 0.0f;

                    ImGui.BeginColumns("##" + "RPProperties" + numColumns.ToString(), numColumns);

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Break Time");
                    ImGui.NextColumn();

                    float inputValue = pt.game__LiftGraphRailNodeParam.BreakTime;

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.InputFloat("###RPBreakTime", ref inputValue))
                    {
                        pt.game__LiftGraphRailNodeParam.BreakTime = inputValue;
                    }

                    ImGui.NextColumn();

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Rotation");
                    ImGui.NextColumn();

                    System.Numerics.Vector3 inputRotation = new System.Numerics.Vector3(pt.game__LiftGraphRailNodeParam.Rotation.X,
                        pt.game__LiftGraphRailNodeParam.Rotation.Y, pt.game__LiftGraphRailNodeParam.Rotation.Z);

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.DragFloat3("###RPRotation", ref inputRotation))
                    {
                        pt.game__LiftGraphRailNodeParam.Rotation = new ByamlVector3F(inputRotation.X, inputRotation.Y, inputRotation.Z);
                    }

                    ImGui.NextColumn();

                    ImGui.PopItemWidth();
                    ImGui.EndColumns();
                }
            };

            return point;
        }

        public List<NodeBase> GetSelected()
        {
            var selected = new List<NodeBase>();
            foreach (var groupNode in this.Root.Children)
            {
                if (groupNode.IsSelected)
                    selected.Add(groupNode);

                foreach (var pointNode in groupNode.Children)
                {
                    if (pointNode.IsSelected)
                        selected.Add(pointNode);
                }
            }

            return selected;
        }

        /// <summary>
        /// Collects all existing hashes and instance IDs from all rails in the scene.
        /// </summary>
        private (HashSet<ulong> hashes, HashSet<string> instanceIDs) CollectAllHashesAndInstanceIDs()
        {
            HashSet<ulong> allHashes = new HashSet<ulong>();
            HashSet<string> allInstanceIDs = new HashSet<string>();

            foreach (IDrawable obj in MapEditor.Scene.Objects)
            {
                if (obj is RenderablePath && ((RenderablePath)obj).UINode.Tag is MuRail)
                {
                    var rail = (MuRail)((RenderablePath)obj).UINode.Tag;
                    if (rail.InstanceID != null)
                        allInstanceIDs.Add(rail.InstanceID);
                    allHashes.Add(rail.Hash);

                    foreach (MuRailPoint point in rail.Points)
                    {
                        allHashes.Add(point.Hash);
                    }
                }
            }

            return (allHashes, allInstanceIDs);
        }

        /// <summary>
        /// Initializes point hashes for a newly created path.
        /// Call this after CreateLinearStandard/CreateBezierStandard/CreateBezierCircle.
        /// </summary>
        private void InitializePointHashes(RenderablePath path)
        {
            var rail = path.UINode.Tag as MuRail;
            if (rail == null) return;

            var (allHashes, _) = CollectAllHashesAndInstanceIDs();

            // Clear existing points and rebuild from renderer
            rail.Points.Clear();

            foreach (var renderPoint in path.PathPoints)
            {
                // Generate unique hash for this point
                ulong pointHash = GenHash();
                while (allHashes.Contains(pointHash))
                    pointHash = GenHash();
                allHashes.Add(pointHash);

                // Create MuRailPoint with proper hash and position
                var muPoint = new MuRailPoint
                {
                    Hash = pointHash,
                    Translate = new ByamlVector3F(
                        renderPoint.Transform.Position.X / 10.0f,
                        renderPoint.Transform.Position.Y / 10.0f,
                        renderPoint.Transform.Position.Z / 10.0f),
                    Rotate = new ByamlVector3F(
                        renderPoint.Transform.Rotation.X,
                        renderPoint.Transform.Rotation.Y,
                        renderPoint.Transform.Rotation.Z)
                };

                // Handle bezier control points
                if (path.InterpolationMode == RenderablePath.Interpolation.Bezier)
                {
                    muPoint.Control0 = new ByamlVector3F(
                        renderPoint.ControlPoint1.Transform.Position.X / 10.0f,
                        renderPoint.ControlPoint1.Transform.Position.Y / 10.0f,
                        renderPoint.ControlPoint1.Transform.Position.Z / 10.0f);
                    muPoint.Control1 = new ByamlVector3F(
                        renderPoint.ControlPoint2.Transform.Position.X / 10.0f,
                        renderPoint.ControlPoint2.Transform.Position.Y / 10.0f,
                        renderPoint.ControlPoint2.Transform.Position.Z / 10.0f);
                }

                rail.Points.Add(muPoint);
                renderPoint.UINode.Tag = muPoint;

                // Setup UI drawer for the point (same as CreatePathPoint does)
                SetupPointUIDrawer(renderPoint, muPoint);
            }
        }

        /// <summary>
        /// Sets up the UI drawer for a path point. Extracted from CreatePathPoint for reuse.
        /// </summary>
        unsafe private void SetupPointUIDrawer(RenderablePathPoint point, MuRailPoint pt)
        {
            point.UINode.TagUI.UIDrawer = null;
            point.UINode.TagUI.UIDrawer += delegate
            {
                var mPoint = (point.UINode as RenderablePath.PointNode).Point;
                var points = mPoint.ParentPath.GetSelectedPoints();

                ImguiBinder.LoadProperties(mPoint.Transform, (sender, e) =>
                {
                    var handler = (ImguiBinder.PropertyChangedCustomArgs)e;
                    var type = mPoint.Transform.GetType().GetProperty(handler.Name);

                    List<IRevertable> revertables = new List<IRevertable>();
                    foreach (var selPt in points)
                    {
                        var editTransform = selPt.Transform;
                        revertables.Add(new TransformUndo(new TransformInfo(editTransform)));

                        type.SetValue(editTransform, sender);
                        editTransform.UpdateMatrix(true);
                    }
                    GLContext.ActiveContext.Scene.AddToUndo(revertables);
                    GLContext.ActiveContext.UpdateViewport = true;
                });

                if (ImGui.CollapsingHeader($"{TranslationSource.GetText("CONTROL_POINTS")}", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    bool updated = false;

                    ImGui.Columns(2);

                    ImGui.Text($"Has Control Points");
                    ImGui.NextColumn();

                    float colwidth = ImGui.GetColumnWidth();
                    ImGui.SetColumnOffset(1, ImGui.GetWindowWidth() * 0.25f);
                    ImGui.PushItemWidth(colwidth);

                    bool hasctpoints = pt.hasControlPoints;
                    if (ImGui.Checkbox("##HashControlPoints", ref hasctpoints))
                    {
                        pt.hasControlPoints = hasctpoints;
                    }
                    ImGui.PopItemWidth();
                    ImGui.NextColumn();

                    ImGui.Text($"Point 1");
                    ImGui.NextColumn();

                    ImGui.SetColumnOffset(1, ImGui.GetWindowWidth() * 0.25f);
                    ImGui.PushItemWidth(colwidth);

                    updated |= ImGuiHelper.InputTKVector3($"##point1", mPoint.ControlPoint1.Transform, "Position");
                    ImGui.PopItemWidth();
                    ImGui.NextColumn();

                    ImGui.Text($"Point 2");
                    ImGui.NextColumn();

                    ImGui.SetColumnOffset(1, ImGui.GetWindowWidth() * 0.25f);
                    ImGui.PushItemWidth(colwidth);
                    updated |= ImGuiHelper.InputTKVector3($"##point2", mPoint.ControlPoint2.Transform, "Position");
                    ImGui.PopItemWidth();
                    ImGui.NextColumn();

                    ImGui.Columns(1);

                    if (updated)
                    {
                        point.ControlPoint1.Transform.UpdateMatrix(true);
                        point.ControlPoint2.Transform.UpdateMatrix(true);
                        GLContext.ActiveContext.UpdateViewport = true;
                    }
                }

                if (ImGui.CollapsingHeader($"Properties", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    int numColumns = 2;
                    float colwidth = 0.0f;

                    ImGui.BeginColumns("##" + "Properties" + numColumns.ToString(), numColumns);

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Hash");
                    ImGui.NextColumn();

                    ulong inputValue = pt.Hash;
                    ulong* ulongptr = &inputValue;

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.InputScalar($"###PathPointHashBox", ImGuiDataType.U64, (IntPtr)ulongptr))
                    {
                        pt.Hash = (ulong)inputValue;
                    }

                    ImGui.NextColumn();

                    ImGui.PopItemWidth();
                    ImGui.EndColumns();
                }

                if (ImGui.CollapsingHeader($"Tower Control Properties", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    int numColumns = 2;
                    float colwidth = 0.0f;

                    ImGui.BeginColumns("##" + "TCProperties" + numColumns.ToString(), numColumns);

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Checkpoint HP");
                    ImGui.NextColumn();

                    int inputValueInt = pt.spl__GachiyaguraRailNodeParam.CheckPointHP;

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.InputInt("###CheckpointHP", ref inputValueInt))
                    {
                        pt.spl__GachiyaguraRailNodeParam.CheckPointHP = inputValueInt;
                    }

                    ImGui.NextColumn();

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Fill Up Type");
                    ImGui.NextColumn();

                    string inputString = pt.spl__GachiyaguraRailNodeParam.FillUpType;

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.InputText("###FillUpType", ref inputString, 0x1000))
                    {
                        pt.spl__GachiyaguraRailNodeParam.FillUpType = inputString;
                    }

                    ImGui.NextColumn();

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Position Offset");
                    ImGui.NextColumn();

                    System.Numerics.Vector3 inputOffset = new System.Numerics.Vector3(pt.spl__GachiyaguraRailNodeParam.PositionOffset.X * 10.0f,
                        pt.spl__GachiyaguraRailNodeParam.PositionOffset.Y * 10.0f, pt.spl__GachiyaguraRailNodeParam.PositionOffset.Z * 10.0f);

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.DragFloat3("###PositionOffset", ref inputOffset))
                    {
                        pt.spl__GachiyaguraRailNodeParam.PositionOffset = new ByamlVector3F(inputOffset.X / 10.0f, inputOffset.Y / 10.0f, inputOffset.Z / 10.0f);
                    }

                    ImGui.NextColumn();

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Rotation Degree");
                    ImGui.NextColumn();

                    inputOffset = new System.Numerics.Vector3(pt.spl__GachiyaguraRailNodeParam.RotationDeg.X,
                        pt.spl__GachiyaguraRailNodeParam.RotationDeg.Y, pt.spl__GachiyaguraRailNodeParam.RotationDeg.Z);

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.DragFloat3("###RotationDeg", ref inputOffset))
                    {
                        pt.spl__GachiyaguraRailNodeParam.RotationDeg = new ByamlVector3F(inputOffset.X, inputOffset.Y, inputOffset.Z);
                    }

                    ImGui.NextColumn();

                    ImGui.PopItemWidth();
                    ImGui.EndColumns();
                }

                if (ImGui.CollapsingHeader($"Rail Point Properties", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    int numColumns = 2;
                    float colwidth = 0.0f;

                    ImGui.BeginColumns("##" + "RPProperties" + numColumns.ToString(), numColumns);

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Break Time");
                    ImGui.NextColumn();

                    float inputValueFloat = pt.game__LiftGraphRailNodeParam.BreakTime;

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.InputFloat("###RPBreakTime", ref inputValueFloat))
                    {
                        pt.game__LiftGraphRailNodeParam.BreakTime = inputValueFloat;
                    }

                    ImGui.NextColumn();

                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("Rotation");
                    ImGui.NextColumn();

                    System.Numerics.Vector3 inputRotation = new System.Numerics.Vector3(pt.game__LiftGraphRailNodeParam.Rotation.X,
                        pt.game__LiftGraphRailNodeParam.Rotation.Y, pt.game__LiftGraphRailNodeParam.Rotation.Z);

                    colwidth = ImGui.GetColumnWidth();
                    ImGui.PushItemWidth(colwidth - 6);

                    if (ImGui.DragFloat3("###RPRotation", ref inputRotation))
                    {
                        pt.game__LiftGraphRailNodeParam.Rotation = new ByamlVector3F(inputRotation.X, inputRotation.Y, inputRotation.Z);
                    }

                    ImGui.NextColumn();

                    ImGui.PopItemWidth();
                    ImGui.EndColumns();
                }
            };
        }

        private ulong GenHash()
        {
            System.Random random = new System.Random();

            // Generate Hash
            uint thirtyBits = (uint)random.Next(1 << 30);
            uint twoBits = (uint)random.Next(1 << 2);
            ulong sixtyBits = (uint)random.Next(1 << 30);
            ulong sixtytwoBits = (uint)random.Next(1 << 2);

            ulong Hash = (ulong)(twoBits | (ulong)thirtyBits << 2 | sixtyBits << 32 | sixtytwoBits << 62);

            return Hash;
        }

        private string GenInstanceID()
        {
            System.Random random = new System.Random();
            string InstanceID = "";

            for (int i = 0; i < 8; i++)
            {
                InstanceID += UInt32ToHashString((uint)random.Next(0, 16));
            }
            InstanceID += "-";

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                    InstanceID += UInt32ToHashString((uint)random.Next(0, 16));

                InstanceID += "-";
            }

            for (int i = 0; i < 12; i++)
            {
                InstanceID += UInt32ToHashString((uint)random.Next(0, 16));
            }

            return InstanceID;
        }

        private string UInt32ToHashString(uint Input)
        {
            string ret = "";

            if (Input == 0)
            {
                return "0";
            }

            while (Input > 0)
            {
                switch (Input % 16)
                {
                    case 0:
                        ret += "0";
                        break;

                    case 1:
                        ret += "1";
                        break;

                    case 2:
                        ret += "2";
                        break;

                    case 3:
                        ret += "3";
                        break;

                    case 4:
                        ret += "4";
                        break;

                    case 5:
                        ret += "5";
                        break;

                    case 6:
                        ret += "6";
                        break;

                    case 7:
                        ret += "7";
                        break;

                    case 8:
                        ret += "8";
                        break;

                    case 9:
                        ret += "9";
                        break;

                    case 10:
                        ret += "a";
                        break;

                    case 11:
                        ret += "b";
                        break;

                    case 12:
                        ret += "c";
                        break;

                    case 13:
                        ret += "d";
                        break;

                    case 14:
                        ret += "e";
                        break;

                    case 15:
                        ret += "f";
                        break;
                }

                Input /= 16;
            }

            return ret;
        }

        public void CreateNewPathFromScratch()
        {
            List<ulong> mAllHash = new List<ulong>();
            List<string> mAllInstanceID = new List<string>();

            foreach (IDrawable obj in MapEditor.Scene.Objects)
            {
                if (obj is RenderablePath && ((RenderablePath)obj).UINode.Tag is MuRail)
                {
                    mAllInstanceID.Add(((MuRail)((RenderablePath)obj).UINode.Tag).InstanceID);
                    mAllHash.Add(((MuRail)((RenderablePath)obj).UINode.Tag).Hash);

                    foreach (MuRailPoint Point in ((MuRail)((RenderablePath)obj).UINode.Tag).Points)
                    {
                        mAllHash.Add(Point.Hash);
                    }
                }
            }

            string InstanceID = GenInstanceID();
            while (mAllInstanceID.Contains(InstanceID))
                InstanceID = GenInstanceID();

            mAllInstanceID.Add(InstanceID);

            ulong Hash = GenHash();
            while (mAllHash.Contains(Hash))
                Hash = GenHash();

            mAllHash.Add(Hash);

            ulong HashPoint1 = GenHash();
            while (mAllHash.Contains(HashPoint1))
                HashPoint1 = GenHash();

            ulong HashPoint2 = GenHash();
            while (mAllHash.Contains(HashPoint2))
                HashPoint2 = GenHash();

            MuRail newRail = new MuRail();
            newRail.InstanceID = InstanceID;
            newRail.Hash = Hash;

            newRail.Points.Add(new MuRailPoint() { Hash = HashPoint1, Translate = new ByamlVector3F(0.0f, 50.0f, 0.0f) });
            newRail.Points.Add(new MuRailPoint() { Hash = HashPoint2, Translate = new ByamlVector3F(0.0f, -50.0f, 0.0f) });

            ReloadPath(newRail);
        }

        public void InsertNewPathNodeInkRail(RenderablePath renderer, MuRail rail)
        {
            List<ulong> mAllHash = new List<ulong>();
            List<string> mAllInstanceID = new List<string>();

            foreach (IDrawable obj in MapEditor.Scene.Objects)
            {
                if (obj is RenderablePath && ((RenderablePath)obj).UINode.Tag is MuRail)
                {
                    mAllInstanceID.Add(((MuRail)((RenderablePath)obj).UINode.Tag).InstanceID);
                    mAllHash.Add(((MuRail)((RenderablePath)obj).UINode.Tag).Hash);

                    foreach (MuRailPoint Point in ((MuRail)((RenderablePath)obj).UINode.Tag).Points)
                    {
                        mAllHash.Add(Point.Hash);
                    }
                }
            }

            ulong Hash = GenHash();
            while (mAllHash.Contains(Hash))
                Hash = GenHash();

            MuRailPoint pt = new MuRailPoint()
            {
                Hash = Hash,
                Translate = new ByamlVector3F(renderer.Transform.Position.X / 10.0f, renderer.Transform.Position.Y / 10.0f,
                renderer.Transform.Position.Z / 10.0f)
            };

            rail.Points.Add(pt);

            RenderablePathPoint point = CreatePathPoint(renderer, pt);

            //Add the point to the list
            renderer.AddPoint(point);

            //Set the tag information of the node
            point.UINode.Tag = pt;

            // Connect each point from the previous point
            if (renderer.PathPoints.Count > 0)
                renderer.PathPoints[renderer.PathPoints.Count - 1].AddChild(point);
        }

        private void RemovePathNodeInkRail(RenderablePath renderer, MuRail rail, int Index)
        {
            // Special case 1: index is 0 (need to define new head)
            if (Index == 0)
            {
                renderer.PathPoints[0].RemoveChild(renderer.PathPoints[1]);
                renderer.PathPoints.RemoveAt(0);
                renderer.UINode.Children.RemoveAt(0);

                rail.Points.RemoveAt(0);
            }
            // Special case 2: index is the last one (need to delete the tail)
            else if (Index == renderer.PathPoints.Count - 1)
            {
                renderer.PathPoints[renderer.PathPoints.Count - 2].RemoveChild(renderer.PathPoints[renderer.PathPoints.Count - 1]);
                renderer.PathPoints.RemoveAt(renderer.PathPoints.Count - 1);
                renderer.UINode.Children.RemoveAt(renderer.PathPoints.Count - 1);

                rail.Points.RemoveAt(renderer.PathPoints.Count - 1);
            }
            // Case 3: index is in the list
            else
            {
                RenderablePathPoint prev = renderer.PathPoints[Index - 1];
                RenderablePathPoint next = renderer.PathPoints[Index + 1];

                prev.RemoveChild(renderer.PathPoints[Index]);
                renderer.PathPoints[Index].RemoveChild(next);
                prev.AddChild(next);

                renderer.PathPoints.RemoveAt(Index);
                renderer.UINode.Children.RemoveAt(Index);

                rail.Points.RemoveAt(Index);
            }
        }

        void EditTransformObjectSelector(RenderablePath renderer, MuRail rail, int Index)
        {
            var selected = ObjectEditor.Root.Children;
            if (selected.Count == 0)
                return;

            ObjectDataSelector selector = new ObjectDataSelector(selected);
            MapStudio.UI.DialogHandler.Show(TranslationSource.GetText("SELECT_OBJECT"), 400, 800, () =>
            {
                selector.Render();
            }, (result) =>
            {
                if (!result)
                    return;

                rail.Points[Index].Translate = new ByamlVector3F(
                    selector.GetTranslate().X / 10.0f,
                    selector.GetTranslate().Y / 10.0f,
                    selector.GetTranslate().Z / 10.0f);

                selector.SetSelectedObjectToUsed();
                rail.Points[Index].HasBeenEditedWithSelector = true;

                renderer.PathPoints[Index].Transform.Position = new Vector3(selector.GetTranslate().X, selector.GetTranslate().Y, selector.GetTranslate().Z);
            });
        }

        private void CreateLinearPath()
        {
            RenderablePath path = new RenderablePath();
            this.Add(path);
            path.CreateLinearStandard(100);
            ((MuRail)path.UINode.Tag).IsClosed = false;
            InitializePointHashes(path);
            PrepareObject(path);

            if (!Root.IsChecked)
                Root.IsChecked = true;
        }


        private void CreateBezierPathCircle()
        {
            RenderablePath path = new RenderablePath();
            this.Add(path);
            path.CreateBezierCircle(20);
            ((MuRail)path.UINode.Tag).IsClosed = true;
            InitializePointHashes(path);
            PrepareObject(path);

            if (!Root.IsChecked)
                Root.IsChecked = true;
        }



        private void CreateBezierPathStandard()
        {
            RenderablePath path = new RenderablePath();
            this.Add(path);
            path.CreateBezierStandard(20);
            ((MuRail)path.UINode.Tag).IsClosed = false;
            InitializePointHashes(path);
            PrepareObject(path);

            if (!Root.IsChecked)
                Root.IsChecked = true;
        }

        private void PrepareObject(RenderablePath path)
        {
            var context = GLContext.ActiveContext;
            context.Scene.DeselectAll(context);

            path.Translate(context.Camera.GetViewPostion());
            path.IsSelected = true;
        }



        public void Add(RenderablePath path, bool undo = false)
        {
            //Add the tree node of the path
            PreparePath(path);
            MapEditor.AddRender(path, undo);
        }

        public void Remove(RenderablePath path)
        {
            MapEditor.RemoveRender(path);
        }

        public void Remove(MuRail rail) // path)
        {
            var render = this.Renderers.FirstOrDefault(x => ((RenderablePath)x).UINode.Tag == rail);
            if (render != null)
                MapEditor.RemoveRender(render);
        }

        List<IDrawable> copied = new List<IDrawable>();

        public void CopySelected()
        {
            var selected = Renderers.Where(x => ((RenderablePath)x).IsSelected).ToList();

            copied.Clear();
            copied = selected;
        }

        public void PasteSelected()
        {
            GLContext.ActiveContext.Scene.BeginUndoCollection();
            GLContext.ActiveContext.Scene.DeselectAll(GLContext.ActiveContext);

            foreach (RenderablePath originalRail in copied)
            {
                MuRail originalRailData = originalRail.UINode.Tag as MuRail;

                // Sync current renderer positions back to MuRailPoint data before cloning
                for (int i = 0; i < originalRail.PathPoints.Count && i < originalRailData.Points.Count; i++)
                {
                    var renderPoint = originalRail.PathPoints[i];
                    var pointData = originalRailData.Points[i];

                    // Update position (divide by 10 to convert from render space)
                    pointData.Translate = new ByamlVector3F(
                        renderPoint.Transform.Position.X / 10.0f,
                        renderPoint.Transform.Position.Y / 10.0f,
                        renderPoint.Transform.Position.Z / 10.0f);

                    // Update rotation
                    pointData.Rotate = new ByamlVector3F(
                        renderPoint.Transform.Rotation.X,
                        renderPoint.Transform.Rotation.Y,
                        renderPoint.Transform.Rotation.Z);

                    // Update scale (only if not default 1,1,1)
                    if (renderPoint.Transform.Scale != Vector3.One)
                    {
                        pointData.Scale = new ByamlVector3F(
                            renderPoint.Transform.Scale.X,
                            renderPoint.Transform.Scale.Y,
                            renderPoint.Transform.Scale.Z);
                    }

                    // Update control points for bezier curves
                    if (originalRail.InterpolationMode == RenderablePath.Interpolation.Bezier)
                    {
                        pointData.Control0 = new ByamlVector3F(
                            renderPoint.ControlPoint1.Transform.Position.X / 10.0f,
                            renderPoint.ControlPoint1.Transform.Position.Y / 10.0f,
                            renderPoint.ControlPoint1.Transform.Position.Z / 10.0f);

                        pointData.Control1 = new ByamlVector3F(
                            renderPoint.ControlPoint2.Transform.Position.X / 10.0f,
                            renderPoint.ControlPoint2.Transform.Position.Y / 10.0f,
                            renderPoint.ControlPoint2.Transform.Position.Z / 10.0f);
                    }
                }

                List<ulong> mAllHash = new List<ulong>();
                List<string> mAllInstanceID = new List<string>();

                foreach (IDrawable obj in MapEditor.Scene.Objects)
                {
                    if (obj is RenderablePath && ((RenderablePath)obj).UINode.Tag is MuRail)
                    {
                        mAllInstanceID.Add(((MuRail)((RenderablePath)obj).UINode.Tag).InstanceID);
                        mAllHash.Add(((MuRail)((RenderablePath)obj).UINode.Tag).Hash);

                        foreach (MuRailPoint Point in ((MuRail)((RenderablePath)obj).UINode.Tag).Points)
                        {
                            mAllHash.Add(Point.Hash);
                        }
                    }
                }

                string InstanceID = GenInstanceID();
                while (mAllInstanceID.Contains(InstanceID))
                    InstanceID = GenInstanceID();

                mAllInstanceID.Add(InstanceID);

                ulong Hash = GenHash();
                while (mAllHash.Contains(Hash))
                    Hash = GenHash();

                mAllHash.Add(Hash);

                MuRail newRail = originalRailData.Clone();
                newRail.Hash = Hash;
                newRail.InstanceID = InstanceID;

                for (int i = 0; i < newRail.Points.Count; i++)
                {
                    ulong HashPoint1 = GenHash();
                    while (mAllHash.Contains(HashPoint1))
                        HashPoint1 = GenHash();

                    mAllHash.Add(HashPoint1);

                    newRail.Points[i].Hash = HashPoint1;
                }

                ReloadPath(newRail);
            }

            GLContext.ActiveContext.Scene.EndUndoCollection();
        }



        public void RemoveSelected()
        {
            GLContext.ActiveContext.Scene.BeginUndoCollection();

            List<RenderablePath> removedPaths = new List<RenderablePath>();
            foreach (RenderablePath path in Renderers)
            {
                if (path.EditMode && path.PathPoints.Any(x => x.IsSelected))
                    path.RemoveSelected();
                else if (path.IsSelected)
                    removedPaths.Add(path);
            }

            if (removedPaths.Count > 0)
            {
                GLContext.ActiveContext.Scene.AddToUndo(
                    new EditableObjectDeletedUndo(GLContext.ActiveContext.Scene, removedPaths));
            }

            foreach (var path in removedPaths)
                Remove(path);

            GLContext.ActiveContext.Scene.EndUndoCollection();
        }

        public void OnMouseDown(MouseEventInfo mouseInfo) { }
        public void OnMouseUp(MouseEventInfo mouseInfo) { }
        public void OnMouseMove(MouseEventInfo mouseInfo) { }

        public void OnKeyDown(KeyEventInfo keyInfo)
        {
            if (keyInfo.IsKeyDown(InputSettings.INPUT.Scene.Delete))
                RemoveSelected();
            if (keyInfo.IsKeyDown(InputSettings.INPUT.Scene.Copy) && GetSelected().Count > 0)
                CopySelected();
            if (keyInfo.IsKeyDown(InputSettings.INPUT.Scene.Paste))
                PasteSelected();
        }
    }
}
