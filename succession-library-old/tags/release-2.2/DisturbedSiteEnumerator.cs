using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Landis
{
    /// <summary>
    /// Enumerator for the disturbed sites in a landscape.
    /// </summary>
    public class DisturbedSiteEnumerator
        : IEnumerable<MutableActiveSite>, IEnumerator<MutableActiveSite>
    {
        private IEnumerator<MutableActiveSite> activeSiteEtor;
        private ISiteVar<bool> disturbed;

        //---------------------------------------------------------------------

        public MutableActiveSite Current
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
            return ((IEnumerable<MutableActiveSite>) this).GetEnumerator();
        }

        //---------------------------------------------------------------------

        IEnumerator<MutableActiveSite> IEnumerable<MutableActiveSite>.GetEnumerator()
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
