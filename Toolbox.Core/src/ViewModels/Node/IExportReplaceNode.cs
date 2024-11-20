using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Core;

namespace Toolbox.Core
{
    public interface IExportReplaceNode
    {
        FileFilter[] ReplaceFilter { get; }
        FileFilter[] ExportFilter { get; }

        void Replace(string fileName);
        void Export(string fileName);
    }
}
