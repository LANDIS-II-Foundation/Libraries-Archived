namespace Landis.Raster.GDAL
{
	public struct BlockLocation
	{
		public readonly int XOffset;
		public readonly int YOffset;

		//---------------------------------------------------------------------

		public BlockLocation(int xOffset,
		                     int yOffset)
		{
			XOffset = xOffset;
			YOffset = yOffset;
		}

		//---------------------------------------------------------------------

		public static bool operator==(BlockLocation a, BlockLocation b)
		{
			return (a.XOffset == b.XOffset) && (a.YOffset == b.YOffset);
		}

		//---------------------------------------------------------------------

		public static bool operator!=(BlockLocation a, BlockLocation b)
		{
			return (a.XOffset != b.XOffset) || (a.YOffset != b.YOffset);
		}

		//---------------------------------------------------------------------

		public override bool Equals(object obj)
		{
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType())
				return false;
			BlockLocation loc = (BlockLocation)obj;
			return this == loc;
		}

		//---------------------------------------------------------------------

		public override int GetHashCode()
		{
			return (int)(XOffset ^ YOffset);
		}

		//---------------------------------------------------------------------

		public override string ToString()
		{
			return "(" + XOffset + ", " + YOffset + ")";
		}
	}
}
