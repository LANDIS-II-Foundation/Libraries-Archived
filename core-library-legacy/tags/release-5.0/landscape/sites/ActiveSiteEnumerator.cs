using System.Collections;
using System.Collections.Generic;

namespace Landis.Landscape
{
	/// <summary>
	/// Enumerator for the active sites in a landscape.
	/// </summary>
	public class ActiveSiteEnumerator
		: IEnumerator<MutableActiveSite>
	{
		private LocationAndIndex locationAndIndex;
		private MutableActiveSite currentSite;
		private ActiveSiteMapEnumerator mapEtor;

		//---------------------------------------------------------------------

		public MutableActiveSite Current
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

		internal ActiveSiteEnumerator(ILandscape    landscape,
		                              ActiveSiteMap activeSiteMap)
		{
			locationAndIndex = new LocationAndIndex();
			mapEtor = activeSiteMap.GetEnumerator();
			mapEtor.UseForCurrentEntry(locationAndIndex);

			if (landscape.ActiveSiteCount > 0) {
				//	Get location of first active site so we have a valid
				//	location for mutable active site ctor.
				mapEtor.MoveNext();
				currentSite = new MutableActiveSite(landscape, locationAndIndex);
				mapEtor.Reset();
			}
		}

		//---------------------------------------------------------------------

		public bool MoveNext()
		{
			return mapEtor.MoveNext();
		}

		//---------------------------------------------------------------------

		public void Reset()
		{
			mapEtor.Reset();
		}

		//---------------------------------------------------------------------

		public IEnumerator<MutableActiveSite> GetEnumerator()
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
