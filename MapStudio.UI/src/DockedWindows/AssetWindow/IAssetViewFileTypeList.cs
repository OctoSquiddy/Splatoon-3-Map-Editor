using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapStudio.UI
{
    public interface IAssetViewFileTypeList
    {
        Dictionary<string, Type> FileTypes { get; }
    }
}
