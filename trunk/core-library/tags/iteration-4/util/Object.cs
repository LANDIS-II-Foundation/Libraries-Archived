namespace Landis.Util
{
	public static class Object
	{
		public enum CompareResult
		{
			ReferToSame,
			OneIsNull,
			DifferentInstances
		}

		//---------------------------------------------------------------------

		public static CompareResult Compare(object x,
											object y)
		{
			int nullCount = 0;
			if (x == null)
				nullCount++;
			if (y == null)
				nullCount++;
			switch (nullCount) {
				case 1:
					return CompareResult.OneIsNull;
				case 2:
					return CompareResult.ReferToSame;
			}
			if (x == y)
				return CompareResult.ReferToSame;
			else
				return CompareResult.DifferentInstances;
		}
	}
}
