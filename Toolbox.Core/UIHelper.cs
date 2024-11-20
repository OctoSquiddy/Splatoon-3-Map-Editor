using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Core
{
    public class UIHelper
    {
        static EventHandler ViewportUpdated;

        public static void SubscibeViewportUpdate(EventHandler handler) {
            Console.WriteLine($"SubscibeViewportUpdate {handler}");
            ViewportUpdated = handler;
        }

        public static void UpdateViewport() {
            ViewportUpdated?.Invoke(null, EventArgs.Empty);
        }
    }
}
