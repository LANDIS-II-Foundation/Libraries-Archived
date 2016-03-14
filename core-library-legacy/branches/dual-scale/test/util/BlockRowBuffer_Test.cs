// Copyright 2007 University of Wisconsin
// Author: James Domingo, UW-Madison, FLEL

using Landis.DualScale;
using NUnit.Framework;

namespace Landis.Test.Util
{
	[TestFixture]
	public class BlockRowBuffer_Test
	{
		[Test]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void NullLandscape()
		{
            BlockRowBuffer<int> buffer = new BlockRowBuffer<int>(null);
		}

		//---------------------------------------------------------------------

        private BlockRowBuffer<int> CreateBuffer(int landscapeColumns,
                                                 int blockSize)
        {
            MockLandscape landscape = new MockLandscape(1, landscapeColumns);
            landscape.BlockSize = blockSize;
            return new BlockRowBuffer<int>(landscape);
        }

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void GetColumn0()
		{
            BlockRowBuffer<int> buffer = CreateBuffer(100, 3);
            int i = buffer[0];
		}

		//---------------------------------------------------------------------

		[Test]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void SetColumn0()
		{
            BlockRowBuffer<int> buffer = CreateBuffer(100, 3);
            buffer[0] = -1;
		}

		//---------------------------------------------------------------------

        private void CreateAndTestBuffer(int landscapeColumns,
                                         int blockSize,
                                         int expectedBufferColumns)
        {
            BlockRowBuffer<int> buffer = CreateBuffer(landscapeColumns, blockSize);

            Assert.IsNotNull(buffer);
            Assert.AreEqual(expectedBufferColumns, buffer.Columns);
            for (int col = 1; col <= buffer.Columns; col++) {
                buffer[col] = col;
                Assert.AreEqual(col, buffer[col]);
            }
        }

		//---------------------------------------------------------------------

		[Test]
		public void Cols20_BlockSize4()
		{
            CreateAndTestBuffer(20, 4, 5);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Cols21_BlockSize4()
		{
            CreateAndTestBuffer(21, 4, 6);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Cols22_BlockSize4()
		{
            CreateAndTestBuffer(22, 4, 6);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Cols23_BlockSize4()
		{
            CreateAndTestBuffer(23, 4, 6);
		}

		//---------------------------------------------------------------------

		[Test]
		public void Cols24_BlockSize4()
		{
            CreateAndTestBuffer(24, 4, 6);
		}
    }
}
