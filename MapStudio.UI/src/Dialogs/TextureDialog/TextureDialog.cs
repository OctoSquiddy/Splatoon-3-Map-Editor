using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ImGuiNET;
using Toolbox.Core;
using System.IO;
using System.ComponentModel;
using Toolbox.Core.Imaging;
using GLFrameworkEngine;
using UIFramework;

namespace MapStudio.UI
{
    public class TextureDialog : Window
    {
        public override string Name => TranslationSource.GetText("TEXTURE_DIALOG");

        /// <summary>
        /// The supported formats for loading into the dialog.
        /// </summary>
        public static string[] SupportedExtensions = new string[] {
            ".png",".jpeg",".jpg",".bmp",".gif",".tga", ".tif", ".tiff", ".exr",
        };

        /// <summary>
        /// The list of imported textures.
        /// </summary>
        public List<ImportedTexture> Textures = new List<ImportedTexture>();

        //Selected editor indices
        List<int> SelectedIndices = new List<int>();

        //Supported formats to select from
        private List<TexFormat> SupportedFormats = new List<TexFormat>();
        
        //Image display
        private GLTexture2D DecodedTexture;

        //The thread to encode/decode the texture.
        private Thread Thread;

        //task display
        private string TaskProgress = "";

        private int ActiveTextureIndex = -1;

        bool finishedEncoding = false;

        private TexFormat DefaultFormat;
        private IPlatformSwizzle PlatformSwizzle;

        public TextureDialog(Type textureType, TexFormat defaultFormat = TexFormat.RGBA8_UNORM) {
            DefaultFormat = defaultFormat;

            var instance = (STGenericTexture)Activator.CreateInstance(textureType);
            var formatList = instance.SupportedFormats;

            //Check if in tool encoder supports them
            foreach (var format in formatList){
                foreach (var decoder in FileManager.GetTextureDecoders()) {
                    if (decoder.CanEncode(format) && !SupportedFormats.Contains(format)) {

                        SupportedFormats.Add(format);
                    }
                }
            }

            if (SupportedFormats.Count == 0)
                SupportedFormats.Add(defaultFormat);
        }

        public void OnLoad() {
            DecodedTexture = GLTexture2D.CreateUncompressedTexture(1, 1);
        }

        public ImportedTexture AddTexture(string fileName) {
            if (!File.Exists(fileName))
                throw new Exception($"Invalid input file path! {fileName}");
            //File not supported, return
            if (!SupportedExtensions.Contains(Path.GetExtension(fileName).ToLower()))
                return null;

            var tex = new ImportedTexture(fileName);
            Textures.Add(tex);

            tex.Format = tex.TryDetectFormat(DefaultFormat);

            return tex;
        }

        public ImportedTexture AddTexture(string name, byte[] rgbaData, uint width, uint height, uint mipCount, TexFormat format)
        {
            var tex = new ImportedTexture(name, rgbaData, width, height, mipCount, format);
            Textures.Add(tex);
            return tex;
        }

        public override void Render()
        {
            //Display the first image
            if (ActiveTextureIndex == -1)
                ReloadImageDisplay();

            if (ImGui.IsKeyPressed((int)ImGuiKey.Enter))
            {
                //finish encoding all textures that haven't encoded yet
                //Execute before draw for progress bar to update
                UIManager.ActionExecBeforeUIDraw = () =>
                {
                    EncodeAll();
                    Dispose();
                    DialogHandler.ClosePopup(true);
                };
            }

            ImGui.Columns(3);
            DrawList();
            ImGui.NextColumn();

            ImGui.Text(TaskProgress);
            DrawCanvas();
            ImGui.NextColumn();
            DrawProperties();
            ImGui.NextColumn();
            ImGui.Columns(1);
        }

        private void DrawList()
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg]);

            if (ImGui.BeginChild("##texture_dlg_list")){
                ImGui.Columns(2);

                //Force a selection
                if (SelectedIndices.Count == 0)
                    SelectedIndices.Add(0);

                for (int i = 0; i < Textures.Count; i++)
                {
                    bool isSelected = SelectedIndices.Contains(i);

                    if (ImGui.Selectable(Textures[i].Name, isSelected, ImGuiSelectableFlags.SpanAllColumns)) {

                        if (!ImGui.GetIO().KeyShift)
                            SelectedIndices.Clear();

                        SelectedIndices.Add(i);

                        //Selection range
                        if (ImGui.GetIO().KeyShift)
                        {
                            bool selectRange = false;
                            for (int j = 0; j < Textures.Count; j++)
                            {
                                if (SelectedIndices.Contains(j) || j == i)
                                {
                                    if (!selectRange)
                                        selectRange = true;
                                    else
                                        selectRange = false;
                                }
                                if (selectRange && !SelectedIndices.Contains(j))
                                    SelectedIndices.Add(j);
                            }
                        }
                        ReloadImageDisplay();
                    }
                    if (ImGui.IsItemFocused() && !isSelected)
                    {
                        if (!ImGui.GetIO().KeyShift)
                            SelectedIndices.Clear();

                        SelectedIndices.Add(i);
                        ReloadImageDisplay();
                    }

                    ImGui.NextColumn();
                    ImGui.Text(Textures[i].Format.ToString());
                    ImGui.NextColumn();
                }
                ImGui.Columns(1);
            }
            ImGui.EndChild();

            ImGui.PopStyleColor();
        }

        private byte[] decodedImage;

        private void DrawCanvas()
        {
            if (DecodedTexture == null)
                OnLoad();

            var selectedIndex = SelectedIndices.FirstOrDefault();
            var texture = Textures[selectedIndex];

            if (finishedEncoding) {
                //Display the decoded data as an RGBA texture
                DecodedTexture.Width = texture.Width;
                DecodedTexture.Height = texture.Height;
                DecodedTexture.Reload(texture.Width, texture.Height, decodedImage);
                finishedEncoding = false;
            }

            if (ImGui.BeginChild("##texture_dlg_canvas"))
            {
                var size = ImGui.GetWindowSize();

                //background
                var pos = ImGui.GetCursorPos();
                ImGui.Image((IntPtr)IconManager.GetTextureIcon("CHECKERBOARD"), size);
                //image

                //Aspect size

                #region Calculate Aspect Size
                float tw, th, tx, ty;

                int w = DecodedTexture.Width;
                int h = DecodedTexture.Height;

                double whRatio = (double)w / h;
                if (DecodedTexture.Width >= DecodedTexture.Height)
                {
                    tw = size.X;
                    th = (int)(tw / whRatio);
                }
                else
                {
                    th = size.Y;
                    tw = (int)(th * whRatio);
                }

                //Rectangle placement
                tx = (size.X - tw) / 2;
                ty = (size.Y - th) / 2;

                #endregion

                ImGui.SetCursorPos(new Vector2(pos.X, pos.Y + ty));
                ImGui.Image((IntPtr)DecodedTexture.ID, new Vector2(tw, th));
            }
            ImGui.EndChild();
        }

        private void DrawProperties()
        {
            if (Textures.Count == 0)
                return;

            //There is always a selected texture
            var selectedIndex = SelectedIndices.FirstOrDefault();
            var texture = Textures[selectedIndex];
            var size = ImGui.GetWindowSize();

            var wndsize = new Vector2(ImGui.GetColumnWidth(), ImGui.GetWindowHeight() - ImGui.GetCursorPosY() - 36);
            if (ImGui.BeginChild("##texture_dlg_properties", wndsize))
            {
                if (ImGui.BeginCombo("Format", texture.Format.ToString()))
                {
                    foreach (var format in SupportedFormats)
                    {
                        bool isSelected = format == texture.Format;
                        if (ImGui.Selectable(format.ToString()))
                        {
                            //Disable current display
                            DecodedTexture.Reload(1, 1, new byte[4]);
                            //Multi edit
                            foreach (var selection in SelectedIndices)
                            {
                                Textures[selection].Format = format;
                                //Re encode format
                                Textures[selection].Encoded = false;

                                //Auto set channel layouts based on format. 
                                //Generally this is how they are always setup by default
                                if (format.ToString().Contains("BC4"))
                                {
                                    texture.ChannelRed = STChannelType.Red;
                                    texture.ChannelGreen = STChannelType.Red;
                                    texture.ChannelBlue = STChannelType.Red;
                                    texture.ChannelAlpha = STChannelType.Red;
                                }
                                else if (format.ToString().Contains("BC5"))
                                {
                                    texture.ChannelRed = STChannelType.Red;
                                    texture.ChannelGreen = STChannelType.Green;
                                    texture.ChannelBlue = STChannelType.Zero;
                                    texture.ChannelAlpha = STChannelType.One;
                                }
                                else
                                {
                                    texture.ChannelRed = STChannelType.Red;
                                    texture.ChannelGreen = STChannelType.Green;
                                    texture.ChannelBlue = STChannelType.Blue;
                                    texture.ChannelAlpha = STChannelType.Alpha;
                                }
                            }
                            ReloadImageDisplay();
                        }
                        if (isSelected)
                            ImGui.SetItemDefaultFocus();
                    }

                    ImGui.EndCombo();
                }

                if (ImGuiHelper.InputFromUint(TranslationSource.GetText("MIP_COUNT"), texture, "MipCount", 1, false))
                {
                    foreach (var index in SelectedIndices)
                        this.Textures[index].MipCount = texture.MipCount;
                }

                ImGuiHelper.BoldTextLabel(TranslationSource.GetText("WIDTH"), texture.Width.ToString());
                ImGuiHelper.BoldTextLabel(TranslationSource.GetText("HEIGHT"), texture.Height.ToString());

                ImGuiHelper.ComboFromEnum<STChannelType>("Channel Red", texture, "ChannelRed");
                ImGuiHelper.ComboFromEnum<STChannelType>("Channel Green", texture, "ChannelGreen");
                ImGuiHelper.ComboFromEnum<STChannelType>("Channel Blue", texture, "ChannelBlue");
                ImGuiHelper.ComboFromEnum<STChannelType>("Channel Alpha", texture, "ChannelAlpha");

                if (ImGui.CollapsingHeader("Array Info", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    if (texture.IsArrayType)
                    {
                        if (ImGui.BeginCombo("##Array", $"Surface {texture.ActiveArrayIndex}"))
                        {
                            for (int i = 0; i < texture.Surfaces.Count; i++)
                            {
                                bool selected = i == texture.ActiveArrayIndex;
                                if (ImGui.Selectable($"Surface {i}", selected))
                                {
                                    texture.ActiveArrayIndex = i;
                                    ReloadImageDisplay();
                                }

                                if (selected)
                                    ImGui.SetItemDefaultFocus();
                            }
                            ImGui.EndCombo();
                        }

                        ImGui.SameLine();
                        if (ImGui.Button($"   {IconManager.ADD_ICON}   "))
                        {
                            var dlg = new ImguiFileDialog();
                            dlg.SaveDialog = false;
                            dlg.MultiSelect = true;
                            foreach (var ext in SupportedExtensions)
                                dlg.AddFilter(ext, ext);
                            if (dlg.ShowDialog())
                            {
                                foreach (var file in dlg.FilePaths)
                                    texture.AddSurface(file);

                                //Select added surface
                                texture.ActiveArrayIndex = texture.Surfaces.Count - 1;
                                ReloadImageDisplay();
                            }
                        }
                        ImGui.SameLine();
                        if (ImGui.Button($"   {IconManager.DELETE_ICON}   "))
                        {
                            if (texture.ActiveArrayIndex != 0)
                            {
                                texture.RemoveSurface(texture.ActiveArrayIndex);
                                texture.ActiveArrayIndex -= 1;
                                ReloadImageDisplay();
                            }
                        }
                    }
                }

                DrawSwizzleSettings(texture);
            }
            ImGui.EndChild();

            ImGui.SetCursorPos(new Vector2(size.X - 160, size.Y - 35));

            var buttonSize = new Vector2(70, 30);
            //Don't allow applying till an encoding operation is finished
            if (encoding)
            {
                var disabled = ImGui.GetStyle().Colors[(int)ImGuiCol.TextDisabled];
                ImGui.PushStyleColor(ImGuiCol.Text, disabled);
                ImGui.Button("Ok", buttonSize);
                ImGui.PopStyleColor();
            }
            else
            {
                if (ImGui.Button("Ok", buttonSize))
                {
                    //finish encoding all textures that haven't encoded yet
                    //Execute before draw for progress bar to update
                    UIManager.ActionExecBeforeUIDraw = () =>
                    {
                        EncodeAll();
                        Dispose();
                        DialogHandler.ClosePopup(true);
                    };


                    /*    BackgroundWorker bg = new BackgroundWorker();
                        bg.DoWork += delegate
                        {
                            //finish encoding all textures that haven't encoded yet
                            EncodeAll();
                        };
                        bg.RunWorkerCompleted += delegate
                        {
                            Dispatcher.InvokeAsync(() => Debug.LogException(ex));



                            Dispose();
                            DialogHandler.FinishTask(true);
                        };
                        bg.RunWorkerAsync();*/
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("Cancel", buttonSize))
            {
                Dispose();
                DialogHandler.ClosePopup(false);
            }
        }

        private void DrawSwizzleSettings(ImportedTexture texture)
        {
            if (texture.PlatformSwizzle is WiiUSwizzle)
                DrawWiiUSwizzle((WiiUSwizzle)texture.PlatformSwizzle);
        }

        private void DrawWiiUSwizzle(WiiUSwizzle swizzle)
        {
            if (ImGui.CollapsingHeader("Settings", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGuiHelper.InputFromUint("Swizzle", swizzle, "Swizzle");
            }

            if (ImGui.CollapsingHeader("Advanced"))
            {
                var mode = swizzle.TileMode;
                ImguiCustomWidgets.ComboScrollable("Tile Mode", swizzle.TileMode.ToString(), ref mode, () =>
                {
                    swizzle.TileMode = mode;
                });
                var aamode = swizzle.AAMode;
                ImguiCustomWidgets.ComboScrollable("AA Mode", swizzle.AAMode.ToString(), ref aamode, () =>
                {
                    swizzle.AAMode = aamode;
                });
                var surfaceDim = swizzle.SurfaceDimension;
                ImguiCustomWidgets.ComboScrollable("Surface Dim", swizzle.SurfaceDimension.ToString(), ref surfaceDim, () =>
                {
                    swizzle.SurfaceDimension = surfaceDim;
                });
                var resFlags = swizzle.ResourceFlags;
                ImguiCustomWidgets.ComboScrollable("Resource Flags", swizzle.ResourceFlags.ToString(), ref resFlags, () =>
                {
                    swizzle.ResourceFlags = resFlags;
                });
            }
        }

        private void Finish()
        {
            var current = Thread.CurrentThread;

            Thread = new Thread((ThreadStart)(() =>
            {
                EncodeAll();
            }));
            Thread.Start();
            Thread.Join();

            if (Thread.CurrentThread != current)
                throw new Exception();

            Dispose();
            DialogHandler.FinishTask(true);
        }

        public void Apply()
        {
            EncodeAll();
            Dispose();
        }

        private void Dispose()
        {
            //Dispose all raw texture file data (encoded is kept)
            foreach (var texture in Textures)
                texture.Dispose();
        }

        private void ReloadImageDisplay()
        {
            if (Textures.Count == 0)
                return;

            var selectedIndex = SelectedIndices.FirstOrDefault();
            var texture = Textures[selectedIndex];

            ActiveTextureIndex = selectedIndex;

            Task task = Task.Factory.StartNew(DisplayEncodedTexture);
            task.Wait();
        }

        private bool encoding = false;

        private void DisplayEncodedTexture()
        {
            if (Textures.Count == 0 || encoding)
                return;

            TaskProgress = "Encoding texture..";

            var selectedIndex = SelectedIndices.FirstOrDefault();
            var texture = Textures[selectedIndex];

            Thread = new Thread((ThreadStart)(() =>
            {
                encoding = true;

                try
                {
                    //Encode the current format
                    if (!texture.Encoded)
                    {
                        texture.EncodeTexture(texture.ActiveArrayIndex);
                        texture.Encoded = true;
                    }
                    TaskProgress = "Decoding texture..";

                    //Decode the newly encoded image data
                    decodedImage = texture.DecodeTexture(texture.ActiveArrayIndex);
                    //Check if the texture has been changed or not while the thread is running
                    if (texture != Textures[selectedIndex])
                        return;

                    TaskProgress = $"Encoded {texture.Format} in {texture.EncodingTime}";
                    finishedEncoding = true;
                    encoding = false;
                }
                catch
                {
                    TaskProgress = $"Failed to encode {texture.Format}!";
                    encoding = false;
                }
            }));
            Thread.Start();
        }

        //Encodes all textures present in the dialog including all surface levels
        private void EncodeAll()
        {
            if (Textures.Count == 0)
                return;

            if (Textures.Count > 0)
                ProcessLoading.Instance.IsLoading = true;

            //Also do not run this on a thread for now. 
            //All operations need to be finished before the dialog can close and dispose any resources.
            for (int j = 0; j < Textures.Count; j++)
            {
                ProcessLoading.Instance.Update(j, Textures.Count, $"Encoding {Textures[j].Name}");

                //Encode the current format
                for (int i = 0; i < Textures[j].Surfaces.Count; i++)
                {
                    if (!Textures[j].Encoded)
                    {
                        try
                        {
                            Textures[j].EncodeTexture(i);
                            Textures[j].Encoded = true;
                        }
                        catch (Exception ex)
                        {
                            DialogHandler.ShowException(ex);
                        }
                    }
                }
            }
            ProcessLoading.Instance.Update(100, 100, $"Finished Encoding!");
            ProcessLoading.Instance.IsLoading = false;
        }
    }
}
