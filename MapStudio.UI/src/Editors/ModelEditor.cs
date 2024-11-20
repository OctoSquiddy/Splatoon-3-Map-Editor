using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core.ViewModels;
using MapStudio.UI;
using GLFrameworkEngine;
using Toolbox.Core;
using OpenTK;

namespace MapStudio.UI
{
    /// <summary>
    /// Represents an editor space for a model file type.
    /// </summary>
    public class ModelEditor : FileEditor
    {
        public string Name => "MODEL_EDITOR";

        private NodeBase ModelFolder = new NodeBase(TranslationSource.GetText("MODELS"));
        private NodeBase TextureFolder = new NodeBase(TranslationSource.GetText("TEXTURES"));

        public ModelEditor()
        {

        }

        public void LoadNodeTree()
        {

        }

        public void AddMesh(GenericMeshRender render)
        {
            //Prepare remove handling
            render.RemoveCallback += delegate {
                //Remove the node from the model list
                ModelFolder.Children.Remove(render.UINode);
                //Dispose the mesh.
                render.Dispose();
            };
            //Prepare add handling
            render.AddCallback += delegate {
                //Add the node from the model list
                ModelFolder.Children.Add(render.UINode);
            };
            //Add to the scene for viewing
            GLContext.ActiveContext.Scene.AddRenderObject(render);
        }

        public void AddTexture(NodeBase textureNode) {
            TextureFolder.AddChild(textureNode);
        }
    }
}
