using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Landis.Util
{
	public struct ExpectedLine
	{
		public readonly int Number;
		public readonly string Text;

		//---------------------------------------------------------------------

		public ExpectedLine(int    number,
		                    string text)
		{
			this.Number = number;
			this.Text   = text;
		}

		//---------------------------------------------------------------------

		public static List<ExpectedLine> ReadLines(string path)
		{
			List<ExpectedLine> lines = new List<ExpectedLine>();
			int prevLineNum = 0;
			LineReader reader = new FileLineReader(path);
			string line;
			while ((line = reader.ReadLine()) != null) {
				Regex pattern = new Regex(@"^(\d+):(.*)");
				Match match = pattern.Match(line);
				if (! match.Success)
					throw new LineReaderException(reader,
					                              "Line does not start with a number and colon");
				int number = int.Parse(match.Groups[1].Value);
				string text = match.Groups[2].Value;
				if (number == 0)
					throw new LineReaderException(reader,
					                              "The expected line number must be > 0");
				ExpectedLine expectedLine = new ExpectedLine(number, text);
				if (prevLineNum > 0 && expectedLine.Number <= prevLineNum) {
					throw new LineReaderException(reader,
					                              "Expected line number ({0}) \u2264 expected line number ({1}) on previous line",
					                              expectedLine.Number,
					                              prevLineNum);
				}
                lines.Add(expectedLine);
                prevLineNum = expectedLine.Number;
			}
			reader.Close();
			return lines;
		}

		//---------------------------------------------------------------------
/*
		private static
			System.ApplicationException MakeApplicationException(string path,
			                                                     int    line,
			                                                     string mesg)
		{
			string exceptionMesg = System.String.Format(
						"File \"{0}\", line {1}:  {2}", path, line, mesg);
			return new System.ApplicationException(exceptionMesg);
		}
*/	}
}
