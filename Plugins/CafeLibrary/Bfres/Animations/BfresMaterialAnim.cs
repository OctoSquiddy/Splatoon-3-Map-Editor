using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core.Animations;
using Toolbox.Core;
//using BfresLibrary;
using Syroot.NintenTools.NSW.Bfres;
using GLFrameworkEngine;
using Toolbox.Core.ViewModels;
using MapStudio.UI;
using UIFramework;

namespace CafeLibrary.Rendering
{
    public class BfresMaterialAnim : STAnimation, IContextMenu, IPropertyUI, IEditableAnimation
    {
        private string ModelName = null;

        //Root for animation tree
        public TreeNode Root { get; set; }

        public List<string> TextureList = new List<string>();

        public MaterialAnim MaterialAnim;

        public ResFile ResFile { get; set; }

        public NodeBase UINode { get; set; }

        public Type GetTypeUI() => typeof(MaterialParamAnimationEditor);

        public void OnLoadUI(object uiInstance) { }

        public void OnRenderUI(object uiInstance)
        {
          //  var editor = (MaterialParamAnimationEditor)uiInstance;
          //  editor.LoadEditor(this);
        }

        public BfresMaterialAnim() { }

        public BfresMaterialAnim(ResFile resFile, MaterialAnim anim, string name)
        {
            Root = new AnimationTree.AnimNode(this);
            ResFile = resFile;
            ModelName = name;
            MaterialAnim = anim;
            UINode = new NodeBase(anim.Name) { Tag = this };
            UINode.CanRename = true;
            UINode.OnHeaderRenamed += delegate
            {
                OnNameChanged(UINode.Header);
            };
            UINode.Icon = '\uf0e7'.ToString();

            CanPlay = false; //Default all animations to not play unless toggled in list
            Reload(anim);
        }

        public BfresMaterialAnim(MaterialAnim anim, string name) {
            ModelName = name;
            CanPlay = false; //Default all animations to not play unless toggled in list
            Reload(anim);
        }

        public void OnSave()
        {
            MaterialAnim.FrameCount = (int)this.FrameCount;
            MaterialAnim.Loop = this.Loop;

            if (!IsEdited)
                return;

            //Generate anim data
            // MaterialAnimConverter.ConvertAnimation(this, MaterialAnim);
        }

        public void OnNameChanged(string newName)
        {
            string previousName = MaterialAnim.Name;
            MaterialAnim.Name = newName;
        }

        public void InsertParamKey(string material, ShaderParam param)
        {
            this.IsEdited = true;

            var group = this.AnimGroups.FirstOrDefault(x => x.Name == material);
            //Add new material group if doesn't exist
            if (group == null) {
                group = new MaterialAnimGroup() { Name = material, };
                this.AnimGroups.Add(group);
                //Add UI node
                if (!Root.Children.Any(x => x.Header == group.Name))
                    Root.AddChild(MaterialAnimUI.GetGroupNode(this, null, group));
            }
            var paramAnimGroup = (ParamAnimGroup)group.SubAnimGroups.FirstOrDefault(x => x.Name == param.Name);
            //Add new param group if doesn't exist
            if (paramAnimGroup == null) {
                paramAnimGroup = new ParamAnimGroup() { Name = param.Name, };
                group.SubAnimGroups.Add(paramAnimGroup);
            }
            //Insert key to param group
            paramAnimGroup.InsertParamKey(this.Frame, param);
            //Add UI node
            var matNode = Root.Children.FirstOrDefault(x => x.Header == group.Name);
            if (!matNode.Children.Any(x => x.Header == paramAnimGroup.Name))
                matNode.AddChild(MaterialAnimUI.CreateParamNodeHierachy(this, null, paramAnimGroup, group));
        }

        public void InsertTextureKey(string material, string sampler, string texture)
        {
            this.IsEdited = true;

            var group = (MaterialAnimGroup)this.AnimGroups.FirstOrDefault(x => x.Name == material);
            //Add new material group if doesn't exist
            if (group == null)
            {
                group = new MaterialAnimGroup() { Name = material, };
                this.AnimGroups.Add(group);
                //Add UI node
                if (!Root.Children.Any(x => x.Header == group.Name))
                    Root.AddChild(MaterialAnimUI.GetGroupNode(this, null, group));
            }
            var samplerTrack = (SamplerTrack)group.Tracks.FirstOrDefault(x => x.Name == sampler);
            //Add new sampler track if doesn't exist
            if (samplerTrack == null)
            {
                samplerTrack = new SamplerTrack() { Name = sampler, };
                samplerTrack.InterpolationType = STInterpoaltionType.Step;
                group.Tracks.Add(samplerTrack);
            }

            if (!TextureList.Contains(texture))
                TextureList.Add(texture);

            //Insert key to sampler track
            var keyFrame = samplerTrack.KeyFrames.FirstOrDefault(x => x.Frame == this.Frame);
            if (keyFrame == null)
                samplerTrack.Insert(new STKeyFrame(this.Frame, TextureList.IndexOf(texture)));
            else
                keyFrame.Value = TextureList.IndexOf(texture);
            //Add UI node
            var matNode = Root.Children.FirstOrDefault(x => x.Header == group.Name);
            if (!matNode.Children.Any(x => x.Header == samplerTrack.Name))
                matNode.AddChild(MaterialAnimUI.CreateSamplerNodeHierachy(this, samplerTrack, group));
        }

        public MenuItemModel[] GetContextMenuItems()
        {
            return new MenuItemModel[]
            {
                new MenuItemModel("Export", ExportAction),
                new MenuItemModel(""),
                new MenuItemModel("Rename", () => UINode.ActivateRename = true),
                new MenuItemModel(""),
                new MenuItemModel("Delete", DeleteAction)
            };
        }

        private void ExportAction()
        {
            var dlg = new ImguiFileDialog();
            dlg.SaveDialog = true;
            dlg.FileName = $"{MaterialAnim.Name}.json";
            dlg.AddFilter(".bfmaa", ".bfmaa");
            dlg.AddFilter(".json", ".json");

            if (dlg.ShowDialog()) {
                OnSave();
                MaterialAnim.Export(dlg.FilePath, ResFile);
            }
        }

        private void DeleteAction()
        {
            int result = TinyFileDialog.MessageBoxInfoYesNo("Are you sure you want to remove these animations? Operation cannot be undone.");
            if (result != 1)
                return;

            UINode.Parent.Children.Remove(UINode);
        }

        public BfresMaterialAnim Clone() {
            BfresMaterialAnim anim = new BfresMaterialAnim();
            anim.Name = this.Name;
            anim.ModelName = this.ModelName;
            anim.FrameCount = this.FrameCount;
            anim.Frame = this.Frame;
            anim.Loop = this.Loop;
            anim.TextureList = this.TextureList;
            anim.AnimGroups = this.AnimGroups;
            return anim;
        }

        public List<Material> GetMaterialData(string name)
        {
            List<Material> materials = new List<Material>();
            foreach (var model in this.ResFile.Models)
            {
                foreach (var mat in model.Materials)
                {
                    if (mat.Name == name)
                        materials.Add(mat);

                }
            }
            return materials;
        }

        public BfresMaterialRender[] GetMaterials()
        {
            List<BfresMaterialRender> materials = new List<BfresMaterialRender>();

            if (!DataCache.ModelCache.ContainsKey(ModelName))
                return null;

            var models = ((BfresRender)DataCache.ModelCache[ModelName]).Models;
            if (models.Count == 0) return null;

            if (!((BfresRender)DataCache.ModelCache[ModelName]).InFrustum)
                return null;

            foreach (var model in models)
            {
                if (model.IsVisible)
                {
                    foreach (BfresMeshRender mesh in model.MeshList)
                        materials.Add((BfresMaterialRender)mesh.MaterialAsset);
                }
            }
            return materials.ToArray();
        }

        public override void NextFrame()
        {
            var materials = GetMaterials();
            if (materials == null)
                return;

            int numMats = 0;
            foreach (var mat in materials)
            {
                foreach (MaterialAnimGroup group in AnimGroups)
                {
                    if (group.Name != mat.Name)
                        continue;

                    ParseAnimationTrack(group, mat);
                    numMats++;
                }
            }
            if (numMats > 0)
                MapStudio.UI.AnimationStats.MaterialAnims += 1;
        }

        private void ParseAnimationTrack(STAnimGroup group, BfresMaterialRender mat)
        {
            foreach (var track in group.GetTracks())
            {
                if (track is SamplerTrack)
                    ParseSamplerTrack(mat, (SamplerTrack)track);
                if (track is ParamTrack)
                    ParseParamTrack(mat, group, (ParamTrack)track);
            }

            foreach (var subGroup in group.SubAnimGroups)
                ParseAnimationTrack(subGroup, mat);
        }

        private void ParseSamplerTrack(BfresMaterialRender material, SamplerTrack track)
        {
            if (TextureList.Count == 0)
                return;

            if (material.Material.AnimatedSamplers.ContainsKey(track.Name))
                material.Material.AnimatedSamplers.Remove(track.Name);

            var value = (int)track.GetFrameValue(this.Frame);
            var texture = TextureList[value];
            if (texture != null)
                material.Material.AnimatedSamplers.Add(track.Name, texture);
        }

        private void ParseParamTrack(BfresMaterialRender matRender, STAnimGroup group, ParamTrack track)
        {

        }

        public void Reload(MaterialAnim anim)
        {

        }

        public class MaterialAnimGroup : STAnimGroup
        {
            public List<STAnimationTrack> Tracks = new List<STAnimationTrack>();

            public override List<STAnimationTrack> GetTracks() { return Tracks; }

            public MaterialAnimGroup Clone()
            {
                var matGroup = new MaterialAnimGroup();
                matGroup.Name = this.Name;
                matGroup.Category = this.Category;
                foreach (var group in this.SubAnimGroups)
                {
                    if (group is ICloneable)
                        matGroup.SubAnimGroups.Add(((ICloneable)group).Clone() as STAnimGroup);
                }
                foreach (var track in this.Tracks)
                {
                    if (track is ICloneable)
                        matGroup.Tracks.Add(((ICloneable)track).Clone() as STAnimationTrack);
                }
                return matGroup;
            }
        }

        public class ParamAnimGroup : STAnimGroup, ICloneable
        {
            public List<STAnimationTrack> Tracks = new List<STAnimationTrack>();

            public override List<STAnimationTrack> GetTracks() { return Tracks; }

            public object Clone()
            {
                var paramGroup = new ParamAnimGroup();
                paramGroup.Name = this.Name;
                foreach (BfresAnimationTrack track in this.Tracks)
                    paramGroup.Tracks.Add((STAnimationTrack)track.Clone());
                return paramGroup;
            }

            public void RemoveKey(float frame)
            {
                foreach (var track in Tracks) {
                    track.RemoveKey(frame);
                }
            }

            public void InsertParamKey(float frame, ShaderParam param)
            {

            }

            public void InsertKey(float frame, int offset, float value, string trackName) {
                InsertKey(frame, offset, value, 0, 0, trackName);
            }

            public void InsertKey(float frame, int offset, float value) {
                InsertKey(frame, offset, value, 0 , 0, offset.ToString("X"));
            }

            public void InsertKey(float frame, int offset, float value, float slopeIn, float slopeOut, string trackName)
            {
                var interpolation = STInterpoaltionType.Linear;
                //Deternine what other tracks might be using and use that instead
                if (Tracks.Count > 0)
                    interpolation = Tracks.FirstOrDefault().InterpolationType;

                var editedTrack = Tracks.FirstOrDefault(x => ((ParamTrack)x).ValueOffset == offset);
                if (editedTrack == null)
                {
                    editedTrack = new ParamTrack()
                    {
                        ValueOffset = (uint)offset,
                        Name = trackName,
                        InterpolationType = interpolation
                    };
                    Tracks.Add(editedTrack);
                }

                if (editedTrack.InterpolationType == STInterpoaltionType.Hermite)
                {
                    editedTrack.Insert(new STHermiteKeyFrame()
                    {
                        Frame = frame,
                        Value = value,
                        TangentIn = slopeIn,
                        TangentOut = slopeOut,
                    });
                }
                else if (editedTrack.InterpolationType == STInterpoaltionType.Linear)
                {
                    editedTrack.Insert(new STKeyFrame()
                    {
                        Frame = frame,
                        Value = value,
                    });
                }
                else if (editedTrack.InterpolationType == STInterpoaltionType.Step)
                {
                    editedTrack.Insert(new STKeyFrame()
                    {
                        Frame = frame,
                        Value = value,
                    });
                }
            }
        }

        public class SamplerTrack : BfresAnimationTrack
        {
            public override object Clone()
            {
                var track = new SamplerTrack();
                this.Clone(track);
                return track;
            }
        }

        public class ParamTrack : BfresAnimationTrack
        {
            /// <summary>
            /// The offset value of the value offset in byte length.
            /// </summary>
            public uint ValueOffset { get; set; } 

            public bool IsInt32 { get; set; }

            public ParamTrack() { }

            public ParamTrack(uint offset, float value, string name)
            {
                ValueOffset = offset;
                this.KeyFrames.Add(new STKeyFrame(0, value));
                Name = name;
            }

            public override object Clone()
            {
                var track = new ParamTrack();
                track.ValueOffset = this.ValueOffset;
                this.Clone(track);
                return track;
            }
        }
    }
}
