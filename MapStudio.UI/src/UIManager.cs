using System;
using System.Collections.Generic;
using System.Text;

namespace MapStudio.UI
{
    public class UIManager
    {
        /// <summary>
        /// When a menu is clicked, it is executed in the render loop normally
        /// Instead we may need to execute ouside the loop so we can update the rendering during the action
        /// </summary>
        public static Action ActionExecBeforeUIDraw = null;

        public static Dictionary<string, Type> CreateNewEditors = new Dictionary<string, Type>();

        /// <summary>
        /// Adds a new UI to the main UI backend.
        /// </summary>
        public static void Subscribe(UI_TYPE type, string name, Type editor) {
            if (type == UI_TYPE.NEW_FILE)
            {
                if (!CreateNewEditors.ContainsKey(name))
                    CreateNewEditors.Add(name, editor);
            }
        }

        public enum UI_TYPE
        {
            NEW_FILE,
        }
    }
}
