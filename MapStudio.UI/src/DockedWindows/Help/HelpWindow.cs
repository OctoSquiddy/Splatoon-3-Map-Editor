using System;
using System.Collections.Generic;
using System.Text;
using UIFramework;

namespace MapStudio.UI
{
    public class HelpWindow : DockWindow
    {
        public override string Name => "HELP";

        public EventHandler UIDrawer;

        public HelpWindow(DockSpaceWindow parent) : base(parent)
        {

        }

        public override void Render() {
            UIDrawer?.Invoke(this, EventArgs.Empty);
        }
    }
}
