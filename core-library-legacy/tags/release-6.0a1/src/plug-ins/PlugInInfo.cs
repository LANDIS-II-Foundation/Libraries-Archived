namespace Landis.PlugIns
{
    /// <summary>
    /// Information about a plug-in.
    /// </summary>
    public class PlugInInfo
        : Edu.Wisc.Forest.Flel.Util.PlugIns.Info
    {
        PlugInType type;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="name">The plug-in's name</param>
        /// <param name="type">The plug-in's type</param>
		/// <param name="implementationName">The AssemblyQualifiedName of the
		/// class that implements the plug-in.</param>
        public PlugInInfo(string     name,
                          PlugInType type,
                          string     implementationName)
            : base(name, typeof(PlugIn), implementationName)
        {
            this.type = type;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The plug-in's type.
        /// </summary>
        public PlugInType PlugInType
        {
            get {
                return type;
            }
        }
    }
}
