using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapStudio.UI
{
    /// <summary>
    /// A file type list to determine how to load/save file assets.
    /// </summary>
    public class AssetViewFileTypes : IAssetViewFileTypeList
    {
        public Dictionary<string, Type> FileTypes => new Dictionary<string, Type>()
        {

        };

        public class ImageFileMeta
        {

        }
    }
}
