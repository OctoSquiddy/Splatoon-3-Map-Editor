using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;

namespace MapStudio.UI
{
    public interface IProjectAsset
    {
        string FilePath { get; set; }
        string ProjectFilePath { get; set; }

        bool Identify(IFileFormat fileFormat);
        void Load(Workspace workspace, IFileFormat fileFormat);
    }
}
