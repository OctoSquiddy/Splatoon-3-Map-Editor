using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Core.ViewModels;

namespace Toolbox.Core
{
    public interface IContextMenu
    {
        MenuItemModel[] GetContextMenuItems();
    }
}
