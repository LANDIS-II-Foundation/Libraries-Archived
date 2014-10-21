// Copyright 2005 University of Wisconsin

using Edu.Wisc.Forest.Flel.Util;

using FormatException = System.FormatException;

namespace Landis.Library.SiteHarvest
{
    /// <summary>
    /// Methods for parsing cohort age ranges.
    /// </summary>
    public static class AgeRangeParsing
    {
        private static ParseMethod<ushort> uShortParse;

        //---------------------------------------------------------------------

        static AgeRangeParsing()
        {
            //  Register the local method for parsing a cohort age or age range.
            InputValues.Register<AgeRange>(ParseAgeOrRange);
            Type.SetDescription<AgeRange>("cohort age or age range");
            uShortParse = InputValues.GetParseMethod<ushort>();
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

        /// <summary>
        /// Parses a word for a cohort age or an age range (format: age-age).
        /// </summary>
        public static AgeRange ParseAgeOrRange(string word)
        {
            int delimiterIndex = word.IndexOf('-');
            if (delimiterIndex == -1) {
                ushort age = ParseAge(word);
                if (age == 0)
                    throw new FormatException("Cohort age must be > 0");
                return new AgeRange(age, age);
            }

            string startAge = word.Substring(0, delimiterIndex);
            string endAge = word.Substring(delimiterIndex + 1);
            if (endAge.Contains("-"))
                throw new FormatException("Valid format for age range: #-#");
            if (startAge == "") {
                if (endAge == "")
                    throw new FormatException("The range has no start and end ages");
                else
                    throw new FormatException("The range has no start age");
            }
            ushort start = ParseAge(startAge);
            if (start == 0)
                throw new FormatException("The start age in the range must be > 0");
            if (endAge == "")
                    throw new FormatException("The range has no end age");
            ushort end = ParseAge(endAge);
            if (start > end)
                throw new FormatException("The start age in the range must be <= the end age");
            return new AgeRange(start, end);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Parses a cohort age from a text value.
        /// </summary>
        public static ushort ParseAge(string text)
        {
            try {
                return uShortParse(text);
            }
            catch (System.OverflowException) {
                throw new FormatException(text + " is too large for an age; max = 65,535");
            }
            catch (System.Exception) {
                throw new FormatException(text + " is not a valid integer");
            }
        }
    }
}