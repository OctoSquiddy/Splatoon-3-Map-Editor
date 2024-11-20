using System.Collections.Generic;
using System.Diagnostics;

namespace SampleMapEditor
{
    /// <summary>
    /// Represents an object in the course which can be referenced by its <see cref="InstanceID"/>.
    /// </summary>
    [ByamlObject]
    [DebuggerDisplay("{GetType().Name}  Id={InstanceID}")]
    public abstract class UnitObject
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets a number identifying this object. Can be non-unique or 0 without any issues.
        /// </summary>
        [ByamlMember]
        public string InstanceID { get; set; }

        //[ByamlMember]
        //public int UnitIdNum { get; set; }
    }
}
