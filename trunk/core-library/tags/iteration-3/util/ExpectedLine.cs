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
			System.IO.StreamReader sr = new System.IO.StreamReader(path);
			string line;
			int lineNumber = 0;
			while ((line = sr.ReadLine()) != null) {
				lineNumber++;
				Regex pattern = new Regex(@"^(\d+):(.*)");
				Match match = pattern.Match(line);
				if (! match.Success)
					throw MakeApplicationException(path, lineNumber,
					                               "Line does not start with "
					                               + "number and colon");
				int number = int.Parse(match.Groups[1].Value);
				string text = match.Groups[2].Value;
				if (number == 0)
					throw MakeApplicationException(path, lineNumber,
					                               "Expected line number must "
					                               + "be > 0");
				ExpectedLine expectedLine = new ExpectedLine(number, text);
				if (prevLineNum > 0 && expectedLine.Number <= prevLineNum) {
					string mesg = System.String.Format("Expected line number"
					                                   + " ({0}) <= expected"
					                                   + " line number ({1})"
					                                   + " on previous line",
					                                   expectedLine.Number,
					                                   prevLineNum);
					throw MakeApplicationException(path, lineNumber, mesg);
				}
                lines.Add(expectedLine);
                prevLineNum = expectedLine.Number;
			}
			sr.Close();
			return lines;
		}

		//---------------------------------------------------------------------

		private static
			System.ApplicationException MakeApplicationException(string path,
			                                                     int    line,
			                                                     string mesg)
		{
			string exceptionMesg = System.String.Format(
						"File \"{0}\", line {1}:  {2}", path, line, mesg);
			return new System.ApplicationException(exceptionMesg);
		}
	}
}
