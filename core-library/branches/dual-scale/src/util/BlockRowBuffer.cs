// Copyright 2007 University of Wisconsin
// Author: James Domingo, UW-Madison, FLEL

using Edu.Wisc.Forest.Flel.Util;
using System;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.DualScale
{
    /// <summary>
    /// A buffer of data values for a single row of blocks on a landscape.
    /// </summary>
    /// <remarks>
    /// A row of blocks is a row of broad-scale sites.
    /// </remarks>
    public class BlockRowBuffer<T>
    {
        private int columns;
        private T[] values;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance based on the number of broad-scale
        /// columns in a landscape.
        /// </summary>
        public BlockRowBuffer(ILandscape landscape)
        {
            Require.ArgumentNotNull(landscape);
            columns = (int) Math.Ceiling(((double) landscape.Columns) / landscape.BlockSize);
            values = new T[columns + 1];
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The number of broad-scale columns in the buffer.
        /// </summary>
        public int Columns
        {
            get {
                return columns;
            }
        }

        //---------------------------------------------------------------------

        ///	<summary>
        /// Gets or sets the value for a particular column in the buffer.
        /// </summary>
        /// <exception cref="System.IndexOutOfRangeException">
        /// The column is < 1 or > # of columns in the buffer.
        /// </exception>
        public T this[int column]
        {
            get {
                if (column < 1)
                    throw new IndexOutOfRangeException("column cannot be < 1");
                return values[column];
            }

            set {
                if (column < 1)
                    throw new IndexOutOfRangeException("column cannot be < 1");
                values[column] = value;
            }
        }
    }
}
