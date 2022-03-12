using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Typeable.Date.Functions
{
    public static class DateConverter
    {
        #region Private Variables

        /// <summary>
        /// To extract plus (+) operator from expression
        /// </summary>
        private static string _plusOperatorPattern = @"([+])";

        /// <summary>
        /// To minus plus (-) operator from expression
        /// </summary>
        private static string _minusOperatorPattern = @"([-])";

        /// <summary>
        /// Alphabets e.g D => Days, M => Months, Y => Years
        /// </summary>
        private static string _alphabetPattern = @"[a-zA-Z]";

        /// <summary>
        /// To extract separators e.g ',', '-', ' ', '/', '.'
        /// </summary>
        private static string _separatorsPattern = @"[^A-Za-z0-9]";

        /// <summary>
        /// Pattern to validate custom date expression
        /// </summary>
        private static string _dateExpressionPattern = @"(([+]|[-])([\d]{1,2})([Y]|[M]|[D])$)";

        /// <summary>
        /// Date separators, date, month and year are separated with one these characters
        /// </summary>
        private static char[] _dateSeparators = new char[] { '-', ',', '/', ' ', '.' };

        /// <summary>
        /// Abbreviations of month names
        /// </summary>
        private static string[] _months = new string[]
        {
            "Jan",
            "Feb",
            "Mar",
            "Apr",
            "May",
            "Jun",
            "Jul",
            "Aug",
            "Sep",
            "Oct",
            "Nov",
            "Dec"
        };

        #endregion

        /// <summary>
        /// Static array of date captions e.g Today, Tomorrow, etc
        /// </summary>
        public static string[] DateCaptions { get; set; } = new string[]
        {
            "today",
            "tomorrow",
            "yesterday",
            "end of month",
            "end of last month",
            "end of week",
            "end of last week",
            "start of month",
            "start of last month",
            "start of quarter",
            "end of quarter"
        };

        /// <summary>
        /// Convert caption date typed in words to date with formatting in 'yyyy-MM-dd' culture
        /// </summary>
        /// <param name="caption">Date caption e.g Today, Tomorrow, Dec-10-2021 + 5D, etc</param>
        /// <returns>Formatted date string</returns>
        public static string ConvertCaptionToDate(string caption)
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime convertedDate = DateTime.MinValue;

            // if month and date is input, then add year when case is valid
            if (caption.Split(_dateSeparators, StringSplitOptions.RemoveEmptyEntries).Length == 2)
            {
                char separator = caption.ToCharArray().FirstOrDefault(s => _dateSeparators.Contains(s));
                string modifiedCaption = caption.Replace(separator.ToString(), string.Empty);

                if (modifiedCaption.ToCharArray().All(s => char.IsDigit(s)) || IsContainMonth(caption))
                    caption += $"{separator}{currentDate.Year}";
            }

            if (DateTime.TryParse(caption, out DateTime dt))
                return dt.ToString("yyyy-MM-dd");

            switch (caption.Trim().ToLower())
            {
                case "today":
                    convertedDate = currentDate;
                    break;

                case "tomorrow":
                    convertedDate = currentDate.AddDays(1);
                    break;

                case "yesterday":
                    convertedDate = currentDate.AddDays(-1);
                    break;

                case "end of month":
                    convertedDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
                    break;

                case "end of last month":
                    convertedDate = new DateTime(currentDate.Year, currentDate.Month - 1, DateTime.DaysInMonth(currentDate.Year, currentDate.Month - 1));
                    break;

                case "end of week":
                    GetCalculatedDayOfWeek((int)currentDate.DayOfWeek, currentDate.Day, out int endDayOfWeek);
                    convertedDate = new DateTime(currentDate.Year, currentDate.Month, endDayOfWeek);
                    break;

                case "end of last week":
                    convertedDate = GetEndDayOfLastWeek(currentDate);
                    break;

                case "start of month":
                    convertedDate = new DateTime(currentDate.Year, currentDate.Month, 01);
                    break;

                case "start of last month":
                    convertedDate = new DateTime(currentDate.Year, currentDate.Month - 1, 01);
                    break;

                case "start of quarter":
                    GetStartMonthFromQuarter(currentDate.Month, out int startMonthOfQuarter);
                    convertedDate = new DateTime(currentDate.Year, startMonthOfQuarter, 1);
                    break;

                case "end of quarter":
                    GetEndMonthFromQuarter(currentDate.Month, out int endMonthOfQuarter);
                    convertedDate = new DateTime(currentDate.Year, endMonthOfQuarter, DateTime.DaysInMonth(currentDate.Year, endMonthOfQuarter));
                    break;

                default:
                    char @operator = '\0';
                    string splittedCaption = null;
                    string splittedExpression = null;

                    if (Regex.IsMatch(caption, _plusOperatorPattern))
                    {
                        @operator = '+';
                        splittedCaption = caption.Split(@operator, StringSplitOptions.RemoveEmptyEntries)[0];
                        splittedExpression = caption.Split(@operator, StringSplitOptions.RemoveEmptyEntries)[1];
                    }
                    else if (Regex.IsMatch(caption, _minusOperatorPattern) || caption.ToCharArray().Where(s => s == '-').Count() > 1)
                    {
                        @operator = '-';
                        string reversedCaption = ReverseString(caption);
                        int indexOfOperator = reversedCaption.IndexOf('-');
                        splittedExpression = ReverseString(reversedCaption.Substring(0, indexOfOperator));
                        splittedCaption = ReverseString(reversedCaption.Substring(indexOfOperator + 1, caption.Length - (indexOfOperator + 1)));
                    }
                    else
                        break;

                    string formattedExpr = (@operator + splittedExpression).Replace(" ", string.Empty);
                    Match dateExpr = Regex.Match(formattedExpr, _dateExpressionPattern, RegexOptions.IgnoreCase);

                    if (dateExpr.Success)
                        convertedDate = ExpressionToDate(dateExpr.Value, splittedCaption);
                    break;
            }

            return convertedDate == DateTime.MinValue ? string.Empty : convertedDate.ToString("yyyy-MM-dd");
        }

        #region Private Methods

        /// <summary>
        /// Reverses a string
        /// </summary>
        /// <param name="str">Input string value</param>
        /// <returns>Returns reversed string</returns>
        private static string ReverseString(string str)
        {
            char[] captionArray = str.ToCharArray();
            Array.Reverse(captionArray);
            return new string(captionArray);
        }

        /// <summary>
        /// Checks wether caption contains month abbreviation
        /// If yes => true
        /// Else no => false
        /// </summary>
        /// <param name="caption">Date caption e.g Today, Tomorrow, Dec-10-2021 + 5D, etc</param>
        /// <returns>Returns true or false</returns>
        private static bool IsContainMonth(string caption)
        {
            char[] characters = caption.ToCharArray().Where(s => char.IsLetter(s)).ToArray();
            string extractedLetters = new string(characters);

            if (_months.Contains(extractedLetters))
                return true;

            return false;
        }

        /// <summary>
        /// Gets start month of current quarter
        /// </summary>
        /// <param name="currentmonth">Current month</param>
        /// <param name="month">Out calculated month</param>
        private static void GetStartMonthFromQuarter(int currentmonth, out int month)
        {
            GetQuarter(currentmonth, out int quarter);

            if (quarter == 1)
                month = 1;
            else if (quarter == 2)
                month = 4;
            else if (quarter == 3)
                month = 7;
            else if (quarter == 4)
                month = 10;
            else
                throw new ArgumentException($"Invalid start quarter '{quarter}'. Date Converter Exception in 'TCube.Report.QueryBuilder.ExpressionToSql'.");
        }

        /// <summary>
        /// Gets end month of quarter
        /// </summary>
        /// <param name="currentmonth">Current month</param>
        /// <param name="month">Out calculated month</param>
        private static void GetEndMonthFromQuarter(int currentmonth, out int month)
        {
            GetQuarter(currentmonth, out int quarter);

            if (quarter == 1)
                month = 3;
            else if (quarter == 2)
                month = 6;
            else if (quarter == 3)
                month = 9;
            else if (quarter == 4)
                month = 12;
            else
                throw new ArgumentException($"Invalid end quarter '{quarter}'. Date Converter Exception in 'TCube.Report.QueryBuilder.ExpressionToSql'.");
        }

        /// <summary>
        /// Returns quarter of the year
        /// </summary>
        /// <param name="month">Month in number</param>
        private static void GetQuarter(int month, out int quarter)
        {
            if (month > 0 && month <= 3)
                quarter = 1;
            else if (month > 3 && month <= 6)
                quarter = 2;
            else if (month > 6 && month <= 9)
                quarter = 3;
            else if (month > 9 && month <= 12)
                quarter = 4;
            else
                throw new ArgumentException("Could not found quarter Date Converter Exception in 'TCube.Report.QueryBuilder.ExpressionToSql'.");
        }

        /// <summary>
        /// Calculates the day of week
        /// </summary>
        /// <param name="dayOfWeek">Current day of week</param>
        /// <param name="currentDay">Current day of month</param>
        /// <param name="mode">Either start or end</param>
        /// <param name="day">Out calculated day</param>
        private static void GetCalculatedDayOfWeek(int dayOfWeek, int currentDay, out int day)
        {
            if (dayOfWeek < 5)
            {
                while (dayOfWeek < 5)
                {
                    dayOfWeek++;
                    currentDay++;
                }
            }
            else if (dayOfWeek == 6)
            {
                currentDay--;
            }

            day = currentDay;
        }

        /// <summary>
        /// Gets last day of week
        /// </summary>
        /// <param name="currentDate">Current date</param>
        /// <param name="day">Out calculated end day of week</param>
        /// <param name="weekIndex">Weeks in month starts with 0</param>
        private static DateTime GetEndDayOfLastWeek(DateTime currentDate)
        {
            int endDayOfWeek = 0;

            for (int i = currentDate.Day; i > 0; i--)
            {
                if (new DateTime(currentDate.Year, currentDate.Month, i).DayOfWeek == DayOfWeek.Friday
                    && new DateTime(currentDate.Year, currentDate.Month, i) < currentDate.AddDays(-3))
                {
                    endDayOfWeek = i;
                    break;
                }
            }

            if (endDayOfWeek == 0)
            {
                DateTime localCurrentDate = currentDate;
                while (true)
                {
                    localCurrentDate = localCurrentDate.AddDays(-1);
                    if (localCurrentDate.DayOfWeek == DayOfWeek.Friday && localCurrentDate < currentDate.AddDays(-3))
                        break;
                }

                endDayOfWeek = localCurrentDate.Day;
                currentDate = localCurrentDate;
            }

            return new DateTime(currentDate.Year, currentDate.Month, endDayOfWeek);
        }

        /// <summary>
        /// Gets valid custom date by parsing expression into datetime.
        /// </summary>
        /// <param name="validExpr">Date expression must be valid input without white spaces</param>
        /// <param name="dateCaption">Date caption e.g Today, Tomorrow, Dec-10-2021 + 5D, etc</param>
        /// <returns>Returns custom date from expression</returns>
        private static DateTime ExpressionToDate(string validExpr, string dateCaption)
        {
            if (!string.IsNullOrEmpty(validExpr))
            {
                Match alphabet = Regex.Match(validExpr, _alphabetPattern);
                int variableNumber = int.Parse(validExpr.Split(alphabet.Value)[0]);
                DateTime.TryParse(ConvertCaptionToDate(dateCaption), out DateTime convertedDate);

                switch (alphabet.Value.ToUpper()[0])
                {
                    case 'D': // increment / decrement days
                        convertedDate = convertedDate.AddDays(variableNumber);
                        break;

                    case 'M': // increment / decrement months
                        convertedDate = convertedDate.AddMonths(variableNumber);
                        break;

                    case 'Y': // increment / decrement years
                        convertedDate = convertedDate.AddYears(variableNumber);
                        break;

                    default: // if no operator matched set date to DateTime MIN-VALUE
                        convertedDate = DateTime.MinValue;
                        break;
                }

                return convertedDate;
            }

            return DateTime.MinValue;
        }

        #endregion
    }
}