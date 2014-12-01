/*
 * Copyright 2008 Green Code LLC
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using Landis.Library.SiteHarvest;
using System.Collections.Generic;
using System.Text;

namespace Landis.Library.BiomassHarvest
{
    /// <summary>
    /// Static class for partial thinning.
    /// </summary>
    public static class PartialThinning
    {
        public static PartialCohortSelectors CohortSelectors { get; private set; }
        private static IDictionary<ushort, Percentage> percentages;

        //---------------------------------------------------------------------

        static PartialThinning()
        {
            // Force the harvest library to register its read method for age
            // ranges.  Then replace it with this project's read method that
            // handles percentages for partial thinning.
            AgeRangeParsing.InitializeClass();
            InputValues.Register<AgeRange>(PartialThinning.ReadAgeOrRange);

            percentages = new Dictionary<ushort, Percentage>();
            CohortSelectors = new PartialCohortSelectors();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <remarks>
        /// Client code can use this method to explicitly control when the
        /// class' static constructor is invoked.
        /// </remarks>
        public static void InitializeClass()
        {
        }

        //---------------------------------------------------------------------

        public static InputValueException MakeInputValueException(string value,
                                                                  string message)
        {
            return new InputValueException(value,
                                           string.Format("\"{0}\" is not a valid percentage for partial thinning", value),
                                           new MultiLineText(message));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a percentage for partial thinning of a cohort age or age
        /// range.
        /// </summary>
        /// <remarks>
        /// The percentage is bracketed by parentheses.
        /// </remarks>
        public static InputValue<Percentage> ReadPercentage(StringReader reader,
                                                            out int      index)
        {
            TextReader.SkipWhitespace(reader);
            index = reader.Index;

            //  Read left parenthesis
            int nextChar = reader.Peek();
            if (nextChar == -1)
                throw new InputValueException();  // Missing value
            if (nextChar != '(')
                throw MakeInputValueException(TextReader.ReadWord(reader),
                                              "Value does not start with \"(\"");
            StringBuilder valueAsStr = new StringBuilder();
            valueAsStr.Append((char) (reader.Read()));

            //  Read whitespace between '(' and percentage
            valueAsStr.Append(ReadWhitespace(reader));

            //  Read percentage
            string word = ReadWord(reader, ')');
            if (word == "")
                throw MakeInputValueException(valueAsStr.ToString(),
                                              "No percentage after \"(\"");
            valueAsStr.Append(word);
            Percentage percentage;
            try {
                percentage = Percentage.Parse(word);
            }
            catch (System.FormatException exc) {
                throw MakeInputValueException(valueAsStr.ToString(),
                                              exc.Message);
            }
            if (percentage < 0.0 || percentage > 1.0)
                throw MakeInputValueException(valueAsStr.ToString(),
                                              string.Format("{0} is not between 0% and 100%", word));

            //  Read whitespace and ')'
            valueAsStr.Append(ReadWhitespace(reader));
            char? ch = TextReader.ReadChar(reader);
            if (! ch.HasValue)
                throw MakeInputValueException(valueAsStr.ToString(),
                                              "Missing \")\"");
            valueAsStr.Append(ch.Value);
            if (ch != ')')
                throw MakeInputValueException(valueAsStr.ToString(),
                                              string.Format("Value ends with \"{0}\" instead of \")\"", ch));

            return new InputValue<Percentage>(percentage, valueAsStr.ToString());
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads whitespace from a string reader.
        /// </summary>
        public static string ReadWhitespace(StringReader reader)
        {
            StringBuilder whitespace = new StringBuilder();
            int i = reader.Peek();
            while (i != -1 && char.IsWhiteSpace((char) i)) {
                whitespace.Append((char) reader.Read());
                i = reader.Peek();
            }
            return whitespace.ToString();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a word from a string reader.
        /// </summary>
        /// <remarks>
        /// The word is terminated by whitespace, the end of input, or a
        /// particular delimiter character.
        /// </remarks>
        public static string ReadWord(StringReader reader,
                                      char         delimiter)
        {
            StringBuilder word = new StringBuilder();
            int i = reader.Peek();
            while (i != -1 && ! char.IsWhiteSpace((char) i) && i != delimiter) {
                word.Append((char) reader.Read());
                i = reader.Peek();
            }
            return word.ToString();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a cohort age or a range of ages (format: age-age) followed
        /// by an optional percentage for partial thinning.
        /// </summary>
        /// <remarks>
        /// The optional percentage is bracketed by parenthesis.
        /// </remarks>
        public static InputValue<AgeRange> ReadAgeOrRange(StringReader reader,
                                                          out int      index)
        {
            TextReader.SkipWhitespace(reader);
            index = reader.Index;

            string word = ReadWord(reader, '(');
            if (word == "")
                throw new InputValueException();  // Missing value

            AgeRange ageRange = AgeRangeParsing.ParseAgeOrRange(word);

            //  Does a percentage follow?
            TextReader.SkipWhitespace(reader);
            if (reader.Peek() == '(') {
                int ignore;
                InputValue<Percentage> percentage = ReadPercentage(reader, out ignore);
                percentages[ageRange.Start] = percentage;
            }

            return new InputValue<AgeRange>(ageRange, word);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates and stores a specific-ages cohort selector for a species
        /// if at least one percentage was found among the list of ages and
        /// ranges that were read.
        /// </summary>
        /// <returns>
        /// True if a selector was created and stored in the CohortSelectors
        /// property.  False is returned if no selector was created because
        /// there were no percentages read for any age or range.
        /// </returns>
        public static bool CreateCohortSelectorFor(ISpecies species,
                                                   IList<ushort> ages,
                                                   IList<AgeRange> ageRanges)
        {
            if (percentages.Count == 0)
                return false;
            else
            {
                CohortSelectors[species] = new SpecificAgesCohortSelector(ages, ageRanges, percentages);
                percentages.Clear();
                return true;
            }
        }
    }
}
