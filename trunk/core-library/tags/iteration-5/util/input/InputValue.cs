namespace Landis.Util
{
	/// <summary>
	/// Input value.
	/// </summary>
	public class InputValue<T>
	{
		private T actualValue;
		private string valueAsStr;

		//---------------------------------------------------------------------

		/// <summary>
		/// The actual input value.
		/// </summary>
		public T Actual
		{
			get {
				return actualValue;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The string representation of the input value (usually entered by
		/// user).
		/// </summary>
		public string String
		{
			get {
				return valueAsStr;
			}
		}

		//---------------------------------------------------------------------

		public InputValue(T      actualValue,
		                  string valueAsStr)
		{
			this.actualValue = actualValue;
			this.valueAsStr = valueAsStr;
		}

		//---------------------------------------------------------------------

		public override string ToString()
		{
			return valueAsStr;
		}

		//---------------------------------------------------------------------

		public static implicit operator T(InputValue<T> inputValue)
		{
			return inputValue.Actual;
		}
	}
}
