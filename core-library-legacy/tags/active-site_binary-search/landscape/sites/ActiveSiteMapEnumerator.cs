using Edu.Wisc.Forest.Flel.Util;
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

		private int rowIntervalIndex;
		private ActiveSiteMap.Interval rowInterval;

		private int activeRowOffset;
		private ActiveSiteMap.ActiveRow activeRow;

		private int colIntervalIndexForRow;
		private int colIntervalIndexForMap;
		private ActiveSiteMap.Interval columnInterval;

		//---------------------------------------------------------------------

		public LocationAndIndex Current
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

				rowIntervalIndex = 0;
				rowInterval = map.RowIntervals[rowIntervalIndex];
				currentEntry.Row = rowInterval.Start;

				activeRowOffset = 0;
				activeRow = map.ActiveRows[activeRowOffset];

				colIntervalIndexForRow = 0;
				colIntervalIndexForMap = 0;
				columnInterval = map.ColumnIntervals[colIntervalIndexForMap];
				currentEntry.Column = columnInterval.Start;

				currentEntry.Index = 0;

				moveNextNotCalled = false;
				return true;
			}
			
			//	Advance to the next data index.
			currentEntry.Index = currentEntry.Index + 1;
			if (currentEntry.Index >= map.Count) {
				atEnd = true;
				return false;
			}

			//	Note: because we have a valid data index, we are guaranteed
			//  not to go past the end of the lists for column intervals,
			//	active rows, and row intervals.

			//	If not at end of current column interval, advance to the next
			//	column.
			if (currentEntry.Column < columnInterval.End) {
				currentEntry.Column = currentEntry.Column + 1;
				return true;
			}

			//	End of current column interval, so advance to the next column
			//	interval.
			colIntervalIndexForMap++;
			columnInterval = map.ColumnIntervals[colIntervalIndexForMap];
			currentEntry.Column = columnInterval.Start;

			colIntervalIndexForRow++;
			if (colIntervalIndexForRow < activeRow.IntervalCount) {
				//	The new column interval is in the current active row.
				return true;
			}

			//  The new column interval is not in the current active row, so
			//	advance to the next row in the current row interval.
			colIntervalIndexForRow = 0;
			activeRowOffset++;
			activeRow = map.ActiveRows[activeRowOffset];
			if (currentEntry.Row < rowInterval.End) {
				currentEntry.Row = currentEntry.Row + 1;
				return true;
			}

			//	End of current row interval so advance to the next row
			//	interval.
			rowIntervalIndex++;
			rowInterval = map.RowIntervals[rowIntervalIndex];
			currentEntry.Row = rowInterval.Start;
			return true;
		}

		//---------------------------------------------------------------------

		internal void Reset()
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
