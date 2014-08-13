// This file is part of the Input Parameters library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/parameters/trunk/

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Library.Parameters
{
    /// <summary>
    /// Extension methods for InputValue objects.
    /// </summary>
    public static class InputValue_ExtensionMethods
    {
        /// <summary>
        /// Parses a string to a designated type and throws an error when it is unable to do so
        /// the advantage over C# designated parse functions is that it avoids the throw catch structures in the code
        /// </summary>
        private static T TryParse<T>(string value, string label)
             where T : System.IComparable<T>
         {
            T return_value;
            try
            {
                return_value = (T)System.Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                throw new System.Exception("Expecting type " + typeof(T) + " for " + label);
            }
            return return_value;
        }

        /// <summary>
        /// Is x &lt; y ?
        /// </summary>
        public static bool LessThan<T>(T x, T y)
            where T : System.IComparable<T>
        {
            return x.CompareTo(y) < 0;
        }

        //--------------------------------------------------------------------

        /// <summary>
        /// Is x > y ?
        /// </summary>
        public static bool GreaterThan<T>(T x, T y)
            where T : System.IComparable<T>
        {
            return x.CompareTo(y) > 0;
        }

        //--------------------------------------------------------------------

        private static string MakeExceptionMessage<T>(string parameterName,
                                                      T value,
                                                      string error,
                                                      params object[] errorArgs)
        {
            string message = string.Format("Input value {0}", value);
            if (parameterName != null)
                message += " for " + parameterName;
            message += " is " + string.Format(error, errorArgs);
            return message;
        }

        //--------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputValue"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static T CheckGreaterThanOrEqual<T>(this InputValue<T> inputValue, T min)
            where T : System.IComparable<T>
        { 
            return CheckGreaterThanOrEqual(inputValue,min,inputValue.String);
        }
        /// <summary>
        /// Check if an input value is > or = a minimum value.
        /// </summary>
        /// <example>
        /// This example checks if an int input value is positive.
        /// <code>
        /// InputValue&lt;int> fooParam = // read from input
        /// int foo = fooParam.CheckGreaterThanOrEqual(1, "FooParameter");
        /// </code>
        /// </example>
        /// <returns>The actual input value if it > or = the minimum.</returns>
        /// <exception cref="InputValueException">
        /// Thrown if the value is &lt; than the minimum.</exception>
        public static T CheckGreaterThanOrEqual<T>(this T inputValue,
                                                   T min,
                                                   string parameterName)
            where T : System.IComparable<T>
        {
            if (LessThan(inputValue, min))
            {
                string message = MakeExceptionMessage(parameterName, inputValue,
                                                      "< {0}", min);
                //string message = (parameterName == null)
                //    ? "Input value {0} is < {1}"
                //    : "Input value for " + parameterName + " {0} is < {1}";
                throw new InputValueException(parameterName, message);
            }
            return inputValue;
        }
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputValue"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static T CheckGreaterThanOrEqual<T>(this  InputVar<string> inputValue,
                                                T min)
            where T : System.IComparable<T>
        {
            T value = TryParse<T>(inputValue.Value.Actual.ToString(), inputValue.Name);
            return CheckGreaterThanOrEqual<T>(value, min, inputValue.Name);
        }


        //--------------------------------------------------------------------
        /// <summary>
        /// Check if an input value is &lt; or = a maximum value.
        /// </summary>
        /// <example>
        /// This example checks if a float input value is no more than 0.75.
        /// <code>
        /// InputValue&lt;float> fooParam = // read from input
        /// float foo = fooParam.CheckLessThanOrEqual(0.75, "FooParameter");
        /// </code>
        /// </example>
        /// <returns>The actual input value if it &lt; or = the maximum.</returns>
        /// <exception cref="InputValueException">
        /// Thrown if the value is > than the maximum.</exception>
        public static T CheckLessThanOrEqual<T>(this  InputValue<T> inputValue,
                                                T max)
            where T : System.IComparable<T>
        {
            return CheckLessThanOrEqual<T>(inputValue.Actual, max, inputValue.String);
        }
         
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputValue"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static T CheckGreaterThanOrEqual<T>(this  InputValue<string> inputValue,
                                                T min)
            where T : System.IComparable<T>
        {
            T value = TryParse<T>(inputValue, inputValue.String);
            return CheckGreaterThanOrEqual<T>(value, min, inputValue.String);
        }
        /// <summary>
        /// Check if an input value is &lt; or = a maximum value.
        /// </summary>
        /// <example>
        /// This example checks if a float input value is no more than 0.75.
        /// <code>
        /// InputValue&lt;float> fooParam = // read from input
        /// float foo = fooParam.CheckLessThanOrEqual(0.75, "FooParameter");
        /// </code>
        /// </example>
        /// <returns>The actual input value if it &lt; or = the maximum.</returns>
        /// <exception cref="InputValueException">
        /// Thrown if the value is > than the maximum.</exception>
        public static T CheckLessThanOrEqual<T>(this  T inputValue,
                                                T max,
                                                string parameterName)
            where T : System.IComparable<T>
        {
            if (GreaterThan(inputValue, max))
            {
                string message = MakeExceptionMessage(parameterName, inputValue, "> {0}", max);

                throw new InputValueException(parameterName, message);
            }
            return inputValue;
        }

        /// <summary>
        /// Check if an input value is within a specified range.
        /// </summary>
        /// <example>
        /// This example checks if an int input value is between 1 and 5.
        /// <code>
        /// InputValue&lt;int> fooParam = // read from input
        /// int foo = fooParam.CheckInRange(1, 5, "FooParameter");
        /// </code>
        /// </example>
        /// <returns>The actual input value if it is within range.</returns>
        /// <exception cref="InputValueException">
        /// Thrown if the value is outside the range.</exception>
        public static T CheckInRange<T>(T inputValue,
                                        T min,
                                        T max,
                                        string parameterName)
            where T : System.IComparable<T>
        {
            if (LessThan(inputValue, min) || GreaterThan(inputValue, max))
            {
                string message = MakeExceptionMessage(parameterName, inputValue, "not between {0:0.0} and {1:0.0}", min, max);


                throw new InputValueException(parameterName, message);
            }
            return inputValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputValue"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static T CheckInRange<T>(this  InputVar<string> inputValue,
                                        T min,
                                        T max)
            where T : System.IComparable<T>
        {
            T value = TryParse<T>(inputValue.Value.Actual.ToString(), inputValue.Name);
            if (LessThan(value, min) || GreaterThan(value, max))
            {
                string message = MakeExceptionMessage(inputValue.Name, inputValue.Value, "not between {0:0.0} and {1:0.0}", min, max);


                throw new InputValueException(inputValue.Name, message);
            }
            return value;
        }
    }
}
