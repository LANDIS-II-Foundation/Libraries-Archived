using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.Succession
{
    /// <summary>
    /// Site variables related to succession.
    /// </summary>
    internal static class SiteVars
    {
        private static ISiteVar<int> timeOfLast;
        private static ISiteVar<byte> shade;
        private static ISiteVar<bool> disturbed;

        //---------------------------------------------------------------------

        internal static ISiteVar<int> TimeOfLast
        {
            get {
                return timeOfLast;
            }
        }

        //---------------------------------------------------------------------

        internal static ISiteVar<byte> Shade
        {
            get {
                return shade;
            }
        }

        //-----------------------------------------------------------------

        internal static ISiteVar<bool> Disturbed
        {
            get {
                return disturbed;
            }
        }

        //---------------------------------------------------------------------

        internal static void Initialize()
        {
            timeOfLast = Model.Core.Landscape.NewSiteVar<int>();
            shade      = Model.Core.Landscape.NewSiteVar<byte>();
            disturbed  = Model.Core.Landscape.NewSiteVar<bool>();

            Model.Core.RegisterSiteVar(timeOfLast, "TimeOfLastSuccession");
            Model.Core.RegisterSiteVar(shade, "Shade");
        }
    }
}
