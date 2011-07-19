using Landis.PlugIns;

namespace Landis.Test.PlugIns
{
    //  A plug-in for testing; does nothing.
    public class NullPlugIn
        : PlugIn
    {
        public NullPlugIn(string     name,
                          PlugInType type)
            : base(name, type)
        {
        }

        //---------------------------------------------------------------------

        public override void Initialize(string dataFile,
                                        ICore  modelCore)
        {
        }

        //---------------------------------------------------------------------

        public override void Run()
        {
        }
    }
}
