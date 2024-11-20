using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core
{
    public interface IPluginConfig
    {
        void DrawUI();

        unsafe void DrawInSettings();
    }
}
