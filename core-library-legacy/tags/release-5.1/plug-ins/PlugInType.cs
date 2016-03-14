//  Author: Jimm Domingo, UW-Madison, FLEL

using Edu.Wisc.Forest.Flel.Util;
using System;

namespace Landis.PlugIns
{
    /// <summary>
    /// The type of a plug-in.
    /// </summary>
    /// <remarks>
    /// Each type has a unique name, for example: "succession", "output",
    /// "disturbance:wind".  For types that belong to the same group, the name
    /// of the group appears first followed by a colon.  For example,
    /// "disturbance:fire" and "disturbance:wind".
    /// </remarks>
    public class PlugInType
    {
        private string name;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="name">The type's name.</param>
        public PlugInType(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name of plug-in type is null or empty string.");
            this.name = name;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The type's name.
        /// </summary>
        public string Name
        {
            get {
                return name;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if the type is a member of a group of types.
        /// </summary>
        public bool IsMemberOf(string groupName)
        {
            return (name == groupName) || name.StartsWith(groupName + ":");
        }

        //---------------------------------------------------------------------

        public override string ToString()
        {
            return name;
        }

        //---------------------------------------------------------------------

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        //---------------------------------------------------------------------

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            PlugInType type = (PlugInType) obj;
            return name == type.name;
        }
    }
}
