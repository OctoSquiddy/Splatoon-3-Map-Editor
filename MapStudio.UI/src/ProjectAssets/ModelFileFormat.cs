using System;
using System.Collections.Generic;
using Toolbox.Core;
using GLFrameworkEngine;

namespace MapStudio.UI
{
    /// <summary>
    /// Represents a model file that is imported into a workspace.
    /// This keeps track of file loading/save information and meta information for projects.
    /// </summary>
    public class ModelFileFormat : IProjectAsset
    {
        public string FilePath { get; set; }
        public string ProjectFilePath { get; set; }

        public bool Identify(IFileFormat fileFormat)
        {
            return fileFormat is IModelFormat;
        }

        public void Load(Workspace workspace, IFileFormat fileFormat)
        {
            var modelFormat = (IModelFormat)fileFormat;
            var genericModel = modelFormat.ToGeneric();
            foreach (var mesh in genericModel.Meshes)
            {
                var render = new GenericMeshRender(mesh);
                //((ModelEditor)workspace.ActiveEditor).AddMesh(render);
            }
        }
    }
}
