namespace Landis.Landscape
{
	//!  Building block -- base class for grids with a particular data type.
	public class GridWithType
		: Grid
	{
		private System.Type dataType;

		//---------------------------------------------------------------------

		public System.Type DataType {
			get {
				return dataType;
			}
		}

		//---------------------------------------------------------------------

            //!<  Create a grid.
		protected GridWithType(GridDimensions dimensions,
                               System.Type    dataType)
            : base(dimensions)
		{
			this.dataType = dataType;
		}

		//---------------------------------------------------------------------

            //!<  Create a grid.
		protected GridWithType(uint        rows,
					           uint        columns,
                               System.Type dataType)
            : base(rows, columns)
		{
			this.dataType = dataType;
		}
	}
}
