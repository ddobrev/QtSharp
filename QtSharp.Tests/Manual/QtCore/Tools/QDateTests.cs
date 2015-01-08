using System;
using NUnit.Framework;
using QtCore;
using QtCore.Qt;

namespace QtSharp.Tests.Manual.QtCore.Tools
{
    [TestFixture]
    public class QDateTests
    {
        private QDate _qDate;

        [SetUp]
        public void Init()
        {
            // TODO: Add Init code.
            _qDate = new QDate(2014, 9, 20);
        }

        [TearDown]
        public void Dispose()
        {
            // TODO: Add tear down code.
            _qDate.Dispose();
        }

        #region Ctor
        [Test]
        public void TestEmptyConstructor()
        {
            var s = new QDate();
            Assert.NotNull(s.__Instance);
        }

        [Test]
        public void TestDateConstructor()
        {
            var s = new QDate(2014, 9, 20);

            Assert.AreEqual(2014, s.Year);
            Assert.AreEqual(9, s.Month);
            Assert.AreEqual(20, s.Day);
        }
        #endregion

        [Test]
        public void TestAddDays()
        {
            var i = 5;
            using (var newDate = _qDate.AddDays(i))
            {
                Assert.AreEqual(20 + i, newDate.Day);
            }
        }

        [Test]
        public void TestAddMonths()
        {
            var i = 2;

            using (var newDate = _qDate.AddMonths(i))
            {
                Assert.AreEqual(9 + i, newDate.Month);                
            }
        }

        [Test]
        public void TestAddYears()
        {
            var i = 2;

            using (var newDate = _qDate.AddYears(i))
            {
                Assert.AreEqual(2014 + i, newDate.Year);                
            }
        }

        [Test]
        public void TestCurrentDate()
        {
            var net = DateTime.Now;
            var q = QDate.CurrentDate;

            Assert.AreEqual(net.Day, q.Day);
            Assert.AreEqual(net.Month, q.Month);
            Assert.AreEqual(net.Year, q.Year);
            Assert.AreEqual((int) net.DayOfWeek, q.DayOfWeek == 7 ? 0 : q.DayOfWeek);
            Assert.AreEqual(net.DayOfYear, q.DayOfYear);
        }

        [Test]
        public void Test_Day_DayOfWeek_DayOfYear_Month_Year()
        {
            var net = new DateTime(2014, 09, 20);
            var q = new QDate(2014, 09, 20);

            Assert.AreEqual(net.Day, q.Day);
            Assert.AreEqual(net.Month, q.Month);
            Assert.AreEqual(net.Year, q.Year);
            Assert.AreEqual((int)net.DayOfWeek, q.DayOfWeek);
            Assert.AreEqual(net.DayOfYear, q.DayOfYear);
            Assert.AreEqual(DateTime.DaysInMonth(2014, 09), q.DaysInMonth);
            Assert.AreEqual(
                (new DateTime(2014, 12, 31).Subtract(new DateTime(2014, 01, 01)).TotalDays) + 1,
                q.DaysInYear);
        }

        //[Ignore("Bug")]
        [Test]
        public void TestDaysTo()
        {
            var q = new QDate(1995, 5, 17);
            var q2 = new QDate(1995, 5, 20);

            var res1 = q.DaysTo(q2);
            Assert.AreEqual(3, res1);

            var res2 = q2.DaysTo(q);
            Assert.AreEqual(-3, res2);
        }

        [Test]
        public void TestFromJulianDay()
        {
            var q = QDate.FromJulianDay(2456921);
            var s = _qDate;

            Assert.AreEqual(_qDate.Day, q.Day);
            Assert.AreEqual(_qDate.Year, q.Year);
            Assert.AreEqual(_qDate.Month, q.Month);
        }

        [Test]
        public void TestToJulianDay()
        {
            var q = _qDate.ToJulianDay();

            Assert.AreEqual(2456921, q);
        }

        #region FromString
        [Test]
        public void TestFromStringWithDateFormat()
        {
            var f = DateFormat.ISODate;
            var d = QDate.FromString("2014-09-20", f);

            Assert.AreEqual(20, d.Day);
            Assert.AreEqual(09, d.Month);
            Assert.AreEqual(2014, d.Year);
        }

        [Test]
        public void TestFromStringWithStringFormat()
        {
            var d = QDate.FromString("1MM12car2003", "d'MM'MMcaryyyy");

            Assert.AreEqual(1, d.Day);
            Assert.AreEqual(12, d.Month);
            Assert.AreEqual(2003, d.Year);
        }

        [Test]
        public void TestFromStringWithInvalidStringFormatReturningDefaultValues()
        {
            var d1 = QDate.FromString("1.30", "M.d");
            Assert.AreEqual(30, d1.Day);
            Assert.AreEqual(1, d1.Month);
            Assert.AreEqual(1900, d1.Year);

            var d2 = QDate.FromString("20000110", "yyyyMMdd");
            Assert.AreEqual(10, d2.Day);
            Assert.AreEqual(1, d2.Month);
            Assert.AreEqual(2000, d2.Year);

            var d3 = QDate.FromString("20000110", "yyyyMd");
            Assert.AreEqual(10, d3.Day);
            Assert.AreEqual(1, d3.Month);
            Assert.AreEqual(2000, d3.Year);
        }
        #endregion

        [Test]
        public unsafe void TestGetDate()
        {
            int d = 0;
            int m = 0;
            int y = 0;

            _qDate.GetDate(&y, &m, &d);

            Assert.AreEqual(_qDate.Day, d);
            Assert.AreEqual(_qDate.Month, m);
            Assert.AreEqual(_qDate.Year, y);
        }

        [Test]
        public void TestIsLeapYear()
        {
            var net = DateTime.IsLeapYear(2014);
            var q = QDate.IsLeapYear(2014);
            Assert.AreEqual(net, q);

            var net2 = DateTime.IsLeapYear(2040);
            var q2 = QDate.IsLeapYear(2040);
            Assert.AreEqual(net2, q2);
        }

        [Test]
        public void TestIsValid_StaticFunc()
        {
            var q = QDate.IsValid(2002, 5, 17);
            Assert.IsTrue(q);

            var q2 = QDate.IsValid(2002, 2, 30);
            Assert.IsFalse(q2);

            var q3 = QDate.IsValid(2004, 2, 29);
            Assert.IsTrue(q3);

            var q4 = QDate.IsValid(2000, 2, 29);
            Assert.IsTrue(q4);

            var q5 = QDate.IsValid(2006, 2, 29);
            Assert.IsFalse(q5);

            var q6 = QDate.IsValid(2100, 2, 29);
            Assert.IsFalse(q6);

            var q7 = QDate.IsValid(1202, 6, 6);
            Assert.IsTrue(q7);
        }

        [Test]
        public void TestIsNull()
        {
            var q = new QDate(2002, 5, 17);
            Assert.IsFalse(q.IsNull);

            var q2 = new QDate(2002, 2, 30);
            Assert.IsTrue(q2.IsNull);

            var q3 = new QDate(2004, 2, 29);
            Assert.IsFalse(q3.IsNull);

            var q4 = new QDate(2000, 2, 29);
            Assert.IsFalse(q4.IsNull);

            var q5 = new QDate(2006, 2, 29);
            Assert.IsTrue(q5.IsNull);

            var q6 = new QDate(2100, 2, 29);
            Assert.IsTrue(q6.IsNull);

            var q7 = new QDate(1202, 6, 6);
            Assert.IsFalse(q7.IsNull);
        }

        [Test]
        [Culture("de")]
        public void TestLongDayNameDe()
        {

            var q1 = QDate.LongDayName(1);
            Assert.AreEqual("Montag", q1);

            var q2 = QDate.LongDayName(2);
            Assert.AreEqual("Dienstag", q2);

            var q3 = QDate.LongDayName(3);
            Assert.AreEqual("Mittwoch", q3);

            var q4 = QDate.LongDayName(4);
            Assert.AreEqual("Donnerstag", q4);

            var q5 = QDate.LongDayName(5);
            Assert.AreEqual("Freitag", q5);

            var q6 = QDate.LongDayName(6);
            Assert.AreEqual("Samstag", q6);

            var q7 = QDate.LongDayName(7);
            Assert.AreEqual("Sonntag", q7);
        }

        [Test]
        [Culture("en")]
        public void TestLongDayNameEn()
        {

            var q1 = QDate.LongDayName(1);
            Assert.AreEqual("Monday", q1);

            var q2 = QDate.LongDayName(2);
            Assert.AreEqual("Tuesday", q2);

            var q3 = QDate.LongDayName(3);
            Assert.AreEqual("Wednesday", q3);

            var q4 = QDate.LongDayName(4);
            Assert.AreEqual("Thursday", q4);

            var q5 = QDate.LongDayName(5);
            Assert.AreEqual("Friday", q5);

            var q6 = QDate.LongDayName(6);
            Assert.AreEqual("Saturday", q6);

            var q7 = QDate.LongDayName(7);
            Assert.AreEqual("Sunday", q7);
        }

        [Test]
        [Culture("de")]
        public void TestLongMonthNameDe()
        {

            var q1 = QDate.LongMonthName(1);
            Assert.AreEqual("Januar", q1);

            var q2 = QDate.LongMonthName(7);
            Assert.AreEqual("Juli", q2);

            var q3 = QDate.LongMonthName(12);
            Assert.AreEqual("Dezember", q3);
        }

        [Test]
        [Culture("en")]
        public void TestLongMonthNameEn()
        {

            var q1 = QDate.LongMonthName(1);
            Assert.AreEqual("January", q1);

            var q2 = QDate.LongMonthName(7);
            Assert.AreEqual("July", q2);

            var q3 = QDate.LongMonthName(12);
            Assert.AreEqual("December", q3);
        }

        [Test]
        public void TestMonth()
        {
            var m = _qDate.Month;
            Assert.AreEqual(9, m);
        }

        [Test]
        public void TestSetDate()
        {
            var m = _qDate.SetDate(2000, 01, 01);

            Assert.IsTrue(m);

            Assert.AreEqual(01, _qDate.Day);
            Assert.AreEqual(01, _qDate.Month);
            Assert.AreEqual(2000, _qDate.Year);
        }

        [Test]
        [Culture("de")]
        public void TestShortDayNameDe()
        {
            var q1 = QDate.ShortDayName(1);
            Assert.AreEqual("Mo", q1);

            var q2 = QDate.ShortDayName(2);
            Assert.AreEqual("Di", q2);

            var q3 = QDate.ShortDayName(3);
            Assert.AreEqual("Mi", q3);

            var q4 = QDate.ShortDayName(4);
            Assert.AreEqual("Do", q4);

            var q5 = QDate.ShortDayName(5);
            Assert.AreEqual("Fr", q5);

            var q6 = QDate.ShortDayName(6);
            Assert.AreEqual("Sa", q6);

            var q7 = QDate.ShortDayName(7);
            Assert.AreEqual("So", q7);
        }

        [Test]
        [Culture("en")]
        public void TestShortDayNameEn()
        {

            var q1 = QDate.ShortDayName(1);
            Assert.AreEqual("Mon", q1);

            var q2 = QDate.ShortDayName(2);
            Assert.AreEqual("Tue", q2);

            var q3 = QDate.ShortDayName(3);
            Assert.AreEqual("Wed", q3);

            var q4 = QDate.ShortDayName(4);
            Assert.AreEqual("Thu", q4);

            var q5 = QDate.ShortDayName(5);
            Assert.AreEqual("Fri", q5);

            var q6 = QDate.ShortDayName(6);
            Assert.AreEqual("Sat", q6);

            var q7 = QDate.ShortDayName(7);
            Assert.AreEqual("Sun", q7);
        }

        [Test]
        [Culture("de")]
        public void TestShortMonthNameDe()
        {
            var q1 = QDate.LongMonthName(1);
            Assert.AreEqual("Januar", q1);

            var q2 = QDate.LongMonthName(7);
            Assert.AreEqual("Juli", q2);

            var q3 = QDate.LongMonthName(12);
            Assert.AreEqual("Dezember", q3);
        }

        [Test]
        [Culture("en")]
        public void TestShortMonthNameEn()
        {
            var q1 = QDate.LongMonthName(1);
            Assert.AreEqual("Jan", q1);

            var q2 = QDate.LongMonthName(7);
            Assert.AreEqual("Jul", q2);

            var q3 = QDate.LongMonthName(12);
            Assert.AreEqual("Dec", q3);
        }

        [Test]
        public void TestToString()
        {
            var q = new QDate(1969, 07, 20);

            var s1 = q.ToString("dd.MM.yyyy");

            Assert.AreEqual("20.07.1969", s1);
        }

        [Test]
        public void TestToStringWithDateFormat()
        {
            var f = DateFormat.ISODate;

            var q = new QDate(1969, 07, 20);

            var s1 = q.ToString(f);

            Assert.AreEqual("1969-07-20", s1);
        }

        [Test]
        public unsafe void TestWeekNumber()
        {
            var q = new QDate(2014, 11, 20);

            var week = q.WeekNumber();

            Assert.AreEqual(47, week);
        }

        [Test]
        public void TestYear()
        {
            var q = new QDate(2014, 11, 20);

            Assert.AreEqual(2014, q.Year);
        }

        [Test]
        public void TestNotEqualOperator()
        {
            var q = new QDate(2014, 11, 20);
            var q2 = new QDate(2014, 11, 22);

            Assert.AreNotEqual(q, q2);
        }

        [Test]
        public void TestLessOperator()
        {
            var q = new QDate(2014, 11, 20);
            var q2 = new QDate(2014, 11, 22);

            var res = (q < q2); // true

            //Assert.Less(q, q2);

            Assert.IsTrue(res);
        }

        [Test]
        public void TestLessEqualOperator()
        {
            var q = new QDate(2014, 11, 21);
            var q2 = new QDate(2014, 11, 22);

            var res = (q <= q2); // true

            //Assert.LessOrEqual(q, q2);

            Assert.IsTrue(res);
        }

        [Test]
        public void TestEqualOperator()
        {
            var q = new QDate(2014, 11, 22);
            var q2 = new QDate(2014, 11, 22);

            Assert.AreEqual(q, q2);
        }

        [Test]
        public void TestGreaterOperator()
        {
            var q = new QDate(2014, 11, 20);
            var q2 = new QDate(2014, 11, 22);

            var res = (q > q2); // false

            //Assert.Greater(q, q2);

            Assert.IsFalse(res);
        }

        [Test]
        public unsafe void TestGreaterEqualOperator()
        {
            var q = new QDate(2014, 11, 23);
            var q2 = new QDate(2014, 11, 22);

            var res = (q >= q2); // true

            //Assert.GreaterOrEqual(q, q2);

            Assert.IsTrue(res);
        }
    }
}