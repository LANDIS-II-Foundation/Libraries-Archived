using System.Diagnostics;
using IO = System.IO;

namespace Landis.Util
{
	public class InputVar<T>
		: InputVariable
	{
		private InputValue<T> myValue;
		private ReadMethod<T> readMethod;
		private int index;

		//---------------------------------------------------------------------

		public InputValue<T> Value
		{
			get {
				return myValue;
			}
		}

		//---------------------------------------------------------------------

		public ReadMethod<T> ReadMethod
		{
			set {
				Trace.Assert( value != null );
				readMethod = value;
			}
		}

		//---------------------------------------------------------------------

		public int Index
		{
			get {
				return index;
			}
		}

		//---------------------------------------------------------------------

		public InputVar(string name)
			: base(name)
		{
			myValue = null;
			readMethod = InputValues.GetReadMethod<T>();
			index = -1;
		}

		//---------------------------------------------------------------------

		public override void ReadValue(StringReader reader)
		{
			try {
				myValue = readMethod(reader, out index);
			}
			catch (System.Exception exception) {
				string message = string.Format("Error reading input value for {0}",
				                               Name);
				throw new InputVariableException(this, message, exception);
			}
		}
	}
}
