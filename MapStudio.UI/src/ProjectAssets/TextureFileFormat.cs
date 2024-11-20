using System;
using System.IO;
using Toolbox.Core;
using Toolbox.Core.ViewModels;
using GLFrameworkEngine;

namespace MapStudio.UI
{
    /// <summary>
    /// Represents a model file that is imported into a workspace.
    /// This keeps track of file loading/save information and meta information for projects.
    /// </summary>
    public class TextureFileFormat : IProjectAsset
    {
        public string FilePath { get; set; }
        public string ProjectFilePath { get; set; }

        public bool Identify(IFileFormat fileFormat)
        {
            return fileFormat is STGenericTexture;
        }

        public void Load(Workspace workspace, IFileFormat fileFormat)
        {
            var texture = (STGenericTexture)fileFormat;
            var node = new NodeBase(Path.GetFileNameWithoutExtension(texture.Name));
            node.Tag = texture;
          //  ((ModelEditor)workspace.ActiveEditor).AddTexture(node);
        }
    }
}
