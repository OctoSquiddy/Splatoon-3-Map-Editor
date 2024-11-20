using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GLFrameworkEngine;

namespace MapStudio.UI
{
    public class AssetViewFileExplorer : IAssetLoader
    {
        public string Name => "Project Files";

        public bool IsFilterMode => false;

        private AssetViewWindow AssetViewer;

        public AssetViewFileExplorer(AssetViewWindow assetViewer) {
            AssetViewer = assetViewer;
        }

        public List<AssetItem> Reload()
        {
            var projectFile = Workspace.ActiveWorkspace.Resources;
            if (string.IsNullOrEmpty(projectFile.ProjectFolder))
                return new List<AssetItem>();

            return AssetViewer.LoadFromFolder(new AssetFolder(projectFile.ProjectFolder));
        }

        public bool UpdateFilterList()
        {
            return false;
        }
    }
}
