using System.Collections.Generic;

namespace Landis.Landscape
{
	public class ActiveSiteMap
	{
		//---------------------------------------------------------------------

#if TEST_INTERNALS
		public struct Interval
#else
		private struct Interval
#endif
		{
			private uint start;
			private uint end;
			private uint startOffset;

			//-----------------------------------------------------------------

			public uint Start {
				get {
					return start;
				}
			}

			//-----------------------------------------------------------------

			private void ValidateEnd(uint end)
			{
				if (end < start)
					throw new System.ApplicationException(
	                                   string.Format("value for end ({0}) < "
					              					 + "start ({1})",
					                                 end, start));
			}

			//-----------------------------------------------------------------

			public uint End {
				get {
					return end;
				}
				set {
					ValidateEnd(value);
					end = value;
				}
			}

			//-----------------------------------------------------------------

			public uint StartOffset {
				get {
					return startOffset;
				}
			}

			//-----------------------------------------------------------------

			public Interval(uint start,
			                uint end,
			                uint startOffset)
			{
				if (start == 0)
					throw new System.ApplicationException("start is 0");
				this.start 		 = start;
				this.end 		 = end;
				this.startOffset = startOffset;
				ValidateEnd(end);
			}

			//-----------------------------------------------------------------

			//	The offset associated with a value in the interval.  If value
			//	is not in the interval, returns null.
			public uint? Offset(uint value)
			{
				if (start <= value && value <= end)
					return startOffset + (value - start);
				else
					return null;
			}

			//-----------------------------------------------------------------

			//	Search for a value in a sorted list of intervals.
			//	Return the offset associated with the value or null.
			public static uint? Search(List<Interval> intervals,
					                   uint			  value)
			{
				foreach (Interval interval in intervals) {
					//	Have we reached an interval past where the desired
					//	value would be?
					if (value < interval.Start)
						break;
					uint? valueOffset = interval.Offset(value);
					if (valueOffset != null)
						return valueOffset;
				}
				return null;
			}
		}

		//---------------------------------------------------------------------

#if TEST_INTERNALS
		public struct ActiveRow
#else
		private struct ActiveRow
#endif
		{
			private uint intervalCount;
			private uint firstIntervalOffset;

			//-----------------------------------------------------------------

			public uint IntervalCount {
				get {
					return intervalCount;
				}
			}

			//-----------------------------------------------------------------

			public uint FirstIntervalOffset {
				get {
					return firstIntervalOffset;
				}
			}

			//-----------------------------------------------------------------

			public ActiveRow(uint intervalCount,
			                 uint firstIntervalOffset)
			{
				if (intervalCount == 0)
					throw new System.ApplicationException(
					                               "interval count is 0");
				this.intervalCount 		 = intervalCount;
				this.firstIntervalOffset = firstIntervalOffset;
			}
		}

		//---------------------------------------------------------------------

		private uint count;
		private List<Interval>  rowIntervals;
		private List<ActiveRow> activeRows;
		private List<Interval>  columnIntervals;

		//---------------------------------------------------------------------

		public uint Count
		{
			get {
				return count;
			}
		}

		//---------------------------------------------------------------------

#if TEST_INTERNALS
		public List<Interval> RowIntervals
		{
			get {
				return rowIntervals;
			}
		}
#endif

		//---------------------------------------------------------------------

#if TEST_INTERNALS
		public List<ActiveRow> ActiveRows
		{
			get {
				return activeRows;
			}
		}
#endif

		//---------------------------------------------------------------------

#if TEST_INTERNALS
		public List<Interval> ColumnIntervals
		{
			get {
				return columnIntervals;
			}
		}
#endif

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance using an input data grid.
		/// </summary>
		public ActiveSiteMap(IInputGrid<bool> activeSites)
		{
			Initialize(activeSites);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance using an indexable data grid.
		/// </summary>
		public ActiveSiteMap(IIndexableGrid<bool> activeSites)
		{
			Initialize(new InputGrid<bool>(activeSites));
		}

		//---------------------------------------------------------------------

		private void Initialize(IInputGrid<bool> activeSites)
		{
			this.rowIntervals    = new List<Interval>();
			this.activeRows      = new List<ActiveRow>();
			this.columnIntervals = new List<Interval>();

			bool inRowInterval = false;
			Interval rowInterval = new Interval();
			for (uint row = 1; row <= activeSites.Rows; ++row) {
				uint colIntervalCount = 0;
				uint firstColIntervalIndex = 0;
				bool inColumnInterval = false;
				Interval columnInterval = new Interval();
				for (uint column = 1; column <= activeSites.Columns; ++column) {
					if (activeSites.ReadValue()) {
						this.count++;
						if (inColumnInterval) {
							//  extend column interval
							columnInterval.End = column;
						}
						else {
							//  start a new column interval
							columnInterval = new Interval(column, column,
							                              count - 1);
							inColumnInterval = true;
							colIntervalCount++;
							if (colIntervalCount == 1)
								firstColIntervalIndex = (uint)
														columnIntervals.Count;
						}
					}
					else {
						// current site is inactive
						if (inColumnInterval) {
							//  end of current column interval
							this.columnIntervals.Add(columnInterval);
							inColumnInterval = false;
						}
					}
				}  // for each column

				// at end of current row
				if (colIntervalCount > 0) {
					//  current row is an active row
					if (inColumnInterval) {
						//  last column was active, so add its interval
						this.columnIntervals.Add(columnInterval);
					}
					this.activeRows.Add(new ActiveRow(colIntervalCount,
					                             	  firstColIntervalIndex));
					if (inRowInterval) {
						//  extend row interval
						rowInterval.End = row;
					}
					else {
						//	start a new row interval
						rowInterval = new Interval(row, row,
						                           (uint)(activeRows.Count-1));
						inRowInterval = true;
					}
				}
				else {
					//	current row is not an active row
					if (inRowInterval) {
						this.rowIntervals.Add(rowInterval);
						inRowInterval = false;
					}
				}
			}  // for each row

			if (inRowInterval) {
				//	last row was an active row, so add its row interval
				this.rowIntervals.Add(rowInterval);
			}
		}

		//---------------------------------------------------------------------

		public uint this[Location location]
		{
			get {
				uint? rowOffset = Interval.Search(rowIntervals, location.Row);
				if (rowOffset == null)
					return this.count;
				ActiveRow activeRow = activeRows[(int)rowOffset.Value];
				List<Interval> colIntervalsInRow =
					columnIntervals.GetRange((int)
					                         activeRow.FirstIntervalOffset,
					                         (int) activeRow.IntervalCount);
				uint? colOffset = Interval.Search(colIntervalsInRow,
				                                  location.Column);
				if (colOffset == null)
					return this.count;
				else
					return colOffset.Value;
			}
		}

		//---------------------------------------------------------------------

		public class Entry
		{
			private Location location;
			private uint index;

			//-----------------------------------------------------------------

			public Location Location {
				get {
					return location;
				}
			}

			//-----------------------------------------------------------------

			public uint Index {
				get {
					return index;
				}
			}

			//-----------------------------------------------------------------

			public Entry(Location location,
			             uint     index)
			{
				this.location = location;
				this.index    = index;
			}
		}

		//---------------------------------------------------------------------

		public IEnumerator<Entry> GetEnumerator()
		{
			foreach (Interval rowInterval in this.rowIntervals) {
				for (uint row = rowInterval.Start; row <= rowInterval.End;
				     							   ++row) {
					int activeRowOffset = (int) (rowInterval.StartOffset +
					                             (row - rowInterval.Start));
					ActiveRow activeRow = this.activeRows[activeRowOffset];
					for (int colIntervalIndex = 0;
					     	colIntervalIndex < activeRow.IntervalCount;
					     	++colIntervalIndex) {
						int colIntervalOffset = (int)
												(activeRow.FirstIntervalOffset
							 					 + colIntervalIndex);
						Interval columnInterval = this.columnIntervals[
															colIntervalOffset];
						uint index = columnInterval.StartOffset;
						for (uint column = columnInterval.Start;
						    	column <= columnInterval.End; ++column) {
							yield return new Entry(new Location(row, column),
								                   index);
							index++;
						}
					}
				}
			}
		}
	}
}
