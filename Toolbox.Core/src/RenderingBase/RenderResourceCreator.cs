using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core
{
    public class RenderResourceCreator
    {
        public static event ReturnTextureEventHandler CreateTextureInstance;

        public delegate IRenderableTexture ReturnTextureEventHandler(object sender, EventArgs args);

        public static IRenderableTexture CreateTexture(STGenericTexture texture)
        {
            if (CreateTextureInstance != null)
                return CreateTextureInstance(texture, EventArgs.Empty);
            else
                throw new Exception("");
        }
    }
}
