using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;

//using BfresLibrary;
//using BfresLibrary.GX2;
//using BfresLibrary.Helpers;

using Syroot.NintenTools.NSW.Bfres;
using Syroot.NintenTools.NSW.Bfres.GX2;
using Syroot.NintenTools.NSW.Bfres.Helpers;

using Toolbox.Core;
using OpenTK;
using MapStudio.UI;
using CafeLibrary.ModelConversion;
using Toolbox.Core.ViewModels;
using Toolbox.Core.IO;
using CafeLibrary.Rendering;
using GLFrameworkEngine;
using OpenTK.Graphics.OpenGL;

namespace CafeLibrary
{
    public class ModelFolder : NodeBase
    {
        public override string Header => "Models";

        public ResFile ResFile { get; set; }

        /// <summary>
        /// The target model to make edits for.
        /// </summary>
        public FMDL TargtModel { get; set; }

        /// <summary>
        /// The model list.
        /// </summary>
        public List<FMDL> Models = new List<FMDL>();

        private BFRES BfresWrapper;

        public ModelFolder(BFRES bfres, ResFile resFile)
        {
            BfresWrapper = bfres;
            ResFile = resFile;

            ContextMenus.Add(new MenuItemModel("Import", ImportNewModelDialog));

            //Import config
            this.Tag = this;
 
            foreach (var model in ResFile.Models) {
                FMDL fmdl = new FMDL(bfres, resFile, model);
                this.AddChild(fmdl.UINode);
                Models.Add(fmdl);
            }
            TargtModel = Models.FirstOrDefault();
        }

        public void OnSave()
        {
            ResFile.Models.Clear();

            foreach (FMDL fmdl in Models) {
                ResFile.Models.Add(fmdl.Model);

                foreach (FSHP fshp in fmdl.Meshes) {
                    fshp.Shape.VertexBufferIndex = (ushort)fmdl.Model.VertexBuffers.IndexOf(fshp.VertexBuffer);
                    if (!fmdl.Model.Materials.Contains(fshp.Material.Material))
                        fshp.Shape.MaterialIndex = 0;
                    else
                        fshp.Shape.MaterialIndex = (ushort)fmdl.Model.Materials.IndexOf(fshp.Material.Material);
                }
            }
        }

        public void CheckErrors()
        {
            foreach (FMDL fmdl in Models)
                fmdl.CheckErrors();
        }

        private void ImportNewModelDialog()
        {
            var dlg = new ImguiFileDialog();
            dlg.SaveDialog = false;
            dlg.AddFilter(".bfmdl", ".bfmdl");
            dlg.AddFilter(".dae", ".dae");
            dlg.AddFilter(".fbx", ".fbx");
        }

        public void RemoveModel(FMDL fmdl)
        {
            int result = TinyFileDialog.MessageBoxInfoYesNo(string.Format("Are you sure you want to remove {0}? Operation cannot be undone.", fmdl.Model.Name));
            if (result != 1)
                return;

            //Remove from UI
            Children.Remove(fmdl.UINode);
            Models.Remove(fmdl);
            //Remove from bfres data
            ResFile.Models.Remove(fmdl.Model);
            //Remove renderer
            fmdl.ModelRenderer?.Dispose();
            BfresWrapper.Renderer.Models.Remove(fmdl.ModelRenderer);
            //remove target model
            if (TargtModel == fmdl)
            {
                if (Models.Count > 0)
                    TargtModel = Models.FirstOrDefault();
                else
                    TargtModel = null;
            }
            GLContext.ActiveContext.UpdateViewport = true;
        }
    }

    /// <summary>
    /// Represents a wrapper to edit UI, render and bfres model data
    /// </summary>
    public class FMDL : STGenericModel, IContextMenu, IRenamableNode, IPropertyUI
    {
        public Model Model { get; set; }
        public ResFile ResFile { get; set; }
        public BFRES BfresWrapper { get; set; }

        //UI Folders
        private readonly NodeBase SkeletonFolder = new NodeBase("Skeleton");
        private readonly NodeBase MeshFolder = new NodeBase("Meshes");
        private readonly NodeBase MaterialFolder = new NodeBase("Materials");
        //UI Node
        public NodeBase UINode;

        public override string Name 
        { 
            get => Model.Name;
            set
            {
                if (Model.Name != value)
                {
                    string name = Model.Name;

                    Model.Name = value;
                    UINode.Header = value;
                }
            }
        }

        public Type GetTypeUI() => typeof(BfresModelUI);

        public void OnLoadUI(object uiInstance) { }

        public FMDL ModelWrapper => this.UINode.Parent.Parent.Tag as FMDL;

        public void OnRenderUI(object uiInstance)
        {
            var editor = (BfresModelUI)uiInstance;
            editor.Render(this);
        }

        //
        public BfresModelRender ModelRenderer;

        public void Renamed(string text) {
            Model.Name = text;
        }

        public FMDL(BFRES bfres, ResFile resFile, Model model)
        {
            BfresWrapper = bfres;
            ResFile = resFile;
            Model = model;

            //UI nodes
            UINode = new NodeBase(model.Name);
            UINode.Tag = this;
            UINode.HasCheckBox = true;
            UINode.AddChild(MeshFolder);
            UINode.AddChild(MaterialFolder);
            UINode.AddChild(SkeletonFolder);

            SkeletonFolder.TagUI.UIDrawer += delegate
            {
            };

            MaterialFolder.ContextMenus.Add(new MenuItemModel("Add Material", AddMaterialDialog));

            if (ModelRenderer == null) {
                ModelRenderer = new BfresModelRender();
                BfresWrapper.Renderer.Models.Add(ModelRenderer);
            }
            ReloadModel();
        }
        
        public List<FSHP> GetSelectedMeshes() {
            return this.Meshes.Where(x => ((FSHP)x).UINode.IsSelected).Cast<FSHP>().ToList();
        }

        /// <summary>
        /// Gets all selected materials either by selected mesh or material UI node.
        /// </summary>
        public List<FMAT> GetSelectedMaterials()
        {
            List<FMAT> materials = new List<FMAT>();
            foreach (FSHP shape in this.Meshes)
            {
                if (shape.UINode.IsSelected && !materials.Contains(shape.Material))
                    materials.Add(shape.Material);
            }
            foreach (FMAT mat in this.Materials)
            {
                if (mat.UINode.IsSelected && !materials.Contains(mat))
                    materials.Add(mat);
            }
            return materials;
        }

        public void CheckErrors()
        {
            foreach (FMAT fmat in Materials)
                fmat.CheckErrors();
            foreach (FSHP fshp in Meshes)
                fshp.CheckErrors();
        }

        private void ReloadModel()
        {
            Materials.Clear();
            Meshes.Clear();
            MaterialFolder.Children.Clear();
            MeshFolder.Children.Clear();

            //Dispose any previous renderers
            ModelRenderer.Dispose();
            //Update the model data used for skeleton info
            ModelRenderer.ModelData = this;
            ModelRenderer.Meshes.Clear();

            //Hide caustics area
            if (Model.Name == "CausticsArea")
                ModelRenderer.IsVisible = false;

            //Skeletons, materials and meshes
            this.Skeleton = new FSKL(this, Model.Skeleton);

            if (ModelRenderer.SkeletonRenderer != null)
                ModelRenderer.SkeletonRenderer.Dispose();

            ModelRenderer.SkeletonRenderer = new SkeletonRenderer(this.Skeleton);

            foreach (var mat in Model.Materials) {
                var fmat = new FMAT(BfresWrapper, this, ResFile, mat);
                //Add to model
                Materials.Add(fmat);
                //Add UI
                MaterialFolder.AddChild(fmat.UINode);
            }
            foreach (var shape in Model.Shapes) {
                var fshp = new FSHP(ResFile, (FSKL)Skeleton, Model, shape, Materials);
                //Add to model
                Meshes.Add(fshp);
                //Add UI
                MeshFolder.AddChild(fshp.UINode);
                //Add to renderer
                fshp.MeshAsset = BfresLoader.AddMesh(ResFile, BfresWrapper.Renderer, ModelRenderer, Model, shape);
                fshp.SetupRender(fshp.MeshAsset);
            }

            SkeletonFolder.Children.Clear();
            foreach (var bone in ModelRenderer.SkeletonRenderer.Bones)
                if (bone.Parent == null)
                    SkeletonFolder.AddChild(bone.UINode);
        }

        public MenuItemModel[] GetContextMenuItems()
        {
            return new MenuItemModel[]
            {
                new MenuItemModel("Export", ExportDialog),
                new MenuItemModel("Replace", ReplaceDialog),
                new MenuItemModel(""),
                new MenuItemModel("Rename", () => UINode.ActivateRename = true),
                new MenuItemModel(""),
                new MenuItemModel("Add", AddModelDialog),
                new MenuItemModel(""),
                new MenuItemModel("Delete", () => {
                    BfresWrapper.ModelFolder.RemoveModel(this);
                }),
            };
        }

        private void AddMaterialDialog()
        {
            var dlg = new ImguiFileDialog();
            dlg.MultiSelect = true;
            dlg.AddFilter(".json", ".json");
            dlg.AddFilter(".bfmat", ".bfmat");
            dlg.AddFilter(".zip", ".zip");

            if (dlg.ShowDialog())
            {
                foreach (var file in dlg.FilePaths)
                    AddMaterialDialog(file);
            }
        }

        public void AddMaterialDialog(string filePath)
        {
            if (filePath.EndsWith(".zip"))
            {
                var fmat = new FMAT(BfresWrapper, this, ResFile, new Material());
                MaterialFolder.AddChild(fmat.UINode);

                fmat.Name = Path.GetFileNameWithoutExtension(filePath);
                fmat.LoadPreset(filePath, false);
                //Rename dupes
                fmat.Name = Utils.RenameDuplicateString(fmat.Name, this.Materials.Select(x => x.Name).ToList());
                Materials.Add(fmat);
                if (!this.Model.Materials.Any(x => x.Name == fmat.Name))
                    this.Model.Materials.Add(fmat.Material);
            }
            else
            {
                Material material = new Material();

                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    material.Import(stream);
                }

                var fmat = new FMAT(BfresWrapper, this, ResFile, material);
                MaterialFolder.AddChild(fmat.UINode);
                //Rename dupes
                fmat.Name = Utils.RenameDuplicateString(fmat.Name, this.Materials.Select(x => x.Name).ToList());
                Materials.Add(fmat);
                if (!this.Model.Materials.Any(x => x.Name == fmat.Name))
                    this.Model.Materials.Add(fmat.Material);
            }
        }

        private void ExportDialog()
        {
            var dlg = new ImguiFileDialog();
            dlg.SaveDialog = true;
            dlg.FileName = $"{this.Model.Name}.dae";
            dlg.AddFilter(".bfmdl", ".bfmdl");
            dlg.AddFilter(".dae", ".dae");

            if (dlg.ShowDialog())
            {
                if (dlg.FilePath.EndsWith(".bfmdl"))
                    Model.Export(dlg.FilePath, ResFile);
                else
                {
                    var scene = BfresModelExporter.FromGeneric(ResFile, Model);
                    IONET.IOManager.ExportScene(scene, dlg.FilePath, new IONET.ExportSettings() { });
                    BfresWrapper.ExportTextures(Path.GetDirectoryName(dlg.FilePath));
                }
            }
        }

        public void ReplaceDialog()
        {
            var dlg = new ImguiFileDialog();
            dlg.SaveDialog = false;
            dlg.AddFilter(".bfmdl", ".bfmdl");
            dlg.AddFilter(".dae", ".dae");
            dlg.AddFilter(".fbx", ".fbx");
        }

        private void AddModelDialog()
        {
            var dlg = new ImguiFileDialog();
            dlg.SaveDialog = false;
            dlg.AddFilter(".bfmdl", ".bfmdl");
            dlg.AddFilter(".dae", ".dae");
            dlg.AddFilter(".fbx", ".fbx");
        }

        public void RemoveMesh(FSHP fshp)
        {
            //Remove from UI and model
            Meshes.Remove(fshp);
            MeshFolder.Children.Remove(fshp.UINode);
            //Remove from bfres data
            Model.Shapes.Remove(fshp.Shape);
            Model.VertexBuffers.Remove(fshp.VertexBuffer);
            //Remove the renderer
            if (fshp.MeshAsset != null)
            {
                if (ModelRenderer.Meshes.Contains(fshp.MeshAsset))
                    ModelRenderer.Meshes.Remove(fshp.MeshAsset);

                fshp.MeshAsset?.Dispose();
            }
        }

        public void RemoveMaterial(FMAT mat)
        {
            mat.UINode.IsSelected = false;
            //Remove from UI and model
            Materials.Remove(mat);
            MaterialFolder.Children.Remove(mat.UINode);
            //Remove from bfres data
            Model.Materials.Remove(mat.Material);

            foreach (FSHP mesh in mat.GetMappedMeshes())
            {
                if (mesh.Material == mat) {
                    //Empty material
                    mesh.AssignMaterial(new FMAT(BfresWrapper, this, ResFile, new Material()));
                }
            }
        }
    }

    /// <summary>
    /// Represents a wrapper to edit UI, render and bfres material data
    /// </summary>
    public class FMAT : STGenericMaterial, IContextMenu, IPropertyUI, IDragDropNode, IRenamableNode
    {
        /// <summary>
        /// The bfres material data.
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// The bfres data.
        /// </summary>
        public ResFile ResFile { get; set; }

        /// <summary>
        /// The UI tree node.
        /// </summary>
        public NodeBase UINode { get; set; }

        /// <summary>
        /// The name of the material.
        /// </summary>
        public override string Name
        {
            get { return Material.Name; }
            set { 
                Material.Name = value;
                //Update UI too
                if (UINode.Header != value)
                    UINode.Header = value;
            }
        }

        /// <summary>
        /// The shader archive name.
        /// </summary>
        public string ShaderArchive
        {
            get
            {
                if (Material.ShaderAssign == null)
                    return "";
                return Material.ShaderAssign.ShaderArchiveName;
            }
        }

        /// <summary>
        /// The shader model name.
        /// </summary>
        public string ShaderModel
        {
            get
            {
                if (Material.ShaderAssign == null)
                    return "";
                return Material.ShaderAssign.ShadingModelName;
            }
        }

        public Dictionary<string, ShaderParam> ShaderParams { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, ShaderParam> AnimatedParams = new Dictionary<string, ShaderParam>();

        /// <summary>
        /// 
        /// </summary>
        public BfresMaterialRender MaterialAsset;
        
        public Dictionary<string, string> ShaderOptions { get; set; }

        public List<string> Samplers { get; set; }
        public Dictionary<string, string> AnimatedSamplers { get; set; }

        public ResDict SamplerAssign { get; set; }

        public GLMaterialBlendState BlendState { get; set; } = new GLMaterialBlendState();

        /// <summary>
        /// Checks if the material is valid or not. 
        /// Determines based on having empty render data or not.
        /// </summary>
        public bool IsMaterialInvalid => Material != null && Material.RenderInfos.Count == 0;

        public string ShaderArchiveName { get; set; }
        public string ShaderModelName { get; set; }

        public CullMode CullState
        {
            get
            {
                if (CullFront && CullBack) return CullMode.Both;
                else if (CullBack) return CullMode.Back;
                else if (CullFront) return CullMode.Front;
                else
                    return CullMode.None;
            }
            set
            {
                CullBack = false;
                CullFront = false;

                if (value == CullMode.Both)
                {
                    CullBack = true;
                    CullFront = true;
                }
                else if (value == CullMode.Front)
                    CullFront = true;
                else if (value == CullMode.Back)
                    CullBack = true;
            }
        }

        public enum CullMode
        {
            None,
            Front,
            Back,
            Both,
        }

        public bool CullFront = false;
        public bool CullBack = true;

        public bool IsTransparent { get; set; }

        public void Renamed(string text) {
            Material.Name = text;
        }

        BFRES BfresWrapper;

        public Dictionary<string, GenericRenderer.TextureView> GetTextures() => BfresWrapper.Renderer.Textures;

        public FMDL ModelWrapper { get; private set; }

        public FMAT() { }

        public FMAT(BFRES bfres, FMDL fmdl, ResFile resFile, Material mat)
        {
            ModelWrapper = fmdl;
            BfresWrapper = bfres;
            ResFile = resFile;
            Material = mat;
            UINode = new NodeBase(mat.Name);
            UINode.Tag = this;
            UINode.Icon = $"material_{this.Name}";
            UINode.IconDrawer += delegate
            {
                DrawIcon();
            };
            UINode.OnSelected += delegate
            {
                foreach (FSHP mesh in this.GetMappedMeshes())
                    if (mesh.Material == this && mesh.MeshAsset != null)
                        mesh.MeshAsset.IsMaterialSelected = UINode.IsSelected;
            };
            ReloadMaterial(mat);
        }

        public void TryInsertParamAnimKey(ShaderParam param) {
            BfresWrapper.TryInsertParamKey(Material.Name, param);
        }

        public override List<STGenericMesh> GetMappedMeshes()
        {
            List<STGenericMesh> meshes = new List<STGenericMesh>();
            for (int i = 0; i < ModelWrapper.Meshes.Count; i++)
                if (((FSHP)ModelWrapper.Meshes[i]).Material == this)
                    meshes.Add(ModelWrapper.Meshes[i]);

            return meshes;
        }

        public void CheckErrors()
        {

        }

        public void OnParamUpdated(ShaderParam param, bool captureKeys = false)
        {
            var fmdl = UINode.Parent.Parent.Tag as FMDL;
            //Update renderer
            foreach (FSHP mesh in fmdl.Meshes)
            {
                if (this == mesh.Material && mesh.MeshAsset != null)
                {
                    if (mesh.MeshAsset.MaterialAsset is BfshaRenderer)
                    {
                        ((BfshaRenderer)mesh.MeshAsset.MaterialAsset).ReloadRenderState(Material, mesh.MeshAsset);
                        ((BfshaRenderer)mesh.MeshAsset.MaterialAsset).UpdateMaterialBlock = true;
                    }
                }
            }

            //Possible param animation recording
            if (BfresWrapper.Workspace.GraphWindow.IsRecordMode && captureKeys)
                BfresWrapper.TryInsertParamKey(Material.Name, param);
        }

        public void UpdateMaterialBlock()
        {
            //Update renderer
            foreach (FSHP mesh in ModelWrapper.Meshes)
            {
                if (this == mesh.Material && mesh.MeshAsset != null)
                {
                    if (mesh.MeshAsset.MaterialAsset is BfshaRenderer)
                        ((BfshaRenderer)mesh.MeshAsset.MaterialAsset).UpdateMaterialBlock = true;
                }
            }
        }

        public void OnTextureUpdated(string sampler, string texture, bool captureKeys = false)
        {
            //Possible texture animation recording
            if (BfresWrapper.Workspace.GraphWindow.IsRecordMode && captureKeys)
                BfresWrapper.TryInsertTextureKey(Material.Name, sampler, texture);
        }

        public void ExportTextureMapData(string texture)
        {
            var tex = (TextureNode)BfresWrapper.TextureFolder.Children.FirstOrDefault(x => x.Header == texture);
            if (tex != null)
                tex.ExportTextureDialog();
        }

        public void EditTextureMapData(string texture)
        {
            var tex = (TextureNode)BfresWrapper.TextureFolder.Children.FirstOrDefault(x => x.Header == texture);
            if (tex != null)
                tex.ReplaceTextureDialog();
        }

        public void SelectTextureMapData(string texture)
        {
            var tex = (TextureNode)BfresWrapper.TextureFolder.Children.FirstOrDefault(x => x.Header == texture);
            if (tex != null)
            {
                BfresWrapper.Workspace.Outliner.DeselectAll();
                tex.IsSelected = true;
                BfresWrapper.Workspace.ScrollToSelectedNode(tex);
            }
        }

        public void InsertTextureKey(string sampler, string texture)
        {
            BfresWrapper.TryInsertTextureKey(Material.Name, sampler, texture);
        }

        public void DrawIcon()
        {
            if (IsMaterialInvalid) {
                this.UINode.Icon = IconManager.WARNING_ICON.ToString();
                return;
            }
            UINode.Icon = $"material_{this.Name}";

            if (MaterialAsset == null)
                return; 

            if (!IconManager.HasIcon(UINode.Icon)) {
                GLTexture tex = MaterialAsset.RenderIcon(64);
                IconManager.TryAddIcon(UINode.Icon, tex);
            }
        }

        public void ReloadTextureMap(int index)
        {
            var texSampler = Material.Samplers[index];
            var texMap = this.TextureMaps[index];

            texMap.MagFilter = STTextureMagFilter.Linear;
            texMap.MinFilter = STTextureMinFilter.LinearMipmapLinear;
            texMap.WrapU = BfresMatGLConverter.ConvertWrapMode(texSampler.WrapModeU);
            texMap.WrapV = BfresMatGLConverter.ConvertWrapMode(texSampler.WrapModeV);
            texMap.LODBias = texSampler.LODBias;
            texMap.MaxLOD = texSampler.MaxLOD;
            texMap.MinLOD = texSampler.MinLOD;

            Material.TextureRefs[index] = texMap.Name;

            if (MaterialAsset != null)
            {
                //dispose old icon first
                IconManager.RemoveTextureIcon(UINode.Icon);
                //render out new icon
                GLTexture tex = MaterialAsset.RenderIcon(64);
                IconManager.TryAddIcon(UINode.Icon, tex);
            }
            GLContext.ActiveContext.UpdateViewport = true;
        }

        public void LoadPreset(string filePath, bool keepTextures = true) 
        {

        }

        public void SaveAsPreset(string filePath, bool exportTextures = true, bool exportAnims = true)
        {

        }

        //Gets a unique shader name based on the file data contents
        private string GetShaderName(byte[] data, string internalName)
        {
            string hash = Toolbox.Core.Hashes.Cryptography.Crc32.Compute(data).ToString("X");
            //Limit to the same size of the string as these are hex edited internally
            int max_length = internalName.Length;
            return $"{hash}".PadLeft(max_length, '_');
        }

        //Saves shader to disc with a custom internal name
        private void SaveShaderPreset(string filePath, byte[] data, string internalName)
        {
            //preset already exists
            if (File.Exists(filePath))
                return;

            var mem = new MemoryStream();
            using (var reader = new Toolbox.Core.IO.FileReader(data))
            using (var writer = new Toolbox.Core.IO.FileWriter(mem)) {
                writer.Write(data);

                //Name offset
                if (true)
                {
                    reader.SetByteOrder(false);
                    reader.SeekBegin(16);
                    uint nameOffset = reader.ReadUInt32();

                    //Write the name from the name offset
                    writer.SeekBegin(nameOffset);
                    writer.WriteString(internalName);
                }
                else
                {
                    reader.SetByteOrder(true);
                    reader.SeekBegin(20);
                    uint nameOffset = reader.ReadUInt32();

                    //Write the name from the name offset
                    writer.SeekBegin(nameOffset + 20);
                    writer.WriteString(internalName);
                }
            }
            File.WriteAllBytes(filePath, mem.ToArray());
        }

        private STTextureType GetTextureType(string sampler)
        {
            switch (sampler)
            {
                case "_a0": return STTextureType.Diffuse;
                case "_n0": return STTextureType.Normal;
                case "_s0": return STTextureType.Specular;
                case "_e0": return STTextureType.Emission;
                default:
                    return STTextureType.None;
            }
        }

        public MenuItemModel[] GetContextMenuItems()
        {
            return new MenuItemModel[]
            {
                new MenuItemModel("Export", ExportDialog),
                new MenuItemModel("Replace", ReplaceDialog),
                new MenuItemModel(""),
                new MenuItemModel("Rename", () => UINode.ActivateRename = true),
                new MenuItemModel(""),
                new MenuItemModel("Remove", Remove),
            };
        }

        private void Remove()
        {
            int result = TinyFileDialog.MessageBoxInfoYesNo(string.Format("Are you sure you want to remove {0}? Operation cannot be undone.", this.Material.Name));
            if (result == 1)
            {
                foreach (var mat in ModelWrapper.GetSelectedMaterials())
                    this.ModelWrapper.RemoveMaterial(mat);
            }
        }

        public Type GetTypeUI() => typeof(BfresMaterialEditor);

        public void OnLoadUI(object uiInstance) { }

        public void OnRenderUI(object uiInstance) {
            var editor = (BfresMaterialEditor)uiInstance;
            editor.LoadEditor(this);
        }

        private void ExportDialog()
        {
            var dlg = new ImguiFileDialog();
            dlg.SaveDialog = true;
            dlg.FileName = $"{Material.Name}.zip";
            dlg.AddFilter(".bfmat", ".bfmat");
            dlg.AddFilter(".json", ".json");
            dlg.AddFilter(".zip", ".zip");

            if (dlg.ShowDialog()) {
                if (dlg.FilePath.EndsWith(".zip"))
                    SaveAsPreset(dlg.FilePath, BfresMaterialEditor.exportTextures);
                else
                    Material.Export(dlg.FilePath, ResFile);
            }
        }

        private void ReplaceDialog()
        {
            var dlg = new ImguiFileDialog();
            dlg.SaveDialog = false;
            dlg.AddFilter(".bfmat", ".bfmat");
            dlg.AddFilter(".json", ".json");
            dlg.AddFilter(".zip", ".zip");

            if (dlg.ShowDialog())
            {
                try
                {
                    if (dlg.FilePath.EndsWith(".zip"))
                        LoadPreset(dlg.FilePath, true);
                    else
                    {
                        //Material.Import(dlg.FilePath, ResFile);
                        Replace(Material);
                    }
                }
                catch (Exception ex)
                {
                    TinyFileDialog.MessageBoxErrorOk("File not a valid material!");
                }
            }
        }

        public void Replace(Material material)
        {
            Material = material;
            ReloadMaterial(material);
            material.Name = UINode.Header;

            //Update based on the index used. Each mesh assigned should be updated
            var fmdl = UINode.Parent.Parent.Tag as FMDL;
            foreach (FSHP mesh in fmdl.Meshes)
            {
                if (this == mesh.Material)
                    mesh.UpdateMaterial();
            }

            if (MaterialAsset != null)
            {
                GLTexture tex = MaterialAsset.RenderIcon(64);
                IconManager.TryAddIcon(UINode.Icon, tex, true);
            }
        }

        public void ReloadAttributes()
        {
            //Update based on the index used. Each mesh assigned should be updated
            var fmdl = UINode.Parent.Parent.Tag as FMDL;
            foreach (FSHP mesh in fmdl.Meshes)
            {
                if (this == mesh.Material)
                {
                    // var attributes = BfresGLLoader.LoadAttributes(ParentModel, Shape, Material.Material);
                    // MeshAsset.UpdateAttributes(attributes);
                    mesh.ApplyVertexData();
                }
            }
            GLContext.ActiveContext.UpdateViewport = true;
        }

        public void ReloadAttributeBuffers()
        {
            //Update based on the index used. Each mesh assigned should be updated
            var fmdl = UINode.Parent.Parent.Tag as FMDL;
            foreach (FSHP mesh in fmdl.Meshes)
            {
                if (this == mesh.Material)
                    mesh.UpdateAttributeBuffers();
            }
            GLContext.ActiveContext.UpdateViewport = true;
        }

        public void ReloadMaterialRenderer()
        {
            //Update based on the index used. Each mesh assigned should be updated
            var fmdl = UINode.Parent.Parent.Tag as FMDL;
            foreach (FSHP mesh in fmdl.Meshes)
            {
                if (this == mesh.Material)
                    mesh.UpdateMaterial();
            }
            GLContext.ActiveContext.UpdateViewport = true;
        }

        public void ReloadMaterial(Material material)
        {
            if (UINode != null)
                UINode.Icon = $"material_{this.Name}";

            Samplers = new List<string>();
            AnimatedSamplers = new Dictionary<string, string>();
            ShaderParams = new Dictionary<string, ShaderParam>();
            AnimatedParams = new Dictionary<string, ShaderParam>();
            ShaderOptions = new Dictionary<string, string>();
            TextureMaps = new List<STGenericTextureMap>();

            // SamplerAssign = material.ShaderAssign.SamplerAssigns;
            foreach (var key in material.ShaderAssign.SamplerAssigns)
                SamplerAssign.Add(key);

            // Trying to fix sampler assign
            if (SamplerAssign == null && material.TextureRefs.Count > 0)
            {
                SamplerAssign = new ResDict();
                SamplerAssign.Add("_a0");
            }

            ShaderArchiveName = material.ShaderAssign.ShaderArchiveName;
            ShaderModelName = material.ShaderAssign.ShadingModelName;

            foreach (var param in material.ShaderParams)
                ShaderParams.Add(param.Name, param);
            foreach (var option in material.ShaderAssign.ShaderOptions)
                ShaderOptions.Add(option, option);

            for (int i = 0; i < material.TextureRefs.Count; i++)
            {
                string name = material.TextureRefs[i];
                Sampler sampler = material.Samplers[i];
                var texSampler = material.Samplers[i];
                string samplerName = name.Contains("_Alb") ? "_a0" : name.Contains("_Nrm") ? "_n0" : "";
                string fragSampler = samplerName;

                Samplers.Add(fragSampler);
                TextureMaps.Add(new STGenericTextureMap()
                {
                    Name = name,
                    Sampler = samplerName,
                    MagFilter = STTextureMagFilter.Linear,
                    MinFilter = STTextureMinFilter.LinearMipmapNearest,
                    WrapU = BfresMatGLConverter.ConvertWrapMode(texSampler.WrapModeU),
                    WrapV = BfresMatGLConverter.ConvertWrapMode(texSampler.WrapModeV),
                    LODBias = texSampler.LODBias,
                    MaxLOD = texSampler.MaxLOD,
                    MinLOD = texSampler.MinLOD,
                });
            }
            BfresMatGLConverter.ConvertRenderState(this, material);
        }

        public string GetTextureBySampler(string sampler = "_a0")
        {
            for (int i = 0; i < TextureMaps.Count; i++)
            {
                if (Material.Samplers[i].Name == sampler)
                    return TextureMaps[i].Name;
            }
            return "";
        }
    }

    public class FSHP : STGenericMesh, IPropertyUI, IRenamableNode
    {
        /// <summary>
        /// The shape section storing face indices and bounding data.
        /// </summary>
        public Shape Shape { get; set; }

        /// <summary>
        /// The vertex buffer storing vertex data and attributes.
        /// </summary>
        public VertexBuffer VertexBuffer { get; set; }

        /// <summary>
        /// The model in which the data in this section is parented to.
        /// </summary>
        public Model ParentModel { get; set; }

        /// <summary>
        /// The material data mapped to the mesh.
        /// </summary>
        public FMAT Material { get; set; }

        /// <summary>
        /// The skeleton used in the parent model.
        /// </summary>
        public FSKL ParentSkeleton { get; set; }

        /// <summary>
        /// The file in which the data in this section is parented to.
        /// </summary>
        public ResFile ParentFile { get; set; }

        /// <summary>
        /// The UI tree node.
        /// </summary>
        public NodeBase UINode { get; set; }

        /// <summary>
        /// The mesh renderer.
        /// </summary>
        public BfresMeshRender MeshAsset { get; set; }

        //material assignment before being changed
        private FMAT beforeDroppedMaterial;
        private string beforeDroppedTexture;

        public Type GetTypeUI() => typeof(BfresShapeEditor);

        public void OnLoadUI(object uiInstance) { }

        public FMDL ModelWrapper => this.UINode.Parent.Parent.Tag as FMDL;

        public void OnRenderUI(object uiInstance) {
            var editor = (BfresShapeEditor)uiInstance;
            editor.Render(this);
        }

        public void Renamed(string text)
        {
            Shape.Name = text;
            this.UINode.Header = text;
        }

        public void UpdateTransformedVertices(Matrix4 matrix)
        {
            Syroot.Maths.Vector4F[] position = new Syroot.Maths.Vector4F[Vertices.Count];
            Syroot.Maths.Vector4F[] normals = new Syroot.Maths.Vector4F[Vertices.Count];

            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i].Position = Vector3.TransformPosition(Vertices[i].Position, matrix);
                Vertices[i].Normal = Vector3.TransformPosition(Vertices[i].Normal, matrix);

                position[i] = new Syroot.Maths.Vector4F(
                     Vertices[i].Position.X, Vertices[i].Position.Y, Vertices[i].Position.Z, 0);
                normals[i] = new Syroot.Maths.Vector4F(
                   Vertices[i].Normal.X, Vertices[i].Normal.Y, Vertices[i].Normal.Z, 0);
            }

            VertexBufferHelper helper = new VertexBufferHelper(VertexBuffer, ParentFile.ByteOrder);
            foreach (var att in helper.Attributes)
            {
                if (att.Name == "_p0")
                    att.Data = position;
                if (att.Name == "_n0")
                    att.Data = normals;
            }
            this.VertexBuffer = helper.ToVertexBuffer();
            //Update bfres model
            ParentModel.VertexBuffers[Shape.VertexBufferIndex] = this.VertexBuffer;
        }

        public void SetupRender(BfresMeshRender meshRender) {
            MeshAsset = meshRender;
            this.UpdateMaterial();

            //Attach a tree node to the renderer. This will allow scrolling to outliner when renderer is selected
            MeshAsset.UINode = this.UINode;
            //Remove this wrapper if renderer is removed
            MeshAsset.OnRemoved += delegate
            {
                var fmdl = this.UINode.Parent.Parent.Tag as FMDL;
                fmdl.RemoveMesh(this);
            };
            //Setup drag/drop events
            //The user can drag/drop tree nodes and other UI elements to the mesh itself
            MeshAsset.OnDragDropped += (droppedItem, e) =>
            {
                //Dropped texture
                if (droppedItem is STGenericTexture)
                {
                    string name = ((STGenericTexture)droppedItem).Name;
                    AssignTextureToMaterial(name);
                }
                //Dropped material
                if (droppedItem is FMAT) {
                    AssignMaterial((FMAT)droppedItem);
                }
            };
            MeshAsset.OnDragDroppedOnEnter += delegate
            {
                //droppable material (revert)
                beforeDroppedMaterial = this.Material;
                //droppable texture (revert)
                beforeDroppedTexture = Material.GetTextureBySampler("_a0");
            };
            MeshAsset.OnDragDroppedOnLeave += delegate
            {
                //Revert each dropped option if the cursor leaves the mesh
                if (beforeDroppedMaterial != null)
                    AssignMaterial(beforeDroppedMaterial);
                if (beforeDroppedTexture != null)
                    AssignTextureToMaterial(beforeDroppedTexture);
            };
        }

        private void AssignTextureToMaterial(string name)
        {
            //Assigns a texture to the material
            for (int i = 0; i < Material.TextureMaps.Count; i++)
            {
                if (Material.Material.Samplers[i].Name == "_a0") {
                    Material.TextureMaps[i].Name = name;
                    Material.ReloadTextureMap(i);
                }
            }
        }

        public void CheckErrors()
        {
            if (Material.IsMaterialInvalid)
                StudioLogger.WriteWarning($"No proper material appled for {Material.Name}!");

            foreach (var att in Material.Material.ShaderAssign.AttribAssigns)
            {
                //Check if present in the raw shader. 
                if (Material.MaterialAsset is BfshaRenderer)
                {
                    if (!((BfshaRenderer)Material.MaterialAsset).HasAttribute(att))
                        continue;
                }

                if (!VertexBuffer.Attributes.Any(x => x.Name == att))
                    StudioLogger.WriteWarning($"Mesh {Shape.Name} Missing attribute {att} for material.");
            }
        }

        public override List<STGenericMaterial> GetMaterials() {
            return new List<STGenericMaterial>() { this.Material };
        }

        public void AssignMaterial(FMAT fmat)
        {
            if (Material == fmat)
                return;

            int index = ParentModel.Materials.IndexOf(fmat.Material);
            if (index != -1)
                Shape.MaterialIndex = (ushort)index;
            Material = fmat;

            UpdateMaterial();
            GLContext.ActiveContext.UpdateViewport = true;
        }

        /// <summary>
        /// Updates the material specific attributes from the shader to the mesh assets custom vao.
        /// </summary>
        public void UpdateMaterialAttributes()
        {
            if (MeshAsset == null)
                return;

            if (MeshAsset.MaterialAsset is BfshaRenderer)
                ((BfshaRenderer)MeshAsset.MaterialAsset).OnMeshUpdated(MeshAsset);
        }

        /// <summary>
        /// Updates the material renderer.
        /// </summary>
        public void UpdateMaterial()
        {
            if (MeshAsset == null || MeshAsset.MaterialAsset == null)
                return;

            var fmdl = this.UINode.Parent.Parent.Tag as FMDL;
            BfresLoader.UpdateMaterial(MeshAsset.MaterialAsset as BfresMaterialRender, fmdl.BfresWrapper.Renderer, Material,
                fmdl.ModelRenderer, MeshAsset, Material.Material);
        }

        /// <summary>
        /// Updates the current set of attributes to try matching the required data in the shader binary
        /// </summary>
        public void UpdateAttributeBuffers()
        {
            var attributeMapping = Material.Material.ShaderAssign.AttribAssigns;
            bool updateBuffer = false;
            foreach (var att in attributeMapping)
            {
                if (Material.MaterialAsset is BfshaRenderer)
                {
                    if (!((BfshaRenderer)Material.MaterialAsset).HasAttribute(att))
                        continue;
                }

                if (!VertexBuffer.Attributes.Any(x => x.Name == att))
                {
                    StudioLogger.WriteWarning($"No attribute {att} found from material {Material.Name}. Updating buffer data to fix this.");
                    switch (att)
                    {
                        case "_u1":
                        case "_u2":
                        case "_u3":
                            //Remap to uv0 if no other layer is present.
                            attributeMapping[attributeMapping.IndexOf(att)] = "_u0";
                            break;
                        case "_t0":
                            this.CalculateTangentBitangent(0);
                            AddAttributeData("_t0", Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat.Format_10_10_10_2_SNorm);
                            updateBuffer = true;
                            break;
                        case "_b0":
                            this.CalculateTangentBitangent(0);
                            AddAttributeData("_b0", Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat.Format_10_10_10_2_SNorm);
                            updateBuffer = true;
                            break;
                        case "_c0":
                            this.SetVertexColor(Vector4.One, 0);
                            AddAttributeData("_c0", Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat.Format_8_8_8_8_UNorm);
                            updateBuffer = true;
                            break;
                    }
                }
            }

            if (updateBuffer)
                ApplyVertexData();
        }

        public FSHP(ResFile resFile, FSKL skeleton, Model model, Shape shape, List<STGenericMaterial> materials)
        {
            ParentFile = resFile;
            ParentModel = model;
            Shape = shape;
            ParentSkeleton = skeleton;
            BoneIndex = shape.BoneIndex;
            VertexBuffer = model.VertexBuffers[shape.VertexBufferIndex];
            Material = (FMAT)materials[shape.MaterialIndex];
            VertexSkinCount = shape.VertexSkinCount;
            UINode = new NodeBase(shape.Name);
            UINode.HasCheckBox = true;
            UINode.OnChecked += delegate
            {
                if (MeshAsset != null)
                    MeshAsset.IsVisible = UINode.IsChecked;
                GLContext.ActiveContext.UpdateViewport = true;
            };
            UINode.Tag = this;
            UINode.ContextMenus.Add(new MenuItemModel("Rename", () => UINode.ActivateRename = true));
            UINode.ContextMenus.Add(new MenuItemModel(""));

            UINode.ContextMenus.Add(new MenuItemModel("Recalculate Tangent/Bitangent", RecalculateTanBitanAction));

            MenuItemModel lodMenu = new MenuItemModel("LOD");
            UINode.ContextMenus.Add(lodMenu);

            MenuItemModel uvsMenu = new MenuItemModel("UVs");
            uvsMenu.MenuItems.Add(new MenuItemModel("Flip Vertical", FlipUVsVerticalAction));
            uvsMenu.MenuItems.Add(new MenuItemModel("Flip Horizontal", FlipUVsHorizontalAction));
            UINode.ContextMenus.Add(uvsMenu);

            MenuItemModel normalsMenu = new MenuItemModel("Normals");
            normalsMenu.MenuItems.Add(new MenuItemModel("Smooth", SmoothNormalsAction));
            normalsMenu.MenuItems.Add(new MenuItemModel("Recalculate", RecalculateNormalsAction));
            UINode.ContextMenus.Add(normalsMenu);

            MenuItemModel colorsMenu = new MenuItemModel("Colors");
            colorsMenu.MenuItems.Add(new MenuItemModel("Set White", SetVertexColorsWhiteAction));
            colorsMenu.MenuItems.Add(new MenuItemModel("Set Color", SetVertexColorsAction));
            UINode.ContextMenus.Add(colorsMenu);

            UINode.ContextMenus.Add(new MenuItemModel("Recalculate Bounding Box", CalculateBoundingBox));

            UINode.ContextMenus.Add(new MenuItemModel(""));
            UINode.ContextMenus.Add(new MenuItemModel("Remove", RemoveAction));

            Name = shape.Name;

            UpdateVertexBuffer();
            UpdatePolygonGroups();
        }

        private void RemoveAction()
        {
            int result = MapStudio.UI.TinyFileDialog.MessageBoxInfoYesNo("Are you sure you want to delete these meshes? Operation cannot be undone!");
            if (result != 1)
                return;

            if (MeshAsset != null)
            {
                foreach (BfresModelRender model in ModelWrapper.BfresWrapper.Renderer.Models)
                {
                    if (model.Meshes.Contains(MeshAsset))
                    {
                        model.Meshes.Remove(MeshAsset);
                        MeshAsset.OnRemoved?.Invoke(MeshAsset, EventArgs.Empty);
                    }
                }
            }
        }

        public void EncodeAttribute(string name, Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat format)
        {
            foreach (var mesh in ModelWrapper.GetSelectedMeshes())
            {
                mesh.VertexBuffer.Attributes.FirstOrDefault(x => x.Name == name).Format = format;
                mesh.ApplyVertexData();
            }
        }

        private void FlipUVsHorizontalAction()
        {
            foreach (var mesh in ModelWrapper.GetSelectedMeshes())
            {
                mesh.FlipUvsHorizontal();
                mesh.ApplyVertexData();
            }
        }

        private void FlipUVsVerticalAction()
        {
            foreach (var mesh in ModelWrapper.GetSelectedMeshes())
            {
                mesh.FlipUvsVertical();
                mesh.ApplyVertexData();
            }
        }

        private void SetVertexColorsAction()
        {
            if (!VertexBuffer.Attributes.Any(x => x.Name == "_c0"))
            {
                int result = TinyFileDialog.MessageBoxInfoYesNo(string.Format("Mesh {0} does not have vertex colors found. Do you want to create them? (will increase file size)", this.Name));
                if (result != 1)
                    return;

                AddAttributeData("_c0", Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat.Format_8_8_8_8_UNorm);
            }

            var color = System.Numerics.Vector4.One;

            ImguiCustomWidgets.ColorDialog(color, (outputColor) =>
            {
                foreach (var mesh in ModelWrapper.GetSelectedMeshes())
                {
                    mesh.SetVertexColor(new Vector4(outputColor.X, outputColor.Y, outputColor.Z, outputColor.W));
                    mesh.ApplyVertexData();
                }
            });
        }

        private void SetVertexColorsWhiteAction()
        {
            if (!VertexBuffer.Attributes.Any(x => x.Name == "_c0"))
            {
                int result = TinyFileDialog.MessageBoxInfoYesNo(string.Format("Mesh {0} does not have vertex colors found. Do you want to create them? (will increase file size)", this.Name));
                if (result != 1)
                    return;

                AddAttributeData("_c0", Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat.Format_8_8_8_8_UNorm);
            }

            foreach (var mesh in ModelWrapper.GetSelectedMeshes())
            {
                mesh.SetVertexColor(Vector4.One);
                mesh.ApplyVertexData();
            }
        }

        private void RecalculateNormalsAction()
        {
            foreach (var mesh in ModelWrapper.GetSelectedMeshes())
            {
                mesh.CalculateNormals();
                mesh.ApplyVertexData();
            }
        }

        private void SmoothNormalsAction()
        {
            bool cancel = false;
            foreach (var mesh in ModelWrapper.GetSelectedMeshes())
            {
                mesh.SmoothNormals(ref cancel);
                mesh.ApplyVertexData();
            }
        }

        private void RecalculateTanBitanAction()
        {
            foreach (var mesh in ModelWrapper.GetSelectedMeshes())
            {
                mesh.CalculateTangentBitangent(0);
                mesh.ApplyVertexData();
            }
        }

        private void CalculateBoundingBox()
        {
            foreach (var mesh in this.ModelWrapper.GetSelectedMeshes())
            {
                //Generate boundings for the mesh
                //If a mesh's vertex data is split into parts, we can create sub meshes with their own boundings
                //Sub meshes are used for large meshes where they need to cull parts of a mesh from it's bounding box
                //Sub meshes do not allow multiple materials, that is from the shape itself
                //These are added in the order of the index list, with an offset/count for the indices being used as a sub mesh
                var boundingBox = CalculateBoundingBox(mesh.Vertices, mesh.Shape.VertexSkinCount > 0);

                mesh.Shape.SubMeshBoundings.Clear();
                mesh.Shape.SubMeshBoundings.Add(boundingBox); //Create bounding for total mesh
                mesh.Shape.SubMeshBoundings.Add(boundingBox); //Create bounding for single sub meshes

                var min = boundingBox.Center - boundingBox.Extent;
                var max = boundingBox.Center + boundingBox.Extent;
                float sphereRadius = (float)(boundingBox.Center.Length + boundingBox.Extent.Length);
               // float sphereRadius = GetBoundingSphereFromRegion(min, max);

                mesh.Shape.RadiusArray.Clear();
                mesh.Shape.RadiusArray.Add(sphereRadius); //Total radius (per LOD)
            }
        }

        private static float GetBoundingSphereFromRegion(Syroot.Maths.Vector3F min, Syroot.Maths.Vector3F max)
        {
            // The radius should be the hypotenuse of the triangle.
            // This ensures the sphere contains all points.
            Syroot.Maths.Vector3F lengths = max - min;
            return CalculateRadius(lengths.X / 2.0f, lengths.Y / 2.0f);
        }

        private static float CalculateRadius(float horizontalLeg, float verticalLeg)
        {
            return (float)Math.Sqrt((horizontalLeg * horizontalLeg) + (verticalLeg * verticalLeg));
        }

        private static Bounding CalculateBoundingBox(List<STVertex> vertices,bool isSmoothSkinning)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            for (int i = 0; i < vertices.Count; i++)
            {
                minX = Math.Min(minX, vertices[i].Position.X);
                minY = Math.Min(minY, vertices[i].Position.Y);
                minZ = Math.Min(minZ, vertices[i].Position.Z);
                maxX = Math.Max(maxX, vertices[i].Position.X);
                maxY = Math.Max(maxY, vertices[i].Position.Y);
                maxZ = Math.Max(maxZ, vertices[i].Position.Z);
            }

            return CalculateBoundingBox(
                new Syroot.Maths.Vector3F(minX, minY, minZ),
                new Syroot.Maths.Vector3F(maxX, maxY, maxZ));
        }

        private static Bounding CalculateBoundingBox(Syroot.Maths.Vector3F min, Syroot.Maths.Vector3F max)
        {
            Syroot.Maths.Vector3F center = max + min;
            float xxMax = GetExtent(max.X, min.X);
            float yyMax = GetExtent(max.Y, min.Y);
            float zzMax = GetExtent(max.Z, min.Z);

            var extend = new Syroot.Maths.Vector3F(xxMax, yyMax, zzMax);

            return new Bounding()
            {
                Center = new Syroot.Maths.Vector3F(center.X, center.Y, center.Z),
                Extent = new Syroot.Maths.Vector3F(extend.X, extend.Y, extend.Z),
            };
        }

        private static float GetExtent(float max, float min)
        {
            return (float)Math.Max(Math.Sqrt(max * max), Math.Sqrt(min * min));
        }

        public void UpdateTransform()
        {
            int boneIndex = Shape.BoneIndex;
            var bone = ParentSkeleton.Bones[boneIndex];
            this.Transform = new ModelTransform()
            {
                Position = bone.AnimationController.Position,
                Rotation = bone.AnimationController.Rotation,
                Scale = bone.AnimationController.Scale,
            };
            this.Transform.Update();
        }

        public void UpdateRenderedVertexBuffer()
        {
            if (MeshAsset != null)
                BfresLoader.UpdateVertexBuffer(ParentFile, ParentModel, Shape, Material.Material, MeshAsset);
        }

        public void UpdateAttributes()
        {
            if (MeshAsset != null)
                BfresLoader.UpdateAttributes(ParentModel, Shape, Material.Material, MeshAsset);

        }

        public void AddAttributeData(string attribute, Syroot.NintenTools.NSW.Bfres.GFX.AttribFormat format)
        {
            if (this.VertexBuffer.Attributes.Any(x => x.Name == attribute))
                return;

            this.VertexBuffer.Attributes.Add(new VertexAttrib()
            {
                Name = attribute,
                Format = format
            });
        }

        /// <summary>
        /// Updates the current buffer from the shapes vertex buffer data.
        /// </summary>
        public void UpdateVertexBuffer()
        {
            //Load the vertex buffer into the helper to easily access the data.
            VertexBufferHelper helper = new VertexBufferHelper(VertexBuffer, ParentFile.ByteOrder);

            //Loop through all the vertex data and load it into our vertex data
            Vertices = new List<STVertex>();

            //Get all the necessary attributes
            var positions = TryGetValues(helper, "_p0");
            var normals = TryGetValues(helper, "_n0");
            var texCoords = TryGetChannelValues(helper, "_u");
            var colors = TryGetChannelValues(helper, "_c");
            var tangents = TryGetValues(helper, "_t0");
            var bitangents = TryGetValues(helper, "_b0");
            var weights0 = TryGetValues(helper, "_w0");
            var indices0 = TryGetValues(helper, "_i0");

            var boneIndexList = ParentSkeleton.Skeleton.MatrixToBoneList;

            if (VertexSkinCount == 0)
                UpdateTransform();

            //Get the position attribute and use the length for the vertex count
            for (int v = 0; v < positions.Length; v++)
            {
                STVertex vertex = new STVertex();
                Vertices.Add(vertex);

                vertex.Position = new Vector3(positions[v].X, positions[v].Y, positions[v].Z);
                if (normals.Length > 0)
                    vertex.Normal = new Vector3(normals[v].X, normals[v].Y, normals[v].Z);

                if (texCoords.Length > 0)
                {
                    vertex.TexCoords = new Vector2[texCoords.Length];
                    for (int i = 0; i < texCoords.Length; i++)
                        vertex.TexCoords[i] = new Vector2(texCoords[i][v].X, texCoords[i][v].Y);
                }
                if (colors.Length > 0)
                {
                    vertex.Colors = new Vector4[colors.Length];
                    for (int i = 0; i < colors.Length; i++)
                        vertex.Colors[i] = new Vector4(
                            colors[i][v].X, colors[i][v].Y,
                            colors[i][v].Z, colors[i][v].W);
                }

                if (tangents.Length > 0)
                    vertex.Tangent = new Vector4(tangents[v].X, tangents[v].Y, tangents[v].Z, tangents[v].W);
                if (bitangents.Length > 0)
                    vertex.Bitangent = new Vector4(bitangents[v].X, bitangents[v].Y, bitangents[v].Z, bitangents[v].W);

                for (int i = 0; i < VertexBuffer.VertexSkinCount; i++)
                {
                    if (i > 3)
                        break;

                    int index = boneIndexList[(int)indices0[v][i]];
                    vertex.BoneIndices.Add(index);
                    if (weights0.Length > 0)
                        vertex.BoneWeights.Add(weights0[v][i]);
                    else
                        vertex.BoneWeights.Add(1.0f);

                    if (VertexSkinCount == 1)
                    {
                        var bone = ParentSkeleton.Bones[index];
                        vertex.Position = Vector3.TransformPosition(vertex.Position, bone.Transform);
                        vertex.Normal = Vector3.TransformNormal(vertex.Normal, bone.Transform);
                    }
                }
            }
        }

        public void ApplyVertexData()
        {
            StudioLogger.WriteLine($"Updating mesh buffer {Shape.Name}");

            //Load the vertex buffer into the helper to easily access the data.
            VertexBufferHelper helper = new VertexBufferHelper(VertexBuffer, ParentFile.ByteOrder);

            string[] supportedAttributes = new string[] { "_p0", "_n0", "_u0", "_u1", "_c0", "_t0", "_b0",/* "_w0", "_i0" */};

            //Resize each buffer
            //Only update what is supported
            foreach (var att in helper.Attributes)
            {
                //Skip unsupported attributes to prevent breaking
                if (!supportedAttributes.Contains(att.Name))
                    continue;

                StudioLogger.WriteLine($"Updating attribute {att.Name}");
                att.Data = SetAttributeData(att);
            }
            //Apply buffer helper
            this.VertexBuffer = helper.ToVertexBuffer();
            //Update bfres model
            ParentModel.VertexBuffers[Shape.VertexBufferIndex] = this.VertexBuffer;
            //Update display
            UpdateRenderedVertexBuffer();

            //Update attributes used for custom materials.
            UpdateMaterialAttributes();

            GLContext.ActiveContext.UpdateViewport = true;
        }

        private Syroot.Maths.Vector4F[] SetAttributeData(VertexBufferHelperAttrib att)
        {
            var data = new Syroot.Maths.Vector4F[Vertices.Count];
            for (int v = 0; v < Vertices.Count; v++)
            {
                STVertex vertex = Vertices[v];
                switch (att.Name)
                {
                    case "_p0":
                        data[v] = new Syroot.Maths.Vector4F(
                            vertex.Position.X, vertex.Position.Y, vertex.Position.Z, 0);
                        break;
                    case "_n0":
                        data[v] = new Syroot.Maths.Vector4F(
                            vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z, 0);
                        break;
                    case "_t0":
                        data[v] = new Syroot.Maths.Vector4F(
                            vertex.Tangent.X, vertex.Tangent.Y, vertex.Tangent.Z, vertex.Tangent.W);
                        break;
                    case "_b0":
                        data[v] = new Syroot.Maths.Vector4F(
                            vertex.Bitangent.X, vertex.Bitangent.Y, vertex.Bitangent.Z, vertex.Bitangent.W);
                        break;
                    case "_u0":
                        data[v] = new Syroot.Maths.Vector4F(
                            vertex.TexCoords[0].X, vertex.TexCoords[0].Y, 0, 0);
                        break;
                    case "_u1":
                        data[v] = new Syroot.Maths.Vector4F(
                            vertex.TexCoords[1].X, vertex.TexCoords[1].Y, 0, 0);
                        break;
                    case "_c0":
                        data[v] = new Syroot.Maths.Vector4F(
                            vertex.Colors[0].X, vertex.Colors[0].Y, vertex.Colors[0].Z, vertex.Colors[0].W);
                        break;
                    case "_w0":
                        data[v] = new Syroot.Maths.Vector4F(
                            vertex.BoneWeights.Count > 0 ? vertex.BoneWeights[0] : 0,
                            vertex.BoneWeights.Count > 1 ? vertex.BoneWeights[1] : 0,
                            vertex.BoneWeights.Count > 2 ? vertex.BoneWeights[2] : 0,
                            vertex.BoneWeights.Count > 3 ? vertex.BoneWeights[3] : 0);
                        break;
                    case "_i0":
                        data[v] = new Syroot.Maths.Vector4F(
                            vertex.BoneIndices.Count > 0 ? vertex.BoneIndices[0] : 0,
                            vertex.BoneIndices.Count > 1 ? vertex.BoneIndices[1] : 0,
                            vertex.BoneIndices.Count > 2 ? vertex.BoneIndices[2] : 0,
                            vertex.BoneIndices.Count > 3 ? vertex.BoneIndices[3] : 0);
                        break;
                }
            }
            return data;
        }

        //Gets attributes with more than one channel
        private Syroot.Maths.Vector4F[][] TryGetChannelValues(VertexBufferHelper helper, string attribute)
        {
            List<Syroot.Maths.Vector4F[]> channels = new List<Syroot.Maths.Vector4F[]>();
            for (int i = 0; i < 10; i++)
            {
                if (helper.Attributes.Any(x => x.Name == $"{attribute}{i}"))
                    channels.Add(helper[$"{attribute}{i}"].Data);
                else
                    break;
            }
            return channels.ToArray();
        }

        //Gets the attribute data given the attribute key.
        private Syroot.Maths.Vector4F[] TryGetValues(VertexBufferHelper helper, string attribute)
        {
            if (helper.Attributes.Any(x => x.Name == attribute))
                return helper[attribute].Data;
            else
                return new Syroot.Maths.Vector4F[0];
        }

        /// <summary>
        /// Updates the current polygon groups from the shape data.
        /// </summary>
        public void UpdatePolygonGroups()
        {
            PolygonGroups = new List<STPolygonGroup>();
            foreach (var mesh in Shape.Meshes)
            {
                //Set group as a level of detail
                var group = new STPolygonGroup();
                group.Material = Material;
                group.GroupType = STPolygonGroupType.LevelOfDetail;
                //Load indices into the group
                var indices = mesh.GetIndices().ToArray();
                for (int i = 0; i < indices.Length; i++)
                    group.Faces.Add(indices[i]);

                if (!PrimitiveTypes.ContainsKey(mesh.PrimitiveType))
                    throw new Exception($"Unsupported primitive type! {mesh.PrimitiveType}");

                //Set the primitive type
                group.PrimitiveType = PrimitiveTypes[mesh.PrimitiveType];
                //Set the face offset (used for level of detail meshes)
                group.FaceOffset = (int)mesh.SubMeshes[0].Offset;
                PolygonGroups.Add(group);
                break;
            }
        }

        //Converts bfres primitive types to generic types used for rendering.
        Dictionary<Syroot.NintenTools.NSW.Bfres.GFX.PrimitiveType, STPrimitiveType> PrimitiveTypes = new Dictionary<Syroot.NintenTools.NSW.Bfres.GFX.PrimitiveType, STPrimitiveType>()
        {
            { Syroot.NintenTools.NSW.Bfres.GFX.PrimitiveType.Triangles, STPrimitiveType.Triangles },
            { Syroot.NintenTools.NSW.Bfres.GFX.PrimitiveType.LineStrip, STPrimitiveType.LineLoop },
            { Syroot.NintenTools.NSW.Bfres.GFX.PrimitiveType.Lines, STPrimitiveType.Lines },
            //{ GX2PrimitiveType.TriangleFan, STPrimitiveType.TriangleFans },
            //{ Syroot.NintenTools.NSW.Bfres.GFX.PrimitiveType.Quads, STPrimitiveType.Quad },
            //{ Syroot.NintenTools.NSW.Bfres.GFX.PrimitiveType., STPrimitiveType.QuadStrips },
            { Syroot.NintenTools.NSW.Bfres.GFX.PrimitiveType.TriangleStrip, STPrimitiveType.TriangleStrips },
        };
    }

    public class FSKL : STSkeleton
    {
        public Skeleton Skeleton { get; set; }
        public FMDL Model { get; set; }

        public FSKL(FMDL fmdl, Skeleton skeleton)
        {
            Model = fmdl;
            Skeleton = skeleton;

            Reload();
        }

        public void Reload()
        {
            foreach (var bone in Skeleton.Bones)
            {
                var genericBone = new BfresBone(this, Skeleton, bone)
                {
                    Name = bone.Name,
                    ParentIndex = bone.ParentIndex,
                    Position = new OpenTK.Vector3(
                        bone.Position.X,
                        bone.Position.Y,
                        bone.Position.Z),
                    Scale = new OpenTK.Vector3(
                        bone.Scale.X,
                        bone.Scale.Y,
                        bone.Scale.Z),
                };

                if (bone.FlagsRotation == BoneFlagsRotation.EulerXYZ)
                {
                    genericBone.EulerRotation = new OpenTK.Vector3(
                        bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z);
                }
                else
                    genericBone.Rotation = new OpenTK.Quaternion(
                         bone.Rotation.X, bone.Rotation.Y,
                         bone.Rotation.Z, bone.Rotation.W);

                Bones.Add(genericBone);
            }

            Reset();
        }

        public class BfresBone : STBone, IPropertyUI
        {
            public Bone BoneData { get; set; }
            public Skeleton ParentSkeletonData { get; set; }

            public bool RigidSkinning => BoneData.RigidMatrixIndex != -1;
            public bool SmoothSkinning => BoneData.SmoothMatrixIndex != -1;

            public bool UseSmoothMatrix => SmoothSkinning && !RigidSkinning;

            public Type GetTypeUI() => typeof(BoneEditor);

            public void OnLoadUI(object uiInstance) { }

            public BfresBone(FSKL skeleton) : base(skeleton)
            {
            }

            public BfresBone(FSKL skeleton, Skeleton fskl, Bone bone) : base(skeleton)
            {
                ParentSkeletonData = fskl;
                BoneData = bone;
            }

            public void InsertKey(BfresSkeletalAnim.InsertFlags flags) {
                ((FSKL)this.Skeleton).Model.BfresWrapper.TryInsertBoneKey(this, flags);
            }

            /// <summary>
            /// Updates the drawn bone transform back to the bfres file data
            /// </summary>
            public void UpdateBfresTransform()
            {
                BoneData.Position = new Syroot.Maths.Vector3F(
                this.Position.X,
                this.Position.Y,
                this.Position.Z);
                BoneData.Scale = new Syroot.Maths.Vector3F(
                    this.Scale.X,
                    this.Scale.Y,
                    this.Scale.Z);

                if (ParentSkeletonData.FlagsRotation == SkeletonFlagsRotation.EulerXYZ)
                {
                    BoneData.Rotation = new Syroot.Maths.Vector4F(
                        this.EulerRotation.X,
                        this.EulerRotation.Y,
                        this.EulerRotation.Z, 1.0f);
                }
                else
                {
                    BoneData.Rotation = new Syroot.Maths.Vector4F(
                        this.Rotation.X,
                        this.Rotation.Y,
                        this.Rotation.Z,
                        this.Rotation.W);
                }
                //Update the flags when transform has been adjusted
                UpdateTransformFlags();
            }

            /// <summary>
            /// Updates the current bone transform flags.
            /// These flags determine what matrices can be ignored for matrix updating.
            /// </summary>
            public void UpdateTransformFlags()
            {
                BoneFlagsTransform flags = 0;

                //SRT checks to update matrices
                if (this.Position == Vector3.Zero)
                    flags |= BoneFlagsTransform.TranslateZero;
                if (this.Scale == Vector3.One)
                {
                    flags |= BoneFlagsTransform.ScaleOne;
                    flags |= BoneFlagsTransform.ScaleVolumeOne;
                }
                if (this.Rotation == Quaternion.Identity)
                    flags |= BoneFlagsTransform.RotateZero;

                //Extra scale flags
                if (this.Scale.X == this.Scale.Y && this.Scale.X == this.Scale.Z)
                    flags |= BoneFlagsTransform.ScaleUniform;

                BoneData.FlagsTransform = flags;
            }

            /// <summary>
            /// Gets the transformation of the bone without it's parent transform applied.
            /// </summary>
            public override Matrix4 GetTransform()
            {
                var transform = Matrix4.Identity;
                if (BoneData.FlagsTransform.HasFlag(BoneFlagsTransform.Identity))
                    return transform;

                if (!BoneData.FlagsTransform.HasFlag(BoneFlagsTransform.ScaleOne))
                    transform *= Matrix4.CreateScale(Scale);
                if (!BoneData.FlagsTransform.HasFlag(BoneFlagsTransform.RotateZero))
                    transform *= Matrix4.CreateFromQuaternion(Rotation);
                if (!BoneData.FlagsTransform.HasFlag(BoneFlagsTransform.TranslateZero))
                    transform *= Matrix4.CreateTranslation(Position);

                return transform;
            }

            public void OnRenderUI(object uiInstance)
            {
                var editor = (BoneEditor)uiInstance;
                editor.LoadEditor(this);
            }
        }
    }
}
