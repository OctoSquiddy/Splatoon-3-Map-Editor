using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Core.ModelView;
using Toolbox.Core.ViewModels;

namespace Toolbox.Core
{
    public class CompressionMenu
    {
        public static MenuItemModel[] GetMenuItems()
        {
            List<MenuItemModel> items = new List<MenuItemModel>();
            foreach (var file in FileManager.GetCompressionFormats())
                items.Add(CreateMenu(file));
            return items.ToArray();
        }

        public static MenuItemModel CreateMenu(ICompressionFormat format) {
            var item = new CompressionMenuItem(format);
            return item;
        }

        public class CompressionMenuItem : MenuItemModel
        {
            public ICompressionFormat Format;

            public CompressionMenuItem(ICompressionFormat format) : base(format.ToString())
            {
                Format = format;
                MenuItems.Add(new MenuItemModel("Decompress", DecompressMenu)) ;
                MenuItems.Add(new MenuItemModel("Compress", CompressMenu)
                {
                    IsEnabled = Format.CanCompress,
                });
            }

            private void DecompressMenu(object sender, EventArgs e)
            {

            }

            private void CompressMenu(object sender, EventArgs e)
            {

            }
        }
    }
}
