using Edu.Wisc.Forest.Flel.Util;
using System.Collections;
using System.Collections.Generic;

namespace Landis.Landscape
{
	public class ActiveSiteMapEnumerator
		: IEnumerator<LocationAndIndex>
	{
		private ActiveSiteMap map;
		private bool atEnd;
		private bool moveNextNotCalled;
		private LocationAndIndex currentEntry;

		//---------------------------------------------------------------------

		public LocationAndIndex Current
		{
			get {
				return currentEntry;
			}
		}

		//---------------------------------------------------------------------

		object IEnumerator.Current
		{
			get {
				return currentEntry;
			}
		}

		//---------------------------------------------------------------------

		internal ActiveSiteMapEnumerator(ActiveSiteMap map)
		{
			this.map = map;
			this.atEnd = (map.Count == 0);
			this.moveNextNotCalled = true;
		}

		//---------------------------------------------------------------------

		internal void UseForCurrentEntry(LocationAndIndex entry)
		{
			Require.ArgumentNotNull(entry);
			currentEntry = entry;
		}

		//---------------------------------------------------------------------

		public bool MoveNext()
		{
			if (atEnd)
				return false;

			if (moveNextNotCalled) {
				if (currentEntry == null)
					currentEntry = new LocationAndIndex();

				//	We can only get here if map.Count > 0, so we have a
				//	first active site.
				currentEntry.Location = map.FirstActive.Location;
				currentEntry.Index = map.FirstActive.Index;

				moveNextNotCalled = false;
				return true;
			}

			if (map.GetNextActive(ref currentEntry))
				return true;

			atEnd = true;
			return false;
		}

		//---------------------------------------------------------------------

		public void Reset()
		{
			this.atEnd = (map.Count == 0);
			this.moveNextNotCalled = true;
		}

		//---------------------------------------------------------------------

		public IEnumerator<LocationAndIndex> GetEnumerator()
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
