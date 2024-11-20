using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Core
{
    public class FileFilter
    {
        public string Extension { get; set; }

        public string Description { get; set; }

        public FileFilter(string extension) {
            Extension = extension;
        }

        public FileFilter(string extension, string info) {
            Extension = extension.Replace("*", "");
            Description = info;
        }

        public static string CreateFilter(FileFilter[] filters, bool filterAll = true)
        {
            string filter = "";
            for (int i = 0; i < filters.Length; i++) {
                filter += $"{filters[i].Description} (*{filters[i].Extension})|*{filters[i].Extension}|";
            }
            if (filterAll)
                filter += "All files(*.*) | *.*";
            return filter;
        }
    }
}
