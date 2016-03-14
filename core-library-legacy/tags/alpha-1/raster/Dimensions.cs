namespace Landis.Raster
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
	}
}
