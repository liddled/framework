using System;
using System.Globalization;

namespace DL.Framework.Common
{
    [Serializable]
	public class BusDate : IComparable
	{
        #region Constructors

        public BusDate(BusDate date)
        {
            JulianDate = date.JulianDate;
        }

        public BusDate(DateTime dateTime)
        {
            JulianDate = JDay(dateTime.Day, dateTime.Month, dateTime.Year);
        }
        
        public BusDate(int day, int month, int year)
        {
            SetDate(day, month, year);
        }

        public BusDate(int sort8Date)
        {
            var year = sort8Date / 10000;
            var month = ((sort8Date - (10000 * year)) / 100);
            var day = (sort8Date - (10000 * year) - (100 * month));

            SetDate(day, month, year);
        }

        #endregion

        #region Public Overrides

        public override bool Equals(object rhs)
        {
            var busDate = rhs as BusDate;
            return busDate != null && JulianDate == busDate.JulianDate;
        }

        public override int GetHashCode()
        {
            return JulianDate.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0:00}-{1:00}-{2:0000}", Day, Month, Year);
        }

        public string ToString(string format)
        {
            return ToDateTime().ToString(format);
        }

        public string ToString(DateView dateView)
        {
            string date;

            switch (dateView)
            {
                case DateView.Year:
                    date = DateTime.ToString("yyyy");
                    break;
                case DateView.Month:
                    date = DateTime.ToString("MMM, yyyy");
                    break;
                case DateView.Day:
                    date = DateTime.ToString("MMM dd, yyyy");
                    break;
                default:
                    date = TextRepTx.ToText(dateView, ETextRepType.Display);
                    break;
            }

            return date;
        }

        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day);
        }

        #endregion

        #region Operators

        public static BusDate operator +(BusDate lhs, int days)
        {
            var newDate = new BusDate(lhs);
            newDate.JulianDate += days;
            return newDate;
        }

        public static BusDate operator -(BusDate lhs, int days)
        {
            var newDate = new BusDate(lhs);
            newDate.JulianDate -= days;
            return newDate;
        }

        public static bool operator ==(BusDate lhs, BusDate rhs)
        {
            return Equals(lhs, rhs);
        }

        public static bool operator !=(BusDate lhs, BusDate rhs)
        {
            return Equals(lhs, rhs) == false;
        }

        public static bool operator >(BusDate lhs, BusDate rhs)
        {
            return lhs.JulianDate > rhs.JulianDate;
        }

        public static bool operator >=(BusDate lhs, BusDate rhs)
        {
            return lhs.JulianDate >= rhs.JulianDate;
        }

        public static bool operator <(BusDate lhs, BusDate rhs)
        {
            return lhs.JulianDate < rhs.JulianDate;
        }

        public static bool operator <=(BusDate lhs, BusDate rhs)
        {
            return lhs.JulianDate <= rhs.JulianDate;
        }

        #endregion

        #region Public Properties

        public int JulianDate { get; set; }

        public int Day
        {
            get
            {
                int day, month, year, dayOfWeek;
                JDate(JulianDate, out day, out month, out year, out dayOfWeek);
                return day;
            }
        }

        public int Month
        {
            get
            {
                int day, month, year, dayOfWeek;
                JDate(JulianDate, out day, out month, out year, out dayOfWeek);
                return month;
            }
        }

        public int Year
        {
            get
            {
                int day, month, year, dayOfWeek;
                JDate(JulianDate, out day, out month, out year, out dayOfWeek);
                return year;
            }
        }

        public int Sort8Date
        {
            get { return (Year * 10000) + (Month * 100) + Day; }
        }

        public int MonthSort8Date
        {
            get { return (Year * 10000) + (Month * 100) + 1; }
        }

        public int YearSort8Date
        {
            get { return (Year * 10000) + (1 * 100) + 1; }
        }

        public DateTime DateTime
        {
            get { return new DateTime(Year, Month, Day); }
        }

        #endregion

        #region Public Static Properties

        public static BusDate Null
        {
            get { return null; }
        }

        public static BusDate MonthToDate
        {
            get { return new BusDate(DateTime.Today); }
        }

        public static BusDate Today
        {
            get { return new BusDate(DateTime.Today); }
        }

        #endregion

        #region Public Static Methods

        public static int JDay(int day, int month, int year)
		{
			if (month > 2)
			{
				month -= 3;
			}
			else
			{
				month += 9;
				year -= 1;
			}

			int c = year / 100;
			int ya = year - (100 * c);

			return (146097*c)/4 + (1461*ya)/4 + (153*month + 2)/5 + day + 1721119;
		}

		public static void JDate(int julian, out int day, out int month, out int year, out int dayOfWeek)
		{
			dayOfWeek= (julian + 1) % 7;

			julian -= 1721119;
			year = (4 * julian - 1)/146097;
			julian = 4 * julian - 1 - 146097 * year;
			day = julian/4;
			julian = (4 * day + 3)/1461;
			day = 4 * day + 3 - 1461 * julian;
			day = (day + 4)/4;
			month = (5 * day - 3)/153;
			day = 5 * day - 3 - 153 * month;
			day = (day + 5) / 5;
			year = 100 * year + julian;

			if(month < 10)
			{
				month += 3;
			}
			else
			{
				month -= 9;
				++year;
			}
		}

        public static int GetDaysInMonth(int month, int year)
        {
            Calendar calendar = new GregorianCalendar();
            int daysinmonth = calendar.GetDaysInMonth(year, month);
            return daysinmonth;
        }

        public static BusDate GetDate(BusDate date, DateView dateView)
        {
            return GetDate(date, dateView, 0);
        }

        public static BusDate GetNextDate(BusDate date, DateView dateView)
        {
            return GetDate(date, dateView, 1);
        }

        public static BusDate GetPreviousDate(BusDate date, DateView dateView)
        {
            return GetDate(date, dateView, -1);
        }

        private static BusDate GetDate(BusDate date, DateView dateView, int add)
        {
            DateTime adjustedDate;

            switch (dateView)
            {
                case DateView.Day:
                    adjustedDate = date.DateTime.AddDays(add);
                    return new BusDate(adjustedDate.Day, adjustedDate.Month, adjustedDate.Year);
                case DateView.Month:
                    adjustedDate = date.DateTime.AddMonths(add);
                    return new BusDate(1, adjustedDate.Month, adjustedDate.Year);
                case DateView.Year:
                    adjustedDate = date.DateTime.AddYears(add);
                    return new BusDate(1, 1, adjustedDate.Year);
                default:
                    return Today;
            }
        }

		#endregion

        #region Private Methods

        private void SetDate(int day, int month, int year)
        {
            if (year < 1 || year > 9999)
                throw new ArgumentOutOfRangeException("year", year, "Year must be in range 1..9999");

            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException("month", month, "Month must be in range 1..12");

            if (day > GetDaysInMonth(month, year))
                throw new ArgumentException(string.Format("The day {0} exceeds the actual number of days in month {1} for year {2}.", day, month, year));

            JulianDate = JDay(day, month, year);
        }

        #endregion

        #region Parse Methods

        public static bool TryParseExact(string s, out BusDate result)
        {
            result = null;

            DateTime d;
            if (!DateTime.TryParseExact(s, "yyyyMMdd", null, DateTimeStyles.None, out d))
                return false;

            result = new BusDate(d);
            return true;
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            var cmp = (BusDate)obj;
            return JulianDate.CompareTo(cmp.JulianDate);
        }

        #endregion
    }
}