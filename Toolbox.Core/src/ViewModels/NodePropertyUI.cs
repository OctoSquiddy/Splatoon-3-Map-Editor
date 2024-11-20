using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core.ViewModels
{
    public class NodePropertyUI
    {
        /// <summary>
        /// The tag for drawing properties on. This overrides the parent node tag.
        /// </summary>
        public object Tag;
        /// <summary>
        /// The UI event for drawing properties.
        /// </summary>
        public EventHandler UIDrawer;
    }
}
