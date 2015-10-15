using Landis.Landscape;

namespace Landis
{
    /// <summary>
    /// Interface to a form of plant reproduction.
    /// </summary>
    public interface IReproduction
    {
        /// <summary>
        /// Does the reproduction at a site.
        /// </summary>
        /// <param name="site">
        /// The site where the reproduction is to be done.
        /// </param>
        void Do(ActiveSite site);
    }
}
