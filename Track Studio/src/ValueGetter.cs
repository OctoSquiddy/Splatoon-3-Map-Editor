using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleMapEditor;

namespace ValueGetter
{
    public class ValueGetter
    {
        public static string GetModPath()
        {
            return MapStudio.UI.GlobalSettings.Current != null ? MapStudio.UI.GlobalSettings.Current.Program.ModPath : null;
        }
    }
}
