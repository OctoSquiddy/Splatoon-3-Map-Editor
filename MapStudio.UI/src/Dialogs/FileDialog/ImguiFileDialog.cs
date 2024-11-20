using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using Toolbox.Core;

namespace MapStudio.UI
{
    /// <summary>
    /// A file dialog for open/saving files.
    /// </summary>
    public class ImguiFileDialog
    {
        static Dictionary<string, int> SelectedFilters = new Dictionary<string, int>();

        /// <summary>
        /// Determines to add a filter for all files or not.
        /// </summary>
        public bool FilterAll { get; set; } = true;

        /// <summary>
        /// Determines if the dialog is for saving or opening instead.
        /// </summary>
        public bool SaveDialog { get; set; }

        /// <summary>
        /// The file list of all selected files.
        /// </summary>
        public string[] FilePaths = new string[0];

        /// <summary>
        /// The file path of the first file selected.
        /// </summary>
        public string FilePath => FilePaths.FirstOrDefault();

        /// <summary>
        /// The file name to display in the dialog when loaded.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Determines to allow multi selection or not.
        /// </summary>
        public bool MultiSelect = false;

        string dialogKey;

        readonly List<FileFilter> filters = new List<FileFilter>();

        /// <summary>
        /// Adds a filter from a given extension and description.
        /// </summary>
        public void AddFilter(string ext, string description) {
            filters.Add(new FileFilter(ext, description));
        }

        /// <summary>
        /// Adds a filter from a given FileFilter instance.
        /// </summary>
        public void AddFilter(FileFilter filter) {
            filters.Add(filter);
        }

        /// <summary>
        /// Shows the dialog and returns true if successfully selected a file.
        /// Can provide a unique key to determine what filter was previously used to select.
        /// </summary>
        public bool ShowDialog(string key = "")
        {
            dialogKey = key;

            if (!SelectedFilters.ContainsKey(key))
                SelectedFilters.Add(key, 0);

            if (SaveDialog)
            {
                var ofd = TinyFileDialog.SaveFileDialog(filters, FileName);
                if (!string.IsNullOrEmpty(ofd))
                {
                    this.FilePaths = new string[] { ofd };
                    return true;
                }
            }
            else
            {
                var ofd = TinyFileDialog.OpenFileDialog(filters, FileName, MultiSelect);
                if (!string.IsNullOrEmpty(ofd))
                {

                    this.FilePaths = ofd.Split('|');
                    return true;
                }
            }

            return false;
        }
    }
}
