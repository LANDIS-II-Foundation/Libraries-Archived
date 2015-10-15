using Landis.SpatialModeling;

namespace Landis.Library.Succession
{
    /// <summary>
    /// A form of reproduction.
    /// </summary>
    public interface IFormOfReproduction
    {
        /// <summary>
        /// Tries to reproduce one or more species at a site using this form of
        /// reproduction.
        /// </summary>
        /// <param name="site">
        /// The site where the form of reproduction is tried.
        /// </param>
        /// <returns>
        /// true if at least one species successfully reproduces.
        /// </returns>
        bool TryAt(ActiveSite site);

        //---------------------------------------------------------------------

        /// <summary>
        /// If this form of reproduction is successful at a site, does it
        /// preclude trying any remaining forms that have not yet been tried?
        /// </summary>
        bool PrecludeRemainingForms
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Resets the form of reproduction at a site because it will not be
        /// tried.
        /// </summary>
        /// <param name="site">
        /// The site where the form of reproduction will not be tried.
        /// </param>
        void NotTriedAt(ActiveSite site);
    }
}
