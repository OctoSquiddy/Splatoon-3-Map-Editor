using System;
using System.Collections.Generic;
using System.Text;
using UIFramework;

namespace MapStudio.UI
{
    public class ToolWindow : DockWindow
    {
        public override string Name => "TOOLS";

        public EventHandler UIDrawer;

        public ToolWindow(DockSpaceWindow parent) : base(parent)
        {

        }

        public override void Render() {
            UIDrawer?.Invoke(this, EventArgs.Empty);
        }
    }
}
