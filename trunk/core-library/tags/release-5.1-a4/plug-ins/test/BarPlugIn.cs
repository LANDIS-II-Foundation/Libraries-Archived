using Landis.PlugIns;

namespace Landis.Test.PlugIns
{
	//  A test plug-in.
	public class BarPlugIn
        : NullPlugIn
    {
        public const string PlugInName = "Bar";
        public const string TypeName = "output";

        //---------------------------------------------------------------------

        public BarPlugIn()
            : base(PlugInName, new PlugInType(TypeName))
        {
        }
	}
}
