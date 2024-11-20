using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLFrameworkEngine;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Toolbox.Core;
using System.Drawing;
using Toolbox.Core.IO;

namespace MapStudio.UI
{
    public class ImageEditorViewport : Viewport2D
    {
        public STGenericTexture ActiveTexture;

        ImageEditorBackground ImageBackground;

        public ImageEditorViewport(ImageEditor editor)
        {
            ImageBackground = new ImageEditorBackground(editor);
        }

        public override void RenderScene()
        {
            var shader = GlobalShaders.GetShader("IMAGE_EDITOR");
            shader.Enable();

            ImageBackground.Draw(ActiveTexture, Width, Height, Camera);
        }

        public void Reset() {
        }
    }
}
