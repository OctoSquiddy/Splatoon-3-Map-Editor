using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Core
{
    /// <summary>
    /// Represents a UI element that can be selected.
    /// This should be used on elements that can be selected by selection tools.
    /// </summary>
    public interface ISelectableElement
    {
        bool IsSelected { get; set; }
    }
}
