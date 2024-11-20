using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Core
{
    /// <summary>
    /// Represents a texture that is mapped to a material.
    /// </summary>
    [Serializable]
    public class STGenericTextureMap
    {
        /// <summary>
        /// Gets or sets the texture name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sampler name. 
        /// This is an optional value that can determine texture types passed to the shader.
        /// </summary>
        public string Sampler { get; set; }

        /// <summary>
        /// The type of texture to use for rendering and exporting as.
        /// </summary>
        public STTextureType Type { get; set; } = STTextureType.None;

        /// <summary>
        /// Determines how to wrap the texture horizontal when a border is reached in a UV map.
        /// </summary>
        public STTextureWrapMode WrapU { get; set; } = STTextureWrapMode.Repeat;

        /// <summary>
        /// Determines how to wrap the texture vertical when a border is reached in a UV map.
        /// </summary>
        public STTextureWrapMode WrapV { get; set; } = STTextureWrapMode.Repeat;

        public STTextureMagFilter MagFilter { get; set; } = STTextureMagFilter.Linear;
        public STTextureMinFilter MinFilter { get; set; } = STTextureMinFilter.LinearMipmapNearest;

        public float MinLOD { get; set; }
        public float MaxLOD { get; set; }

        public float LODBias { get; set; }

        /// <summary>
        /// The transformation used for the current texture map.
        /// </summary>
        public virtual STTextureTransform Transform { get; set; }

        /// <summary>
        /// Gets the texture used by the texture map.
        /// </summary>
        /// <returns></returns>
        public virtual STGenericTexture GetTexture()
        {
            return null;
        }
    }
}
