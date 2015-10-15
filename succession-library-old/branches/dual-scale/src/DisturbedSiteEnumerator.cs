using Edu.Wisc.Forest.Flel.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis
{
    /// <summary>
    /// Enumerator for the disturbed sites in a landscape.
    /// </summary>
    public class DisturbedSiteEnumerator
        : IEnumerable<ActiveSite>, IEnumerator<ActiveSite>
    {
        private IEnumerator<ActiveSite> activeSiteEtor;
        private ISiteVar<bool> disturbed;

        //---------------------------------------------------------------------

        public ActiveSite Current
        {
            get {
                return activeSiteEtor.Current;
            }
        }

        //---------------------------------------------------------------------

        object IEnumerator.Current
        {
            get {
                return activeSiteEtor.Current;
            }
        }

        //---------------------------------------------------------------------

        public DisturbedSiteEnumerator(ILandscape     landscape,
                                       ISiteVar<bool> disturbedSiteVar)
        {
            Require.ArgumentNotNull(landscape);
            Require.ArgumentNotNull(disturbedSiteVar);
            if (disturbedSiteVar.Landscape != landscape)
                throw new ArgumentException("Disturbed site variable refers to different landscape");

            activeSiteEtor = landscape.ActiveSites.GetEnumerator();
            disturbed = disturbedSiteVar;
        }

        //---------------------------------------------------------------------

        public bool MoveNext()
        {
            while (activeSiteEtor.MoveNext())
                if (disturbed[activeSiteEtor.Current])
                    return true;
            return false;
        }

        //---------------------------------------------------------------------

        public void Reset()
        {
            activeSiteEtor.Reset();
        }

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ActiveSite>) this).GetEnumerator();
        }

        //---------------------------------------------------------------------

        IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator()
        {
            Reset();
            return this;
        }

        //---------------------------------------------------------------------

        void System.IDisposable.Dispose()
        {
        }
    }
}
