using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapStudio.UI
{
    /// <summary>
    /// Represents a tool window drawer for displaying the active tool window of an editor.
    /// </summary>
    public interface IToolWindowDrawer
    {
        void Render();
    }
}
