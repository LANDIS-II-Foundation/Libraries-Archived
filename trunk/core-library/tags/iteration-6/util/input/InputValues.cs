using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using IO = System.IO;

namespace Landis.Util
{
	public static class InputValues
	{
		private static Dictionary<string, object> readMethods;
		private static Dictionary<string, string> typeDescs;

		//---------------------------------------------------------------------

		static InputValues()
		{
			readMethods = new Dictionary<string, object>();
			typeDescs = new Dictionary<string, string>();

			NumberStyles integerStyle = NumberStyles.Integer |
										NumberStyles.AllowThousands;
			Register<byte, NumberStyles>(byte.Parse, integerStyle, "integer");
			Register<sbyte, NumberStyles>(sbyte.Parse, integerStyle, "integer");
			Register<short, NumberStyles>(short.Parse, integerStyle, "integer");
			Register<ushort, NumberStyles>(ushort.Parse, integerStyle, "integer");
			Register<int, NumberStyles>(int.Parse, integerStyle, "integer");
			Register<uint, NumberStyles>(uint.Parse, integerStyle, "integer");
			Register<long, NumberStyles>(long.Parse, integerStyle, "integer");
			Register<ulong, NumberStyles>(ulong.Parse, integerStyle, "integer");

			NumberStyles floatStyle = NumberStyles.Float |
									  NumberStyles.AllowThousands;
			Register<float, NumberStyles>(float.Parse, floatStyle, "number");
			Register<double, NumberStyles>(double.Parse, floatStyle, "number");

			Register<string>(String.Read, "string");
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Register a method to read a value of a specific type from a text
		/// text reader.
		/// </summary>
		public static void Register<T>(ReadMethod<T> method,
		                               string        typeDescription)
		{
			string typeFullName = typeof(T).FullName;
			readMethods[typeFullName] = method;
			typeDescs[typeFullName] = typeDescription;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Register a method to parse a word for a value of a specific type.
		/// </summary>
		public static void Register<T>(ParseMethod<T> method,
		                               string         typeDescription)
		{
			Register<T>(new ParseMethodWrapper<T>(method).Read,
			            typeDescription);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Register a 2-parameter method to parse a word for a value of a
		/// specific type.
		/// </summary>
		public static void Register<T, Parameter2Type>(ParseMethod2<T, Parameter2Type> method,
		                                               Parameter2Type                  parameter2,
		                                               string                          typeDescription)
		{
			Register<T>(new ParseMethod2Wrapper<T, Parameter2Type>(method, parameter2).Parse,
			            typeDescription);
		}

		//---------------------------------------------------------------------

		public static ReadMethod<T> GetReadMethod<T>()
		{
			try {
				object readMethod = readMethods[typeof(T).FullName];
				return (ReadMethod<T>) readMethod;
			}
			catch (KeyNotFoundException) {
				string mesg = string.Format("No parse or read method registered for {0}",
				                            typeof(T).FullName);
				throw new ApplicationException(mesg);
			}
		}

		//---------------------------------------------------------------------

		public static string GetDescription<T>()
		{
			try {
				return typeDescs[typeof(T).FullName];
			}
			catch (KeyNotFoundException) {
				return typeof(T).Name;
			}
		}

		//---------------------------------------------------------------------

		private static T GetStaticConstant<T>(string fieldName)
		{
			Type type = typeof(T);
			System.Reflection.FieldInfo field = type.GetField(fieldName);
			if (field.IsStatic && field.IsLiteral && field.FieldType == type)
				return (T) field.GetValue(null);
			throw new InvalidOperationException(type.FullName);
		}

		//---------------------------------------------------------------------

		public static T GetMinValue<T>()
		{
			return GetStaticConstant<T>("MinValue");
		}

		//---------------------------------------------------------------------

		public static T GetMaxValue<T>()
		{
			return GetStaticConstant<T>("MaxValue");
		}

		//---------------------------------------------------------------------

		public static string GetFormat<T>()
		{
			switch (Type.GetTypeCode(typeof(T))) {
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return "#,##0";

				case TypeCode.Single:
				case TypeCode.Double:
					return "g";

				default:
					throw new InvalidOperationException(typeof(T).FullName);
			}
		}

		//---------------------------------------------------------------------

		public static string GetNumericDescription<T>()
		{
			string signAndPrecision;
			switch (Type.GetTypeCode(typeof(T))) {
				case TypeCode.Byte:
					signAndPrecision = "unsigned 8-bit";
					break;
				case TypeCode.SByte:
					signAndPrecision = "8-bit";
					break;
				case TypeCode.Int16:
					signAndPrecision = "16-bit";
					break;
				case TypeCode.Int32:
					signAndPrecision = "32-bit";
					break;
				case TypeCode.Int64:
					signAndPrecision = "64-bit";
					break;
				case TypeCode.UInt16:
					signAndPrecision = "unsigned 16-bit";
					break;
				case TypeCode.UInt32:
					signAndPrecision = "unsigned 32-bit";
					break;
				case TypeCode.UInt64:
					signAndPrecision = "unsigned 64-bit";
					break;
				case TypeCode.Single:
					signAndPrecision = "single-precision";
					break;
				case TypeCode.Double:
					signAndPrecision = "double-precision";
					break;

				default:
					throw new InvalidOperationException(typeof(T).FullName);
			}
			return signAndPrecision + " " + GetDescription<T>();
		}
	}
}
