using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using GLFrameworkEngine;
using Toolbox.Core;
using MapStudio.UI;

namespace MapStudio.UI
{
    /// <summary>
    /// Stores file resource info for loading and saving individual files.
    /// </summary>
    public class ProjectResources
    {
        /// <summary>
        /// The main project file for configuring settings.
        /// </summary>
        public ProjectFile ProjectFile = new ProjectFile();

        /// <summary>
        /// A list of individual files loaded into the project.
        /// </summary>
        public List<IFileFormat> Files = new List<IFileFormat>();

        public string ProjectFolder { get; set; }

        public bool UpdateCubemaps = false;

        public void LoadFolder(string folder, string filePath)
        {
            ProjectFile.WorkingDirectory = folder;
        }

        public void LoadProject(string filePath, GLContext context, Workspace workspace)
        {
            ProjectFile = ProjectFile.Load(filePath);
            ProjectFolder = Path.GetDirectoryName(filePath);

            string dir = ProjectFile.WorkingDirectory;

            foreach (var asset in ProjectFile.FileAssets)
                workspace.LoadFileFormat(Path.Combine(ProjectFolder,asset), true);

            ProjectFile.LoadSettings(context, workspace);
            ProjectFile.WorkingDirectory = dir;
        }

        public void SaveProject(string filePath, GLContext context, Workspace workspace)
        {
            string folder = System.IO.Path.GetDirectoryName(filePath);
            ProjectFolder = folder;
            //Load settings
            ProjectFile.ApplySettings(context, workspace);
            //Load project files.
            ProjectFile.FileAssets.Clear();
            foreach (var asset in this.Files) //Save the file data with relative to project directory
            {
                string path = asset.FileInfo.FileName;

                ProjectFile.FileAssets.Add(path);
                if (!File.Exists(Path.Combine(ProjectFolder,path)) && File.Exists(asset.FileInfo.FilePath))
                    File.Copy(asset.FileInfo.FilePath, Path.Combine(ProjectFolder,path));
                else
                    asset.Save(File.OpenWrite(Path.Combine(ProjectFolder,path)));
            }
            //Save json file
            ProjectFile.Save(filePath);
        }   

        public void SaveFileData() {
            foreach (var file in Files)
            {
                Toolbox.Core.IO.STFileSaver.SaveFileFormat(file, file.FileInfo.FilePath);
                StudioLogger.WriteLine(string.Format(TranslationSource.GetText("SAVED_FILE"), file.FileInfo.FilePath));
            }
        }

        public void AddFile(IFileFormat file) {
            if (!Files.Contains(file))
                Files.Add(file);
        }
    }
}
