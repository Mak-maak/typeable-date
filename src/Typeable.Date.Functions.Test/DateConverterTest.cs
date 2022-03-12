using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Typeable.Date.Functions.Test
{
    [TestClass]
    public class DateConverterTest
    {
        [TestMethod]
        public void Today_ConvertCaptionToDate_ReturnTodayDate()
        {
            var date = DateConverter.ConvertCaptionToDate("Today");
            Assert.AreEqual(DateTime.Now.Date.ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void Yesterday_ConvertCaptionToDate_ReturnYesterdayDate()
        {
            var date = DateConverter.ConvertCaptionToDate("Yesterday");
            Assert.AreEqual(DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void Tomorrow_ConvertCaptionToDate_ReturnTomorrowDate()
        {
            var date = DateConverter.ConvertCaptionToDate("Tomorrow");
            Assert.AreEqual(DateTime.Now.AddDays(1).Date.ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void EndOfMonth_ConvertCaptionToDate_ReturnEndOfMonthDate()
        {
            var date = DateConverter.ConvertCaptionToDate("End Of Month");
            DateTime dt = DateTime.Now;
            DateTime dtEndOfMonth = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
            Assert.AreEqual(dtEndOfMonth.Date.ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void EndOfLastMonth_ConvertCaptionToDate_ReturnEndOfLastMonthDate()
        {
            var date = DateConverter.ConvertCaptionToDate("End Of Last Month");
            DateTime dt = DateTime.Now;
            DateTime dtEndOfLastMonth = new DateTime(dt.Year, dt.Month - 1, DateTime.DaysInMonth(dt.Year, dt.Month - 1));
            Assert.AreEqual(dtEndOfLastMonth.ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void EndOfWeek_ConvertCaptionToDate_ReturnEndOfWeekDate()
        {
            var date = DateConverter.ConvertCaptionToDate("End Of Week");
            DateTime dt = DateTime.Now;
            GetCalculatedDayOfWeek((int)dt.DayOfWeek, dt.Day, out int day);
            DateTime dtEndOfWeek = new DateTime(dt.Year, dt.Month, day);
            Assert.AreEqual(dtEndOfWeek.ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void EndOfLastWeek_ConvertCaptionToDate_ReturnEndOfLastWeekDate()
        {
            var date = DateConverter.ConvertCaptionToDate("End Of Last Week");
            DateTime dt = DateTime.Now;
            DateTime dtEndOfWeek = GetEndDayOfLastWeek(dt);
            Assert.AreEqual(dtEndOfWeek.ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void StartOfMonth_ConvertCaptionToDate_ReturnStartOfMonthDate()
        {
            var date = DateConverter.ConvertCaptionToDate("Start Of Month");
            DateTime dt = DateTime.Now;
            DateTime dtStartOfMonth = new DateTime(dt.Year, dt.Month, 01);
            Assert.AreEqual(dtStartOfMonth.ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void StartOfLastMonth_ConvertCaptionToDate_ReturnStartOfLastMonthDate()
        {
            var date = DateConverter.ConvertCaptionToDate("Start Of Last Month");
            DateTime dt = DateTime.Now;
            DateTime dtStartOfLastMonth = new DateTime(dt.Year, dt.Month - 1, 01);
            Assert.AreEqual(dtStartOfLastMonth.ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void StartOfQuarter_ConvertCaptionToDate_ReturnStartOfQuarterDate()
        {
            var date = DateConverter.ConvertCaptionToDate("start of quarter");
            DateTime dt = DateTime.Now;
            DateTime dtStartMonthOfQuarter = new DateTime(dt.Year, GetMonthByQuarter(dt, "start"), 01);
            Assert.AreEqual(dtStartMonthOfQuarter.ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void EndOfQuarter_ConvertCaptionToDate_ReturnEndOfQuarterDate()
        {
            var date = DateConverter.ConvertCaptionToDate("end of quarter");
            DateTime dt = DateTime.Now;
            DateTime dtEndMonthOfQuarter = new DateTime(dt.Year, GetMonthByQuarter(dt, "end"), DateTime.DaysInMonth(dt.Year, GetMonthByQuarter(dt, "end")));
            Assert.AreEqual(dtEndMonthOfQuarter.ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void DayCaptionExpression_ConvertExpressionToIncrementDecrementDate_ReturnCorrectDate()
        {
            Assert.AreEqual(true, ManipulateDateWithExpression("5d", 5, '+', 'd'));
            Assert.AreEqual(true, ManipulateDateWithExpression("5d", 5, '-', 'd'));

            Assert.AreEqual(true, ManipulateDateWithExpression("5D", 5, '+', 'd'));
            Assert.AreEqual(true, ManipulateDateWithExpression("5D", 5, '-', 'd'));
        }

        [TestMethod]
        public void MonthCaptionExpression_ConvertExpressionToIncrementDecrementDate_ReturnCorrectDate()
        {
            Assert.AreEqual(true, ManipulateDateWithExpression("5m", 5, '+', 'm'));
            Assert.AreEqual(true, ManipulateDateWithExpression("5m", 5, '-', 'm'));

            Assert.AreEqual(true, ManipulateDateWithExpression("5M", 5, '+', 'm'));
            Assert.AreEqual(true, ManipulateDateWithExpression("5M", 5, '-', 'm'));
        }

        [TestMethod]
        public void YearCaptionExpression_ConvertExpressionToIncrementDecrementDate_ReturnCorrectDate()
        {
            Assert.AreEqual(true, ManipulateDateWithExpression("5y", 5, '+', 'y'));
            Assert.AreEqual(true, ManipulateDateWithExpression("5y", 5, '-', 'y'));

            Assert.AreEqual(true, ManipulateDateWithExpression("5Y", 5, '+', 'y'));
            Assert.AreEqual(true, ManipulateDateWithExpression("5Y", 5, '-', 'y'));
        }

        [TestMethod]
        public void DayDateExpression_ConvertExpressionToIncrementDecrementDate_ReturnCorrectDate()
        {
            DateTime dt;
            string date = DateConverter.ConvertCaptionToDate("Dec-11-2021 + 5d");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddDays(5).ToString("yyyy-MM-dd"), date);

            date = DateConverter.ConvertCaptionToDate("Dec-11-2021 + 5D");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddDays(5).ToString("yyyy-MM-dd"), date);

            date = DateConverter.ConvertCaptionToDate("Dec-11-2021 - 5d");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddDays(-5).ToString("yyyy-MM-dd"), date);

            date = DateConverter.ConvertCaptionToDate("Dec-11-2021 - 5D");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddDays(-5).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void MonthDateExpression_ConvertExpressionToIncrementDecrementDate_ReturnCorrectDate()
        {
            DateTime dt;
            string date = DateConverter.ConvertCaptionToDate("Dec-11-2021 + 5m");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddMonths(5).ToString("yyyy-MM-dd"), date);

            date = DateConverter.ConvertCaptionToDate("Dec-11-2021 + 5M");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddMonths(5).ToString("yyyy-MM-dd"), date);

            date = DateConverter.ConvertCaptionToDate("Dec-11-2021 - 5m");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddMonths(-5).ToString("yyyy-MM-dd"), date);

            date = DateConverter.ConvertCaptionToDate("Dec-11-2021 - 5M");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddMonths(-5).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void YearDateExpression_ConvertExpressionToIncrementDecrementDate_ReturnCorrectDate()
        {
            DateTime dt;
            string date = DateConverter.ConvertCaptionToDate("Dec-11-2021 + 5y");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddYears(5).ToString("yyyy-MM-dd"), date);

            date = DateConverter.ConvertCaptionToDate("Dec-11-2021 + 5Y");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddYears(5).ToString("yyyy-MM-dd"), date);

            date = DateConverter.ConvertCaptionToDate("Dec-11-2021 - 5y");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddYears(-5).ToString("yyyy-MM-dd"), date);

            date = DateConverter.ConvertCaptionToDate("Dec-11-2021 - 5Y");
            DateTime.TryParse("Dec-11-2021", out dt);
            Assert.AreEqual(dt.AddYears(-5).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void TypedDate_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("Dec-11-2021");
            Assert.AreEqual(new DateTime(2021, 12, 11).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void TypedDateSeparatedWithSpaces_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("Dec 11 2021");
            Assert.AreEqual(new DateTime(2021, 12, 11).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void TypedDateSeparatedWithSlash_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("Dec/11/2021");
            Assert.AreEqual(new DateTime(2021, 12, 11).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void TypedDateSeparatedWithComma_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("Dec,11,2021");
            Assert.AreEqual(new DateTime(2021, 12, 11).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void TypedDateSeparatedWithDot_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("Dec.11.2021");
            Assert.AreEqual(new DateTime(2021, 12, 11).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void DateExpressionWithDash_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("Dec-11-2021 - 5d");
            Assert.AreEqual(new DateTime(2021, 12, 11).AddDays(-5).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void DateExpressionWithSpace_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("Dec 11 2021 - 5 D");
            Assert.AreEqual(new DateTime(2021, 12, 11).AddDays(-5).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void DateExpressionWithSlash_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("Dec/11/2021 - 5d");
            Assert.AreEqual(new DateTime(2021, 12, 11).AddDays(-5).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void DateExpressionWithComma_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("Dec,11,2021 - 5d");
            Assert.AreEqual(new DateTime(2021, 12, 11).AddDays(-5).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void DateExpressionWithDot_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("Dec.11.2021 - 5d");
            Assert.AreEqual(new DateTime(2021, 12, 11).AddDays(-5).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void DateWithoutYear_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("11 12");
            Assert.AreEqual(new DateTime(2021, 12, 11).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void DayMonthWithoutYear_ConvertToDate_ReturnCorrectDate()
        {
            string date = DateConverter.ConvertCaptionToDate("Dec 11");
            Assert.AreEqual(new DateTime(2021, 12, 11).ToString("yyyy-MM-dd"), date);
        }

        [TestMethod]
        public void InvalidCaption_ConvertToDate_ReturnEmpty()
        {
            string date = DateConverter.ConvertCaptionToDate("Todayyy");
            Assert.AreEqual(string.Empty, date);
        }

        [TestMethod]
        public void InvalidDate_ConvertToDate_ReturnEmpty()
        {
            string date = DateConverter.ConvertCaptionToDate("Det-32-2021");
            Assert.AreEqual(string.Empty, date);
        }

        [TestMethod]
        public void InvalidExpressionPlusOperator_ConvertToDate_ReturnEmpty()
        {
            string date = DateConverter.ConvertCaptionToDate("Today + 5day");
            Assert.AreEqual(string.Empty, date);
        }

        [TestMethod]
        public void InvalidExpressionMinusOperator_ConvertToDate_ReturnEmpty()
        {
            string date = DateConverter.ConvertCaptionToDate("Today - 5day");
            Assert.AreEqual(string.Empty, date);
        }

        [TestMethod]
        public void InvalidDateWithoutYear_ConvertToDate_ReturnEmpty()
        {
            string date = DateConverter.ConvertCaptionToDate("11--12");
            Assert.AreEqual(string.Empty, date);
        }

        [TestMethod]
        public void InvalidDayMonthWithoutYear_ConvertToDate_ReturnEmpty()
        {
            string date = DateConverter.ConvertCaptionToDate("Nov--15");
            Assert.AreEqual(string.Empty, date);
        }

        #region Private Methods

        private bool ManipulateDateWithExpression(string expr, int days, char @operator, char alphabet)
        {
            List<bool> resultList = new();

            days = @operator == '-' ? (days * -1) : days;

            foreach (string caption in DateConverter.DateCaptions)
            {
                string date = DateConverter.ConvertCaptionToDate(caption + @operator + expr);

                DateTime.TryParse(DateConverter.ConvertCaptionToDate(caption), out DateTime dt);

                if (alphabet == 'd' || alphabet == 'D')
                    dt = dt.AddDays(days);
                else if (alphabet == 'm' || alphabet == 'M')
                    dt = dt.AddMonths(days);
                else if (alphabet == 'y' || alphabet == 'Y')
                    dt = dt.AddYears(days);

                bool status = dt.ToString("yyyy-MM-dd") == date;
                resultList.Add(status);
            }

            return resultList.ToArray().All(s => s == true);
        }

        private int GetMonthByQuarter(DateTime currentDate, string mode)
        {
            int quarter = 0;
            int monthFromQuarter = 0;

            if (currentDate.Month > 0 && currentDate.Month <= 3)
                quarter = 1;
            else if (currentDate.Month > 3 && currentDate.Month <= 6)
                quarter = 2;
            else if (currentDate.Month > 6 && currentDate.Month <= 9)
                quarter = 3;
            else if (currentDate.Month > 9 && currentDate.Month <= 12)
                quarter = 4;

            if (mode == "start")
            {
                if (quarter == 1)
                    monthFromQuarter = 1;
                else if (quarter == 2)
                    monthFromQuarter = 4;
                else if (quarter == 3)
                    monthFromQuarter = 7;
                else if (quarter == 4)
                    monthFromQuarter = 10;
            }
            else if (mode == "end")
            {
                if (quarter == 1)
                    monthFromQuarter = 3;
                else if (quarter == 2)
                    monthFromQuarter = 6;
                else if (quarter == 3)
                    monthFromQuarter = 9;
                else if (quarter == 4)
                    monthFromQuarter = 12;
            }

            return monthFromQuarter;
        }

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

        #endregion
    }
}