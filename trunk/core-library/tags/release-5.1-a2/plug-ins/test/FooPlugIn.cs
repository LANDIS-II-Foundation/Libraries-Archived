using Landis.PlugIns;

namespace Landis.Test.PlugIns
{
    //  A test plug-in.
    public class FooPlugIn
        : NullPlugIn
    {
        public const string PlugInName = "Foo";
        public const string TypeName = "disturbance:foo";

        //---------------------------------------------------------------------

        public FooPlugIn()
            : base(PlugInName, new PlugInType(TypeName))
        {
        }
    }
}
