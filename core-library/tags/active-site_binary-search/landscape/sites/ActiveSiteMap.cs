using System.Collections.Generic;

using log4net;

namespace Landis.Landscape
{
	public class ActiveSiteMap
	{
		//---------------------------------------------------------------------

		internal struct Interval
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
					throw new System.ApplicationException(string.Format("value for end ({0}) < start ({1})", end, start));
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

			//	Search for a value in a sorted list of intervals.
			//	Return the offset associated with the value or null.
			public static uint? Search(uint			  value,
			                           List<Interval> intervals,
					                   int            start,
			                           int			  count)
			{
				while (count > 0) {
					//	Use a binary search, so divide the # of intervals in
					//	half and check the interval at this midway point.
					int left_half_count = count / 2;
					int middle = start + left_half_count;
					if (value < intervals[middle].Start) {
						//	Search the left half of the remaining intervals.
						//	Start index stays the same.
						count = left_half_count;
					}
					else if (intervals[middle].End < value) {
						//	Search the right half of the remaining intervals.
						start = middle + 1;
						count = count - (left_half_count + 1);
					}
					else {
						//	Value is in the middle interval, so compute its
						//	offset.
						return intervals[middle].StartOffset + (value - intervals[middle].Start);
					}
				}
				return null;
			}
		}

		//---------------------------------------------------------------------

		internal struct ActiveRow
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
					throw new System.ApplicationException("interval count is 0");
				this.intervalCount 		 = intervalCount;
				this.firstIntervalOffset = firstIntervalOffset;
			}
		}

		//---------------------------------------------------------------------

		private uint count;
		private Location? firstActive;
		private Location? firstInactive;
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

		public Location? FirstActive
		{
			get {
				return firstActive;
			}
		}

		//---------------------------------------------------------------------

		public Location? FirstInactive
		{
			get {
				return firstInactive;
			}
		}

		//---------------------------------------------------------------------

		internal List<Interval> RowIntervals
		{
			get {
				return rowIntervals;
			}
		}

		//---------------------------------------------------------------------

		internal List<ActiveRow> ActiveRows
		{
			get {
				return activeRows;
			}
		}

		//---------------------------------------------------------------------

		internal List<Interval> ColumnIntervals
		{
			get {
				return columnIntervals;
			}
		}

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
			this.firstActive = null;
			this.firstInactive = null;

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
						if (! this.firstActive.HasValue)
							this.firstActive = new Location(row, column);

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
						if (! this.firstInactive.HasValue)
							this.firstInactive = new Location(row, column);

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

			if (logger.IsDebugEnabled) {
				LogDebug("Active Site Map");
				LogDebug("");
				LogDebug("Input Grid: {0}", activeSites.Dimensions);
				LogDebug("");
				LogDebug("Row Intervals");
				LogDebug("  index: start to end (index of start row in active rows");
				for (int i = 0; i < rowIntervals.Count; i++) {
					Interval interval = rowIntervals[i];
					LogDebug("  {0}: {1} to {2} ({3})", i, interval.Start, interval.End, interval.StartOffset);
				}

				LogDebug("");
				LogDebug("Active Rows");
				LogDebug("  index: # column intervals, index of 1st column interval");
				for (int i = 0; i < activeRows.Count; i++) {
					ActiveRow activeRow = ActiveRows[i];
					LogDebug("  {0}: {1}, {2}", i, activeRow.IntervalCount, activeRow.FirstIntervalOffset);
				}

				LogDebug("");
				LogDebug("Column Intervals");
				LogDebug("  index: start to end (data index of start column");
				for (int i = 0; i < columnIntervals.Count; i++) {
					Interval interval = columnIntervals[i];
					LogDebug("  {0}: {1} to {2} ({3})", i, interval.Start, interval.End, interval.StartOffset);
				}
			}
		}

		//---------------------------------------------------------------------

		private static ILog logger;

		//---------------------------------------------------------------------

		private void LogDebug(string          message,
		                      params object[] mesgArgs)
		{
			logger.Debug(string.Format(message, mesgArgs));
		}

		//---------------------------------------------------------------------

		static ActiveSiteMap()
		{
			logger = LogManager.GetLogger(typeof(ActiveSiteMap));
		}

		//---------------------------------------------------------------------

		public uint this[Location location]
		{
			get {
				uint? rowOffset = Interval.Search(location.Row,
				                                  rowIntervals,
				                                  0, rowIntervals.Count);
				if (rowOffset == null)
					return this.count;
				ActiveRow activeRow = activeRows[(int)rowOffset.Value];
				uint? colOffset = Interval.Search(location.Column,
				                                  columnIntervals,
				                                  (int) activeRow.FirstIntervalOffset,
					                              (int) activeRow.IntervalCount);
				if (colOffset == null)
					return this.count;
				else
					return colOffset.Value;
			}
		}

		//---------------------------------------------------------------------

		public ActiveSiteMapEnumerator GetEnumerator()
		{
			return new ActiveSiteMapEnumerator(this);
		}
	}
}
