using Edu.Wisc.Forest.Flel.Grids;

using System.Collections;
using System.Collections.Generic;

namespace Landis.Landscape
{
	/// <summary>
	/// Enumerator for all the sites in a landscape.
	/// </summary>
	public class SiteEnumerator
		: IEnumerator<Site>
	{
		private bool atEnd;
		private bool moveNextNotCalled;
		private ILandscape landscape;
		private Site currentSite;
		private ActiveSiteEnumerator activeSiteEtor;
		private MutableActiveSite nextActiveSite;
		private MutableSite inactiveSite;

		//---------------------------------------------------------------------

		public Site Current
		{
			get {
				return currentSite;
			}
		}

		//---------------------------------------------------------------------

		object IEnumerator.Current
		{
			get {
				return currentSite;
			}
		}

		//---------------------------------------------------------------------

		internal SiteEnumerator(ILandscape landscape)
		{
			this.landscape = landscape;
			atEnd = (landscape.Count == 0);
			moveNextNotCalled = true;
			if (! atEnd) {
				activeSiteEtor = landscape.ActiveSites.GetEnumerator() as ActiveSiteEnumerator;
				if (landscape.InactiveSiteCount > 0) {
					inactiveSite = new MutableSite(landscape,
					                               landscape.FirstInactiveSite,
					                               false, /* isActive? */
					                               landscape.InactiveSiteDataIndex);
				}
			}
		}

		//---------------------------------------------------------------------

		private void InitializeCurrentSite()
		{
			if (activeSiteEtor.MoveNext()) {
				nextActiveSite = activeSiteEtor.Current;
				if (nextActiveSite.Location == new Location(1,1))
					currentSite = nextActiveSite;
				else
					currentSite = inactiveSite;
			}
			else {
				//	No active sites
				currentSite = inactiveSite;
			}
		}

		//---------------------------------------------------------------------

		public bool MoveNext()
		{
			if (atEnd)
				return false;

			if (moveNextNotCalled) {
				InitializeCurrentSite();
				moveNextNotCalled = false;
				return true;
			}

			Location nextLocation = RowMajor.Next(currentSite.Location, landscape.Columns);
			if (nextLocation.Row > landscape.Rows) {
				atEnd = true;
				return false;
			}

			if (currentSite.IsActive) {
				//	current site is the active site etor's Current property
				//  so advance etor to next active site.
				if (activeSiteEtor.MoveNext()) {
					//	There is one or more active sites remaining.
					nextActiveSite = activeSiteEtor.Current;
					if (nextLocation == nextActiveSite.Location) {
						//	Leave current site pointing to active site etor
					}
					else {
						//	Next site is inactive.
						currentSite = inactiveSite;
						inactiveSite.SetLocation(nextLocation);
					}
				}
				else {
					//	No more active sites left to visit, so next site is
					//	inactive.
					nextActiveSite = null;
					currentSite = inactiveSite;
					inactiveSite.SetLocation(nextLocation);
				}
			}
			else {
				//	Current site is inactive
				if (nextActiveSite != null && nextLocation == nextActiveSite.Location)
					currentSite = nextActiveSite;
				else {
					//	Leave current site pointing to the local inactive site
					//	instance, and just change its location.
					inactiveSite.SetLocation(nextLocation);
				}
			}
			return true;
		}

		//---------------------------------------------------------------------

		public void Reset()
		{
			atEnd = (landscape.Count == 0);
			moveNextNotCalled = true;
			if (! atEnd) {
				activeSiteEtor.Reset();
				if (landscape.InactiveSiteCount > 0)
					inactiveSite.SetLocation(landscape.FirstInactiveSite);
			}
		}

		//---------------------------------------------------------------------

		public IEnumerator<Site> GetEnumerator()
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
