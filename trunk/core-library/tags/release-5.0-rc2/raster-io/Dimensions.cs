namespace Landis.RasterIO
{
	public struct Dimensions
	{
		public int Rows;
		public int Columns;

		//---------------------------------------------------------------------

		public Dimensions(int rows,
		           		  int columns)
		{
			this.Rows = rows;
			this.Columns = columns;
		}

		//---------------------------------------------------------------------

		public override bool Equals(object obj)
		{
			if (obj is Dimensions) {
				Dimensions dims = (Dimensions) obj;
				return this == dims;
			}
			return false;
		}

		//---------------------------------------------------------------------

		public static bool operator==(Dimensions x,
		                              Dimensions y)
		{
			return (x.Rows == y.Rows) && (x.Columns == y.Columns);
		}

		//---------------------------------------------------------------------

		public static bool operator!=(Dimensions x,
		                              Dimensions y)
		{
			return !(x == y);
		}

		//---------------------------------------------------------------------

		public override int GetHashCode()
		{
			return Rows ^ Columns;
		}
	}
}
