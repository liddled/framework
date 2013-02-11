using System;

namespace DL.Framework.Common
{
    public static class DateTimeHelper
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dateTime.AddMilliseconds(unixTimeStamp);
        }
    }
}
